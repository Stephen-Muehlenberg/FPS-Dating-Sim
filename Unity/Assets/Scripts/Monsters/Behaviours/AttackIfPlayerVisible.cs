using UnityEngine;

public class AttackIfPlayerVisible : MonoBehaviour {
  public GameObject projectilePrefab;
  public Transform projectileSpawnPoint;

  public float attackCooldownMin = 7f;
  public float attackCooldownMax = 10f;

  private float timeTillNextAttack;

	void Start () {
    timeTillNextAttack = Random.Range(0, attackCooldownMax);
	}
	
	void Update () {
    if (Time.timeScale == 0) return; // Game is paused; do nothing

    timeTillNextAttack -= TimeUtils.gameplayDeltaTime;
    if (timeTillNextAttack <= 0) {
      attack();
      timeTillNextAttack += Random.Range(attackCooldownMin, attackCooldownMax);
    }
  }

  private void attack() {
    var vectorToTarget = Player.SINGLETON.camera.transform.position - projectileSpawnPoint.position;
    var projectileDirection = Quaternion.LookRotation(vectorToTarget, Vector3.up);
    var projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileDirection);
    projectile.GetComponent<HellfireMortar>().setDistanceToTarget(vectorToTarget.magnitude);
  }
}
