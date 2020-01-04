using UnityEngine;

public class Awareness : MonoBehaviour {
  [Tooltip ("Monster is immediately alerted if the player gets within this range.")]
  public float immediateAlertRadius = 10f;
  [Tooltip ("Monster will be alerted if the player stays within this range for eventualAlertTime seconds.")]
  public float eventualAlertRadius = 30f;
  [Tooltip ("Monster will be alerted if the player stays within eventualAlertRadius for this amount of time.")]
  public float eventualAlertTime = 5f;

  [Tooltip ("Monster is immediately alerted if other monsters within this range become alerted.")]
  public float alliesAlertRadius = 15f;

  public MonoBehaviour[] alertBehaviours;

  public bool alert = false;

  private float immediateAlertRadiusSq;
  private float eventualAlertRadiusSq;
  private float alliesAlertRadiusSq;

  private float timeWithinEventualAlertRadius = 0f;

  private static float distanceSq;

  public void Start() {
    immediateAlertRadiusSq = immediateAlertRadius * immediateAlertRadius;
    eventualAlertRadiusSq = eventualAlertRadius * eventualAlertRadius;
    alliesAlertRadiusSq = alliesAlertRadius * alliesAlertRadius;
  }

  public void Update() {
    if (Time.timeScale == 0) return; // Game is paused; do nothing

    distanceSq = (transform.position - Player.SINGLETON.transform.position).sqrMagnitude;
    
    if (distanceSq < eventualAlertRadiusSq) {
      if (distanceSq < immediateAlertRadiusSq) {
        becomeAlert();
        return;
      }

      timeWithinEventualAlertRadius += TimeUtils.gameplayDeltaTime;
      if (timeWithinEventualAlertRadius >= eventualAlertTime) becomeAlert();
      return;
    }

    if (timeWithinEventualAlertRadius > 0) {
      timeWithinEventualAlertRadius -= TimeUtils.gameplayDeltaTime; // Alertness cools off over time
    }
  }

  public void takeDamage(Damage damage) {
    becomeAlert();
  }

  public void becomeAlert() {
    if (alert) return;

    alert = true;

    // Switch over to alert behaviours
    foreach (MonoBehaviour behaviour in alertBehaviours) {
      behaviour.enabled = true;
    }

    // Remove idle behaviours
    Destroy(GetComponent<IdleBehaviour>());
    Destroy(this);
    
    // Alert nearby allies
    foreach (Monster monster in MonstersController.monsters) {
      distanceSq = (transform.position - monster.transform.position).sqrMagnitude;
      var awareness = monster.GetComponent<Awareness>();
      if (awareness != null && distanceSq <= awareness.alliesAlertRadiusSq) awareness.becomeAlert();
    }
  }
}
