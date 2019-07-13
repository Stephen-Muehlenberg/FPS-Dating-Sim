using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{
  public string eventName;
  public bool destroyOnTrigger = true;

  private void OnTriggerEnter(Collider other)
  {
    if (other.name == "Player")
    {
      if (destroyOnTrigger) Destroy(this.gameObject);
      QuestManager.currentQuest.handleEvent(eventName, gameObject);
    }
  }
}
