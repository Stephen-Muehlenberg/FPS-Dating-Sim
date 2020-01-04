using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CurrentQuestMessage {
  private const float HEIGHT = 65;
  private static Text currentText;

  /**
   * Sets the current quest message, to be displayed at the top of the screen.
   * Messages should be short and easy to read, no more than 1 line, with no full stop.
   * @param animateIn should the message animate in (true, default) or appear immediately (false).
   */
  public static void set(string message, bool animateIn = true) {
    if (currentText != null) clear(animateIn);

    currentText = Object.Instantiate(Resources.Load<GameObject>("UI/CurrentQuestMessage")).GetComponent<Text>();

    var rectTransform = currentText.transform as RectTransform;
    rectTransform.SetParent(MainCanvas.transform);
    rectTransform.offsetMin = new Vector2(0, -10); // Left, top?
    rectTransform.offsetMax = new Vector2(0, HEIGHT - 10); // Right, bottom?
    rectTransform.anchoredPosition = new Vector2(0, (HEIGHT / 2) - 10);

    currentText.text = message;

    if (animateIn) currentText.StartCoroutine(animate(currentText, 0.85f, true, -50));
  }

  private static void setRectPosition(RectTransform rect, float y) {
    rect.offsetMin = new Vector2(0, y); // Left, Pos Y
    rect.offsetMax = new Vector2(0, HEIGHT); // Right, Height
    rect.anchoredPosition = new Vector2(0, y - (HEIGHT / 2));
  }

  /**
   * Removes the current quest message, optionally animating out.
   */
  public static void clear(bool animateOut = true) {
    if (currentText == null) Debug.LogWarning("No message to clear");
    else if (animateOut) currentText.StartCoroutine(animate(currentText, 0.85f, false, -50));
    else Object.Destroy(currentText.gameObject);
  }

  private static IEnumerator animate(Text text, float duration, bool fadeIn, float verticalMotion) {
    float progress = 0;

    var rectTransform = text.transform as RectTransform;
    var offset = new Vector2(0, verticalMotion);
    Vector2 startOffsetMin = rectTransform.offsetMin;
    Vector2 startOffsetMax = rectTransform.offsetMax;
    Vector2 startAnchored = rectTransform.anchoredPosition;
    Vector2 endOffsetMin = startOffsetMin + offset;
    Vector2 endOffsetMax = startOffsetMax + offset;
    Vector2 endAnchored = startAnchored + offset;

    Color startColor = new Color(text.color.r, text.color.g, text.color.b, fadeIn ? 0 : 1);
    Color endColor = new Color(text.color.r, text.color.g, text.color.b, fadeIn ? 1 : 0);
 
    while (progress < 1) {
      if (!TimeUtils.dialogPaused) {
        progress += TimeUtils.dialogDeltaTime / duration;
        rectTransform.offsetMin = Vector2.Lerp(startOffsetMin, endOffsetMin, progress);
        rectTransform.offsetMax = Vector2.Lerp(startOffsetMax, endOffsetMax, progress);
        rectTransform.anchoredPosition = Vector2.Lerp(startAnchored, endAnchored, progress);
        text.color = Color.Lerp(startColor, endColor, progress);
      }

      yield return null;
    }

    if (endColor.a == 0) Object.Destroy(text.gameObject);
    else {
      //  rectTransform.offsetMin = new Vector2(0, endY);
      text.color = endColor; // This may not be necessary anymore?
    }
  }
}
