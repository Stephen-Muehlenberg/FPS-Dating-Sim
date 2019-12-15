using UnityEngine;

public class CafeScene : MonoBehaviour {

  void Start() {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;

    // TODO this is test stuff; remove it
    if (QuestManager.currentQuest == null) QuestManager.start(new Quest_TestKillEnemies());
  }

  void Update() {
    if (Input.GetKeyUp(KeyCode.Escape) && !TimeUtils.dialogPaused) PauseMenu.show();
  }
}
