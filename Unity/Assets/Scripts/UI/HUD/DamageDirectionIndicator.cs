using UnityEngine;
using UnityEngine.UI;

public class DamageDirectionIndicator : MonoBehaviour {
  public static float LIFETIME = 1f;
  private static GameObject prefab;

  private Image image;

  void Start() {
    image = GetComponent<Image>();
  }

  public void Update() {
    if (TimeUtils.gameplayPaused) return;

    image.color = new Color(1, 0, 0, image.color.a - TimeUtils.dialogDeltaTime);
  }

  public static void show(Transform playerTransform, Vector3 damageOrigin) {
    if (prefab == null) {
      prefab = Resources.Load<GameObject>("UI/DamageDirectionIndicator");
    }

    var indicator = Instantiate(prefab, new Vector3(), Quaternion.identity) as GameObject;
    indicator.transform.SetParent(MainCanvas.transform);

    indicator.GetComponent<RectTransform>().anchoredPosition = calculateScreenPosition(playerTransform, damageOrigin);

    // TODO set intensity/duration

    Destroy(indicator, LIFETIME);
  }

  private static Vector2 calculateScreenPosition(Transform playerTransform, Vector3 damageOrigin) {
    var damageDirection = damageOrigin - playerTransform.position;

    // Determine the direction of the damage relative to the player's forward direction
    var relativeX = Vector3.Dot(damageDirection, playerTransform.right);
    var relativeY = Vector3.Dot(damageDirection, playerTransform.forward);

    var halfHeight = Screen.height / 2f;
    var halfWidth = Screen.width / 2f;

    if (relativeX == 0) {
      return new Vector2(0, relativeY > 0 ? 1 : -1); // Directly N or S
    }
    // Compre the direction's x/y ratio to the screen's width/height ratio.
    // This tells us which half of which screen edge the damage occurs in.
    else {
      var gradient = relativeY / relativeX;
      var screenGradient = (float)Screen.height / (float)Screen.width;

      if (relativeX < 0) {
        if (relativeY < 0) {
          if (gradient > screenGradient) {
            return new Vector2(halfHeight / -gradient, -halfHeight); // SSW
          } else {
            return new Vector2(-halfWidth, gradient * -halfWidth); // WSW
          }
        } else {
          if (gradient > -screenGradient) {
            return new Vector2(-halfWidth, -gradient * -halfWidth); // WNW
          } else {
            return new Vector2(halfHeight / gradient, halfHeight); // NNW
          }
        }
      } else {
        if (relativeY < 0) {
          if (gradient > -screenGradient) {
            return new Vector2(halfWidth, gradient * halfWidth); // ESE
          } else {
            return new Vector2(halfHeight / -gradient, -halfHeight); // SSE
          }
        } else {
          if (gradient > screenGradient) {
            return new Vector2(halfHeight / gradient, halfHeight); // NNE
          } else {
            return new Vector2(halfWidth, gradient * halfWidth); // ENE
          }
        }
      }
    }
  }
}
