using UnityEngine;
using UnityEngine.UI;

/**
 * A common UI for allowing the player to select 1 item from up to 4 possibilities.
 */
public class SelectionMenu : MonoBehaviour {
  private const int STATE_NORMAL = 0, STATE_HIGHLIGHTED = 1, STATE_SELECTED = 2, STATE_SELECTED_HIGHLIGHTED = 3;
  private const int FONT_SIZE_NORMAL = 28, FONT_SIZE_HIGHLIGHTED = 32;
  private const float MAX_MOUSE_SELECTION_OFFSET = 5f;
  private const int NO_SELECTION = -1;

  private int currentHighlight;
  private int currentSelection = NO_SELECTION;
  private int mouseDownSelection = NO_SELECTION; // Selected item when the mouse was pressed down. Used for more accurate click detection.

  private Vector2 mouseInput = Vector2.zero; // Cached for re-use
  private float absX, absY; // Cached for re-use
  
  public SelectionMenuItem[] optionItems;
  public Transform highlightTransform;
  public Text hint;
  public int fontSizeNormal, fontSizeHighlighted;

  public Mode mode;
  public SelectionCallback callback;
  public Option[] options;

  public class Option {
    public readonly string text;
    public readonly OptionColors colors;

    public Option(string text) {
      this.text = text;
      colors = OptionColors.Default;
    }
    public Option(string text, OptionColors colors) {
      this.text = text;
      this.colors = colors;
    }
  }

  public class OptionColors {
    public Color normal, selected;

    public OptionColors(Color normal, Color selected) {
      this.normal = normal;
      this.selected = selected;
    }

    public static OptionColors Default = new OptionColors(Color.white, new Color(1, 0.95f, 0.51f));
    public static OptionColors Unselectable = new OptionColors(Color.grey, Color.grey);
  }

  public enum Mode {
    Click, // Press and release [Fire] on an item to select it.
    Hold   // Release [Select Menu] on an item to select it (assumes [Select Menu] is already pressed).
  }

  public enum Style {
    DialogChoice,   // Larger font, no icons
    WeaponSelection // Smaller font, weapon icons
  }

  public delegate void SelectionCallback(int selection, string text);

  /**
   * Displays a SelectionMenu in the style of a dialog choice.
   */
  public static void showDialogChoice(string[] options, SelectionCallback callback) {
    Option[] formattedOptions = new Option[options.Length];
    for (int i = 0; i < options.Length; i++) { formattedOptions[i] = new Option(options[i]); }
    GameObject menu = Instantiate(Resources.Load("UI/SelectionMenuDialog")) as GameObject;
    menu.GetComponent<SelectionMenu>().initialise(formattedOptions, NO_SELECTION, Mode.Click, callback);
  }

  /**
   * Displays a SelectionMenu in the style of a weapon choice, using the currently available Weapons.
   */
  public static void showWeaponChoice(Option[] options, int initialSelection, SelectionCallback callback) {
    GameObject menu = Instantiate(Resources.Load("UI/SelectionMenuWeapon")) as GameObject;
    menu.GetComponent<SelectionMenu>().initialise(options, initialSelection, Mode.Hold, callback);
  }

  /**
   * Displays a SelectionMenu with the specified properties.
   */
  public static void show(Option[] options, int initialSelection, Mode mode, Style style, SelectionCallback callback) {
    string resource;
    if (style == Style.DialogChoice) resource = "UI/SelectionMenuDialog";
    else resource = "UI/SelectionMenuWeapon";
    GameObject menu = Instantiate(Resources.Load(resource)) as GameObject;
    menu.GetComponent<SelectionMenu>().initialise(options, initialSelection, mode, callback);
  }
  
  private void initialise(Option[] options, int initialSelection, Mode mode, SelectionCallback callback) {
    if (options.Length < 1 || options.Length > 4) throw new UnityException("Must be 1 to 4 options, but was " + options.Length);

    this.mode = mode;
    this.callback = callback;

    currentSelection = initialSelection;
    currentHighlight = currentSelection;

    if (currentSelection == NO_SELECTION) {
      highlightTransform.gameObject.SetActive(false);
    }
    else {
      mouseInput = new Vector2(x: currentHighlight == 0 ? -0.1f : currentHighlight == 1 ? 0.1f : 0,
                               y: currentHighlight == 2 ? -0.1f : currentHighlight == 3 ? 0.1f : 0);
      highlightTransform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(-mouseInput.y, mouseInput.x) * Mathf.Rad2Deg);
    }

    for (int i = 0; i < 4; i++) {
      if (options.Length > i) optionItems[i].initialize(
        option: options[i],
        selected: currentSelection == i,
        highlighted: currentHighlight == i,
        fontSizeNormal: fontSizeNormal,
        fontSizeHighlighted: fontSizeHighlighted); 
      else optionItems[i].gameObject.SetActive(false);
    }

    if (mode == Mode.Hold) hint.gameObject.SetActive(false);
  }

  public void Update() {
    if (TimeUtils.dialogPaused) return;

    updateHighlight();
    updateSelection();
  }

  private void updateHighlight() {
    // Add mouse movement to the previous mouse position.
    mouseInput += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * TimeUtils.dialogDeltaTime;
    // Restrain mouse position to the unit circle - it's hypotenuse cannot be greater than 1.
    // This provides a small amount of buffer against minor mouse jitter, so the cursor dousn't instantly dart
    // about, but it's small enough that you can still quickly swap from one side of the circle to the other.
    if (mouseInput.sqrMagnitude > 1) mouseInput = mouseInput.normalized;

    // If nothing initially selected then highlight starts off disabled. Enable it once the mouse has move
    // a reasonably significant distance, so we don't accidentally highlight something with tiny input jitter.
    if (!highlightTransform.gameObject.activeSelf) {
      if (mouseInput.SqrMagnitude() > 0.7f) highlightTransform.gameObject.SetActive(true);
    }
    else {
      highlightTransform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(-mouseInput.y, mouseInput.x) * Mathf.Rad2Deg);

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
  }

  private void setCurrentHighlight(int indexToHighlight) {
    if (currentHighlight == indexToHighlight) return; // No change

    if (currentHighlight != NO_SELECTION) optionItems[currentHighlight].setHighlighted(false);
    optionItems[indexToHighlight].setHighlighted(true);

    currentHighlight = indexToHighlight;
    mouseDownSelection = NO_SELECTION;
  }

  private void updateSelection() {
    // Mode.Click
    if (mode == Mode.Click) {
      // Highlight inactive if no initial selection. Wait until mouse movement before allowing selection.
      if (!highlightTransform.gameObject.activeSelf) return;

      if (Input.GetMouseButtonDown(0)) {
        mouseDownSelection = currentHighlight;
      }
      else if (Input.GetMouseButtonUp(0) && mouseDownSelection == currentHighlight) {
        select(currentHighlight);
      }
    }

    // Mode.Hold
    else {
      // TODO handle other menu buttons
      if (Input.GetMouseButtonUp(2)) {
        select(currentHighlight);
      }
    }
  }

  private void select(int optionIndex) {
    var call = callback;
    var selectionName = optionIndex == NO_SELECTION ? "" : optionItems[optionIndex].text.text;
    Destroy(this.gameObject);

    call.Invoke(optionIndex, selectionName);
  }
}
