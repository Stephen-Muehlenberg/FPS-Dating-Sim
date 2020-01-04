using System.Collections;
using UnityEngine;

public static class CanvasGroupExtensions {
  public static IEnumerator fade(this CanvasGroup group, float alpha, float duration) {
    float deltaPerSecond = (alpha - group.alpha) / duration;
    float t = 0f;
    while (t < duration) {
      if (!TimeUtils.gameplayPaused) {
        group.alpha += deltaPerSecond * TimeUtils.gameplayDeltaTime;
        t += TimeUtils.gameplayDeltaTime;
      }
      yield return null;
    }
    group.alpha = alpha; // Prevent overshoot
  }
}
