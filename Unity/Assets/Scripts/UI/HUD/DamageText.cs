using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour {
  public static float LIFETIME = 1f;
  private static GameObject prefab;

  public Text text;
  public Outline outline;

  private Vector3 origin;
  private Vector3 randomDirection; // Text falls slightly in a random direction, so that multiple texts dont perfectly overlap
  private float age = 0f;

  void Start() {
    origin = transform.position;
    var randomAngle = Random.Range(0f, 360f);
    randomDirection = new Vector3(Mathf.Sin(randomAngle), 0f, Mathf.Cos(randomAngle)) * Random.Range(0.5f, 2f);
  }

  void Update() {
    if (Time.timeScale == 0) return; // Game is paused; do nothing

    age += TimeUtils.dialogDeltaTime;
    // Math basically animates the text up slightly, than falls as if affected by gravity
    var verticalOffset = new Vector3(0, ((1 - Mathf.Pow((age * 9 - 3), 2)) / 6f) + 2f, 0);

    // WorldToScreenPoint() will return a screen position even if location is behind player, so got to check if location is actually visible
    if (Vector3.Dot(Player.SINGLETON.camera.transform.forward, origin + verticalOffset - Player.SINGLETON.transform.position) >= 0)
    {
      text.enabled = true;
      transform.position = Player.SINGLETON.camera.WorldToScreenPoint(origin + verticalOffset + (age * randomDirection));
    //  transform.position = new Vector3(transform.position.x, transform.position.y, 100);
    }
    else text.enabled = false;
  }

  public static void create(int damage, Vector3 origin, bool crit) {
    if (prefab == null) {
      prefab = Resources.Load<GameObject>("UI/DamageText");
    }

    var damageText = Instantiate(prefab, origin, Quaternion.identity) as GameObject;
    var textComponent = damageText.GetComponent<Text>();
    textComponent.text = damage.ToString();
    if (crit) {
      textComponent.fontSize = 40;
      textComponent.color = new Color(0.9f, 0.65f, 0f);
    }
    damageText.transform.SetParent(MainCanvas.transform);
    damageText.transform.SetAsFirstSibling();
    Destroy(damageText, LIFETIME);
  }
}
