using UnityEngine;

public class Monster : MonoBehaviour {
  public enum Type {
    TORCH, INFERNAL, SQUIG, HELLFIRER, OTHER
  }

  public Type type = Type.OTHER;

  public void Start() {
    Monsters.add(this);
  }

  public void die() {
    Monsters.remove(this);
  }
}
