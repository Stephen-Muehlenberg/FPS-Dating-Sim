using System.Collections;
using UnityEngine;
using FirstPersonModule;

public class PauseMenu : MonoBehaviour {
  public static bool visible;

  private bool actionSelected = false; // Ignore subsequent actions after clicking Load

  public static void show() {
    // Create menu
    var prefab = Resources.Load<GameObject>("UI/PauseMenu");
    var instance = Instantiate(prefab);

    // Attach & fit menu to scene canvas
    instance.transform.SetParent(GameObject.Find("Canvas").transform);
    var rectTransform = instance.GetComponent<RectTransform>();
    rectTransform.offsetMax = new Vector2(0, 0);
    rectTransform.offsetMin = new Vector2(0, 0);

    // Enable cursor
    Cursor.lockState = CursorLockMode.Confined;
    Cursor.visible = true;

    if (Player.SINGLETON != null && Player.SINGLETON.GetComponent<FirstPersonController>() != null)
      Player.SINGLETON.GetComponent<FirstPersonController>().enabled = false;
    
    TimeUtils.pauseDialog();

    visible = true;
  }

  public void save() {
    // TODO show some kind of visual indication that the save was successful
    SaveManager.SaveGame();
  }

  public void load() {
    if (actionSelected) return;
    actionSelected = true;

    SaveManager.LoadGame();
  }

  public void showSettings() {
    SettingsMenu.show(true);
    Destroy(this.gameObject);
  }

  public void exitToMainMenu() {
    SceneTransition.fadeTo("main_menu", () => {
      visible = false;
      TimeUtils.resumeDialog();
      TimeUtils.resumeGameplay();
    });
  }

  public void exitGame() {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
  }

  public void dismiss() {
    StartCoroutine(delayedDismiss());
  }

  void Update() {
    if (Input.GetKeyUp(KeyCode.Escape)) StartCoroutine(delayedDismiss());
  }

  // Hack to prevent anything listening to Escape / mouse clicks from firing from the same action used to dismiss the menu.
  private IEnumerator delayedDismiss() {
    yield return 0; // Delay 1 frame, THEN dismiss menu

    // Disable cursor
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;

    Destroy(this.gameObject);
    visible = false;

    TimeUtils.resumeDialog();
    Player.SINGLETON.GetComponent<FirstPersonController>().enabled = true;
  }
}
