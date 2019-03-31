using UnityEngine;

public class TimeUtils : MonoBehaviour {
  public delegate void TransitionCallback();

  public static bool gameplayPaused = false;
  public static bool dialogPaused = false; // Dialog paused implies gameplay also paused
  private static bool gameplayPreviouslyPaused;
  private static float previousTimeScale;

  private static TimeUtils SINGLETON;
  private static float NORMAL_TIME_SCALE = 1f;
  private static float NORMAL_TIME_STEP = 0.02f;

  private static void lazyInitialize() {
    SINGLETON = new GameObject().AddComponent<TimeUtils>();
  }

  public static void pauseGameplay() {
    gameplayPaused = true;
  }

  public static void resumeGameplay() {
    if (Time.timeScale == 0)
      throw new UnityException("Can't manually resume gameplay when timescale is 0");
    gameplayPaused = false;
  }

  public static void pauseDialog() {
    gameplayPreviouslyPaused = gameplayPaused;
    gameplayPaused = true;
    dialogPaused = true;
    previousTimeScale = Time.timeScale;
    Time.timeScale = 0;
  }

  public static void resumeDialog() {
    Time.timeScale = previousTimeScale;
    dialogPaused = false;
    gameplayPaused = gameplayPreviouslyPaused;
  }

  public static void setTimeScale(float scale) {
    Time.timeScale = scale;
    Time.fixedDeltaTime = NORMAL_TIME_STEP * scale;
  }

  public static void clearTimeScale() {
    Time.timeScale = NORMAL_TIME_SCALE;
    Time.fixedDeltaTime = NORMAL_TIME_STEP;
    gameplayPaused = false;
  }

  /* public static void setTimeScale(float scale, float transitionDuration, TransitionCallback callback) {
     if (scale == Time.timeScale) return;
     if (SINGLETON == null) lazyInitialize();
     if (scale == 0) gameplayPaused = true;
     SINGLETON.fade(scale, transitionDuration, callback);
   }*/

  /*  public static void clearTimeScale(float transitionDuration, TransitionCallback callback) {
      if (Time.timeScale == NORMAL_TIME_SCALE) return;
      if (SINGLETON == null) lazyInitialize();
      SINGLETON.fade(NORMAL_TIME_SCALE, transitionDuration, callback);
    }

    // Can't start a coroutine from a static method, so need a singleton
    public void fade(float end, float duration, TransitionCallback callback) {
      StartCoroutine(fadeTimeScale(Time.timeScale, end, duration, callback));
    }

    private static IEnumerator fadeTimeScale(float start, float end, float duration, TransitionCallback callback)
    {
      float timeScaleDelta = end - start;
      float timeSoFar = 0f;
      float timeFraction = timeSoFar / duration;
      float newTimeScale;

      while (timeSoFar < duration)
      {
        newTimeScale = start + (timeScaleDelta * timeFraction);
        Time.timeScale = newTimeScale >= 0 ? newTimeScale : 0; // Prevent timeScale from being < 0
        Time.fixedDeltaTime = Time.timeScale * NORMAL_TIME_STEP;
        timeSoFar += Time.unscaledDeltaTime;
        timeFraction = timeSoFar / duration;
        yield return new WaitForFixedUpdate();
      }

      Time.timeScale = end;
      Time.fixedDeltaTime = end * NORMAL_TIME_STEP;
      gameplayPaused = end > 0;

      if (callback != null) callback.Invoke();
    }*/
}
