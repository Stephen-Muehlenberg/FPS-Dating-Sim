﻿using UnityEngine;

public class Grenade : MonoBehaviour {
  private static float LIFETIME = 3f;
  private static int MIN_DIRECT_DAMAGE = 75;
  private static int MAX_DIRECT_DAMAGE = 100;
  private static int MIN_SPLASH_DAMAGE = 295;
  private static int MAX_SPLASH_DAMAGE = 400;
  public static float RADIUS = 6.5f;
  private static float ROCKET_JUMP_Y_VELOCITY = 10;

  public GameObject explosionPrefab;

  public bool bouncy = false; // If true, only explode when you hit an enemy
  private bool hasntBouncedYet = true;
  private bool alreadyExploded = false;

  void Start() {
    Invoke("explode", LIFETIME); // Automatically explode after expiring
  }

  void OnCollisionEnter(Collision collision) {
    if (collision.collider.CompareTag("Player")) return;

    var hitAnEnemy = collision.collider.CompareTag("Monster");
    Health hitHealth = null;
    Damage? hitDamage = null;

    if (hitAnEnemy && hasntBouncedYet) {
      hitHealth = collision.collider.GetComponentInParent<Health>();
      hitDamage = new Damage(amount: Random.Range(MIN_DIRECT_DAMAGE, MAX_DIRECT_DAMAGE),
                             origin: transform.position,
                             hitPoint: collision.contacts[0].point);

      // Grenade can potentially hit multiple colliders on the same frame. If so, only the first collider gets the bonus damage.
      hasntBouncedYet = false;
    }
    if (hitAnEnemy || !bouncy) explode(hitHealth, hitDamage);
    else hasntBouncedYet = false;
  }

  private void explode(Health hitHealth, Damage? hitDamage) {
    if (alreadyExploded) return;
    alreadyExploded = true;

    // Damage + knock back enemies in blast
    var creaturesHit = Explosion.getCreaturesInBlast(transform.position, RADIUS);
    var randomBaseDamage = Random.Range(MIN_SPLASH_DAMAGE, MAX_SPLASH_DAMAGE);
    foreach (Explosion.CreatureAndDistance creature in creaturesHit) {
      if (creature.gameObject == Player.SINGLETON.gameObject) continue; // Player is immune to grenade damage

      var fractionalDistanceFromBlastCenter = Mathf.Sqrt(creature.distanceSq) / RADIUS;
      var damageMultipler = 1f - (fractionalDistanceFromBlastCenter / 2f); // Multipler ranges from 1 (closest) to 0.5 (furthest)
      var totalDamage = Mathf.RoundToInt(randomBaseDamage * damageMultipler);
      Damage damage;

      if (creature.health == hitHealth) {
        damage = new Damage(amount: hitDamage.Value.amount + totalDamage,
                            origin: hitDamage.Value.origin,
                            hitPoint: hitDamage.Value.hitPoint,
                            source: Weapons.GRENADE_LAUNCHER);
      }
      else {
        damage = new Damage(amount: totalDamage,
                            origin: transform.position,
                            hitPoint: creature.gameObject.transform.position,
                            source: Weapons.GRENADE_LAUNCHER);
      }

      creature.health.takeDamage(damage);
      // TODO knock enemies, objects back
    }

    // Spawn explosion, remove grenade
    GameObject.Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    Destroy(this.gameObject);

    // Rocket jump player IF player has something underfoot - prevents them from continuously rocket jumping off walls.
    RaycastHit hitInfo;
    int layerMask = ~LayerMask.GetMask("Weapons", "Invisible Wall", "Player"); // Ignore these layers
    Physics.Raycast(Player.SINGLETON.transform.position + Vector3.down * -0.1f, Vector3.down, out hitInfo, 2, layerMask);
    if (hitInfo.collider == null) return; // No rocket jump if nothing underfoot
    
    var distanceFromPlayer = (transform.position - Player.SINGLETON.transform.position).magnitude;
    if (distanceFromPlayer < RADIUS / 2) {
      var playerController = Player.SINGLETON.GetComponent<FirstPersonModule.FirstPersonController>();
      playerController.velocity = new Vector3(playerController.velocity.x, ROCKET_JUMP_Y_VELOCITY, playerController.velocity.y);
    }
  }
}
