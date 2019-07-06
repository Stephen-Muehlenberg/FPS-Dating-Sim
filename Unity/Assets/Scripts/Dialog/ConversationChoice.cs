using UnityEngine;
using UnityEngine.UI;

public class ConversationChoice : MonoBehaviour {
  private const float MAX_MOUSE_SELECTION_OFFSET = 5f;
  private const int NO_SELECTION = -1;
  private const string HINT_NO_SELECTIONS = "[Left mouse] to select";
  private const string HINT_VALID_SELECTION = "[Spacebar] to confirm";

  public Text[] optionText;
  public Text hint;

  private Conversation.Action.Choice.OnSelection callback;
  private int currentlyHighlightedIndex = NO_SELECTION;
  private int currentlySelectedIndex = NO_SELECTION;
  private bool firstFrameSkipped = false; // Ignore selections on the first frame as they may have been from a previous dialog

  private Vector2 mouseInput = Vector2.zero; // Cached for re-use
  private float absX, absY; // Cached for re-use

  public static void show(string[] options, Conversation.Action.Choice.OnSelection callback) {
    if (options.Length < 2 || options.Length > 4) throw new UnityException("options must contain 2-4 elements.");
    Object res = Resources.Load("UI/ConversationChoice");
    GameObject instance = GameObject.Instantiate(res) as GameObject;
    RectTransform rectTransform = instance.transform as RectTransform;
    rectTransform.SetParent(MainCanvas.transform);
    rectTransform.offsetMin = new Vector2(0, 0);
    rectTransform.offsetMax = new Vector2(0, 0);

    ConversationChoice ui = instance.GetComponent<ConversationChoice>();
    ui.callback = callback;

    for (int i = 0; i < 4; i++) {
      if (options.Length > i) {
        ui.optionText[i].text = options[i];
        ui.unhighlight(ui.optionText[i]);
        ui.deselect(ui.optionText[i]);
      }
      else ui.optionText[i].enabled = false;
    }

    ui.hint.text = HINT_NO_SELECTIONS;
  }

  public void Update() {
    if (TimeUtils.dialogPaused) return;

    updateHighlight();
    updateSelection();
  }

  private void updateHighlight() {
    // Smooth the mouse input over time, so selection doesn't flicker back and forth between frames.
    // Also cap the maximum distance the "cursor" can go, so you never have to move the mouse too far back.
    mouseInput = new Vector2(Mathf.Clamp(mouseInput.x + Input.GetAxis("Mouse X"), -MAX_MOUSE_SELECTION_OFFSET, MAX_MOUSE_SELECTION_OFFSET),
                             Mathf.Clamp(mouseInput.y + Input.GetAxis("Mouse Y"), -MAX_MOUSE_SELECTION_OFFSET, MAX_MOUSE_SELECTION_OFFSET));

    absX = Mathf.Abs(mouseInput.x);
    absY = Mathf.Abs(mouseInput.y);

    if (absX > absY) { // More horizontal than vertical
      if (mouseInput.x > 0) setCurrentHighlight(1);      // Right
      else if (mouseInput.x < 0) setCurrentHighlight(0); // Left
    }
    else if (absY > absX) { // More vertical than horizontal
      if (mouseInput.y < 0) setCurrentHighlight(2);      // Up
      else if (mouseInput.y > 0) setCurrentHighlight(3); // Down
    }
  }

  private void setCurrentHighlight(int indexToHighlight) {
    if (currentlyHighlightedIndex == indexToHighlight) return; // No change

    if (currentlyHighlightedIndex != NO_SELECTION) unhighlight(optionText[currentlyHighlightedIndex]);
    highlight(optionText[indexToHighlight]);
    currentlyHighlightedIndex = indexToHighlight;
  }

  private void select(Text text) {
    text.color = new Color(1, 0.95f, 0.51f);
  }

  private void deselect(Text text) {
    text.color = Color.white;
  }

  private void highlight(Text text) {
    text.fontSize = 32;
  }

  private void unhighlight(Text text) {
    text.fontSize = 28;
  }

  private void updateSelection() {
    if (!firstFrameSkipped) {
      firstFrameSkipped = true;
      return;
    }

    if (Input.GetMouseButtonUp(0) && currentlyHighlightedIndex != NO_SELECTION) {
      // Deselect
      if (currentlySelectedIndex == currentlyHighlightedIndex) {
        deselect(optionText[currentlySelectedIndex]);
        currentlySelectedIndex = NO_SELECTION;
        hint.text = HINT_NO_SELECTIONS;
      }
      // Select
      else {
        if (currentlySelectedIndex != NO_SELECTION) deselect(optionText[currentlySelectedIndex]);
        select(optionText[currentlyHighlightedIndex]);
        currentlySelectedIndex = currentlyHighlightedIndex;
        hint.text = HINT_VALID_SELECTION;
      }
    }

    if (Input.GetButtonUp("Jump") && currentlySelectedIndex != NO_SELECTION) {
      var call = callback;
      Destroy(this.gameObject);
      call.Invoke(currentlySelectedIndex, optionText[currentlySelectedIndex].text);
    }
  }
}
