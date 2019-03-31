using UnityEngine;

public class Player : MonoBehaviour {
  public static Player SINGLETON;

  new public Camera camera;

	void Awake () {
    SINGLETON = this;
	}
}
