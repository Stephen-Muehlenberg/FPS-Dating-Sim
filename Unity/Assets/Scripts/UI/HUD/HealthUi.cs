using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthUi : MonoBehaviour {
  public Health playerHealth;
  public Image healthRemaining;
  public Image healthBackground;
  public Color backgroundFlashColour;
  public float healthFlashDuration = 0.5f;

  private int lastHealth;
  private Color backgroundNormalColour;

  void Start() {
    playerHealth = Player.SINGLETON.GetComponent<Health>();
    lastHealth = playerHealth.remaining;
    backgroundNormalColour = healthBackground.color;
  }

	void Update () {
    if (playerHealth.remaining != lastHealth) {
      healthRemaining.fillAmount = (playerHealth.remaining / 1000f);
      lastHealth = playerHealth.remaining;
      StopAllCoroutines();
      StartCoroutine(flashHealth());
    }
	}

  private IEnumerator flashHealth() {
    healthBackground.color = backgroundFlashColour;

    float fadeDuration = 0f;
    while (fadeDuration < healthFlashDuration) {
      yield return this;
      fadeDuration += Time.deltaTime;
      healthBackground.color = Color.Lerp(backgroundFlashColour, backgroundNormalColour, fadeDuration / healthFlashDuration);
    }

    healthBackground.color = backgroundNormalColour;
  }
}
