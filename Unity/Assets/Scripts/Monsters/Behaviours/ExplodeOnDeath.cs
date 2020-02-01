using UnityEngine;

public class ExplodeOnDeath : MonoBehaviour
{
  public float range = 1;
  public float damage = 200;
  public GameObject bodyToHide;
  public ParticleSystem explosionParticles;

  public void die() {
    if (bodyToHide != null) Destroy(bodyToHide);
    explosionParticles?.Play();

    Destroy(this.gameObject, 2);
  }
}
