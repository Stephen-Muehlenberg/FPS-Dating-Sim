using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectMenu : MonoBehaviour {
  public delegate void SelectionCallback(List<Weapon> selection);

  private static WeaponSelectMenu singleton;
  private const float MAX_MOUSE_SELECTION_OFFSET = 5f;

  [SerializeField]
  public WeaponSelectItem[] weaponUiItems;
  [SerializeField]
  private Text instructionText;
  [SerializeField]
  private Text controlsText;
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

  private static void initializeSingleton() {
    var prefab = Resources.Load("UI/Weapon Select Menu");
    var instanceObject = Instantiate(prefab) as GameObject;

    var transform = instanceObject.transform as RectTransform;
    transform.SetParent(MainCanvas.transform);
    transform.offsetMin = new Vector2(0, 0);
    transform.offsetMax = new Vector2(0, 0);

    singleton = instanceObject.GetComponent<WeaponSelectMenu>();
  }

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
    if (singleton != null && singleton.mode != Mode.NONE) throw new UnityException("Cannot have two Weapon Selections simultaneously.");
    if (min > max || max < 1 || min > 3) throw new UnityException("" + min + " to " + max + " is not a valid Weapon Select range.");

    if (singleton == null) initializeSingleton();
    singleton.gameObject.SetActive(true);
    singleton.mode = Mode.SELECT;
    singleton.minWeaponSelections = min;
    singleton.maxWeaponSelections = max;
    singleton.selectedWeapons = new List<Weapon>();
    singleton.selectionCallback = callback;
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

  public void Update() {
    if (TimeUtils.dialogPaused) return;

    updateHighlight();
    
    if (mode == Mode.SELECT) updateSelection();
  }

  private void updateHighlight() {
    // Smooth the mouse input over time, so selection doesn't flicker back and forth between frames.
    // Also cap the maximum distance the "cursor" can go, so you never have to move the mouse too far back.
    mouseInput = new Vector2(Mathf.Clamp(mouseInput.x + Input.GetAxis("Mouse X"), -MAX_MOUSE_SELECTION_OFFSET, MAX_MOUSE_SELECTION_OFFSET),
                             Mathf.Clamp(mouseInput.y + Input.GetAxis("Mouse Y"), -MAX_MOUSE_SELECTION_OFFSET, MAX_MOUSE_SELECTION_OFFSET));

    absX = Mathf.Abs(mouseInput.x);
    absY = Mathf.Abs(mouseInput.y);

    // Assuming LEFT = SHOTGUN, RIGHT = MACHINE_GUN, UP = SNIPER_RIFLE, DOWN = GRENADE_LAUNCHER
    if (absX > absY) { // More horizontal than vertical
      if (mouseInput.x > 0) highlight(Weapons.MACHINE_GUN);
      else if (mouseInput.x < 0) highlight(Weapons.SHOTGUN);
    }
    else if (absY > absX) { // More vertical than horizontal
      if (mouseInput.y < 0) highlight(Weapons.SNIPER_RIFLE);
      else if (mouseInput.y > 0) highlight(Weapons.GRENADE_LAUNCHER);
    }
  }

  private void highlight(Weapon newHighlight) {
    if (currentlyHighlightedWeapon == newHighlight) return; // No change

    if (currentlyHighlightedWeapon != null) weaponUiItems[currentlyHighlightedWeapon.index].unhighlight();
    currentlyHighlightedWeapon = newHighlight;
    weaponUiItems[currentlyHighlightedWeapon.index].highlight();
  }

  private void updateSelection() {
    if (!firstFrameSkipped) {
      firstFrameSkipped = true;
      return;
    }

    if (Input.GetMouseButtonUp(0) && canHighlight[currentlyHighlightedWeapon.index]) {
      // Deselect
      if (selectedWeapons.Contains(currentlyHighlightedWeapon)) {
        selectedWeapons.Remove(currentlyHighlightedWeapon);
        weaponUiItems[currentlyHighlightedWeapon.index].deselect();
        updateCanConfirm();
        WeaponSelectParticles.setEnabled(currentlyHighlightedWeapon, false);
      }
      // Select
      else {
        if (selectedWeapons.Count == singleton.maxWeaponSelections) return; // Can't select any more

        selectedWeapons.Add(currentlyHighlightedWeapon);
        weaponUiItems[currentlyHighlightedWeapon.index].select();
        updateCanConfirm();
        WeaponSelectParticles.setEnabled(currentlyHighlightedWeapon, true);
      }
    }

    if (Input.GetButtonUp("Jump") && canConfirmSelection) {
      completeSelection(selectedWeapons);
    }
  }

  private void updateCanConfirm() {
    if ((selectedWeapons.Count >= minWeaponSelections && selectedWeapons.Count <= maxWeaponSelections) != canConfirmSelection) {
      canConfirmSelection = !canConfirmSelection;
      controlsText.text = canConfirmSelection ? "[Jump] to confirm" : "[Fire] to select";
    }
  }
}
