using UnityEngine;

public class Monster : MonoBehaviour {
  public void Start() {
    MonstersController.add(this);
  }

  public void die() {
    MonstersController.remove(this);
  }
}
