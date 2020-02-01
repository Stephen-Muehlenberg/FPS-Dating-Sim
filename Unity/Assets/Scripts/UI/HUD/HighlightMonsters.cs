using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HighlightMonsters : MonoBehaviour {
  private static HighlightMonsters SINGLETON;

  private Vector3 highlightNearPoint;
  private float highlightWithinRangeSq;
  private Callback noMonstersNearCallback;
  private Dictionary<Monster, LocationMarker> monsterMarkers = new Dictionary<Monster, LocationMarker>();

  public void Awake() {
    SINGLETON = this;
    enabled = false; // Off by default
  }

  public static void highlightNear(Vector3 point, float range, Callback noMonstersNearCallback) {
    SINGLETON.highlightNearPoint = point;
    SINGLETON.highlightWithinRangeSq = range * range;
    SINGLETON.noMonstersNearCallback = noMonstersNearCallback;

    foreach (Monster monster in Monsters.monsters) {
      var marker = LocationMarker.add(monster.transform, monster.GetComponent<NavMeshAgent>().height);
      SINGLETON.monsterMarkers.Add(monster, marker);
    }

    Monsters.OnMonstersChanged += SINGLETON.onMonstersChanged;
    SINGLETON.enabled = true;
  }

  public static void clearHighlights() {
    LocationMarker.clear();
    if (SINGLETON == null) return;
    Monsters.OnMonstersChanged -= SINGLETON.onMonstersChanged;
    SINGLETON.monsterMarkers.Clear();
    SINGLETON.enabled = false;
  }

  public void Update() {
    bool inRange;
    int count = 0;

    foreach (Monster monster in monsterMarkers.Keys) {
      inRange = (monster.transform.position - highlightNearPoint).sqrMagnitude <= highlightWithinRangeSq;
      monsterMarkers[monster].gameObject.SetActive(inRange);
      if (inRange) count++;
    }

    if (count == 0) noMonstersNearCallback.Invoke();
  }

  private void onMonstersChanged(Monster monster, bool added, int monstersRemaining) {
    if (added) {
      var marker = LocationMarker.add(monster.transform);
      monsterMarkers.Add(monster, marker);
    }
    else {
      monsterMarkers.Remove(monster);
    }
  }
}
