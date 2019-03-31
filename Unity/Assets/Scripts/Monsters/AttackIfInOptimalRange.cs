using UnityEngine;

public class AttackIfInOptimalRange : MonoBehaviour {
  public GameObject fireballPrefab;
  public Transform spawnPoint;

  public float MIN_DELAY_BETWEEN_SHOTS = 1f;
  public float MAX_DELAY_BETWEEN_SHOTS = 2f;

  private MoveToOptimalRange moveScript;
  private float timeTillNextAttack = 0f;

  private static float ROTATION_SPEED = 3f;

	void Start () {
    moveScript = GetComponent<MoveToOptimalRange>();
	}
	
	void Update () {
    if (Time.timeScale == 0) return; // Game is paused; do nothing

    if (moveScript.repositioning || !moveScript.withinRange) {
      timeTillNextAttack = 0.5f; // Pause briefly after repositioning to allow monster to rotate to face player
      return;
    }

    // If not moving, rotate to face player
    var lookRotation = Quaternion.LookRotation(Player.SINGLETON.transform.position - transform.position);
    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * ROTATION_SPEED);

    timeTillNextAttack -= Time.deltaTime;

    if (timeTillNextAttack <= 0) {
      // TODO make monster fire (mostly) directly forward, not magically fire towards the player
      // TODO make fireball not perfectly accurate
      var targetPoint = Player.SINGLETON.camera.transform.position + (Vector3.down * 0.2f); // Aim slightly below camera to make the fireball a little easier to read
      Instantiate(fireballPrefab, spawnPoint.position, Quaternion.LookRotation(targetPoint - spawnPoint.position, Vector3.up));
      timeTillNextAttack = Random.Range(MIN_DELAY_BETWEEN_SHOTS, MAX_DELAY_BETWEEN_SHOTS);
    }
  }
}
