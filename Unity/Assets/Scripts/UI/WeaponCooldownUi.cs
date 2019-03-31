using UnityEngine;
using UnityEngine.UI;

public class WeaponCooldownUi : MonoBehaviour {
  private const float FLASH_SPEED_MULTIPLIER = 4;
  private static Color CROSSHAIR_READY_COLOUR = new Color(1, 1, 1, 0.5f);
  private static Color CROSSHAIR_CHARGING_COLOUR = new Color(0.9f, 0.9f, 0.9f, 0.9f);

  private static WeaponCooldownUi SINGLETON;

  public Image leftBar;
  public Image rightBar;
  public Image topBar;
  public Image bottomBar;

  private Color flashColour;
  private bool flashInProgress;
  private float flashTime;

  private const float FLASH_FADE_IN_TIME = 0.2f;
  private const float FLASH_HOLD_TIME = 0.1f;
  private const float FLASH_FADE_OUT_TIME = 0.4f;

  public static void setWeapon(Weapon weapon) {
    if (weapon == Weapons.SHOTGUN) SINGLETON.flashColour = new Color(1, 0.25f, 0.25f, 1);
    else if (weapon == Weapons.MACHINE_GUN) SINGLETON.flashColour = new Color(0.25f, 1, 0.25f, 1);
    else if (weapon == Weapons.SNIPER_RIFLE) SINGLETON.flashColour = new Color(0.1f, 0.9f, 1, 1);
    else if (weapon == Weapons.GRENADE_LAUNCHER) SINGLETON.flashColour = new Color(1, 0.5f, 0, 1);
    else throw new UnityException("Unknown weapon " + weapon.name);
  }

  public static void startCooldown() {
    SINGLETON.flashInProgress = false;
    SINGLETON.leftBar.color = CROSSHAIR_CHARGING_COLOUR;
    SINGLETON.rightBar.color = CROSSHAIR_CHARGING_COLOUR;
    SINGLETON.topBar.color = CROSSHAIR_CHARGING_COLOUR;
    SINGLETON.bottomBar.color = CROSSHAIR_CHARGING_COLOUR;
    SINGLETON.leftBar.fillAmount = 0;
    SINGLETON.rightBar.fillAmount = 0;
    SINGLETON.topBar.fillAmount = 0;
    SINGLETON.bottomBar.fillAmount = 0;
  }

  public static void showProgress(float progress) {
    SINGLETON.leftBar.fillAmount = progress;
    SINGLETON.rightBar.fillAmount = progress;
    SINGLETON.topBar.fillAmount = progress;
    SINGLETON.bottomBar.fillAmount = progress;
  }

  public static void showReady() {
    SINGLETON.flashInProgress = true;
    SINGLETON.flashTime = 0;
  }
  
  public static void clearProgress() {
    SINGLETON.flashInProgress = false;
    SINGLETON.leftBar.color = CROSSHAIR_READY_COLOUR;
    SINGLETON.rightBar.color = CROSSHAIR_READY_COLOUR;
    SINGLETON.topBar.color = CROSSHAIR_READY_COLOUR;
    SINGLETON.bottomBar.color = CROSSHAIR_READY_COLOUR;
    SINGLETON.leftBar.fillAmount = 1;
    SINGLETON.rightBar.fillAmount = 1;
    SINGLETON.topBar.fillAmount = 1;
    SINGLETON.bottomBar.fillAmount = 1;
  }

  public void Awake() {
    SINGLETON = this;
  }

  public void Update() {
    if (TimeUtils.gameplayPaused) return;

    if (flashInProgress) {
      flashTime += Time.deltaTime;

      if (flashTime <= FLASH_FADE_IN_TIME) {
        leftBar.color = Color.Lerp(CROSSHAIR_CHARGING_COLOUR, flashColour, flashTime / FLASH_FADE_IN_TIME);
        rightBar.color = leftBar.color;
        topBar.color = leftBar.color;
        bottomBar.color = leftBar.color;
      }
      else if (flashTime > FLASH_FADE_IN_TIME + FLASH_HOLD_TIME) {
        if (flashTime >= FLASH_FADE_IN_TIME + FLASH_HOLD_TIME + FLASH_FADE_OUT_TIME) {
          flashInProgress = false;
          leftBar.color = CROSSHAIR_READY_COLOUR;
          rightBar.color = CROSSHAIR_READY_COLOUR;
          topBar.color = CROSSHAIR_READY_COLOUR;
          bottomBar.color = CROSSHAIR_READY_COLOUR;
        }
        else {
          leftBar.color = Color.Lerp(flashColour, CROSSHAIR_READY_COLOUR, (flashTime - FLASH_FADE_IN_TIME - FLASH_HOLD_TIME) / FLASH_FADE_OUT_TIME);
          rightBar.color = leftBar.color;
          topBar.color = leftBar.color;
          bottomBar.color = leftBar.color;
        }
      }
    }
  }
}
