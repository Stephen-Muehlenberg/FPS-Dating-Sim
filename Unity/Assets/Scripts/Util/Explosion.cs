using System.Collections.Generic;
using UnityEngine;

public class Explosion {
  public class CreatureAndDistance {
    public GameObject gameObject;
    public Health health;
    public float distanceSq;

    public CreatureAndDistance(GameObject gameObject, Health health, float distanceSq) {
      this.gameObject = gameObject;
      this.health = health;
      this.distanceSq = distanceSq;
    }
  }

  public static List<CreatureAndDistance> getCreaturesInBlast(Vector3 origin, float radius) {
    var creaturesHit = new List<CreatureAndDistance>();

    foreach (Collider collider in Physics.OverlapSphere(origin, radius)) {
      var health = collider.GetComponentInParent<Health>();
      if (health == null) continue; // Not a creature, or creature already killed

      var gameObject = health.gameObject; // Get the root object of the creature
      var distanceSq = (origin - collider.transform.position).sqrMagnitude;
      var newCreature = true;
      var indexToReplace = -1;

      for (int i = 0; i < creaturesHit.Count; i++) {
        if (creaturesHit[i].gameObject == gameObject) {
          newCreature = false;

          // New closest collider found; replace the existing reference
          if (creaturesHit[i].distanceSq > distanceSq) indexToReplace = i;
          break;
        }
      }

      if (newCreature) creaturesHit.Add(new CreatureAndDistance(gameObject, health, distanceSq));
      else if (indexToReplace > -1) creaturesHit[indexToReplace] = new CreatureAndDistance(gameObject, health, distanceSq);
    }

    return creaturesHit;
  }
}
