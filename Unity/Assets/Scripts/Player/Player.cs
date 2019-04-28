using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour {
  public static Player SINGLETON;

  new public Camera camera;
  public FirstPersonModule.FirstPersonController firstPersonController;

	void Awake () {
    SINGLETON = this;
	}

  public Coroutine setCrouchHeight(float height, float duration) {
    return StartCoroutine(_setCrouchHeight(height, duration));
  }

  private IEnumerator _setCrouchHeight(float height, float duration) {
    var player = Player.SINGLETON;
    var headbob = player.GetComponent<FirstPersonModule.FirstPersonController>().headbob;
    var baseHeadHeight = headbob.baseHeadHeight;
    var camera = Camera.main;
    var t = 0f;
    var timeMultiplier = 1f / duration;

    while (t < 1) {
      headbob.setBaseHeadHeight(Mathf.SmoothStep(baseHeadHeight, height, t));
      t += Time.deltaTime * timeMultiplier;
      yield return null;
    }
    headbob.baseHeadHeight = height;
  }

  public Coroutine setLookDirection(Vector3 direction, float duration) {
    return StartCoroutine(_setLookDirection(direction, duration));
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
      t += Time.deltaTime * timeMultiplier;
      yield return null;
    }
  }

  public Coroutine smoothMove(Vector3 destination, float duration) {
    return StartCoroutine(_smoothMove(destination, duration));
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
      t += Time.deltaTime * timeMultiplier;
      yield return null;
    }
  }
}
