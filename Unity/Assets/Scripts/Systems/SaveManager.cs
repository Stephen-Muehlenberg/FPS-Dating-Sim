using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class SaveManager {
  private static readonly string SAVE_FILE = Application.dataPath + "/FPSDS_save_file.txt";
  private static readonly string SETTINGS_FILE = Application.dataPath + "/FPSDS_settings.txt";

  public static bool savedGameExists() {
    return File.Exists(SAVE_FILE);
  }

  public static void SaveSettings() {
    var data = Settings.compileSaveData();

    // TODO try/catch this
    var formatter = new BinaryFormatter();
    var file = File.OpenWrite(SETTINGS_FILE);
    formatter.Serialize(file, data);
    file.Close();
  }

  public static Settings.SaveData LoadSettings() {
    if (!File.Exists(SETTINGS_FILE)) {
      Settings.resetToDefault();
      return null;
    }

    // TODO try/catch
    var formatter = new BinaryFormatter();
    var file = File.OpenRead(SETTINGS_FILE);
    var data = formatter.Deserialize(file) as Settings.SaveData;
    file.Close();

    Settings.set(data);
    return data;
  }

  public static void SaveGame() {
    var data = compileSaveData();

    // TODO try/catch this
    var formatter = new BinaryFormatter();
    var file = File.OpenWrite(SAVE_FILE);
    formatter.Serialize(file, data);
    file.Close();
  }

  public static void LoadGame() {
    if (File.Exists(SAVE_FILE)) {
      // TODO try/catch this
      var formatter = new BinaryFormatter();
      var file = File.OpenRead(SAVE_FILE);
      var data = formatter.Deserialize(file) as SaveData;
      file.Close();

      applySavedData(data);
    }
    else {
      throw new FileNotFoundException("Save file not found!");
    }
  }

  private static SaveData compileSaveData() {
    return new SaveData {
      affinityRose = Affinity.Rose,
      affinityMay = Affinity.May,
      affinityVanessa = Affinity.Vanessa,
      affinityFizzy = Affinity.Fizzy,

      currentQuestName = QuestManager.currentQuest.name
    };
  }

  private static void applySavedData(SaveData data) {
    Affinity.Rose = data.affinityRose;
    Affinity.May = data.affinityMay;
    Affinity.Vanessa = data.affinityVanessa;
    Affinity.Fizzy = data.affinityFizzy;

    QuestManager.resume(data.currentQuestName, null);
  }
}

[Serializable]
class SaveData {
  public float affinityRose;
  public float affinityMay;
  public float affinityVanessa;
  public float affinityFizzy;

  public string currentQuestName;
  // TODO save args hashtable
}
