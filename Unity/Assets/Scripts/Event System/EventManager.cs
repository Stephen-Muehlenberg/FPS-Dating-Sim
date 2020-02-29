using System.Collections.Generic;
using UnityEngine;

public class EventManager {
  public class Context {
    public const int PLAYER_HIT = 0;
    public const int ENEMY_KILLED = 1;
    public const int WEAPON_MISSED = 2;
    public const int WEAPON_FATIGUED = 3; // Fatigued = 60% exhausted
    public const int WEAPON_EXHAUSTED = 4; // Exhausted = unable to fire
    public const int WEAPON_RECOVERED = 5; // Weapon is able to fire again (though still exhausted)
  }

  // Common info
  public const string QUEST_NAME = "quest_name";
  public const string QUEST_STATE = "quest_state";
  public const string PLAYER_HP = "player_hp"; // 1000 hp = full health
  public const string CURRENT_WEAPON = "player_weapon"; // -1 = none
  public const string ROSE_AVAILABLE = "rose_available"; // 1 = true, 0 = false
  public const string MAY_AVAILABLE = "rose_available"; // 1 = true, 0 = false
  public const string VANESSA_AVAILABLE = "rose_available"; // 1 = true, 0 = false
  public const string FIZZY_AVAILABLE = "rose_available"; // 1 = true, 0 = false
  // Other info
  public const string TRIGGERED_BY = "triggered_by";

  private static Dictionary<string, int> args;

  // Logging is potentially slow, so remove logging code from actual builds.
#if UNITY_EDITOR
  private static bool log = false;
  private static System.Diagnostics.Stopwatch stopwatch;

  private static string contextName(int context) {
    switch (context) {
      case 0: return "Player hit";
      case 1: return "Enemy killed";
      case 2: return "Weapon missed";
      case 3: return "Weapon fatigued";
      case 4: return "Weapon exhausted";
      case 5: return "Weapon recovered";
    }
    return "UNKNOWN CONTEXT";
  }
#endif

  public static void accept(int context) {
#if UNITY_EDITOR
    stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif
    handleEvent(context, gatherGlobalContext());
  }

  public static void accept(int context, string arg1, int value1) {
#if UNITY_EDITOR
    stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif
    args = gatherGlobalContext();
    args.Add(arg1, value1);
    handleEvent(context, args);
  }

  public static void accept(int context, string arg1, int value1, string arg2, int value2) {
#if UNITY_EDITOR
    stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif
    args = gatherGlobalContext();
    args.Add(arg1, value1);
    args.Add(arg2, value2);
    handleEvent(context, args);
  }

  public static void accept(int context, string arg1, int value1, string arg2, int value2, string arg3, int value3) {
#if UNITY_EDITOR
    stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif
    args = gatherGlobalContext();
    args.Add(arg1, value1);
    args.Add(arg2, value2);
    args.Add(arg3, value3);
    handleEvent(context, args);
  }

  private static void handleEvent(int context, Dictionary<string, int> args) {
#if UNITY_EDITOR
    if (log) {
      string s = "";
      foreach (string key in args.Keys) s += key + " " + args[key] + ", ";
      s = s.Substring(0, s.Length - 2);
      Debug.Log("EventManager.accept() - context = " + contextName(context) + " (" + context + ")\n" + s);
    }
#endif

    List<EventsDb.Event> bestMatches = getBestMatches(context, args);

#if UNITY_EDITOR
    if (log) Debug.Log("Found " + bestMatches.Count + " matches");
    if (log) Debug.Log("SEARCH COMPLETED in " + stopwatch.ElapsedMilliseconds + " millis / " + (stopwatch.ElapsedMilliseconds / 16.6666f) + " frames.");
#endif

    if (bestMatches.Count == 0) return;

    bestMatches[Random.Range(0, bestMatches.Count)].response.Invoke();
  }

  private static List<EventsDb.Event> getBestMatches(int context, Dictionary<string, int> args) {
    int bestScore = 0;
    List<EventsDb.Event> bestMatches = new List<EventsDb.Event>();

    foreach (EventsDb.Event e in EventsDb.eventsFor(context)) {
      // For optimization reasons, events are ordered by highest score to lowest.
      // If we've found a match and all remaining events are lower scoring, return early.
      if (e.score < bestScore) return bestMatches;

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

    return bestMatches;
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
