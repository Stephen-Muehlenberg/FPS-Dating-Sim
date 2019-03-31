using UnityEngine;

// Handles general functions for Scenes with combat
public class CombatScene : MonoBehaviour {
  public static bool paused = false;

  void Awake() {
    // Reset the monsters
    MonstersController.removeAll();
  }

	void Start () {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  void Update() {
    if (Input.GetKeyUp(KeyCode.Escape) && !PauseMenu.visible) PauseMenu.show();
  }
}
