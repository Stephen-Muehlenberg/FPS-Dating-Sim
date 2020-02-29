using System.Collections;
using UnityEngine;

public class SniperRifle : WeaponController {
  protected override int getId() { return 2; }

  public Transform bulletOrigin;
  public AudioSource audioSource;
  public LineRenderer lineRenderer;
  public GameObject hitParticlePrefab;

  private const float RANGE = 1000f;
  private const int DAMAGE_MIN = 400, DAMAGE_MAX = 500;
  private const float SCATTER_RADIANS_SCOPRED = 0, SCATTER_RADIANS_UNSCOPED = 12.5f * Mathf.Deg2Rad;
  private const float BULLET_TRAIL_LIFETIME = 0.04f;
  private const float NORMAL_CAMERA_FOV = 75f; // TODO maybe load this dynamically rather than hardcoding it here
  private const float ZOOM_CAMERA_FOV = 40f;
  private const float BULLETTIME_DURATION = 1f;
  private const float BULLETTIME_TIMESCALE = 0.2f;
  private const float BULLET_FORCE = 5000f;

  public float shotDelayRemaining = 0f;
  private IEnumerator bullettimeRoutine = null;

  // Cached for re-use.
  private Vector3 shotScatteredDirection;
  private Vector3 shotEndpoint;

  void Awake() {
    weapon = Weapons.SNIPER_RIFLE;
    weapon.setController(this);
    fatigue = new SniperRifleFatigue_Basic();
  }

  void OnDestroy() {
    stopZoom();
  }
  
  override protected void onUnequip() {
    stopZoom();
  }

  protected override void firePrimary() {
    Fire(Input.GetButton("SecondaryFire") ? SCATTER_RADIANS_SCOPRED : SCATTER_RADIANS_UNSCOPED);
  }

  protected override bool hasSecondaryFire() { return false; }
  protected override void fireSecondary() {}

  new private void Update() {
    base.Update();

    if (TimeUtils.gameplayPaused || !weapon.equipped) return;

    if (Input.GetButtonDown("SecondaryFire")) {
      startZoom();
    }
    if (Input.GetButtonUp("SecondaryFire")) {
      stopZoom();
    }
  }

  private void startZoom() {
    Player.SINGLETON.camera.fieldOfView = ZOOM_CAMERA_FOV;
    bullettimeRoutine = startBulletTime();
    StartCoroutine(bullettimeRoutine);
  }

  private void stopZoom() {
    // Can be called during level tear-down, so gotta check for null Camera and Player
    if (Player.SINGLETON != null) Player.SINGLETON.camera.fieldOfView = NORMAL_CAMERA_FOV;

    // If bullet time hasn't timed out yet, end it manually
    if (bullettimeRoutine != null) {
      StopCoroutine(bullettimeRoutine);
      stopBulletTime();
    }
  }

  private IEnumerator startBulletTime() {
    TimeUtils.startBulletTime(BULLETTIME_TIMESCALE);
    yield return new WaitForSeconds(BULLETTIME_DURATION * BULLETTIME_TIMESCALE);
    stopBulletTime();
  }

  private void stopBulletTime() {
    TimeUtils.stopBulletTime();
    bullettimeRoutine = null;
  }

  private void Fire(float scatterRadians) {
    shotScatteredDirection = bulletOrigin.randomScatter(scatterRadians);

    rayHit = Physics.Raycast(bulletOrigin.position, shotScatteredDirection, out rayHitInfo, RANGE, rayLayerMask);
    shotEndpoint = rayHit ? rayHitInfo.point : bulletOrigin.position + shotScatteredDirection * RANGE;

    lineRenderer.SetPosition(0, bulletOrigin.position);
    lineRenderer.SetPosition(1, shotEndpoint);
    lineRenderer.enabled = true;
    StartCoroutine(showBulletTrail());

    if (rayHit) {
      EnemyHealth enemyHealth = rayHitInfo.collider.GetComponentInParent<EnemyHealth>();
      if (enemyHealth == null) onMiss();
      else onHitEnemy(enemyHealth, shotScatteredDirection);

      // Force
      rayHitInfo.collider.GetComponentInParent<Rigidbody>()
        ?.AddForceAtPosition(shotScatteredDirection * BULLET_FORCE, rayHitInfo.point);
    }
    else onMiss();

    audioSource.Play();
  }

  // TODO see if we can abstract this out to parent class
  private IEnumerator showBulletTrail() {
    lineRenderer.enabled = true;
    yield return new WaitForSeconds(BULLET_TRAIL_LIFETIME);
    lineRenderer.enabled = false;
  }

  private void onHitEnemy(EnemyHealth enemyHealth, Vector3 bulletDirection) {

    // Damage
    enemyHealth.takeDamage(new Damage(
      amount: Random.Range(DAMAGE_MIN, DAMAGE_MAX),
      origin: transform.position,
      hitPoint: rayHitInfo.point,
      source: Weapons.SNIPER_RIFLE,
      hitLocation: rayHitInfo.collider));

    // Particles
    Instantiate(hitParticlePrefab, rayHitInfo.point, Quaternion.Euler(bulletDirection));
  }

  private void onMiss() {
    EventManager.accept(EventManager.Context.WEAPON_MISSED,
                        EventManager.TRIGGERED_BY, Weapons.SNIPER_RIFLE.index);
  }
}
