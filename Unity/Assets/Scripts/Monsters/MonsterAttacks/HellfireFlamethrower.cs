using UnityEngine;

public class HellfireFlamethrower : MonoBehaviour {
  private static float LIFETIME = 0.9f;
  private static float SPEED = 14;
  private static int MIN_DAMAGE = 45, MAX_DAMAGE = 55;

  private float timeToLive;
  private int damageAmount;
  private bool hitPlayer = false;

  private void Start() {
    timeToLive = LIFETIME;
    damageAmount = Random.Range(MIN_DAMAGE, MAX_DAMAGE); // Projectile can hit multiple enemies, so re-use the same damage across all of them to be fair
  }

  void Update() {
    timeToLive -= TimeUtils.gameplayDeltaTime;

    if (timeToLive <= 0) {
      Destroy(this.gameObject);
      return;
    }

    transform.position += transform.forward * TimeUtils.gameplayDeltaTime * SPEED;
    transform.localScale = new Vector3(LIFETIME - timeToLive + 2.5f, 2, 0.5f);
  }

  public void OnTriggerEnter(Collider other) {
    if (other.transform.CompareTag("Player") && !hitPlayer) {
      hitPlayer = true;
      var damage = new Damage(damageAmount, transform.position, transform.position);
      Player.SINGLETON.GetComponent<Health>().takeDamage(damage);
      var firstPersonController = Player.SINGLETON.GetComponent<FirstPersonModule.FirstPersonController>();
      firstPersonController.velocity += transform.forward * 15f;
    }
    else if (other.transform.CompareTag("Monster")) {
      // TODO see if we've hit this monster before. If not, damage it, and keep remember not to damage it again in future
    }
  }
}
