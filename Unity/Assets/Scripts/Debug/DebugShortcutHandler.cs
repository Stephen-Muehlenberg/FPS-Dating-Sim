using UnityEngine;

public class DebugShortcutHandler : MonoBehaviour {
  private bool reloadStarted = false;

  void Update() {
    // Toggle damage text
    if (Input.GetKeyUp(KeyCode.T))
      EnemyHealth.showDamageText = !EnemyHealth.showDamageText;

    // Increase move speed
    else if (Input.GetKeyUp(KeyCode.Equals))
      Player.SINGLETON.firstPersonController.move.runProfile.speed *= 2;

    // Decrease move speed
    else if (Input.GetKeyUp(KeyCode.Minus))
      Player.SINGLETON.firstPersonController.move.runProfile.speed /= 2;

    // Kill all enemies
    else if (Input.GetKeyUp(KeyCode.K))
      Monsters.killAll();
    
    // Reload from the current state
    else if (Input.GetKeyUp(KeyCode.R))
      reloadMission(QuestManager.currentQuest.state - (QuestManager.currentQuest.state % 100));

    // Reload from specified state
    else if (Input.GetKeyUp(KeyCode.Keypad0)) reloadMission(state: 0);
    else if (Input.GetKeyUp(KeyCode.Keypad1)) reloadMission(state: 100);
    else if (Input.GetKeyUp(KeyCode.Keypad2)) reloadMission(state: 200);
    else if (Input.GetKeyUp(KeyCode.Keypad3)) reloadMission(state: 300);
    else if (Input.GetKeyUp(KeyCode.Keypad4)) reloadMission(state: 400);
    else if (Input.GetKeyUp(KeyCode.Keypad5)) reloadMission(state: 500);
    else if (Input.GetKeyUp(KeyCode.Keypad6)) reloadMission(state: 600);
    else if (Input.GetKeyUp(KeyCode.Keypad7)) reloadMission(state: 700);
    else if (Input.GetKeyUp(KeyCode.Keypad8)) reloadMission(state: 800);
    else if (Input.GetKeyUp(KeyCode.Keypad9)) reloadMission(state: 900);
  }

  private void reloadMission(int state) {
    if (reloadStarted) return; // Ignore multiple reload requests
    reloadStarted = true;
    QuestManager.start(QuestManager.currentQuest.name, state);
  }
}
