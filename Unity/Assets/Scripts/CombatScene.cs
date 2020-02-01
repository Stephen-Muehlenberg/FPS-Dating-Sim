using UnityEngine;

// Handles general functions for Scenes with combat
public class CombatScene : MonoBehaviour {
  public static bool paused = false;

  void Awake() {
    // Reset the monsters
    Monsters.removeAll();
  }

	void Start () {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  void Update() {
    if (!TimeUtils.dialogPaused) {
      if (Input.GetKeyUp(KeyCode.Escape)) PauseMenu.show();
      // DEBUG STUFF START
      // Toggle damage text
      else if (Input.GetKeyUp(KeyCode.T)) EnemyHealth.showDamageText = !EnemyHealth.showDamageText;
      // Increase move speed
      else if (Input.GetKeyUp(KeyCode.Equals)) Player.SINGLETON.firstPersonController.move.runProfile.speed *= 2;
      // Decrease move speed
      else if (Input.GetKeyUp(KeyCode.Minus)) Player.SINGLETON.firstPersonController.move.runProfile.speed /= 2;
      // Kill all enemies
      else if (Input.GetKeyUp(KeyCode.K)) {
        Monsters.killAll();
      }
      // DEBUG STUFF END
    }
  }
}
