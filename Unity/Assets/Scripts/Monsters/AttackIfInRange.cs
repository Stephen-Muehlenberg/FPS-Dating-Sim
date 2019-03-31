using UnityEngine;

public class AttackIfInRange : MonoBehaviour {
  public float range = 1f;
  public int damage = 5;
  [Tooltip("Minimum time attacker must be in range before making its first attack")]
  public float chargeTime = 1f;
  public float delayBetweenAttacks = 1f;
  public bool autoTargetPlayer = true;

  private Transform target;
  private bool previouslyInRange = false;
  private float timeTillNextAttack = 0f;
  private float rangeSq;

  public void Start() {
    rangeSq = range * range;
    if (autoTargetPlayer) {
      setTarget(GameObject.FindGameObjectWithTag("Player").transform);
    }
  }

  public void setTarget(Transform target) {
    if (target == this.target) return;

    this.target = target;
    previouslyInRange = false;
  }

  public void Update() {
    if (Time.timeScale == 0) return; // Game is paused; do nothing

    if (target == null) return;

    if ((transform.position - target.position).sqrMagnitude <= rangeSq) {
      if (!previouslyInRange) {
        timeTillNextAttack = chargeTime;
        previouslyInRange = true;
      }

      timeTillNextAttack -= Time.deltaTime;

      if (timeTillNextAttack <= 0) {
        target.SendMessageUpwards("takeDamage", new Damage(damage, transform.position, target.position, null));
        timeTillNextAttack = delayBetweenAttacks;
      }
    }
    else {
      if (previouslyInRange) {
        previouslyInRange = false;
      }
    }
  }
}
