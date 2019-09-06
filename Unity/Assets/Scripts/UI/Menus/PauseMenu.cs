using System.Collections;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
  public bool interactable = true; // False while menu cannot be interacted with, e.g. while another menu is open on top

  public static void show() {
    TimeUtils.pauseDialog();

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

    if (Player.SINGLETON != null) Player.SINGLETON.firstPersonController.enabled = false;
  }

  // Called after navigating away from a sub-menu
  private void resume() {
    interactable = true;
  }

  public void save() {
    if (!interactable) return;
    interactable = false;

    SaveLoadMenu.showSaveMenu(resume);
  }

  public void load() {
    if (!interactable) return;
    interactable = false;

    SaveLoadMenu.showLoadMenu(resume);
  }

  public void showSettings() {
    if (!interactable) return;
    interactable = false;

    SettingsMenu.show(resume);
  }

  public void exitToMainMenu() {
    SceneTransition.fadeTo("main_menu", () => {
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

    TimeUtils.resumeDialog();
    if (Player.SINGLETON != null) Player.SINGLETON.firstPersonController.enabled = true;
  }
}
