using UnityEngine;
using UnityEngine.AI;

public class MoveToOptimalRange : MonoBehaviour {
  public float minOptimalRange = 10f;
  public float maxOptimalRange = 40f;
  public float minTimeBeforeReposition = 7f;
  public float maxTimeBeforeReposition = 14f;
  public float suboptimalRangeTimeMultiplier = 3.5f; // When in a bad position, will reduce reposition cooldown by this instead of by 1.

  [HideInInspector]
  public bool repositioning = false;
  [HideInInspector]
  public bool withinRange = false;

  private NavMeshAgent agent;
  private float minOptimalRangeSq;
  private float maxOptimalRangeSq;
  private float timeTillReposition;

  private static float sqDistanceToTarget;
  private static Quaternion randomDirection;

	void Start () {
    agent = GetComponent<NavMeshAgent>();
    minOptimalRangeSq = minOptimalRange * minOptimalRange;
    maxOptimalRangeSq = maxOptimalRange * maxOptimalRange;
    reposition();
  }
	
	void Update () {
    if (Time.timeScale == 0) return; // Game is paused; do nothing

    // Check if we're still moving
    if (repositioning) repositioning = !agent.atDestination();

    withinRange = (transform.position - Player.SINGLETON.transform.position).sqrMagnitude <= maxOptimalRangeSq;

    if (inOptimalRange()) timeTillReposition -= TimeUtils.gameplayDeltaTime;
    else timeTillReposition -= TimeUtils.gameplayDeltaTime * suboptimalRangeTimeMultiplier;

    if (timeTillReposition <= 0) reposition();
	}

  private bool inOptimalRange() {
    sqDistanceToTarget = (transform.position - Player.SINGLETON.transform.position).sqrMagnitude;
    return sqDistanceToTarget >= minOptimalRangeSq && sqDistanceToTarget <= maxOptimalRangeSq;
  }

  private void reposition() {
    repositioning = true;
    timeTillReposition = Random.Range(minTimeBeforeReposition, maxTimeBeforeReposition);
    // Choose a random location at the ideal range from the player

    sqDistanceToTarget = (transform.position - Player.SINGLETON.transform.position).sqrMagnitude;
    if (sqDistanceToTarget < minOptimalRangeSq) moveAwayFromTarget();
    else if (sqDistanceToTarget > maxOptimalRange) moveTowardTarget();
    else moveRandomly();
  }

  private void moveAwayFromTarget() {
    var playerOffset = transform.position - Player.SINGLETON.transform.position;
    var distanceFromSafety = minOptimalRange - playerOffset.magnitude;
    var safePoint = transform.position + (playerOffset.normalized * (distanceFromSafety + 5f)); // Add a buffer of 5m, just to be sure
    var randomDestination = randomPositionAround(safePoint);
    agent.SetDestination(randomDestination);
  }

  private void moveTowardTarget() {
    var playerOffset = transform.position - Player.SINGLETON.transform.position;
    var distanceFromOptimalRange = playerOffset.magnitude - maxOptimalRange;
    var safePoint = transform.position - (playerOffset.normalized * (distanceFromOptimalRange + 5f)); // Add a buffer of 5m, just to be sure
    var randomDestination = randomPositionAround(safePoint);
    if (agent.isActiveAndEnabled) agent.SetDestination(randomDestination); // TODO this can emit error: "SetDestination can only be called on an active agent"
  }

  private void moveRandomly() {
    agent.SetDestination(randomPositionAround(transform.position));
  }

  private Vector3 randomPositionAround(Vector3 point) {
    randomDirection = Quaternion.AngleAxis(Random.Range(0f, 360f), transform.up);
    return point + (randomDirection * Vector3.forward * Random.Range(-5f, 5f));
  }
}
