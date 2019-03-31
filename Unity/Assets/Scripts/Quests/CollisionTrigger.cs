using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{
  public string eventName;
  public bool triggerOnce = true;

  private void OnTriggerEnter(Collider other)
  {
    if (other.name == "Player")
    {
      if (triggerOnce) Destroy(this.gameObject);
      QuestManager.currentQuest.handleEvent(eventName, gameObject);
    }
  }
}
