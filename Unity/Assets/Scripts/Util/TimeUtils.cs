using UnityEngine;

public class TimeUtils : MonoBehaviour {
  /**
   * GAMEPLAY:  FPS gameplay enabled,  dialog enabled,  menus enabled,  timescale x1.
   * DIALOG:    FPS gameplay paused,   dialog enabled,  menus enabled,  timescale x1.
   * PAUSED:    FPS gameplay paused,   dialog paused,   menus enabled,  timescale x0.
   */
  public enum TimeMode {
    GAMEPLAY, DIALOG, PAUSED
  }

  private static TimeMode _mode = TimeMode.GAMEPLAY;
  public static TimeMode mode {
    get { return _mode; }
    set {
      _mode = value;
      gameplayTimeScale = value == TimeMode.GAMEPLAY ? bulletTimeModifier : 0;
      Time.timeScale = value == TimeMode.PAUSED ? 0 : bulletTimeModifier;
      dialogTimeScale = value != TimeMode.PAUSED ? 1 : 0;
    }
  }

  public static bool gameplayPaused {
    get { return _mode != TimeMode.GAMEPLAY; }
  }
  public static bool dialogPaused {
    get { return _mode == TimeMode.PAUSED; }
  }

  private static float gameplayTimeScale = 1f; // This value takes into account the bulletTimeModifier
  private static float dialogTimeScale = 1f;
  private static float bulletTimeModifier = 1f;

  public static float gameplayDeltaTime {
    get { return gameplayTimeScale * Time.deltaTime; }
  }

  public static float dialogDeltaTime {
    get { return dialogTimeScale * Time.unscaledDeltaTime; }
  }

  /**
   * Multiplies gameplay time scale by the provided timescale. E.g. 0.5 will halve the speed.
   */
  public static void startBulletTime(float timescale) {
    if (timescale <= 0) throw new UnityException("Bullet time timescale must be a positive value.");
    bulletTimeModifier = timescale;
    gameplayTimeScale = _mode == TimeMode.GAMEPLAY ? timescale : 0;
    Time.timeScale = _mode == TimeMode.PAUSED ? 0 : timescale;
  }
  
  public static void stopBulletTime() {
    bulletTimeModifier = 1;
    gameplayTimeScale = _mode == TimeMode.GAMEPLAY ? 1 : 0;
    Time.timeScale = _mode == TimeMode.PAUSED ? 0 : 1;
  }
}
