using UnityEngine;
using System.Collections;

public static class TransformExtensions {
  // Cached to avoid re-allocation. It's assumed these values will be used a lot in normal gameplay.
  private static Vector3 randomAmount;
  private static Quaternion randomDirection;

  /**
   * Returns a randomly* offset vector from this Transform's forward vector, up to the specified number of radians off.
   * * The distribution isn't perfectly even - results will tend to cluster towards the center. However this is probably a desirable feature anyway.
   */
  public static Vector3 randomScatter(this Transform transform, float maxScatterRadians) {
    randomAmount = Vector3.RotateTowards(transform.forward, transform.right, Random.Range(0f, maxScatterRadians), 0f);
    randomDirection = Quaternion.AngleAxis(Random.Range(0f, 360f), transform.forward);
    return randomDirection * randomAmount;
  }

  /**
   * Moves the transform to the destination over (approximately) the duration. Applies dampening to the start and end.
   */
  public static IEnumerator moveSmooth(this Transform transform, Vector3 destination, float duration) { return moveSmooth(transform, destination, duration, null); }
  public static IEnumerator moveSmooth(this Transform transform, Vector3 destination, float duration, Callback callback) {
    Vector3 velocity = Vector3.zero;
    while (transform.position != destination) {
      yield return null;
      if (TimeUtils.dialogDeltaTime > 0) // SmoothDamp returns NaN if used with a delta time of 0
        transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, duration, float.MaxValue, TimeUtils.dialogDeltaTime);
    }
    callback?.Invoke();
  }
}
