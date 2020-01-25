using UnityEngine;
using UnityEngine.UI;

public class EnemyCounter : MonoBehaviour {
  public Text remainingText;
  public Text killsText;
  private int kills = 0;

  void Start() {
    updateMonsterCount(null, true, Monsters.monsters.Count);
  }

  void OnEnable() {
    Monsters.OnMonstersChanged += updateMonsterCount;
  }

  void OnDisable() {
    Monsters.OnMonstersChanged -= updateMonsterCount;
  }

  private void updateMonsterCount(Monster monster, bool added, int monsterCount) {
    remainingText.text = monsterCount.ToString();
    if (!added) {
      kills++;
      killsText.text = kills.ToString();
    }
  }
}
