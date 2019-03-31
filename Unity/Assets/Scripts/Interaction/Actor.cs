using UnityEngine;

public class Actor : MonoBehaviour {
  public Actor setPosition(Vector3 position) {
    transform.position = position;
    return this;
  }

  public Actor setInteraction(string message, LookTarget.InteractionCallback callback) {
    GetComponentInChildren<LookTarget>().setInteraction(message, callback);
    return this;
  }
}
