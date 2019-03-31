using UnityEngine;

public class PlayerDeath : MonoBehaviour {
  public delegate void OnDeathCallback();
  public static OnDeathCallback onDeath;

  public void die() {
    if (onDeath != null) onDeath();
  }
}
