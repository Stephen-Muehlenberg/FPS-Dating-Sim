using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour {
  public static Player SINGLETON;

  new public Camera camera;
  public FirstPersonModule.FirstPersonController firstPersonController;
  public GunSwitch gunSwitch;

	void Awake () {
    SINGLETON = this;
	}

  public PlayerState getState() {
    return new PlayerState(
      posX: transform.position.x,
      posY: transform.position.y,
      posZ: transform.position.z,
      rotX: camera.transform.localRotation.x,
      rotY: transform.rotation.y,
      look: firstPersonController.look.enabled,
      move: firstPersonController.move.enabled,
      jump: firstPersonController.jump.enabled);
  }

  public void setState(PlayerState state) {
    transform.position = new Vector3(state.posX, state.posY, state.posZ);
    transform.rotation = Quaternion.Euler(0, state.rotY, 0);
    camera.transform.localRotation = Quaternion.Euler(state.rotX, 0, 0);
    firstPersonController.look.enabled = state.look;
    firstPersonController.move.enabled = state.move;
    firstPersonController.jump.enabled = state.jump;
  }

  public void setInConversation(bool inConversation, bool forceDisableJump = false) {
    firstPersonController.move.inputEnabled = !inConversation;
    firstPersonController.jump.inputEnabled = !inConversation && !forceDisableJump;
    if (inConversation) Weapons.unequip();
  }

  /**
   * Smoothly move the player to the destination over the course of the duration.
   */
  public Coroutine smoothMove(Vector3 destination, float duration) {
    return StartCoroutine(_smoothMove(destination, duration));
  }

  /**
   * Rotate player to face the direction over the course of the duration.
   */
  public Coroutine setLookDirection(Vector3 direction, float duration) {
    return StartCoroutine(_setLookDirection(direction, duration));
  }

  /**
   * Set the player's camera height over the couse of the duration.
   * Useful for e.g. causing the player to crouch.
   */
  public Coroutine setCrouchHeight(float height, float duration) {
    return StartCoroutine(_setCrouchHeight(height, duration));
  }

  private IEnumerator _smoothMove(Vector3 destination, float duration) {
    Vector3 velocity = Vector3.zero;

    var t = 0f;
    var timeMultiplier = 1f / duration;
    while (t < 1) {
      transform.position = new Vector3(
        Mathf.SmoothStep(transform.position.x, destination.x, t),
        Mathf.SmoothStep(transform.position.y, destination.y, t),
        Mathf.SmoothStep(transform.position.z, destination.z, t));
      t += TimeUtils.dialogDeltaTime * timeMultiplier;
      yield return null;
    }
  }

  private IEnumerator _setLookDirection(Vector3 direction, float duration) {
    Quaternion startingRotation = Quaternion.Euler(camera.transform.localRotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
    Quaternion targetRotation = Quaternion.Euler(direction);
    Quaternion currentRotation;
    var t = 0f;
    var timeMultiplier = 1f / duration;
    while (t < 1) {
      currentRotation = Quaternion.Slerp(startingRotation, targetRotation, t);
      transform.rotation = Quaternion.Euler(0, currentRotation.eulerAngles.y, 0);
      camera.transform.localRotation = Quaternion.Euler(currentRotation.eulerAngles.x, 0, 0);
      t += TimeUtils.dialogDeltaTime * timeMultiplier;
      yield return null;
    }
  }

  private IEnumerator _setCrouchHeight(float height, float duration) {
    var headbob = SINGLETON.firstPersonController.headbob;
    var baseHeadHeight = headbob.baseHeadHeight;
    var camera = Camera.main;
    var t = 0f;
    var timeMultiplier = 1f / duration;

    while (t < 1) {
      headbob.setBaseHeadHeight(Mathf.SmoothStep(baseHeadHeight, height, t));
      t += TimeUtils.dialogDeltaTime * timeMultiplier;
      yield return null;
    }
    headbob.baseHeadHeight = height;
  }
}
