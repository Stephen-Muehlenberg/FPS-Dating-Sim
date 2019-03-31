using UnityEngine;
using UnityEngine.UI;

public class EnemyCounter : MonoBehaviour {
  public Text remainingText;
  public Text killsText;
  private int kills = 0;

  void Start() {
    updateMonsterCount(null, true, MonstersController.monsters.Count);
  }

  void OnEnable() {
    MonstersController.OnMonstersChanged += updateMonsterCount;
  }

  void OnDisable() {
    MonstersController.OnMonstersChanged -= updateMonsterCount;
  }

  private void updateMonsterCount(Monster monster, bool added, int monsterCount) {
    remainingText.text = monsterCount.ToString();
    if (!added) {
      kills++;
      killsText.text = kills.ToString();
    }
  }
}
