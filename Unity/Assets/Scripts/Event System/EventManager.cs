using System.Collections.Generic;
using UnityEngine;

public class EventManager {
  public const string CONTEXT = "context";
  public const string QUEST_NAME = "quest_name";
  public const string QUEST_STATE = "quest_state";
  public const string PLAYER_HP = "player_hp"; // 1000 hp = full health
  public const string CURRENT_WEAPON = "player_weapon"; // -1 = none
  public const string ROSE_AVAILABLE = "rose_available"; // 1 = true, 0 = false
  public const string MAY_AVAILABLE = "rose_available"; // 1 = true, 0 = false
  public const string VANESSA_AVAILABLE = "rose_available"; // 1 = true, 0 = false
  public const string FIZZY_AVAILABLE = "rose_available"; // 1 = true, 0 = false

  public class Context {
    public const int PLAYER_HIT = 0;
    public const int ENEMY_KILLED = 1;
  }

  // Logging is potentially slow, so remove logging code from actual builds.
#if UNITY_EDITOR
  private static bool log = false;
#endif

  public static void accept(int context) {
    Dictionary<string, int> args = gatherGlobalContext();
    args.Add(CONTEXT, context);

#if UNITY_EDITOR
    if (log) {
      string s = "";
      foreach (string key in args.Keys) s += key + " " + args[key] + ", ";
      s = s.Substring(0, s.Length - 2);
      Debug.Log("EventManager.accept()\n" + s);
    }
#endif

    int bestScore = 0;
    List<EventsDb.Event> bestMatches = new List<EventsDb.Event>();

    foreach (EventsDb.Event e in EventsDb.COMMON_EVENTS) {
      if (e.score < bestScore) break;
      if (e.matches(args)) {
#if UNITY_EDITOR
        if (log) Debug.Log("Comparing rule (score = " + e.score + ")\n" + e.ruleToString() + " - NEW MATCH!");
#endif
        bestScore = e.score;
        bestMatches.Add(e);
      }
#if UNITY_EDITOR
      else if (log) Debug.Log("Comparing rule (score = " + e.score + ")\n" + e.ruleToString() + " - does not match");
#endif
    }

#if UNITY_EDITOR
    if (log) Debug.Log("Found " + bestMatches.Count + " matches");
#endif

    if (bestMatches.Count == 0) return;

    bestMatches[Random.Range(0, bestMatches.Count)].response.Invoke();
  }

  private static Dictionary<string, int> gatherGlobalContext() {
    Dictionary<string, int> dict = new Dictionary<string, int>();
    if (Player.SINGLETON != null) {
      dict[PLAYER_HP] = Player.SINGLETON.health.remaining;
      dict[CURRENT_WEAPON] = Weapons.currentlyEquipped?.index ?? -1;
      dict[ROSE_AVAILABLE] = Weapons.SHOTGUN.showInWeaponMenu ? 1 : 0;
      dict[MAY_AVAILABLE] = Weapons.MACHINE_GUN.showInWeaponMenu ? 1 : 0;
      dict[VANESSA_AVAILABLE] = Weapons.SNIPER_RIFLE.showInWeaponMenu ? 1 : 0;
      dict[FIZZY_AVAILABLE] = Weapons.GRENADE_LAUNCHER.showInWeaponMenu ? 1 : 0;
    }
    if (QuestManager.currentQuest != null) {
      // TODO add an id to each Quest
      dict[QUEST_NAME] = QuestManager.currentQuest.name.GetHashCode();
      dict[QUEST_STATE] = QuestManager.currentQuest.state;
    }
    return dict;
  }
}
