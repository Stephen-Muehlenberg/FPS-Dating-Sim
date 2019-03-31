using UnityEngine;

public static class Vector3Extensions {
  /**
   * Square magnitude of the horizontal (x and z) components.
   */
  public static float horizontalSqMagnitude(this Vector3 vector3) {
    return vector3.x * vector3.x + vector3.z * vector3.z;
  }

  /**
   * Normalized vector of the horizontal (x and z) components, with 0 vertical component.
   */
  public static Vector3 horizontalNormalized(this Vector3 vector3) {
    float magnitude = Mathf.Sqrt(vector3.x * vector3.x + vector3.z * vector3.z);
    return new Vector3(vector3.x / magnitude, 0, vector3.z / magnitude);
  }
}
