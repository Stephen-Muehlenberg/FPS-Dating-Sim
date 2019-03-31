using System;

public class Settings {
  [Serializable]
  public class SaveData {
    public float volume;
    public float mouseSensitivity;
    public float textSpeed;
  }

  public static float volume; // Decibels, from -80 to 0
  public static float mouseSensitivity; // 0.5 to 7
  public static float textSpeed; // Number of characters revealed per second, from 0.07 to 0.005

  public static void resetToDefault() {
    volume = 0f;
    mouseSensitivity = 3f;
    textSpeed = 0.025f;
  }

  public static void set(SaveData settings) {
    volume = settings.volume;
    mouseSensitivity = settings.mouseSensitivity;
    textSpeed = settings.textSpeed;
  }

  public static Settings.SaveData compileSaveData() {
    SaveData settings = new SaveData();
    settings.volume = volume;
    settings.mouseSensitivity = mouseSensitivity;
    settings.textSpeed = textSpeed;
    return settings;
  }
}
