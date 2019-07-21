using UnityEngine;

public class CafeScene : MonoBehaviour {

  // TODO remove this after finished working with the level
  public GameObject ceiling;

  void Start() {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    ceiling.SetActive(true); // TODO remove this

    // TODO this is test stuff; remove it
    if (QuestManager.currentQuest == null) QuestManager.start(new Quest_TestKillEnemies());
  }

  void Update() {
    if (Input.GetKeyUp(KeyCode.Escape) && !TimeUtils.dialogPaused) PauseMenu.show();
  }
}
