using UnityEngine;

public class Random3dJitter : MonoBehaviour
{
  public Transform jitterTransform;
  public float jitterSpeed = 1;
  public float jitterScale = 1;

  private float animTime = 0f;
  private Vector3 basePosition;
  private Vector3 seed;

  void Start() {
    basePosition = jitterTransform.localPosition;
    seed = new Vector3(Random.Range(0, 1000),
                       Random.Range(0, 1000),
                       Random.Range(0, 1000));
  }

  void Update()
  {
    if (TimeUtils.gameplayPaused) return;

    animTime += Time.deltaTime * jitterSpeed;
    jitterTransform.localPosition = new Vector3(basePosition.x + (Mathf.PerlinNoise(seed.x, animTime) - 0.5f) * jitterScale,
                                                basePosition.y + (Mathf.PerlinNoise(seed.y, animTime) - 0.5f) * jitterScale,
                                                basePosition.z + (Mathf.PerlinNoise(seed.z, animTime) - 0.5f) * jitterScale);
  }
}
