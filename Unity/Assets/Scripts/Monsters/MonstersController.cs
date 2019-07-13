using System.Collections.Generic;
using UnityEngine;

public class MonstersController : MonoBehaviour {
  public delegate void MonstersChangedCallback(Monster monster, bool added, int monstersRemaining);
  public static event MonstersChangedCallback OnMonstersChanged;

  public static List<Monster> monsters = new List<Monster>(); // TODO this should really be read-only

  public static void add(Monster monster) {
    monsters.Add(monster);
    OnMonstersChanged?.Invoke(monster, true, monsters.Count);
  }

  public static void remove(Monster monster) {
    monsters.Remove(monster);
    OnMonstersChanged?.Invoke(monster, false, monsters.Count);
  }

  public static void killAll() {
    foreach (Monster monster in monsters) { Destroy(monster.gameObject); }
    monsters = new List<Monster>();
    OnMonstersChanged?.Invoke(null, false, 0);
  }

  public static void removeAll() {
    monsters = new List<Monster>();
    OnMonstersChanged?.Invoke(null, false, 0);
  }

  public static int monstersNear(Vector3 centerPoint, float maxDistance) {
    var maxDistSq = maxDistance * maxDistance;
    var nearbyMonsters = 0;

    foreach (Monster monster in monsters) {
      if ((monster.transform.position - centerPoint).sqrMagnitude <= maxDistSq)
        nearbyMonsters++;
    }

    return nearbyMonsters;
  }

  public static Monster findByName(string name) {
    foreach (Monster monster in monsters) {
      if (monster.name.Equals(name)) return monster;
    }
    return null;
  }
}
