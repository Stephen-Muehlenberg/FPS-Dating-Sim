using UnityEngine;

public class FacePlayer : MonoBehaviour {
  private Transform player;

  void Start () {
    player = Player.SINGLETON.transform;
	}
	
	void Update () {
    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
	}
}
