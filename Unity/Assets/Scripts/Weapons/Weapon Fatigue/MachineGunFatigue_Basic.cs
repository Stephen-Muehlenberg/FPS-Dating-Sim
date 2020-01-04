using System;
using UnityEngine;

public class MachineGunFatigue_Basic : WeaponFatigue {
  private const float MAX_FATIGUE = 100; // If fatigue == MAX_FATIGUE, weapon becomes exhausted
  private const float MIN_FATIGUE_COOLDOWN = 80; // Once exhausted, can't fire until fatigue drops to MIN_FATIGUE_COOLDOWN

  private const float FATIGUE_PER_BURST_SHOT = 0.35f;
  private const float FATIGUE_PER_AUTO_SHOT = 0.8f;
  private const float MIN_FATIGUE_BEFORE_PENALTY = 7; // No bonus delay applied while fatigue less than this

  private const float MIN_REST_UNTIL_RECOVERY_STARTS = 1.5f;
  private const float FATIGUE_RECOVERED_PER_SECOND = 1.666f;

  private const float BASE_DELAY = 0.08f;
  private const float DELAY_PER_FATIGUE = 0.001f;

  private float fatigue = 0;
  private float timeSinceLastShot = 0;
  private bool exhausted = false; // Can't fire while exhausted

  override public bool canFire { get { return !exhausted; } }

  override public void firePrimary() {
    fatigue += FATIGUE_PER_BURST_SHOT;
    if (fatigue > MAX_FATIGUE) exhausted = true;
    timeSinceLastShot = 0;
  }

  override public void fireSecondary() {
    fatigue += FATIGUE_PER_AUTO_SHOT;
    if (fatigue > MAX_FATIGUE) exhausted = true;
    timeSinceLastShot = 0;
  }

  override public float primaryCooldown { get {
      return BASE_DELAY + (fatigue > MIN_FATIGUE_BEFORE_PENALTY
        ? (DELAY_PER_FATIGUE * (fatigue - MIN_FATIGUE_BEFORE_PENALTY))
        : 0);
  } }

  override public float secondaryCooldown { get { return primaryCooldown;  } }

  override public void update() {
    if (timeSinceLastShot < MIN_REST_UNTIL_RECOVERY_STARTS) {
      timeSinceLastShot += TimeUtils.gameplayDeltaTime;
      return;
    }

    fatigue -= FATIGUE_RECOVERED_PER_SECOND * TimeUtils.gameplayDeltaTime;
    if (exhausted && fatigue <= MIN_FATIGUE_COOLDOWN) exhausted = false;
    if (fatigue <= 0) fatigue = 0;
  }

  override public float getAsFraction() { return fatigue / MAX_FATIGUE; }
}
