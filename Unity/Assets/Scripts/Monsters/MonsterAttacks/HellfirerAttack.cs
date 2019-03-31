using UnityEngine;

public class HellfirerAttack : MonoBehaviour {
  private static float FLAMETHROWER_RANGE_SQ = 144;
  private static float ROTATION_SPEED = 5f;
  private static float FLAMETHROWER_HITBOX_RATE = 0.25f; // Time between hitbox emissions
  private static float MORTAR_COOLDOWN_MIN = 6.5f, MORTAR_COOLDOWN_MAX = 9;

  public GameObject flameHitboxPrefab;
  public GameObject mortarPrefab;
  public ParticleSystem flameParticles;
  public Transform mortarSpawnPoint;

  private bool previouslyUsingFlamethrower = false;
  private float timeTillNextFlamethrowerHitbox = 0;
  private float timeTillNextMortar;

  public void Start() {
    timeTillNextMortar = Random.Range(MORTAR_COOLDOWN_MIN, MORTAR_COOLDOWN_MAX);
  }

  void Update () {
    if (Time.timeScale == 0) return; // Game is paused; do nothing

    if ((Player.SINGLETON.transform.position - transform.position).sqrMagnitude < FLAMETHROWER_RANGE_SQ) {
      useFlamethrower();
    }
    else useMortar();
	}

  private void useFlamethrower() {
    if (!previouslyUsingFlamethrower) {
      flameParticles.Play();
      previouslyUsingFlamethrower = true;
    }

    transform.rotation = Quaternion.Slerp(
      transform.rotation,
      Quaternion.LookRotation(Player.SINGLETON.transform.position - transform.position, Vector3.up),
      Time.deltaTime * ROTATION_SPEED);

    timeTillNextFlamethrowerHitbox -= Time.deltaTime;
    if (timeTillNextFlamethrowerHitbox <= 0) {
      Instantiate(flameHitboxPrefab, transform.position + transform.forward + Vector3.up * 1.5f, transform.rotation);
      timeTillNextFlamethrowerHitbox = FLAMETHROWER_HITBOX_RATE;
    }
  }

  private void useMortar() {
    if (previouslyUsingFlamethrower) {
      flameParticles.Stop();
      previouslyUsingFlamethrower = false;
      if (timeTillNextMortar < 1.5f) timeTillNextMortar = 1.5f;
    }

    timeTillNextMortar -= Time.deltaTime;
    if (timeTillNextMortar <= 0) {
      var vectorToTarget = Player.SINGLETON.camera.transform.position - mortarSpawnPoint.position;
      var projectileDirection = Quaternion.LookRotation(vectorToTarget, Vector3.up);
      var projectile = Instantiate(mortarPrefab, mortarSpawnPoint.position, projectileDirection);
      projectile.GetComponent<HellfireMortar>().setDistanceToTarget(vectorToTarget.magnitude);

      timeTillNextMortar = Random.Range(MORTAR_COOLDOWN_MIN, MORTAR_COOLDOWN_MAX);
    }
  }
}
