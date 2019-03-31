using System.Collections;
using System.Collections.Generic;
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
    text.GetComponent<Outline>().effectColor = new Color(0.5f, 0.5f, 0.25f);
    text.GetComponent<Outline>().effectDistance = new Vector2(3, 3);
  }

  private void deselect(Text text) {
    text.GetComponent<Outline>().effectColor = Color.black;
    text.GetComponent<Outline>().effectDistance = new Vector2(1, 1);
  }

  private void highlight(Text text) {
    text.fontSize = 32;
    text.color = Color.white;
  }

  private void unhighlight(Text text) {
    text.fontSize = 28;
    text.color = new Color(0.85f, 0.85f, 0.85f);
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

/*
  private const float MAX_MOUSE_SELECTION_OFFSET = 5f;

  private bool[] canHighlight;

  private enum Mode { HIGHLIGHT, SELECT, NONE };
  private Mode mode;
  private Weapon currentlyHighlightedWeapon;

  private Weapon initiallyHighlightedWeapon;
  private int minWeaponSelections, maxWeaponSelections;
  private List<Weapon> selectedWeapons;
  private SelectionCallback selectionCallback;
  private bool firstFrameSkipped = false; // Ignore selections on the first frame as they may have been from a previous dialog
  private bool canConfirmSelection = false; // Current selection is valid and can be confirmed

  private Vector2 mouseInput; // Cached for re-use
  private float absX, absY; // Cached for re-use

  public static void startHighlight(Weapon currentlyHighlighted) {
    if (singleton != null && singleton.mode != Mode.NONE) throw new UnityException("Cannot have two Weapon Selections simultaneously.");

    if (singleton == null) initializeSingleton();

    singleton.gameObject.SetActive(true);
    singleton.mode = Mode.HIGHLIGHT;
    singleton.currentlyHighlightedWeapon = currentlyHighlighted;
    singleton.initiallyHighlightedWeapon = currentlyHighlighted;
    singleton.mouseInput = new Vector2(); // Clear previous input
    singleton.canHighlight = new bool[4] { Weapons.SHOTGUN.canEquip, Weapons.MACHINE_GUN.canEquip, Weapons.SNIPER_RIFLE.canEquip, Weapons.GRENADE_LAUNCHER.canEquip };

    for (int i = 0; i < 4; i++) {
      if (singleton.canHighlight[i]) {
        singleton.weaponUiItems[i].enable();
        if (i == currentlyHighlighted.index) singleton.weaponUiItems[i].highlight();
        else singleton.weaponUiItems[i].unhighlight();
      }
      else singleton.weaponUiItems[i].disable();
    }

    singleton.instructionText.enabled = false;
    singleton.controlsText.enabled = false;
  }

  public static Weapon endHighlight() {
    if (singleton == null || singleton.mode != Mode.HIGHLIGHT) throw new UnityException("Must call startHighlight() before endHighlight().");

    singleton.mode = Mode.NONE;
    singleton.gameObject.SetActive(false);

    if (singleton.canHighlight[singleton.currentlyHighlightedWeapon.index]) return singleton.currentlyHighlightedWeapon;
    else return singleton.initiallyHighlightedWeapon;
  }

  public static void select(int min, int max, string message, SelectionCallback callback) {
    singleton.mouseInput = new Vector2(); // Clear previous input
    singleton.canHighlight = new bool[4] { Weapons.SHOTGUN.canEquip, Weapons.MACHINE_GUN.canEquip, Weapons.SNIPER_RIFLE.canEquip, Weapons.GRENADE_LAUNCHER.canEquip };

    for (int i = 0; i < 4; i++) {
      if (singleton.canHighlight[i]) {
        singleton.weaponUiItems[i].enable();
        singleton.weaponUiItems[i].unhighlight();
      }
      else singleton.weaponUiItems[i].disable();
    }

    singleton.instructionText.enabled = true;
    singleton.instructionText.text = message;
    singleton.controlsText.enabled = true;

    WeaponSelectParticles.initialize();
    singleton.updateCanConfirm();
  }

  private void completeSelection(List<Weapon> selectedWeapons) {
    WeaponSelectParticles.stop();
    foreach (WeaponSelectItem item in weaponUiItems) {
      item.deselect();
    }
    mode = Mode.NONE;
    gameObject.SetActive(false);
    selectionCallback.Invoke(selectedWeapons);
  }

  private void updateCanConfirm() {
    if ((selectedWeapons.Count >= minWeaponSelections && selectedWeapons.Count <= maxWeaponSelections) != canConfirmSelection) {
      canConfirmSelection = !canConfirmSelection;
      controlsText.text = canConfirmSelection ? "[Jump] to confirm" : "[Fire] to select";
    }
  }
}
 */
