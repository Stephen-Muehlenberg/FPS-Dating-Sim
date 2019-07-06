using UnityEngine;

public class GrenadeLauncherFatigue_Basic : WeaponFatigue {
  private const float MAX_FATIGUE = 16; // If fatigue == MAX_FATIGUE, weapon becomes exhausted
  private const float MIN_FATIGUE_COOLDOWN = 12.8f; // Once exhausted, can't fire until fatigue drops to MIN_FATIGUE_COOLDOWN

  private const float FATIGUE_PER_SHOT = 1f;
  private const float MIN_FATIGUE_BEFORE_PENALTY = 5; // No bonus delay applied while fatigue less than this

  private const float MIN_REST_UNTIL_RECOVERY_STARTS = 2;
  private const float FATIGUE_RECOVERED_PER_SECOND = 0.4f;

  private const float BASE_DELAY = 0.5f;
  private const float DELAY_PER_FATIGUE = 0.08f;

  private float fatigue = 0;
  private float timeSinceLastShot = 0;
  private bool exhausted = false; // Can't fire while exhausted

  override public bool canFire { get { return !exhausted; } }

  override public void firePrimary() {
    fatigue += FATIGUE_PER_SHOT;
    if (fatigue > MAX_FATIGUE) exhausted = true;
    timeSinceLastShot = 0;
  }

  override public void fireSecondary() { firePrimary(); }

  override public float primaryCooldown {
    get {
      return BASE_DELAY + (fatigue > MIN_FATIGUE_BEFORE_PENALTY
        ? (DELAY_PER_FATIGUE * (fatigue - MIN_FATIGUE_BEFORE_PENALTY))
        : 0);
    }
  }

  override public float secondaryCooldown { get { return primaryCooldown; } }

  override public void update() {
    if (timeSinceLastShot < MIN_REST_UNTIL_RECOVERY_STARTS) {
      timeSinceLastShot += Time.deltaTime;
      return;
    }
    
    fatigue -= FATIGUE_RECOVERED_PER_SECOND * Time.deltaTime;
    if (exhausted && fatigue <= MIN_FATIGUE_COOLDOWN) exhausted = false;
    if (fatigue <= 0) fatigue = 0;
  }

  override public float getAsFraction() { return fatigue / MAX_FATIGUE; }
}
