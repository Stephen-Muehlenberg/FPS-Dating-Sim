﻿using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
  public AudioMixer mixer;
  public Slider volumeSlider;
  public Slider sensitivitySlider;
  public Slider textSpeedSlider;

  private Callback onClosedCallback;

  public static void show() { show(null); }

  public static void show(Callback onClosed) {
    // Create menu
    var prefab = Resources.Load<GameObject>("UI/SettingsMenu");
    var instance = Instantiate(prefab);

    // Attach & fit menu to scene canvas
    instance.transform.SetParent(GameObject.Find("Canvas").transform);
    var rectTransform = instance.GetComponent<RectTransform>();
    rectTransform.offsetMax = new Vector2(0, 0);
    rectTransform.offsetMin = new Vector2(0, 0);

    // Load settings
    var menu = instance.GetComponent<SettingsMenu>();
    menu.onClosedCallback = onClosed;
    menu.volumeSlider.value = dbToDisplayVolume(Settings.volume);
    menu.sensitivitySlider.value = Settings.mouseSensitivity;
    menu.textSpeedSlider.value = inverseTextSpeed(Settings.textSpeed);
  }

  public void setVolume(float volume) {
    var dbs = displayVolumeToDb(volume);
    mixer.SetFloat("MasterVolume", dbs);
    Settings.volume = dbs;
  }

  public void setSensitivity(float sensitivity) {
    Settings.mouseSensitivity = sensitivity;
  }

  public void setTextSpeed(float speed) {
    Settings.textSpeed = inverseTextSpeed(speed);
  }

  public void dismiss() {
    SaveManager.saveSettings();
    Destroy(this.gameObject);
    onClosedCallback?.Invoke();
  }

  void Update() {
    if (Input.GetKeyUp(KeyCode.Escape)) dismiss();
  }

  // Slider returns linear values, but perception of dbs is exponential; map from the former to the latter
  private static float displayVolumeToDb(float displayVolume) {
    var unscaledVolume = Mathf.Pow(displayVolume, 0.25f);
    return (unscaledVolume - 1f) * 80f;
  }

  // Perception of dbs is exponential, but slider uses linear values; map from the former to the latter
  private static float dbToDisplayVolume(float db) {
    var fractionalVolume = (db / 80f) + 1f;
    return Mathf.Pow(fractionalVolume, 4);
  }

  // Need to flip the output of the text speed slider
  private static float inverseTextSpeed(float speed) {
    return 0.075f - speed;
  }
}
