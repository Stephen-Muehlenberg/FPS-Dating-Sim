using System.Collections.Generic;
using UnityEngine;

public class Monsters {
  public delegate void MonstersChangedCallback(Monster monster, bool added, int monstersRemaining);
  public static event MonstersChangedCallback OnMonstersChanged;

  public static List<Monster> monsters = new List<Monster>();
  private static Dictionary<Monster.Type, int> monsterTypeCounts = new Dictionary<Monster.Type, int>();

  // Cached resources
  private static GameObject spawnEffect;
  private static GameObject monsterPrefabTorch;
  private static GameObject monsterPrefabInfernal;
  private static GameObject monsterPrefabSquig;
  private static GameObject monsterPrefabHellfirer;

  public static void add(Monster monster) {
    monsters.Add(monster);
    if (monsterTypeCounts.ContainsKey(monster.type)) monsterTypeCounts[monster.type] = monsterTypeCounts[monster.type] + 1;
    else monsterTypeCounts.Add(monster.type, 1);
    OnMonstersChanged?.Invoke(monster, true, monsters.Count);
  }

  public static void remove(Monster monster) {
    monsters.Remove(monster);
    monsterTypeCounts[monster.type] = monsterTypeCounts[monster.type] - 1;
    OnMonstersChanged?.Invoke(monster, false, monsters.Count);
  }

  public static void removeAll() {
    monsters = new List<Monster>();
    monsterTypeCounts.Clear();
    OnMonstersChanged?.Invoke(null, false, 0);
  }

  public static void killAll() {
    foreach (Monster monster in monsters) { GameObject.Destroy(monster.gameObject); }
    monsters = new List<Monster>();
    monsterTypeCounts.Clear();
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

  public static int count(Monster.Type type) {
    if (monsterTypeCounts.ContainsKey(type)) return monsterTypeCounts[type];
    else return 0;
  }

  public static int torchCount() { return count(Monster.Type.TORCH); }
  public static int infernalCount() { return count(Monster.Type.INFERNAL); }
  public static int squigCount() { return count(Monster.Type.SQUIG); }
  public static int hellfirerCount() { return count(Monster.Type.HELLFIRER); }

  public static Monster findByName(string name) {
    foreach (Monster monster in monsters) {
      if (monster.name.Equals(name)) return monster;
    }
    return null;
  }

  public static void spawnTorch(Vector3 position,
                                Quaternion? rotation = null,
                                bool awareOfPlayer = true,
                                bool animateIn = true) {
    if (monsterPrefabTorch == null) monsterPrefabTorch = Resources.Load<GameObject>("Monsters/Torch");
    spawn(monsterPrefabTorch, position, rotation, awareOfPlayer, animateIn);
  }

  public static void spawnInfernal(Vector3 position,
                                   Quaternion? rotation = null,
                                   bool awareOfPlayer = true,
                                   bool animateIn = true) {
    if (monsterPrefabInfernal == null) monsterPrefabInfernal = Resources.Load<GameObject>("Monsters/Infernal");
    spawn(monsterPrefabInfernal, position, rotation, awareOfPlayer, animateIn);
  }

  public static void spawnSquig(Vector3 position,
                                Quaternion? rotation = null,
                                bool awareOfPlayer = true,
                                bool animateIn = true) {
    if (monsterPrefabSquig == null) monsterPrefabSquig = Resources.Load<GameObject>("Monsters/SquigMonster");
    spawn(monsterPrefabSquig, position, rotation, awareOfPlayer, animateIn);
  }

  public static void spawnHellfirer(Vector3 position,
                                    Quaternion? rotation = null,
                                    bool awareOfPlayer = true,
                                    bool animateIn = true) {
    if (monsterPrefabHellfirer == null) monsterPrefabHellfirer = Resources.Load<GameObject>("Monsters/Hellfirer");
    spawn(monsterPrefabHellfirer, position, rotation, awareOfPlayer, animateIn);
  }

  public static void spawn(GameObject monster, 
                           Vector3 position,
                           Quaternion? rotation = null,
                           bool awareOfPlayer = true,
                           bool animateIn = true) {
    Quaternion rot = rotation ?? Quaternion.Euler(0, Random.Range(0, 360f), 0);

    if (animateIn) {
      if (spawnEffect == null) spawnEffect = Resources.Load<GameObject>("Particles/MonsterSpawnEffect");
      GameObject effect = GameObject.Instantiate(spawnEffect, position, Quaternion.identity);
      effect.GetComponent<MonsterSpawnEffect>().initialise(monster, rot, awareOfPlayer);
    }
    else {
      GameObject instance = GameObject.Instantiate(monster, position, rot);
      if (awareOfPlayer) instance.GetComponent<Awareness>().becomeAlert();
    }
  }
}
