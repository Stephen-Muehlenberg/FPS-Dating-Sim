using UnityEngine;
using UnityEngine.AI;

public class IdleBehaviour : MonoBehaviour {
  [Tooltip ("Minimum time to wait before moving again.")]
  public float minWaitTime = 0f;
  [Tooltip ("Maximum time to wait before moving again.")]
  public float maxWaitTime = 5f;

  [Tooltip ("Minimum time spent moving after waiting.")]
  public float minWanderTime = 1f;
  [Tooltip ("Maximum time spent moving after waiting.")]
  public float maxWanderTime = 2.5f;

  [Tooltip ("Monsters sharing the same flockId will stay near each other. An ID of 0 will ignore others.")]
  public int flockId = 0;
  public float flockRangeSq = 225f;

  private float wanderTimeRemaining;
  private float waitTimeRemaining;
  private NavMeshAgent agent;

  private static float distanceSq;

  void Start() {
    agent = GetComponent<NavMeshAgent>();
    waitTimeRemaining = Random.Range(0f, maxWaitTime);
  }

	void Update () {
		if (waitTimeRemaining > 0) {
      waitTimeRemaining -= TimeUtils.gameplayDeltaTime;
      if (waitTimeRemaining <= 0) {
        startNewWander();
        waitTimeRemaining = 0f;
        wanderTimeRemaining = Random.Range(minWanderTime, maxWanderTime);
      }
    }
    else {
      wanderTimeRemaining -= TimeUtils.gameplayDeltaTime;
      if (wanderTimeRemaining <= 0) {
        agent.ResetPath();
        wanderTimeRemaining = 0f;
        waitTimeRemaining = Random.Range(minWaitTime, maxWaitTime);
      }
    }
	}

  private void startNewWander() {
    var randomDirection = Random.insideUnitCircle.normalized * Random.Range(10f, 30f);
    var newDestination = transform.position + new Vector3(randomDirection.x, 0, randomDirection.y);

    // TODO flocking
   /* if (flockId > 0) {
      Vector3 totalPos = new Vector3();
      int flockSize = 0;
      foreach(Monster monster in MonstersController.monsters) {
        if (monster.GetComponent<IdleBehaviour>().flockId != flockId) continue;
        distanceSq = (transform.position - monster.transform.position).sqrMagnitude;
        if (distanceSq > flockRangeSq) continue;
        totalPos += monster.transform.position;
        flockSize++;
      }
      totalPos /= flockSize;
      newDestination += totalPos;
    }*/

    agent.SetDestination(newDestination);
  }
}
