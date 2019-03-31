using UnityEngine;

public class Debug_InitialializeQuest : MonoBehaviour {
  public string questToInitialize;
  public int state;

  void Awake() {
    // TODO allow setting of a debug position overriding the quest's designated position
    if (QuestManager.currentQuest == null) {
      QuestManager.resume(questToInitialize, null);
    }

    Destroy(this);
  }
}
