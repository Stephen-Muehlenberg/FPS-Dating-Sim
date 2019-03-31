using UnityEngine;

public class EnemyHealth : Health {
  public Collider[] weakPoints;
  public bool showDamageText = true;

  private static int amount; // Static for re-use across all instances
  private static bool isCrit; // Static for re-use across all instances

  public override void takeDamage(Damage damage) {
    if (remaining <= 0) return;

    isCrit = weakPointsContains(damage.hitLocation);
    amount = isCrit ? damage.amount * 2 : damage.amount;
    if (showDamageText) DamageText.create(amount, damage.hitPoint, isCrit);

    remaining -= amount;
    if (remaining <= 0) SendMessage("die");
  }

  private bool weakPointsContains(Collider collider) {
    foreach (Collider weakPoint in weakPoints) {
      if (weakPoint == collider) return true;
    }
    return false;
  }
}
