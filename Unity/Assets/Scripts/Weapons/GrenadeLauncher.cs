using UnityEngine;

public class GrenadeLauncher : WeaponController {
  private const float SCATTER_RADIANS = 7f * Mathf.Deg2Rad;
  private const float INITIAL_VELOCITY = 30f;

  public GameObject grenadePrefab;
  public Transform grenadeSpawnPoint;
  public AudioSource audioSource;

  void Awake() {
    weapon = Weapons.GRENADE_LAUNCHER;
    weapon.setController(this);
    fatigue = new GrenadeLauncherFatigue_Basic();
  }
  
  protected override void firePrimary() { fire(false); }
  protected override void fireSecondary() { fire(true); }

  void fire(bool bouncyGrenade) {
    fatigue.firePrimary();
    cooldownRemaining = fatigue.primaryCooldown; // Primary and secondary have the same fatigue effects

    var grenade = Instantiate(grenadePrefab, grenadeSpawnPoint.position, grenadeSpawnPoint.rotation);
    grenade.GetComponent<Grenade>().bouncy = bouncyGrenade;
    grenade.GetComponent<Rigidbody>().velocity = grenadeSpawnPoint.randomScatter(SCATTER_RADIANS) * INITIAL_VELOCITY;

    audioSource.Play();
  }
}
