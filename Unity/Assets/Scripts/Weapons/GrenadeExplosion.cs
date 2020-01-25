using UnityEngine;

public class GrenadeExplosion : MonoBehaviour {
  private float lifetime = 0f;
  private static float DURATION = 0.25f;
  new public MeshRenderer renderer;
  new public Light light;

  void Start() {
    Destroy(this.gameObject, 1.5f); // Wait long enough for the sound to finish playing
  }

  void Update() {
    if (TimeUtils.gameplayPaused) return;

    lifetime += Time.deltaTime;

    if (lifetime >= DURATION) {
      // Make explosion invisible, but wait for the sound to finish before actually destroying self.
      renderer.enabled = false;
      light.enabled = false;
      Destroy(this);
    }
    else {
      var radius = Mathf.Sqrt(lifetime / DURATION) * Grenade.RADIUS;
      transform.localScale = new Vector3(radius, radius, radius);
    }
  }
}
