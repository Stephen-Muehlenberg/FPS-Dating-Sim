using UnityEngine;

/**
 * Maintains a public static reference to the main canvas in the current scene.
 */
public class MainCanvas : MonoBehaviour
{
  new public static Transform transform;

  void Awake() {
    if (transform != null) Debug.LogWarning("A main canvas already exists!");
    transform = GetComponent<Transform>();
  }
}
