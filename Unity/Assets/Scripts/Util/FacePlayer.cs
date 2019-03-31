using UnityEngine;

public class FacePlayer : MonoBehaviour {
  private Transform player;

  void Start () {
    player = Player.SINGLETON.transform;
	}
	
	void Update () {
    transform.LookAt(player);
	}
}
