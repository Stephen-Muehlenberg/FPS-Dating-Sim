using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class NavMeshAgentExtensions {
  public static bool atDestination(this NavMeshAgent agent) {
    if (agent.pathPending) return false;
    if (agent.remainingDistance > agent.stoppingDistance) return false;
    if (agent.hasPath && agent.velocity.sqrMagnitude > 0) return false;
    return true;
  }
}
