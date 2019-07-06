using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
  public Button resumeButton;
  private bool actionSelected = false; // Ignore subsequent actions after clicking New or Load

  public void Start() {
    Cursor.visible = true;
    Cursor.lockState = CursorLockMode.Confined;
    resumeButton.interactable = false; // Disable immediately, so we don't wait for a disk operation
    resumeButton.interactable = SaveManager.savedGameExists();
  }

  public void newGame() {
    if (actionSelected) return;
    actionSelected = true;

    QuestManager.start(new Quest_Introduction());
  }

  public void resume() {
    if (actionSelected) return;
    actionSelected = true;

//    SaveManager.LoadGame();
    QuestManager.start(new Quest_BedStore());
  }

  public void settings() {
    if (actionSelected) return;

    SettingsMenu.show();
  }

  public void exit() {
    if (actionSelected) return;

#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
  }
}
