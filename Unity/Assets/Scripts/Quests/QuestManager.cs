using UnityEngine;
using System.Collections;

public class QuestManager {
  public static Quest currentQuest;

  // TODO these values are only used for debugging / testing. Remove them when done.
  public static string QUEST_1 = Quest_Introduction.NAME;
  public static string QUEST_2 = Quest_Tutorial.NAME;
  public static string QUEST_3 = Quest_CafeBreak1.NAME;
  public static string QUEST_4 = Quest_Competition.NAME;
  public static string QUEST_5 = Quest_CafeBreak2.NAME;
  public static string QUEST_6 = Quest_BedStore.NAME;
  public static string QUEST_7 = Quest_TestKillEnemies.NAME;

  public static void start(string questName) { start(getQuest(questName)); }
  public static void start(Quest quest) {
    if (currentQuest != null) {
      Debug.LogWarning("Replacing current quest " + currentQuest.name + " with " + quest.name);
      currentQuest.stop();
    }
    currentQuest = quest;
    currentQuest.start();
  }

  public static void resume(string questName, Hashtable args) {
    if (currentQuest != null) {
      Debug.LogWarning("Replacing current quest " + currentQuest.name + " with " + questName + ".");
      currentQuest.stop();
    }
    currentQuest = getQuest(questName);
    if (args == null) currentQuest.start();
    else currentQuest.resume(args);
  }

  private static Quest getQuest(string name) {
    // Gross lookup table, but it'll do for now
    if (name == Quest_Introduction.NAME) return new Quest_Introduction();
    if (name == Quest_Tutorial.NAME) return new Quest_Tutorial();
    if (name == Quest_CafeBreak1.NAME) return new Quest_CafeBreak1();
    if (name == Quest_Competition.NAME) return new Quest_Competition();
    if (name == Quest_CafeBreak2.NAME) return new Quest_CafeBreak2();
    if (name == Quest_BedStore.NAME) return new Quest_BedStore();
    if (name == Quest_TestKillEnemies.NAME) return new Quest_TestKillEnemies();
    throw new UnityException("Unknown quest " + name);
  }
}
