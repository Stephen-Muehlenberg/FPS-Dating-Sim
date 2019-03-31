using UnityEngine;

public class GrenadeExplosion : MonoBehaviour {
  private float lifetime = 0f;
  private static float DURATION = 0.25f;

  void Start() {
    Destroy(this.gameObject, 1.5f); // Wait long enough for the sound to finish playing
  }

  void Update() {
    lifetime += Time.deltaTime;

    if (lifetime >= DURATION) {
      GetComponent<MeshRenderer>().enabled = false;
      GetComponent<Light>().enabled = false;
    }
    else {
      var radius = Mathf.Sqrt(lifetime / DURATION) * Grenade.RADIUS;
      transform.localScale = new Vector3(radius, radius, radius);
    }
  }
}
