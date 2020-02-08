using UnityEngine;
using UnityEngine.UI;

public class SaveLoadMenu : MonoBehaviour {
  private static readonly float DOUBLE_CLICK_MAX_DURATION = 0.3f;

  public Text title;
  public RectTransform listEntryGroup;
  public Transform listEntryItemParent;
  public Scrollbar scrollbar;
  public InputField nameField;
  public Button deleteButton;
  public Button saveLoadButton;

  public GameObject listEntryPrefab;

  private bool modeLoad; // Should dialog display Load state (true) or Save state (false)?
  private Callback onCompleted;

  private bool interactable = true;
  private SaveGameListItem selectedItem = null;

  private float doubleClickTimeRemaining = 0; // Used to detect double clicks

  public static void showLoadMenu() { show(true, null); }
  public static void showLoadMenu(Callback onLoadCompleted) { show(true, onLoadCompleted); }
  public static void showSaveMenu() { show(false, null); }
  public static void showSaveMenu(Callback onSaveCompleted) { show(false, onSaveCompleted); }

  private static void show(bool load, Callback onCompleted) {
    // Create menu
    var prefab = Resources.Load<GameObject>("UI/SaveLoadMenu");
    var instance = Instantiate(prefab);

    // Attach & fit menu to scene canvas
    instance.transform.SetParent(MainCanvas.transform);
    var rectTransform = instance.GetComponent<RectTransform>();
    rectTransform.offsetMax = new Vector2(0, 0);
    rectTransform.offsetMin = new Vector2(0, 0);

    // Initialise instance
    instance.GetComponent<SaveLoadMenu>().initialise(load, onCompleted);
  }

  private void initialise(bool load, Callback onCompleted) {
    this.modeLoad = load;
    this.onCompleted = onCompleted;

    var saveFileInfo = SaveManager.getFileSummaries();

    // For safety, cap the number of files shown to 50
    if (saveFileInfo.Count > 50) saveFileInfo = saveFileInfo.GetRange(0, 50);

    foreach (SaveFileInfo fileInfo in saveFileInfo) {
      var item = Instantiate(listEntryPrefab).gameObject;
      item.transform.SetParent(listEntryItemParent);
      item.GetComponent<SaveGameListItem>().setInfo(this, fileInfo);
    }

    title.text = load ? "Load Game" : "Save Game";
    saveLoadButton.GetComponentInChildren<Text>().text = load ? "Load" : "Save";

    if (load) {
      // Hide the name field, and extend the file list down to include the name field's height
      var nameFieldRect = nameField.transform as RectTransform;
      var nameFieldHeight = nameFieldRect.offsetMax.y - nameFieldRect.offsetMin.y;
      Destroy(nameField.gameObject);
      listEntryGroup.offsetMin = new Vector2(listEntryGroup.offsetMin.x, listEntryGroup.offsetMin.y - nameFieldHeight);
    }

    deleteButton.interactable = false;
    saveLoadButton.interactable = false;
  }

  void Update() {
    if (!interactable) return;

    if (Input.GetKeyUp(KeyCode.Escape)) {
      clickDismiss();
      return;
    }

    if (selectedItem != null && Input.GetKeyUp(KeyCode.Return)) {
      clickSaveLoad();
      return;
    }

    if (doubleClickTimeRemaining > 0) doubleClickTimeRemaining -= Time.deltaTime;
  }

  private void setInteractable(bool interactable) {
    this.interactable = interactable;
    if (!modeLoad) nameField.interactable = interactable;
  }

  public void selectItem(SaveGameListItem item) {
    if (!interactable) return;

    // If file selected for the first time, enable Load / Delete buttons
    if (selectedItem == null) {
      deleteButton.interactable = true;
      saveLoadButton.interactable = true;
    }
    // If file re-selected within a short window of time, it's a double click, so load
    else if (item.info.name == selectedItem.info.name &&
      item.info.playtime == selectedItem.info.playtime &&
      item.info.lastPlayed == selectedItem.info.lastPlayed &&
      doubleClickTimeRemaining > 0) {
      if (modeLoad) load();
      else save();
    }

    if (!modeLoad) nameField.text = item.info.name;

    selectedItem = item;
    doubleClickTimeRemaining = DOUBLE_CLICK_MAX_DURATION;
  }

  public void modifyFileName() {
    // After modifying the save file name, assume we're no longer dealing with an existing file, 
    // so deselect selected item and disable delete button.
    selectedItem = null;
    deleteButton.interactable = false;

    // Can only save when the file has a name.
    saveLoadButton.interactable = nameField.text.Length > 0;
  }
  
  public void clickDelete() {
    if (!interactable) return;
    setInteractable(false);
    delete();
  }

  private void delete() {
    // TODO show confirmation dialog?
    SaveManager.delete(selectedItem.info);
    Destroy(selectedItem.gameObject);
    deleteButton.interactable = false;
    saveLoadButton.interactable = false;
    selectedItem = null;
    setInteractable(true);
  }

  public void clickDismiss() {
    if (!interactable) return;
    setInteractable(false);
    dismiss();
  }

  private void dismiss() {
    if (onCompleted != null) onCompleted.Invoke();
    Destroy(this.gameObject);
  }

  public void clickSaveLoad() {
    if (!interactable) return;
    setInteractable(false);
    // TODO show confirmation dialog?

    if (modeLoad) load();
    else save();
  }

  private void load() {
    // TODO fade out is not working
    ScreenFade.fadeOut(() => {
      GameData? loadedData = SaveManager.load(selectedItem.info);

      if (loadedData.HasValue) {
        GameData.apply(loadedData.Value);
      }
      else {
        ScreenFade.fadeIn(() => { setInteractable(true); });
      }
    });
  }

  private void save() {
    GameData data = GameData.compile();
    SaveManager.save(data, nameField.text, dismiss, () => { /*TODO handle failure*/ });
  }

  private void onLoadFailed() {
    // TODO display load failed message
  }
}
