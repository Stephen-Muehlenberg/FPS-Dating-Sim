using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour {
  [HideInInspector]
  NavMeshAgent navAgent;

	void Start () {
    navAgent = GetComponent<NavMeshAgent>();
  }
	
	void Update () {
    if (TimeUtils.gameplayPaused) return;

    navAgent.SetDestination(Player.SINGLETON.transform.position);
  }
}
