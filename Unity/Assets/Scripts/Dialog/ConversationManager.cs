using UnityEngine;
using UnityEngine.UI;

public class ConversationManager : MonoBehaviour {
  public GameObject textbox;
  public Text speaker;
  public Text message;
  public Image continueIcon;

  private static ConversationManager SINGLETON;
  
  private static Conversation conversation;
  private static int currentConversationIndex;

  private float speedMultiplier;

  private TextRevealer textRevealer = new TextRevealer();
  private string currentMessage;
  private bool textRevealInProgress = false;
  private bool choiceInProgress = false;
  private float waitRemaining = 0f; // Used to indicate a pause in the dialog
  private Conversation.Speaker currentSpeaker;

  public static bool conversationInProgress() { return conversation != null; }

  private static void initializeSingleton() {
    var prefab = Resources.Load<GameObject>("UI/Conversation");
    var conversationManager = Instantiate(prefab) as GameObject;
    SINGLETON = conversationManager.GetComponent<ConversationManager>();

    // Attach to main canvas and resize to fill screen
    conversationManager.transform.SetParent(MainCanvas.transform);
    var rectTransform = conversationManager.transform as RectTransform;
    rectTransform.localPosition = new Vector3(0, 0, 0);
    rectTransform.anchorMin = Vector2.zero;
    rectTransform.anchorMax = Vector2.one;
    rectTransform.sizeDelta = Vector2.zero;

    SINGLETON.textbox.SetActive(false); // Hidden until manually enabled
  }

  // ===== SETUP / TEARDOWN =====

  public static Conversation newConversation() { return new Conversation(); }
  
  public static void start(Conversation conversation) {
    if (SINGLETON == null) initializeSingleton();

    // Prevent player interaction
    TimeUtils.pauseGameplay();
    LookController.disable();

    ConversationManager.conversation = conversation;
    currentConversationIndex = 0;

    SINGLETON.textbox.SetActive(false); // Hidden until manually enabled by an action
    SINGLETON.currentMessage = "";
    SINGLETON.textRevealInProgress = false;
    SINGLETON.choiceInProgress = false;
    SINGLETON.waitRemaining = 0;

    // Clear text and set default text speed
    SINGLETON.setSpeed(Conversation.Speed.NORMAL);

    SINGLETON.processCurrentAction();
  }

  private void finishConversation() {
    textbox.SetActive(false);
    var callback = conversation.callback;
    conversation = null;

    // Re-enable player interaction
    TimeUtils.resumeGameplay();
    LookController.enable();

    // Notify & remove listeners
    callback?.Invoke();
  }

  // ===== ACTIONS =====

  private void processCurrentAction() {
    var action = conversation.actions[currentConversationIndex];

    if (action is Conversation.Action.SetSpeaker) setSpeaker((action as Conversation.Action.SetSpeaker).speaker);
    else if (action is Conversation.Action.OverrideName) overrideSpeakerName((action as Conversation.Action.OverrideName).name);
    else if (action is Conversation.Action.SetSpeed) setSpeed((action as Conversation.Action.SetSpeed).speed);
    else if (action is Conversation.Action.PerformAction) (action as Conversation.Action.PerformAction).callback();
    else if (action is Conversation.Action.SetText) {
      setText((action as Conversation.Action.SetText).text);
      return; // Don't immediately finish this action
    }
    else if (action is Conversation.Action.Wait) {
      waitRemaining = (action as Conversation.Action.Wait).seconds;
      return; // Don't immediately finish this action
    }
    else if (action is Conversation.Action.Choice) {
      choiceInProgress = true;
      setText((action as Conversation.Action.Choice).messageText);
      return; // Don't immediately finish this action
    }
    else throw new UnityException("Unidentified Conversation.Action " + action);

    // For non setText actions, immediately process the next action
    finishCurrentAction();
  }

  private void finishCurrentAction() {
    if (conversation.actions[currentConversationIndex] is Conversation.Action.SetText) {
      SINGLETON.textbox.SetActive(false);
    }

    currentConversationIndex++;
    if (currentConversationIndex < conversation.actions.Count) processCurrentAction();
    else finishConversation();
  }

  private void setSpeaker(Conversation.Speaker speaker) {
    if (speaker == SINGLETON.currentSpeaker) return;

    // Return previous speaker to normal
    if (SINGLETON.currentSpeaker == Conversation.Speaker.ROSE) Actors.getRose().transform.localScale = new Vector3(1, 1, 1);
    else if (SINGLETON.currentSpeaker == Conversation.Speaker.MAY) Actors.getMay().transform.localScale = new Vector3(1, 1, 1);
    else if (SINGLETON.currentSpeaker == Conversation.Speaker.VANESSA) Actors.getVanessa().transform.localScale = new Vector3(1, 1, 1);
    else if (SINGLETON.currentSpeaker == Conversation.Speaker.FIZZY) Actors.getFizzy().transform.localScale = new Vector3(1, 1, 1);

    // Enlarge the current speaker
    if (speaker == Conversation.Speaker.ROSE) Actors.getRose().transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
    else if (speaker == Conversation.Speaker.MAY) Actors.getMay().transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
    else if (speaker == Conversation.Speaker.VANESSA) Actors.getVanessa().transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
    else if (speaker == Conversation.Speaker.FIZZY) Actors.getFizzy().transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);

    SINGLETON.currentSpeaker = speaker;

    // Set dialog box appearance
    this.speaker.text = speaker.name;
    this.speaker.GetComponent<Outline>().effectColor = speaker.outlineColour;
    this.message.GetComponent<Outline>().effectColor = speaker.outlineColour;
    this.message.color = speaker.fillColour;
  }

  private void overrideSpeakerName(string name) {
    this.speaker.text = name;
  }

  private void setSpeed(Conversation.Speed speed) {
    speedMultiplier = speed.multiplier;
  }

  private void setText(string text) {
    textbox.SetActive(true);
    SINGLETON.textRevealer.set(message, text, speedMultiplier, onRevealComplete);
    currentMessage = text;
    textRevealInProgress = true;
    continueIcon.enabled = false;
  }

  private void onRevealComplete() {
    textRevealInProgress = false;

    if (choiceInProgress) {
      var choice = conversation.actions[currentConversationIndex] as Conversation.Action.Choice;
      ConversationChoice.show(choice.options, (selection, text) => {
        choiceInProgress = false;
        var conversationBeforeCallback = conversation;
        choice.callback.Invoke(selection, text);
        // Callback might have triggered a new conversation. Only continue conversation if it hasn't changed.
        if (conversation == conversationBeforeCallback) finishCurrentAction();
      });
    }
    else {
      continueIcon.enabled = true;
    }
  }

  // ===== UPDATE LOOP =====

  void Update () {
    if (TimeUtils.dialogPaused) return;

    if (conversation == null) return;

    if (waitRemaining > 0) {
      waitRemaining -= Time.unscaledDeltaTime;
      if (waitRemaining <= 0) finishCurrentAction();
      return;
    }

    if (textRevealInProgress) {
      // If click before end of message, skip to end. Ignore click if no characters shown yet.
      if (Input.GetButtonUp("PrimaryFire") && message.text.Length > 0) {
        message.text = currentMessage;
        onRevealComplete();
      }

      // Otherwise, reveal characters one at a time.
      else textRevealer.update();
    }

    else if (choiceInProgress) {
      // TODO handle choice
    }

    // Wait for click before showing the next message.
    else if (Input.GetButtonUp("PrimaryFire") || Input.GetButtonUp("Jump")) finishCurrentAction();
  }
}
