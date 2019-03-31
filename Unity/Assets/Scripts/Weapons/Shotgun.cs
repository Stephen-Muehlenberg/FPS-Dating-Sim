﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : WeaponController {
  public Transform bulletOrigin;
  public AudioSource audioSource;
  public LineRenderer[] lineRenderers;

  public AudioClip singleShotSound;
  public AudioClip doubleShotSound;

  private const float RANGE = 200f;
  private const float SCATTER_RADIANS_SINGLE = 4.5f * Mathf.Deg2Rad, SCATTER_RADIANS_DOUBLE = 6f * Mathf.Deg2Rad;
  private const int PELLET_COUNT_SINGLE = 8, PELLET_COUNT_DOUBLE = 16;
  private const int PELLET_DAMAGE_MIN = 60, PELLET_DAMAGE_MAX = 75;
  private const float DELAY_AFTER_SINGLE = 0.4f, DELAY_AFTER_DOUBLE = 1.2f;
  private const float BULLET_TRAIL_LIFETIME = 0.03f;
  private const float BULLET_TRAIL_WIDTH_SINGLE = 0.0125f, BULLET_TRAIL_WIDTH_DOUBLE = 0.035f;
  private const float PELLET_FORCE = 350f;

  public interface ShotgunListener {
    void onFire(int shotsFired);
  }

  private List<ShotgunListener> listeners = new List<ShotgunListener>();
  public void addListener(ShotgunListener listener) { listeners.Add(listener); }
  public void removeListener(ShotgunListener listener) { listeners.Remove(listener); }

  void Awake() {
    weapon = Weapons.SHOTGUN;
    weapon.setController(this);
    fatigue = new ShotgunFatigue_Basic();
  }

  override protected void firePrimary() {
    fire(PELLET_COUNT_SINGLE, SCATTER_RADIANS_SINGLE, BULLET_TRAIL_WIDTH_SINGLE, singleShotSound, 0.6f);
  }

  override protected void fireSecondary() {
    fire(PELLET_COUNT_DOUBLE, SCATTER_RADIANS_DOUBLE, BULLET_TRAIL_WIDTH_DOUBLE, doubleShotSound, 0.8f);
  }

  private void fire(int pelletCount, float scatterRadians, float trailWidth, AudioClip sound, float volume) {
    for (int i = 0; i < pelletCount; i++) {
      var scatteredDirection = bulletOrigin.randomScatter(scatterRadians);

      rayHit = Physics.Raycast(bulletOrigin.position, scatteredDirection, out rayHitInfo, RANGE, rayLayerMask);
      var endpoint = rayHit ? rayHitInfo.point : bulletOrigin.position + scatteredDirection * RANGE;

      lineRenderers[i].SetPosition(0, bulletOrigin.position);
      lineRenderers[i].SetPosition(1, endpoint);
      lineRenderers[i].widthMultiplier = trailWidth;

      if (rayHit) {
        // Damage
        float damageBase = Random.Range(PELLET_DAMAGE_MIN, PELLET_DAMAGE_MAX);
        float damageFalloffMultiplier = Mathf.Pow(5f, (rayHitInfo.distance / -20f));
        int damageMultiplied = (int) (damageBase * damageFalloffMultiplier);
        var damage = new Damage(damageMultiplied, transform.position, rayHitInfo.point, rayHitInfo.collider);
        // TODO send the damage as a single lump sum per target, rather than discrete hits
        rayHitInfo.collider.SendMessageUpwards("takeDamage", damage, SendMessageOptions.DontRequireReceiver);

        // Force
        var rigidBody = rayHitInfo.collider.GetComponentInParent<Rigidbody>();
        if (rigidBody != null) rigidBody.AddForceAtPosition(scatteredDirection * PELLET_FORCE, rayHitInfo.point);
      }
    }

    audioSource.PlayOneShot(sound, volume);

    StartCoroutine(showBulletTrails(pelletCount));

    // TODO upgrade this so we emit more detailed information: how many targets did we hit, who did we hit, how much damage was dealt, how many targets were killed?
    foreach (ShotgunListener listener in listeners) { listener.onFire(1); }
  }

  private IEnumerator showBulletTrails(int trailCount) {
    for (int i = 0; i < trailCount; i++) {
      lineRenderers[i].enabled = true;
    }
    
    yield return new WaitForSeconds(BULLET_TRAIL_LIFETIME);

    for (int i = 0; i < trailCount; i++) {
      lineRenderers[i].enabled = false;
    }
  }
}