using UnityEngine;
using UnityEngine.UI;

public class CombatDialogMessage : MonoBehaviour {
  // TODO make these constants dicatated by user settings
  private const int HEIGHT_LARGE = 94, HEIGHT_SMALL = 80;
  private const int PORTRAIT_LARGE = 88, PORTRAIT_SMALL = 74;
  private const int FONT_LARGE = 32, FONT_SMALL = 28;
  private const int HORIZONTAL_PADDING = 18, VERTICAL_PADDING = 3; // Spacing inside the message between the message bounds and the icon bounds
  private const int NEW_DIALOG_SPACE = 28;
  private const float MESSAGE_LIFETIME = 15f;
  private const float FADE_IN_DURATION = 0.25f, FADE_OUT_DURATION = 5.5f;

  public CanvasGroup canvasGroup;
  public Image image;
  public Text uiText;
  
  private TextRevealer textRevealer;
  private float yPosition = 0; // Destination y position; if not here, will smoothly move here over time
  private float velocity;
  private bool hasMoved = false;
  private bool revealInProgress;
  private float age = 0;
  private bool fadeInComplete = false;

  public static CombatDialogMessage show(Character speaker,
                                         string text,
                                         float revealSpeedMultiplier,
                                         int yPosition,
                                         TextRevealer textRevealer,
                                         Callback revealFinishedCallback) {
    var prefab = Resources.Load<GameObject>("UI/CombatMessage");
    var instance = Instantiate(prefab).GetComponent<CombatDialogMessage>();

    var transform = instance.transform as RectTransform;
    transform.SetParent(MainCanvas.transform);
    instance.setSize(HEIGHT_LARGE, PORTRAIT_LARGE, FONT_LARGE);
    transform.anchoredPosition = new Vector2(0, -HEIGHT_LARGE / 2); // Set a negative y position so it initially animates up into place

    instance.image.sprite = Resources.Load<Sprite>(speaker.portraitResource);

    instance.uiText.GetComponent<Outline>().effectColor = speaker.combatTextOutline;

    instance.textRevealer = textRevealer;
    instance.revealInProgress = true;
    instance.textRevealer.set(instance.uiText, text, revealSpeedMultiplier, () => {
      instance.revealInProgress = false;
      revealFinishedCallback?.Invoke();
    });

    return instance;
  }

  private void setSize(int messageSize, int portraitSize, int fontSize) {
    var rectTransform = transform as RectTransform;
    rectTransform.offsetMin = new Vector2(0, 0); // Left, Pos Y
    rectTransform.offsetMax = new Vector2(0, messageSize); // Right, Height

    image.rectTransform.offsetMin = new Vector2(HORIZONTAL_PADDING, VERTICAL_PADDING);
    image.rectTransform.offsetMax = new Vector2(portraitSize + HORIZONTAL_PADDING, -VERTICAL_PADDING);

    // For some reason, resizing the image does nothing unless you clear and re-assign the sprite
    var sprite = image.sprite;
    image.sprite = null;
    image.sprite = sprite;

    uiText.fontSize = fontSize;
  }

  public enum MessageMoveDistance { MESSAGE, SPACE }

  public void moveUp(MessageMoveDistance distance) {
    if (distance == MessageMoveDistance.SPACE) yPosition += NEW_DIALOG_SPACE;
    else yPosition += HEIGHT_SMALL;

    // If this is the first time moving, need to shrink the dialog down to small
    // size and add extra vertical padding to account for the size change.
    if (!hasMoved) {
      setSize(HEIGHT_SMALL, PORTRAIT_SMALL, FONT_SMALL);
      yPosition += (HEIGHT_LARGE - HEIGHT_SMALL) / 2f;
      hasMoved = true;
    }
  }

  // Ends the message prematurely
  public void abort() {
    revealInProgress = false;
  }

  public void Update() {
    if (TimeUtils.dialogPaused) return;

    // Slide message up if y position has changed
    var rectTransform = transform as RectTransform;
    // TODO this uses Time.deltaTime, instead of TimeUtils.dialogDeltaTime.
    // TODO also remember to check if delta time is 0, as SmoothDamp returns NaN if delta time is 0.
    rectTransform.anchoredPosition = new Vector2(0, Mathf.SmoothDamp(rectTransform.anchoredPosition.y, yPosition, ref velocity, 0.15f));
    
    if (!fadeInComplete) {
      canvasGroup.alpha += Time.deltaTime / FADE_IN_DURATION;
      if (canvasGroup.alpha >= 1) {
        canvasGroup.alpha = 1;
        fadeInComplete = true;
      }
    }

    if (revealInProgress) {
      textRevealer.update();
      return;
    }
    else {
      // Only start the message countdown once all text has finished revealing.
      age += Time.deltaTime;

      if (age > MESSAGE_LIFETIME - FADE_OUT_DURATION) {
        canvasGroup.alpha -= Time.deltaTime / FADE_OUT_DURATION;

        if (age > MESSAGE_LIFETIME) {
          CombatDialogManager.remove(this);
          Destroy(gameObject);
          return;
        }
      }

    }
  }
}
