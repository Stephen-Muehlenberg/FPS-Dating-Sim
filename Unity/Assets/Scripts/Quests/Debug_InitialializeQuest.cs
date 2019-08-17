using UnityEngine;
using System.Collections;

public class Debug_InitialializeQuest : MonoBehaviour {
  public string questToInitialize;
  public int state;

  void Awake() {
    // TODO allow setting of a debug position overriding the quest's designated position
    if (QuestManager.currentQuest == null) {
      var args = new Hashtable {
        {  Quest.KEY_STATE, state }
      };
      QuestManager.resume(questToInitialize, args);
    }

    Destroy(this);
  }
}
