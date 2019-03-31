using UnityEngine;

public class HellfireMortar : MonoBehaviour {
  public GameObject explosionPrefab;

  private float horizontalVelocity;
  private float verticalVelocity = 20f;
  private bool exploded = false;

  private static float GRAVITY = -9.8f;
  private static float EXPLOSION_RADIUS = 5f;
  private static float MIN_DAMAGE = 180, MAX_DAMAGE = 210;

  public void setDistanceToTarget(float distance) {
    horizontalVelocity = distance / 2f;
  }

  void Update() {
    if (Time.timeScale == 0) return; // Game is paused; do nothing

    // TODO use a more accurate method for aiming
    transform.position += transform.forward * horizontalVelocity * Time.deltaTime;
    transform.position += Vector3.up * verticalVelocity * Time.deltaTime;
    verticalVelocity += GRAVITY * Time.deltaTime;
  }

  void OnTriggerEnter(Collider other) {
    if (exploded) return; // Can sometimes hit multiple colliders in a single frame; only explode for the first collider
    exploded = true;

    // Damage + knock back enemies in blast
    var creaturesHit = Explosion.getCreaturesInBlast(transform.position, EXPLOSION_RADIUS);
    var randomBaseDamage = Random.Range(MIN_DAMAGE, MAX_DAMAGE);
    foreach (Explosion.CreatureAndDistance creature in creaturesHit) {
      var fractionalDistanceFromBlastCenter = Mathf.Sqrt(creature.distanceSq) / EXPLOSION_RADIUS;
      var damageMultipler = 1f - (fractionalDistanceFromBlastCenter / 2f); // Multipler ranges from 1 (closest) to 0.5 (furthest)
      var damage = new Damage(Mathf.RoundToInt(randomBaseDamage * damageMultipler), transform.position, creature.gameObject.transform.position);
      creature.gameObject.GetComponent<Health>().takeDamage(damage);
      // TODO knock enemies, objects back
    }

    /*
    // Rocket jump player
    // TODO raycast a short way beneath the player - if they're not above a surface, they cant rocket jump, even if caught in a
    // grenade explosion.
    var distanceFromPlayer = (transform.position - Player.SINGLETON.transform.position).magnitude;
    if (distanceFromPlayer < EXPLOSION_RADIUS / 2) {
      var rigidbody = Player.SINGLETON.GetComponent<Rigidbody>();

      // Reset vertical speed so you can immediately bounce off the ground even if currently hurtling downwards
      if (rigidbody.velocity.y < 0) rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.y);

      rigidbody.AddForce(new Vector3(0, ROCKET_JUMP_FORCE, 0));
    }
    */

    GameObject.Instantiate(explosionPrefab, transform.position, Quaternion.identity);

    GetComponent<Renderer>().enabled = false;
    GetComponent<ParticleSystem>().Stop();
    Destroy(gameObject, 1f); // Wait for particles to finish before destroying self
    Destroy(this);
  }
}

