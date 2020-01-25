using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour {
  private static GameObject scrimCanvas;
  private static Image scrim;
  private static Callback callback;
  private const float DEFAULT_DURATION= 0.75f;
  private static Coroutine currentFade;

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
  // END WORK IN PROGRESS
  // -----------------------------------

  // TODO don't keep scrimCanvas instance alive permanently; kill it after fade is complete.

  private static void lazyIntitialize() {
    scrimCanvas = Instantiate(Resources.Load<GameObject>("UI/Fade Canvas"));
    scrim = scrimCanvas.GetComponentInChildren<Image>();
    DontDestroyOnLoad(scrimCanvas);
  }

  public static void fadeOut(Callback callback = null, 
                             float fadeDuration = DEFAULT_DURATION,
                             bool initialiseToTransparent = false) {
    if (scrimCanvas == null) lazyIntitialize();
    if (currentFade != null) scrimCanvas.GetComponent<ScreenFade>().StopCoroutine(currentFade);

    scrimCanvas.SetActive(true);
    if (initialiseToTransparent) scrim.color = new Color(0, 0, 0, 0);
    ScreenFade.callback = callback;
    
    currentFade = scrimCanvas.GetComponent<ScreenFade>().StartCoroutine(fade(fadeIn: false, duration: fadeDuration));
  }

  public static void fadeIn(Callback callback = null,
                            float fadeDuration = DEFAULT_DURATION,
                            bool initialiseToBlack = false) {
    if (scrimCanvas == null) lazyIntitialize();
    if (currentFade != null) scrimCanvas.GetComponent<ScreenFade>().StopCoroutine(currentFade);

    scrimCanvas.SetActive(true);
    if (initialiseToBlack) scrim.color = new Color(0, 0, 0, 1);
    ScreenFade.callback = callback;

    currentFade = scrimCanvas.GetComponent<ScreenFade>().StartCoroutine(fade(fadeIn: true, duration: fadeDuration));
  }

  private static IEnumerator fade(bool fadeIn, float duration) {
    float targetAlpha = fadeIn ? 0 : 1;
    float alphaPerSecond = (fadeIn ? -1 : 1) / duration;

    while ((fadeIn && scrim.color.a > targetAlpha) || (!fadeIn && scrim.color.a < targetAlpha)) {
      scrim.color = new Color(0, 0, 0, scrim.color.a + (alphaPerSecond * Time.unscaledDeltaTime));
      yield return null;
    }

    scrim.color = new Color(0, 0, 0, targetAlpha);
    if (scrim.color.a == 0) scrimCanvas.SetActive(false); // Disable canvas while it's invisble

    callback.Invoke();
  }
}
