using UnityEngine;
using UnityEngine.UI;

public class CurrentQuestMessage : MonoBehaviour {
  private static float MARGIN_TOP = 65;
  private static CurrentQuestMessage SINGLETON;

  public Text text;

  private void Awake() {
    SINGLETON = this;
  }

  // TODO do some eye catching animation to make it obvious the quest has updated.
  // Maybe make this an optional argument? So we can decide whether to just update
  // the quest (e.g. incremnet a kill count) vs CHANGING the quest.
  public static void set(string message) {
    if (SINGLETON == null) {
      Instantiate(Resources.Load<GameObject>("UI/CurrentQuestMessage"));

      var rectTransform = SINGLETON.transform as RectTransform;
      rectTransform.SetParent(MainCanvas.transform);
      rectTransform.offsetMin = new Vector2(0, 0); // Left, Pos Y
      rectTransform.offsetMax = new Vector2(0, MARGIN_TOP); // Right, Height
      rectTransform.anchoredPosition = new Vector2(0, -MARGIN_TOP / 2);
    }

    SINGLETON.text.enabled = true;
    SINGLETON.text.text = message;
  }

  public static void clear() {
    if (SINGLETON == null) return; // No existing message; do nothing

    SINGLETON.text.enabled = false;
  }
}
