using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
  public Button resumeButton;
  public Button loadButton;
  public bool interactable = true;

  public void Start() {
    Cursor.visible = true;
    Cursor.lockState = CursorLockMode.Confined;

    resumeButton.interactable = false; // Disable immediately, so we don't wait for a disk operation
    loadButton.interactable = false;
    resumeButton.interactable = SaveManager.savedGamesExist();
    loadButton.interactable = resumeButton.interactable;
  }

  public void Update() {
    if (Input.GetKeyUp(KeyCode.Alpha1)) QuestManager.start(QuestManager.QUEST_1);
    else if (Input.GetKeyUp(KeyCode.Alpha2)) QuestManager.start(QuestManager.QUEST_2);
    else if (Input.GetKeyUp(KeyCode.Alpha3)) QuestManager.start(QuestManager.QUEST_3);
    else if (Input.GetKeyUp(KeyCode.Alpha4)) QuestManager.start(QuestManager.QUEST_4);
  }

  public void resumeGame() {
    if (!interactable) return;
    interactable = false;

    ScreenFade.fadeOut(() => {
      GameData? data = SaveManager.loadMostRecent();

      if (data.HasValue) GameData.apply(data.Value);
      else ScreenFade.fadeIn(() => {
        interactable = true;
        // TODO display some sort of "load failed" message
      });
    });
  }

  public void newGame() {
    if (!interactable) return;
    interactable = false;

    GameData.apply(GameData.NEW_GAME);
  }

  public void load() {
    if (!interactable) return;

    SaveLoadMenu.showLoadMenu(onSaveLoadClosed);
  }

  private void onSaveLoadClosed() {
    // Check if there are still any save files. If not, disable the load/resume buttons.
    resumeButton.interactable = false; // Disable immediately, so we don't wait for a disk operation
    loadButton.interactable = false;
    resumeButton.interactable = SaveManager.savedGamesExist();
    loadButton.interactable = resumeButton.interactable;
  }

  public void settings() {
    if (!interactable) return;

    SettingsMenu.show();
  }

  public void exit() {
    if (!interactable) return;
    interactable = false;

#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
  }
}
