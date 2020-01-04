using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition {
  private static Callback onSceneLoaded;
  private static Callback onFadeComplete;
  private static bool fadeIn;
  private static bool transitionInProgress = false;

  /**
   * Loads the specified scene, optionally fading out and in, and providing callbacks for on load and on fade.
   */
  public static void changeTo(string scene, 
                              bool fadeOut = true,
                              bool fadeIn = true, 
                              Callback onSceneLoaded = null,
                              Callback onFadeComplete = null) {
    if (transitionInProgress) throw new UnityException("Cannot transition to scene " + scene + " because a transition is already in progress.");
    transitionInProgress = true;

    SceneTransition.onSceneLoaded = onSceneLoaded;
    SceneTransition.onFadeComplete = onFadeComplete;
    SceneTransition.fadeIn = fadeIn;

    if (fadeOut) ScreenFade.fadeOut(() => {
      SceneManager.sceneLoaded += sceneLoadedCallback;
      SceneManager.LoadScene(scene);
    });
    else {
      SceneManager.sceneLoaded += sceneLoadedCallback;
      SceneManager.LoadScene(scene);
    }
  }

  private static void sceneLoadedCallback(Scene scene, LoadSceneMode mode) {
    SceneManager.sceneLoaded -= sceneLoadedCallback;
    onSceneLoaded?.Invoke();

    // Dumb restriction that you need a Coroutine to wait, and need a MonoBehavior to start a Coroutine.
    MonoBehaviour temp = new GameObject("Temp fade object").AddComponent<DumbMonoBehavior>();

    // Wait for half a second to ensure everything has time to load. 
    // Shouldn't be too noticeable; the user's expecting a brief pause.
    temp.StartCoroutine(delay(() => {
      GameObject.Destroy(temp.gameObject);
      if (fadeIn) ScreenFade.fadeIn(
        initialiseToBlack: true,
        callback: () => {
          transitionInProgress = false;
          onFadeComplete?.Invoke();
        }
      );
      else {
        transitionInProgress = false;
        onFadeComplete?.Invoke();
      }
    }));
  }

  private static IEnumerator delay(Callback callback) {
    yield return new WaitForSeconds(0.5f);
    callback.Invoke();
  }

  // Super dumb hack, but you need a MonoBehaviour to start a coroutine, so we just 
  // create a super temporary one, then destroy it when done.
  private class DumbMonoBehavior : MonoBehaviour {}
}
