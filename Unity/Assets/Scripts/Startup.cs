using UnityEngine;
using UnityEngine.Audio;

public class Startup : MonoBehaviour {
  private static bool settingsInitialized = false;

  public AudioMixer mixer;

#if UNITY_EDITOR
  // We sometimes exit edit mode with timescale=0, which persists between runs.
  // Reset timescale to 1 on startup.
  private void Awake() {
    Time.timeScale = 1;
  }
#endif

  void Start () {
    if (settingsInitialized) return; // Only initialize settings first time a scene loads

    Settings.SaveData savedSettings = SaveManager.loadSettings();
    if (savedSettings == null) Settings.resetToDefault();
    else Settings.set(savedSettings);

    mixer.SetFloat("MasterVolume", Settings.volume);
    settingsInitialized = true;
    Destroy(this);
	}
}
