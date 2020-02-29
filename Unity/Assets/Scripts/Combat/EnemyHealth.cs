using System.Collections.Generic;
using UnityEngine;
using static EventManager;

public class EnemyHealth : Health {
  public Collider[] weakPoints;
  public static bool showDamageText = false; // Used for debugging

  private static int amount; // Static for re-use across all instances
  private static bool isCrit; // Static for re-use across all instances

  public override void takeDamage(Damage damage) {
    if (remaining <= 0) return;

    isCrit = weakPointsContains(damage.hitLocation);
    amount = isCrit ? damage.amount * 2 : damage.amount;
    if (showDamageText) DamageText.create(amount, damage.hitPoint, isCrit);

    updateHealth(damage: amount, source: damage.source?.index ?? -1);
  }

  public override void takeDamage(List<Damage> damages) {
    if (remaining <= 0) return;

    int totalDamage = 0;
    bool isAnyCrit = false;
    Vector3 centerPoint = Vector3.zero;

    foreach(Damage damage in damages) {
      isCrit = weakPointsContains(damage.hitLocation);
      totalDamage += isCrit ? damage.amount * 2 : damage.amount;
      isAnyCrit = isCrit || isAnyCrit;
      centerPoint += damage.hitPoint;
    }

    if (showDamageText) {
      centerPoint /= damages.Count;
      DamageText.create(totalDamage, centerPoint, isCrit);
    }

    updateHealth(damage: totalDamage, source: damages[0].source?.index ?? -1);
  }

  private void updateHealth(int damage, int source) {
    remaining -= damage;
    if (remaining <= 0) {
      SendMessage("die");
      if (source >= 0) // -1 if no source
        EventManager.accept(Context.ENEMY_KILLED,
                            TRIGGERED_BY, source);
    }
  }

  private bool weakPointsContains(Collider collider) {
    foreach (Collider weakPoint in weakPoints) {
      if (weakPoint == collider) return true;
    }
    return false;
  }
}
