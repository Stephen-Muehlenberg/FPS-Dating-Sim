﻿using System;
using System.Collections;

/**
 * Serializable collection of all data necessary to represent a saved game.
 */
[Serializable]
public struct GameData {
  public int playtime; // Duration in seconds
  public DateTime lastPlayed; // Formatted date-time

  public float affinityRose;
  public float affinityMay;
  public float affinityVanessa;
  public float affinityFizzy;

  public string currentQuestName;
  public Hashtable currentQuestArgs;
  // TODO save args hashtable

  public static GameData NEW_GAME = new GameData {
    playtime = 0,
    affinityRose = 0,
    affinityMay = 0,
    affinityVanessa = 0,
    affinityFizzy = 0,
    currentQuestName = Quest_Introduction.NAME,
    currentQuestArgs = null
  };

  /**
   * Extracts current game data from various sources and compiles it into a format for saving.
   */
  public static GameData compile() {
    return new GameData {
      playtime = Playtime.getCount(),
      lastPlayed = DateTime.Now,
      
      affinityRose = Affinity.Rose,
      affinityMay = Affinity.May,
      affinityVanessa = Affinity.Vanessa,
      affinityFizzy = Affinity.Fizzy,

      currentQuestName = QuestManager.currentQuest.name,
      currentQuestArgs = QuestManager.currentQuest.save()
    };
  }

  /**
   * Loads the game data into play.
   */
  public static void apply(GameData data) {
    Playtime.startCount(data.playtime);

    Affinity.Rose = data.affinityRose;
    Affinity.May = data.affinityMay;
    Affinity.Vanessa = data.affinityVanessa;
    Affinity.Fizzy = data.affinityFizzy;

    QuestManager.start(data.currentQuestName, data.currentQuestArgs);
  }
}
