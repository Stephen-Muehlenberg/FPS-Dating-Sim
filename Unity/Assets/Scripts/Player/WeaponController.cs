using UnityEngine;

abstract public class WeaponController : MonoBehaviour {
  public Weapon weapon;
  public WeaponFatigue fatigue;
  protected float cooldown, cooldownRemaining;

  // Cached for re-use across all weapons
  private static float cooldownFraction; // How complete is the cooldown? 0 = not at all, 1 = complete.
  protected static bool rayHit;
  protected static RaycastHit rayHitInfo;
  // Use rayLayerMask = ~LayerMask.GetMask("Weapons", "Invisible Wall", "Player") to ignore the specified layers.
  // Can't use GetMask() in initialization, but the result is a constant unless the layers themselves change, so hardcoded is fine.
  protected static int rayLayerMask = -1793;

  public void setEquipped(bool equipped, bool playEffects) {
    if (equipped) {
      setVisible(true);
      if (playEffects) transform.GetChild(0).gameObject.GetComponentInChildren<ParticleSystem>().Play();
      WeaponCooldownUi.setWeapon(this.weapon);
      WeaponCooldownUi.clearProgress();
      if (cooldownRemaining > 0) WeaponCooldownUi.showProgress(1 - (cooldownRemaining / cooldown));
      onEquip();
    } else {
      onUnequip();
      setVisible(false);
    }
  }

  public void setVisible(bool visible) {
    transform.GetChild(0).gameObject.SetActive(visible);
  }

  virtual protected void onEquip() {}
  virtual protected void onUnequip() {}

  public void Update() {
    if (TimeUtils.gameplayPaused) return;

    fatigue.update();

    if (canFire()) {
      if (Input.GetButton("PrimaryFire")) {
        firePrimary();
        fatigue.firePrimary();
        cooldown = getPrimaryCooldown();
        cooldownRemaining = cooldown;
        WeaponCooldownUi.startCooldown();
      }
      else if (hasSecondaryFire() && Input.GetButton("SecondaryFire")) {
        fireSecondary();
        fatigue.fireSecondary();
        cooldown = getSecondaryCooldown();
        cooldownRemaining = cooldown;
        WeaponCooldownUi.startCooldown();
      }
    }
    else {
      cooldownFraction = updateCooldown();
      if (weapon.equipped) {
        if (cooldownFraction == 1) WeaponCooldownUi.showReady();
        else WeaponCooldownUi.showProgress(cooldownFraction);
      }
    }
  }

  private bool canFire() {
    return weapon.equipped && fatigue.canFire && cooldownRemaining <= 0;
  }

  virtual protected bool hasSecondaryFire() { return true; }

  /**
   * Reduce cooldown by TimeUtils.gameplayDeltaTime.
   * @return the cooldown's progress, as a fraction between 0 and 1.
   */
  private float updateCooldown() {
    if (!fatigue.canFire) return 0;
    cooldownRemaining -= TimeUtils.gameplayDeltaTime;
    if (cooldownRemaining <= 0) return 1;
    return 1f - (cooldownRemaining / cooldown);
  }

  abstract protected void firePrimary();
  abstract protected void fireSecondary();

  virtual protected float getPrimaryCooldown() { return fatigue.primaryCooldown; }
  virtual protected float getSecondaryCooldown() { return fatigue.secondaryCooldown; }
}

/*
 * EVENTS
 * the FIRE() functions should return some sort of summary of the result of the shot
 *  - what about delayed shots, like the grenade? shouldn't assume this event will be emitted immediately
 *  
 * hit/kill/miss primary primary
 * hit/kill/miss secondary
 * on become exhausted
 * fire primary while exhausted
 * fire secondary while exhausted
 * on end exhaustion
 * select weapon
 * cooldown completed
 */
