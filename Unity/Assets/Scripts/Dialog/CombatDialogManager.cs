using UnityEngine;
using System.Collections.Generic;

public class CombatDialogManager : MonoBehaviour {
  // Reveal text a bit slower in combat compared to out of combat
  // TODO make these adjustable via settings
  private static float COMBAT_TEXT_REVEAL_MULTIPLIER = 1.333f;
  private static float DELAY_BETWEEN_MESSAGES_MULTIPLIER = 1.8f;
  private static float DEFAULT_MIN_MESSAGE_TIME = 1.25f;

  private static CombatDialogManager SINGLETON;
  
  private CombatDialog dialog;
  private Callback callback;
  private int currentActionIndex;

  private List<CombatDialogMessage> messages = new List<CombatDialogMessage>();
  private TextRevealer textRevealer = new TextRevealer();
  private float timeTillNextMessage;

  public static bool dialogInProgress() { return SINGLETON.dialog != null; }

  private static void lazyInitialize() {
    // TODO extract this out to its own canvas
    SINGLETON = MainCanvas.transform.gameObject.AddComponent<CombatDialogManager>();
  }
  
  public static void show(CombatDialog dialog, Callback callback) {
    if (SINGLETON == null) lazyInitialize();

    if (SINGLETON.dialog != null) {
      if (SINGLETON.dialog.priority >= dialog.priority) return; // If new dialog isn't greater priority than current dialog, ignore it
      SINGLETON.abortCurrentDialog();
    }

    // Move all existing messages up a bit to make it visually clear this is a new dialog.
    foreach (CombatDialogMessage message in SINGLETON.messages) message.moveUp(CombatDialogMessage.MessageMoveDistance.SPACE);

    SINGLETON.dialog = dialog;
    SINGLETON.callback = callback;
    SINGLETON.currentActionIndex = 0;
    SINGLETON.processCurrentAction();
  }

  public static void remove(CombatDialogMessage message) {
    SINGLETON.messages.Remove(message);
  }

  public static void clearAllMessages() {
    if (SINGLETON == null) return;
    foreach (CombatDialogMessage message in SINGLETON.messages) if (message != null) Destroy(message.gameObject);
    SINGLETON.messages = new List<CombatDialogMessage>();
    SINGLETON.dialog = null; // TODO might need to make use of the message finished callbacks?
  }

  private void finishCurrentDialog() {
    dialog = null;
    callback?.Invoke();
  }

  private void abortCurrentDialog() {
    // TODO
    // Find current CombatDialogMessage
    // If it's still being written, append a '-' character
    // Figure out how best to handle things like whitespace, punctuation
    // Stop it from writing the rest of its message

    foreach (CombatDialogMessage message in SINGLETON.messages) message.abort();
    finishCurrentDialog();
  }

  private  void processCurrentAction() {
    var action = dialog.actions[currentActionIndex];

    if (action is CombatDialog.Action.ShowMessage) {
      showMessage(action as CombatDialog.Action.ShowMessage);
    }
    else if (action is CombatDialog.Action.SetPriority) {
      dialog.priority = (action as CombatDialog.Action.SetPriority).priority;
      finishCurrentAction();
    }
    else if (action is CombatDialog.Action.Wait) {
      timeTillNextMessage = (action as CombatDialog.Action.Wait).duration;
    }
    else if (action is CombatDialog.Action.PerformAction) {
      (action as CombatDialog.Action.PerformAction).callback.Invoke();
      finishCurrentAction();
    }
    else throw new UnityException("Unhandled CombatDialog.Action " + action);
  }

  private void finishCurrentAction() {
    currentActionIndex++;
    if (currentActionIndex < dialog.actions.Count)
      processCurrentAction();
    else
      finishCurrentDialog();
  }

  private void showMessage(CombatDialog.Action.ShowMessage action) {
    var textSpeed = action.speed.multiplier * COMBAT_TEXT_REVEAL_MULTIPLIER;
    var newMessage = CombatDialogMessage.show(action.speaker,
                             action.message,
                             textSpeed,
                             0,
                             textRevealer,
                             null);

    // Move all existing messages up to accommodate this new message
    foreach (CombatDialogMessage message in messages) message.moveUp(CombatDialogMessage.MessageMoveDistance.MESSAGE);

    messages.Add(newMessage);

    timeTillNextMessage = calculateMessageDisplayTime(action.message, textSpeed);
  }

  public void Update() {
    if (TimeUtils.dialogPaused || dialog == null) return;

    timeTillNextMessage -= Time.deltaTime;
    if (timeTillNextMessage <= 0) finishCurrentAction();
  }

  private float calculateMessageDisplayTime(string message, float textSpeed) {
    // TODO ignore markup & whitespace
    return DEFAULT_MIN_MESSAGE_TIME + (message.Length * textSpeed * Settings.textSpeed * DELAY_BETWEEN_MESSAGES_MULTIPLIER);
  }
}
