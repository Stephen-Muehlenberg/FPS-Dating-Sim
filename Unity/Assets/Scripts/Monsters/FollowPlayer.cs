using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour {
  [HideInInspector]
  NavMeshAgent navAgent;

	void Start () {
    navAgent = GetComponent<NavMeshAgent>();
  }
	
	void Update () {
    if (Time.timeScale == 0) return; // Game is paused; do nothing

    navAgent.SetDestination(Player.SINGLETON.transform.position);
  }
}
