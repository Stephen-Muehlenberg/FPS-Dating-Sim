using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour {
  public Actor setPosition(Vector3 position) {
    transform.position = position;
    return this;
  }

  public Actor setInteraction(string message, LookTarget.InteractionCallback callback) {
    GetComponentInChildren<LookTarget>().setInteraction(message, callback);
    return this;
  }

  /**
   * Move to the destination over the course of the duration, with vertical headbob.
   * Headbob height defaults to 0.05, headbob frequency defaults to 4.
   */
  public Coroutine walk(Vector3 destination, float duration) { return StartCoroutine(_walk(destination, duration, 0.05f, 4)); }

  /**
   * Move to the destination over the course of the duration, with vertical headbob.
   * Headbob height defaults to 0.05, headbob frequency defaults to 4.
   */
  public Coroutine walk(Vector3 destination, float duration, float headbobHeight) { return StartCoroutine(_walk(destination, duration, headbobHeight, 4)); }

  /**
   * Move to the destination over the course of the duration, with vertical headbob.
   * Headbob height defaults to 0.05, headbob frequency defaults to 4.
   */
  public Coroutine walk(Vector3 destination, float duration, float headbobHeight, float headbobFrequency) {
    return StartCoroutine(_walk(destination, duration, headbobHeight, headbobFrequency));
  }

  private IEnumerator _walk(Vector3 destination, float duration, float headbobHeight, float headbobFrequency) {
    Vector3 origin = transform.position;
    float t = 0f;
    float randomHeadbobOffset = Random.Range(0, headbobFrequency);

    while (transform.position.x != destination.x || transform.position.z != destination.z) {
      if (TimeUtils.dialogPaused) yield return null;

      t += TimeUtils.dialogDeltaTime;

      Vector3.Lerp(origin, destination, t / duration);
      transform.position = Vector3.Lerp(origin, destination, t / duration) + (Vector3.up * headbobHeight * Mathf.Abs(Mathf.Sin(t * headbobFrequency + randomHeadbobOffset)));
      yield return null;
    }

    transform.position = destination;
  }
}
