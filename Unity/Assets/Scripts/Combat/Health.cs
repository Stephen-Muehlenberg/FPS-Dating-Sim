using UnityEngine;

abstract public class Health : MonoBehaviour {
  public int remaining;
  public abstract void takeDamage(Damage damage);
}

public struct Damage {
  public int amount;
  public Vector3 origin;       // Position of the source of the damage. Useful for e.g. showing damage direction indicator.
  public Vector3 hitPoint;     // Global position of the hit. Useful for e.g. spawning damage numbers.
  public Collider hitLocation; // Which part of the object was hit? Useful for identifying e.g. critical hits.

  public Damage(int amount, Vector3 origin, Vector3 hitPoint) {
    this.amount = amount;
    this.origin = origin;
    this.hitPoint = hitPoint;
    this.hitLocation = null;
  }

  public Damage(int amount, Vector3 origin, Vector3 hitPoint, Collider hitLocation) {
    this.amount = amount;
    this.origin = origin;
    this.hitPoint = hitPoint;
    this.hitLocation = hitLocation;
  }
}
