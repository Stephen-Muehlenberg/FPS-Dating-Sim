using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class SaveManager {
  private static readonly string SAVE_PATH = Application.dataPath + "/Saved Games";
  private static readonly string SAVE_EXTENSION = ".save";
  private static readonly string AUTOSAVE_NAME = "AUTOSAVE";
  private static readonly string SETTINGS_FILE = Application.dataPath + "/Settings.txt";

  /**
   * Checks if there are any saved games in the save directory.
   */
  public static bool savedGamesExist() {
    if (!Directory.Exists(SAVE_PATH)) return false;

    DirectoryInfo dirInfo = new DirectoryInfo(SAVE_PATH);
    FileInfo[] files = dirInfo.GetFiles("*.save");

    return files.Length > 0;
  }

  /**
   * Returns a list of save file summaries for displaying in a save / load menu.
   */
  public static List<SaveFileInfo> getFileSummaries() {
    var saveInfo = new List<SaveFileInfo>();

    if (!Directory.Exists(SAVE_PATH)) {
      return saveInfo;
    }

    DirectoryInfo dirInfo = new DirectoryInfo(SAVE_PATH);
    FileInfo[] files = dirInfo.GetFiles("*.save");

    // TODO cap the number of files loaded to e.g. 50

    foreach (FileInfo file in files) {
      string filename = file.Name.Substring(0, file.Name.Length - SAVE_EXTENSION.Length);
      GameData? data = load(filename);
      if (data.HasValue) {
        saveInfo.Add(new SaveFileInfo(filename, data.Value.playtime.ToString(), "todo"));
      }
    }

    return saveInfo;
  }

  /**
   * Saves the game data into the specified save file.
   */
  public static void save(GameData data, String filename, Callback onCompleted, Callback onFailed) {
    try {
      // Open file
      FileStream file;
      string filePath = getFullSavePath(filename);
      if (Directory.Exists(SAVE_PATH)) {
        if (File.Exists(filePath)) file = File.OpenWrite(filePath);
        else file = File.Create(filePath);
      }
      else {
        Directory.CreateDirectory(SAVE_PATH);
        file = File.Create(filePath);
      }

      // Write to file
      data.lastPlayed = DateTime.Now;
      var formatter = new BinaryFormatter();
      formatter.Serialize(file, data);

      // Close file
      file.Close();
    }
    catch (Exception e) {
      Debug.LogWarning("Save failed!");
      Debug.LogException(e);
      if (onFailed != null) onFailed.Invoke();
      return;
    }

    if (onCompleted != null) onCompleted.Invoke();
  }

  /**
   * Saves the current game to a file matching the autosave name format.
   */
  public static void autosave() {
    // TODO run this in a background thread
    save(GameData.compile(), AUTOSAVE_NAME, null, null);
  }

  /**
   * Loads the specified save file. Returns null on error.
   */
  public static GameData? load(SaveFileInfo save) { return load(save.name); }
  /**
   * Loads the save file with the specified name. Returns null on error.
   */
  public static GameData? load(string filename) {
    GameData? loadedData;// = null;

    try {
      // Open file
      FileStream file;
      string filePath = getFullSavePath(filename);
      if (Directory.Exists(SAVE_PATH)) {
        if (File.Exists(filePath)) file = File.OpenRead(filePath);
        else {
          Debug.LogWarning("File not loaded: can't find file " + filePath);
          return null;
        }
      }
      else {
        Debug.LogWarning("File not loaded: can't find path " + SAVE_PATH);
        return null;
      }

      // Read file
      var formatter = new BinaryFormatter();
      loadedData = formatter.Deserialize(file) as GameData?;

      // Close file
      file.Close();
    }
    catch (Exception e) {
      Debug.Log("Failed to load game!");
      Debug.Log(e);
      return null;
    }

    return loadedData;
  }

  /**
   * Loads the most recently played game. Returns null on error.
   */
  public static GameData? loadMostRecent() {
    string mostRecentGameName = "";
    DateTime? mostRecentGameTime = null;

    if (!Directory.Exists(SAVE_PATH)) return null;

    DirectoryInfo dirInfo = new DirectoryInfo(SAVE_PATH);
    FileInfo[] files = dirInfo.GetFiles("*.save");

    // TODO cap the number of files loaded to e.g. 50

    foreach (FileInfo file in files) {
      string filename = file.Name.Substring(0, file.Name.Length - SAVE_EXTENSION.Length);
      GameData? data = load(filename);
      if (data.HasValue && (!mostRecentGameTime.HasValue || mostRecentGameTime.Value.CompareTo(data.Value.lastPlayed) < 0)) {
        mostRecentGameName = filename;
        mostRecentGameTime = data.Value.lastPlayed;
      }
    }

    if (mostRecentGameTime.HasValue) return load(mostRecentGameName);
    else return null;
  }

  public static void delete(SaveFileInfo save) {
    if (Directory.Exists(SAVE_PATH)) {
      string filePath = getFullSavePath(save.name);
      if (File.Exists(filePath)) {
        File.Delete(filePath);
      }
      else Debug.LogWarning("Save file not deleted: can't find file " + filePath);
    }
    else Debug.LogWarning("Save file not deleted: can't find path " + SAVE_PATH);
  }

  /**
   * Saves the current game settings to the settings file.
   */
  public static void saveSettings() {
    var data = Settings.compileSaveData();
    
    try {
      var formatter = new BinaryFormatter();
      var file = File.OpenWrite(SETTINGS_FILE);
      formatter.Serialize(file, data);
      file.Close();
    }
    catch (Exception e) {
      Debug.LogWarning("Save settings failed!");
      Debug.LogException(e);
    }
  }

  /**
   * Loads stored game settings from the settings file. Returns null if file not found or can't be loaded.
   */
  public static Settings.SaveData loadSettings() {
    if (!File.Exists(SETTINGS_FILE)) return null;

    Settings.SaveData savedSettings;
    
    try {
      var formatter = new BinaryFormatter();
      var file = File.OpenRead(SETTINGS_FILE);
      savedSettings = formatter.Deserialize(file) as Settings.SaveData;
      file.Close();
    }
    catch (Exception e) {
      Debug.LogWarning("Load settings failed!");
      Debug.LogException(e);
      return null;
    }

    return savedSettings;
  }

  private static string getFullSavePath(string filename) { return SAVE_PATH + "/" + filename + SAVE_EXTENSION; }
}

/**
 * Contains save file display information for save / load menus.
 */
public struct SaveFileInfo {
  public string name;
  public string playtime;
  public string lastPlayed;

  public SaveFileInfo(string name, string playtime, string lastPlayed) {
    this.name = name;
    this.playtime = playtime;
    this.lastPlayed = lastPlayed;
  }
}
