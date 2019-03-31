using UnityEngine;

public class DestroyOnDeath : MonoBehaviour {
  public void die() {
    Destroy(this.gameObject);
  }
}
