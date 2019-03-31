using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyFlap : MonoBehaviour {
  public GameObject wingLeft;
  public GameObject wingRight;
  
	void Update () {
    wingLeft.transform.rotation = Quaternion.Euler(0, Mathf.PingPong(Time.time * 640f, 80f) - 40f, 0);
    wingRight.transform.rotation = Quaternion.Euler(0, -wingLeft.transform.rotation.eulerAngles.y, 0);
	}
}
