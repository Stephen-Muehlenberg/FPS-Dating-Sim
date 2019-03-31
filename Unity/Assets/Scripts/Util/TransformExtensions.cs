using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions {
  private static Vector3 randomAmount;
  private static Quaternion randomDirection;

  public static Vector3 randomScatter(this Transform transform, float maxScatterRadians) {
    randomAmount = Vector3.RotateTowards(transform.forward, transform.right, Random.Range(0f, maxScatterRadians), 0f);
    randomDirection = Quaternion.AngleAxis(Random.Range(0f, 360f), transform.forward);
    return randomDirection * randomAmount;
  }
}
