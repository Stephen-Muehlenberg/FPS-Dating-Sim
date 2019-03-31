using UnityEngine;
using UnityEngine.UI;

public class LookText : MonoBehaviour {
  private static Transform textCenter;
  private static Text text;

	void Start () {
    text = GetComponent<Text>();
    text.enabled = false;
	}

  void Update () {
    if (text.enabled) {
      text.transform.position = Player.SINGLETON.camera.WorldToScreenPoint(textCenter.position);
    }
  }

  public static void show(string text, Transform textCenter) {
    LookText.text.text = text;
    LookText.textCenter = textCenter;
    LookText.text.transform.position = Player.SINGLETON.camera.WorldToScreenPoint(textCenter.position);
    LookText.text.enabled = true;
  }

  public static void hide() {
    text.enabled = false;
  }
}
