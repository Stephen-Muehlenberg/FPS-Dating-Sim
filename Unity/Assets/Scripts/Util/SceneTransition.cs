using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition {
  public delegate void TransitionCallback();

  private static TransitionCallback transitionComplete;
  private static TransitionCallback fadeComplete;
  private static bool transitionInProgress = false;

  public static void fadeTo(string scene) { fadeTo(scene, null, null); }
  public static void fadeTo(string scene, TransitionCallback transitionComplete) { fadeTo(scene, transitionComplete, null); }
  public static void fadeTo(string scene, TransitionCallback transitionComplete, TransitionCallback fadeComplete) {
    if (transitionInProgress) throw new UnityException("Cannot transition to scene " + scene + " because a transition is already in progress.");
    transitionInProgress = true;

    SceneTransition.transitionComplete = transitionComplete;
    SceneTransition.fadeComplete = fadeComplete;

    ScreenFade.fadeOut(() => {
      SceneManager.sceneLoaded += fadeoutOnSceneLoad;
      SceneManager.LoadScene(scene);
    });
  }

  private static void fadeoutOnSceneLoad(Scene scene, LoadSceneMode mode) {
    SceneManager.sceneLoaded -= fadeoutOnSceneLoad;
    transitionComplete?.Invoke();

    ScreenFade.fadeIn(() => {
      transitionInProgress = false;
      fadeComplete?.Invoke();
    });
  }

  // Convenience method to allow callbacks on scene change without having to manually unsubscribe the callback.
  // Only works if there's exactly one listener.
  public static void swapTo(string scene, TransitionCallback transitionComplete) {
    // If already at target scene, just trigger the callback
    if (scene == SceneManager.GetActiveScene().name) {
      transitionComplete();
      return;
    }

    if (transitionInProgress) throw new UnityException("Cannot transition to scene " + scene + " because a transition is already in progress.");
    transitionInProgress = true;

    SceneTransition.transitionComplete = transitionComplete;

    SceneManager.sceneLoaded += callbackOnSceneLoad;
    SceneManager.LoadScene(scene);
  }

  private static void callbackOnSceneLoad(Scene scene, LoadSceneMode mode) {
    SceneManager.sceneLoaded -= callbackOnSceneLoad;
    transitionInProgress = false;
    transitionComplete();
  }
}
