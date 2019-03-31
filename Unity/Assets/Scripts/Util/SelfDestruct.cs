using UnityEngine;

public class SelfDestruct : MonoBehaviour {
  public float timeToLive = 1f;

  public void Start() {
    Destroy(this.gameObject, timeToLive);
  }
}
