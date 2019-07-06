using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectItem : MonoBehaviour {
  public Text text;
  public Image image;
  public Color unselectedColour;
  public Color selectedColour;
  public Vector2 imageCenter;

  public void enable() {
    text.enabled = true;
    // TODO clean this up and verify it works
    //image.enabled = true;
    image.color = unselectedColour;
  }

  public void disable() {
    text.enabled = false;
    // TODO clean this up and verify it works
    //image.enabled = false;
    image.color = new Color(0, 0, 0, 0.4f);
  }

  public void highlight() {
    text.fontSize = 32;
    image.rectTransform.offsetMin = new Vector2(imageCenter.x - 84, imageCenter.y - 92);
    image.rectTransform.offsetMax = new Vector2(imageCenter.x + 84, imageCenter.y + 76);
  }

  public void unhighlight() {
    text.fontSize = 22;
    image.rectTransform.offsetMin = new Vector2(imageCenter.x - 64, imageCenter.y - 64);
    image.rectTransform.offsetMax = new Vector2(imageCenter.x + 64, imageCenter.y + 64);
  }

  public void select() {
    text.color = selectedColour;
    image.color = selectedColour;
  }

  public void deselect() {
    text.color = unselectedColour;
    image.color = unselectedColour;
  }
}
