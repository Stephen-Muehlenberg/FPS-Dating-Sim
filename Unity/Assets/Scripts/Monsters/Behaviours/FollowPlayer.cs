using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour {
  [HideInInspector]
  NavMeshAgent navAgent;
  [HideInInspector]
  Stagger stagger;

	void Start () {
    navAgent = GetComponent<NavMeshAgent>();
    stagger = GetComponent<Stagger>();
  }
	
	void Update () {
    if (TimeUtils.gameplayPaused) return;
    if (stagger != null && stagger.isStaggered) return;

    navAgent.SetDestination(Player.SINGLETON.transform.position);
  }

  void StartStagger() {
    if (navAgent != null && navAgent.isOnNavMesh) navAgent.isStopped = true;
  }

  void EndStagger() {
    if (navAgent != null && navAgent.isOnNavMesh) navAgent.isStopped = false;
  }
}
