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
      else if (Input.GetKeyUp(KeyCode.T)) EnemyHealth.showDamageText = !EnemyHealth.showDamageText;
    }
  }
}
