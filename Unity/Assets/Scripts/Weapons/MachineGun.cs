using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MachineGun : WeaponController {
  public Transform bulletOrigin;
  public AudioSource audioSource;
  public LineRenderer lineRenderer;
  public GameObject hitParticlePrefab;

  private const float RANGE = 300f;
  private const int DAMAGE_MIN = 85, DAMAGE_MAX = 105;
  private const float SCATTER_RADIANS_FULL_AUTO = 5f * Mathf.Deg2Rad, SCATTER_RADIANS_BURST = 0.5f * Mathf.Deg2Rad;
  private const int BURST_SHOTS = 3;
  private const float BURST_PAUSE = 0.33f;
  private const float BULLET_TRAIL_LIFETIME = 0.03f;
  private const float BULLET_FORCE = 1250f;
  
  private int burstShotsRemaining;
  private float burstCooldown, burstCooldownRemaining;

  public interface MachineGunListener {
    void onFire(int bulletsRemaining);
    void onReload();
  }

  private List<MachineGunListener> listeners = new List<MachineGunListener>();
  public void addListener(MachineGunListener listener) { listeners.Add(listener); }
  public void removeListener(MachineGunListener listener) { listeners.Remove(listener); }

  void Awake() {
    weapon = Weapons.MACHINE_GUN;
    weapon.setController(this);
    fatigue = new MachineGunFatigue_Basic();
  }
  
  protected override void onEquip() {
    burstShotsRemaining = 0;
  }

  protected override void firePrimary() { fire(SCATTER_RADIANS_BURST); }
  protected override void fireSecondary() { fire(SCATTER_RADIANS_FULL_AUTO); }

  private void fire(float scatterAmount) {
    var scatteredDirection = bulletOrigin.randomScatter(scatterAmount);
    rayHit = Physics.Raycast(bulletOrigin.position, scatteredDirection, out rayHitInfo, RANGE, rayLayerMask);
    var endpoint = rayHit ? rayHitInfo.point : bulletOrigin.position + scatteredDirection * RANGE;

    lineRenderer.SetPosition(0, bulletOrigin.position);
    lineRenderer.SetPosition(1, endpoint);
    StartCoroutine(showBulletTrail());

    audioSource.Play();

    if (rayHit) {
      // Damage
      var damage = new Damage(amount: Random.Range(DAMAGE_MIN, DAMAGE_MAX),
                              origin: Player.SINGLETON.transform.position,
                              hitPoint: rayHitInfo.point,
                              source: Weapons.MACHINE_GUN,
                              hitLocation: rayHitInfo.collider);
      rayHitInfo.collider.SendMessageUpwards("takeDamage", damage, SendMessageOptions.DontRequireReceiver);

      // Particles
      if (rayHitInfo.collider.GetComponentInParent<EnemyHealth>() != null) {
        Instantiate(hitParticlePrefab, rayHitInfo.point, Quaternion.Euler(scatteredDirection));
      }

      // Force
      var rigidBody = rayHitInfo.collider.GetComponentInParent<Rigidbody>();
      if (rigidBody != null) rigidBody.AddForceAtPosition(scatteredDirection * BULLET_FORCE, rayHitInfo.point);
    }
  }

  protected override float getPrimaryCooldown() {
    burstShotsRemaining = BURST_SHOTS - 1;
    burstCooldown = fatigue.primaryCooldown;
    burstCooldownRemaining = burstCooldown;

    return burstCooldown * 3 + BURST_PAUSE;
  }

  new private void Update() {
    base.Update();

    if (burstShotsRemaining > 0 && weapon.equipped && !TimeUtils.gameplayPaused) {
      burstCooldownRemaining -= TimeUtils.gameplayDeltaTime;
      if (burstCooldownRemaining <= 0) {
        fire(SCATTER_RADIANS_BURST);
        burstShotsRemaining--;
        burstCooldownRemaining = burstCooldown;
      }
    }
  }

  private IEnumerator showBulletTrail() {
    lineRenderer.enabled = true;
    yield return new WaitForSeconds(BULLET_TRAIL_LIFETIME);
    lineRenderer.enabled = false;
  }
}
