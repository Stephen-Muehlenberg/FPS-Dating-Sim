using UnityEngine;

public class Fireball : MonoBehaviour {
  private static float SPEED = 15f;
  private float timeToLive = 6f;
  
	void Update () {
    if (Time.timeScale == 0) return; // Game is paused; do nothing

    timeToLive -= Time.deltaTime;
    if (timeToLive <= 0) {
      Destroy(this.gameObject);
      return;
    }

    transform.position += transform.forward * Time.deltaTime * SPEED;
	}

  void OnTriggerEnter(Collider other) {
    var damage = new Damage(Random.Range(90, 110), transform.position, transform.position);
    other.SendMessageUpwards("takeDamage", damage, SendMessageOptions.DontRequireReceiver);
    Destroy(this.gameObject);
  }
}
