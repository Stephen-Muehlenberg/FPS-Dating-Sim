using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Handles setup and general functions for scenes outside the main menu.
 */
public class NonMenuScene : MonoBehaviour {
  void Awake() {
    // Reset the monsters
    Monsters.removeAll();
  }

  void Start() {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  void Update() {
    if (Input.GetKeyUp(KeyCode.Escape) && !TimeUtils.dialogPaused) PauseMenu.show();
  }
}
