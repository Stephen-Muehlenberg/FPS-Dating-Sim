using UnityEngine;
using UnityEngine.UI;

public class SelectionMenuItem : MonoBehaviour {
  private static Vector2 OFFSET_MIN_NORMAL = new Vector2(-64, -64), OFFSET_MAX_NORMAL = new Vector2(64, 64);
  private static Vector2 OFFSET_MIN_HIGHLIGHTED = new Vector2(-84, -92), OFFSET_MAX_HIGHLIGHTED = new Vector2(84, 76);

  public Text text;
  public Image image;
  public Color normalColour;
  public Color highlightedColour;
  public Vector2 imageCenter;
  bool canHighlight;

  private int fontSizeNormal, fontSizeHighlighted;

  public void initialize(SelectionMenu.Option option, bool selected, bool canHighlight, bool highlighted, int fontSizeNormal, int fontSizeHighlighted) {
    normalColour = option.colors.normal;
    highlightedColour = option.colors.selected;
    text.text = option.text;
    text.color = selected ? option.colors.selected : option.colors.normal;
    if (image != null) image.color = selected ? option.colors.selected : option.colors.normal;
    this.fontSizeNormal = fontSizeNormal;
    this.fontSizeHighlighted = fontSizeHighlighted;
    text.fontSize = fontSizeNormal;
    this.canHighlight = canHighlight;

    setHighlighted(highlighted);
  }

  public void setHighlighted(bool highlighted) {
    if (!canHighlight) highlighted = false;

    text.fontSize = highlighted ? fontSizeHighlighted : fontSizeNormal;
    text.color = highlighted ? highlightedColour : normalColour;
    if (image != null) {
      image.rectTransform.offsetMin = imageCenter + (highlighted ? OFFSET_MIN_HIGHLIGHTED : OFFSET_MIN_NORMAL);
      image.rectTransform.offsetMax = imageCenter + (highlighted ? OFFSET_MAX_HIGHLIGHTED : OFFSET_MAX_NORMAL);
      image.color = text.color;
    }
  }
}
