using UnityEngine;

public class Debug_InitialializeQuest : MonoBehaviour {
  public string questToInitialize;
  public int state;

  void Awake() {
    if (QuestManager.currentQuest == null && questToInitialize != "") {
      QuestManager.start(questToInitialize, state);
    }

    Destroy(this);
  }
}
