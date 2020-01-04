using UnityEngine;

public class Stagger : MonoBehaviour
{
  public bool isStaggered = false;
  public float stagger = 0;
  public float threshold;
  public float decay; // Per second
  public float negativeStaggerOnTrigger;
  public float duration = 0.5f;
  public float remaining;
  public float shotgunMultiplier = 1;
  public float machineGunMultiplier = 1;
  public float sniperRifleMultiplier = 1;
  public float grenadeLauncherMultiplier = 1;

  public void takeDamage(Damage damage) {
    stagger += damage.amount * (
      damage.source == Weapons.SHOTGUN ? shotgunMultiplier :
      damage.source == Weapons.MACHINE_GUN ? machineGunMultiplier :
      damage.source == Weapons.SNIPER_RIFLE ? sniperRifleMultiplier :
      damage.source == Weapons.GRENADE_LAUNCHER ? grenadeLauncherMultiplier : 1);

    if (stagger > threshold) {
      stagger -= threshold + negativeStaggerOnTrigger;
      remaining = duration;
      isStaggered = true;
      SendMessage("StartStagger", SendMessageOptions.DontRequireReceiver);
    }
  }

  void Update() {
    if (remaining > 0) {
      remaining -= TimeUtils.gameplayDeltaTime;
      isStaggered = false;
      if (remaining <= 0) SendMessage("EndStagger", SendMessageOptions.DontRequireReceiver);
    }

    if (stagger > 0) {
      stagger -= decay * TimeUtils.gameplayDeltaTime;
      if (stagger < 0) stagger = 0;
    }
    else if (stagger < 0) {
      stagger += decay * TimeUtils.gameplayDeltaTime;
      if (stagger > 0) stagger = 0;
    }
  }

  /*
    * light stagger = interrupt, about 0.5 second stun
    * heavy stagger = interrupt, about 1.25 second stun
    * different weapons inflict different amounts of stagger against different targets
    * 
    * different thresholds and decay speeds
    * light threshold
    * 
    * staggerDecayPerSecond = 180
    * lightStaggerThreshold = 
    * 
    * lightStaggerThreshold
    * lightStaggerDecay 
    * 
    * WEAPON      DPS @ 30% Fatigue    Burst Damage (over 0.333s)
    * SHOTGUN     330                  1080
    * MACHINE GUN 864                  288
    * SNIPER      228                  450
    * GRENADE     549                  485
    * 
    * torch = heavy stagger by grenade explosion
    * squig = heavy stagger by shotgun in the back
    * hellfirer = heaver stagger by continuous machinegun damage
    * infernal = heavy stagger by sniper round
    * 
    * after being staggered, they have a duration of unstaggerable
    * this state decays over time and with damage - it'll wear out after like 6 seconds, but if you're constantly
    *   pounding it then it will wear off after like 3
    * 
    * 
    * stagger decays exponentially quickly. the first 0.5 seconds are almost no decay, then a bit more, then lots more.
    * by ~1.5s, stagger completely decays
    * update:
    *   timeSinceLastDamage += TimeUtils.gameplayDeltaTime
    *   stagger -= timeSinceLastDamage
    *     
    * 
    * one stagger meter
    * stagger takes in damage, but its only a fraction of the percent. like 30% of damage?
    * 
    */
}
