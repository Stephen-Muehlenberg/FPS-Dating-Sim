using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
  public AudioMixer mixer;
  public Slider volumeSlider;
  public Slider sensitivitySlider;
  public Slider textSpeedSlider;

  private bool showPauseMenuOnDismiss;

  public static void show() { show(false); }

  public static void show(bool showPauseMenuOnDismiss) {
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
    menu.showPauseMenuOnDismiss = showPauseMenuOnDismiss;
    menu.volumeSlider.value = dbToDisplayVolume(Settings.volume);
    menu.sensitivitySlider.value = Settings.mouseSensitivity;
    menu.textSpeedSlider.value = inverseTextSpeed(Settings.textSpeed);
  }

  public void setVolume(float volume) {
    var dbs = displayVolumetToDb(volume);
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
    SaveManager.SaveSettings();
    if (showPauseMenuOnDismiss) PauseMenu.show();
    Destroy(this.gameObject);
  }

  void Update() {
    if (Input.GetKeyUp(KeyCode.Escape)) dismiss();
  }

  // Slider returns linear values, but perception of dbs is exponential; map from the former to the latter
  private static float displayVolumetToDb(float displayVolume) {
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
