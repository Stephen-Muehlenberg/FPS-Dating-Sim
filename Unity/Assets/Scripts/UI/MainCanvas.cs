using UnityEngine;

public class MainCanvas : MonoBehaviour
{
  new public static Transform transform;

  void Awake() {
    transform = GetComponent<Transform>();
  }
}
