using UnityEngine;

public class LookTarget : MonoBehaviour {
  public delegate void InteractionCallback(LookTarget interactor);

  public string text;
  public float interactionRange = 1f;
  public Transform textCenter;
  public event InteractionCallback OnInteract;

  public void startLooking() {
    LookText.show(text, textCenter);
  }

  public void stopLooking() {
    LookText.hide();
  }

  public virtual void interact() {
    if (OnInteract != null) OnInteract(this);
    OnInteract = null;
    enabled = false;
  }

  public void setInteraction(string text, InteractionCallback callback) {
    this.enabled = true;
    this.text = text;
    OnInteract += callback;
  }

  public void removeInteraction(InteractionCallback callback) {
    OnInteract -= callback;
    if (OnInteract == null) enabled = false;
  }
}
