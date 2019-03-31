using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepPlayerInSight : MonoBehaviour {
  public Transform sightOrigin;

  // If the player isn't visible for a random duration between these two values, start moving towards player
  public float minTimeUntilFollow;
  public float maxTimeUntilFollow;

  public bool playerInSight = false;

	void Start () {
		
	}
	
	void Update () {
		
	}
}
