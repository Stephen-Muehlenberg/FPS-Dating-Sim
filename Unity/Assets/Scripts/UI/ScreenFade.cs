using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour {
  private static GameObject scrimCanvas;
  private static Image scrim;
  private static Callback callback;
  private static float fadeDuration = 0.75f; // TODO make this adjustable and/or have multiple different durations to choose from

  // -----------------------------------
  // WORK IN PROGRESS
  // todo figure out how to best to use this? then remove from Quest_Introduction
  // Respect timescale? gameplay? dialog?
  // Behind text? / depth
  // duration
  private static void foo() {
    var screenFill = new GameObject("Screen fill");
    var fillImage = screenFill.AddComponent<Image>();
    fillImage.color = Color.black;
    screenFill.transform.SetParent(MainCanvas.transform);
    var rectTransform = screenFill.transform as RectTransform;
    rectTransform.anchorMin = new Vector2(0, 0);
    rectTransform.anchorMax = new Vector2(1, 1);
    rectTransform.offsetMin = new Vector2(0, 0);
    rectTransform.offsetMax = new Vector2(0, 0);
  }

  private IEnumerator fade(Graphic image, float alpha, float fadeDuration, bool destroyImageOnComplete) {
    while (image.color.a != alpha) {
      image.CrossFadeAlpha(alpha, fadeDuration, false);
      yield return null;
    }
    if (destroyImageOnComplete) GameObject.Destroy(image.gameObject);
  }
  // -----------------------------------

  private static void lazyIntitialize() {
    scrimCanvas = Instantiate(Resources.Load<GameObject>("UI/Fade Canvas"));
    scrim = scrimCanvas.GetComponentInChildren<Image>();
    DontDestroyOnLoad(scrimCanvas);
    scrimCanvas.SetActive(false);
  }

  public static void fadeOut(Callback callback) {
    if (scrimCanvas == null) lazyIntitialize();

    scrimCanvas.SetActive(true);
    ScreenFade.callback = callback;
    scrimCanvas.GetComponent<ScreenFade>().StartCoroutine(fadeTo(1));
  }

  public static void fadeIn(Callback callback) {
    if (scrimCanvas == null) lazyIntitialize();

    scrimCanvas.SetActive(true);
    ScreenFade.callback = callback;
    scrimCanvas.GetComponent<ScreenFade>().StartCoroutine(fadeTo(0));
  }

  public static void fadeTo(float targetAlpha, Callback callback) {
    if (scrimCanvas == null) lazyIntitialize();

    scrimCanvas.SetActive(true);
    ScreenFade.callback = callback;
    scrimCanvas.GetComponent<ScreenFade>().StartCoroutine(fadeTo(targetAlpha));
  }

  private static IEnumerator fadeTo(float targetAlpha) {
    if (scrim.color.a < targetAlpha) { // Darken
      while (scrim.color.a < targetAlpha) {
        scrim.color = new Color(0, 0, 0, scrim.color.a + (Time.unscaledDeltaTime / fadeDuration));
        yield return null;
      }
    }
    else if (scrim.color.a > targetAlpha) { // Lighten
      while (scrim.color.a > targetAlpha) {
        scrim.color = new Color(0, 0, 0, scrim.color.a - (Time.unscaledDeltaTime / fadeDuration));
        yield return null;
      }
    }
    else Debug.LogWarning("Attempting to fade to the same colour.");

    scrim.color = new Color(0, 0, 0, targetAlpha);
    if (scrim.color.a == 0) scrimCanvas.SetActive(false); // Disable canvas while it's invisble

    callback();
  }
}
