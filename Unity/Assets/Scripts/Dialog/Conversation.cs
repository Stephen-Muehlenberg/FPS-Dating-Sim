using System.Collections.Generic;
using UnityEngine;

public class Conversation {
  public delegate Conversation DynamicConversation();

  public List<Action> actions = new List<Action>();
  public Callback callback;

  // ===== ACTIONS =====

  public abstract class Action {
    public class SetSpeaker : Action {
      public Character speaker;
      public SetSpeaker(Character speaker) { this.speaker = speaker; }
    }

    public class OverrideName : Action {
      public string name;
      public OverrideName(string name) { this.name = name; }
    }

    public class SetSpeed : Action {
      public Speed speed;
      public SetSpeed(Speed speed) { this.speed = speed; }
    }

    public class SetText : Action {
      public string text;
      public SetText(string text) { this.text = text; }
    }

    public class DynamicInsert : Action {
      public DynamicConversation callback;
      public DynamicInsert(DynamicConversation callback) { this.callback = callback; }
    }
    
    public class Wait : Action {
      public float seconds;
      public Wait(float seconds) { this.seconds = seconds; }
    }
    
    public class Choice : Action {
      public delegate void OnSelection(int selection, string text);

      public string messageText;
      public string[] options;
      public OnSelection callback;

      public Choice(string messageText, string[] options, OnSelection callback) {
        this.messageText = messageText;
        this.options = options;
        this.callback = callback;
      }
    }
    
    // TODO maybe this should have two callbacks - one for normal in-game use, the other for resuming the game?
    public class PerformAction : Action {
      public Callback callback;
      public PerformAction(Callback callback) { this.callback = callback; }
    }
  }

  // ===== ENUMS =====

  public struct Speed {
    public float multiplier;

    public Speed(float multiplier) { this.multiplier = multiplier; }

    public static Speed VERY_SLOW = new Speed(1.5f);   // Drunk, sleepy, silly
    public static Speed SLOW = new Speed(1.25f);       // Cautious, dramatic
    public static Speed NORMAL = new Speed(1);         // Default
    public static Speed FAST = new Speed(0.666f);      // Worried, excited
    public static Speed VERY_FAST = new Speed(0.333f); // Sugar rush, silly
  }

  // ===== BUILDER =====
  public Conversation text(string message) {
    actions.Add(new Action.SetText(message));
    return this;
  }

  public Conversation text(string[] messages) {
    foreach (string message in messages) actions.Add(new Action.SetText(message));
    return this;
  }

  public Conversation text(Character speaker, params string[] messages) {
    actions.Add(new Action.SetSpeaker(speaker));
    if (messages.Length < 1) throw new UnityException("Conversation.text() must include at least 1 message");
    foreach (string message in messages) actions.Add(new Action.SetText(message));
    return this;
  }

  public Conversation speaker(Character speaker) {
    actions.Add(new Action.SetSpeaker(speaker));
    return this;
  }

  public Conversation speaker(Character speaker, string nameOverride) {
    actions.Add(new Action.SetSpeaker(speaker));
    // nameOverride might be null if we ever need to do some logic to determine what name to display.
    if (nameOverride != null) actions.Add(new Action.OverrideName(nameOverride));
    return this;
  }

  public Conversation overrideSpeakerName(string name) {
    actions.Add(new Action.OverrideName(name));
    return this;
  }

  /**
   * Takes a function which generates a new Conversation. When this action is called, it runs the function
   * and inserts the generated conversation into the parent conversation.
   * Useful for dynamically inserting chunks of dialog based on variables that aren't known until the 
   * dialog happens, e.g. reacting to player choices.
   */
  public Conversation dynamicallyInsert(DynamicConversation callback) {
    actions.Add(new Action.DynamicInsert(callback));
    return this;
  }

  /**
   * Displays only the messages from the selected Speaker. Convenience method for dialog where one of several girls could speak.
   */
  public Conversation variableText(Character speaker, string[] roseMessages, string[] mayMessages, string[] vanessaMessages, string[] fizzyMessages) {
    if (speaker == Character.ROSE) return text(speaker, roseMessages);
    else if (speaker == Character.MAY) return text(speaker, mayMessages);
    else if (speaker == Character.VANESSA) return text(speaker, vanessaMessages);
    else if (speaker == Character.FIZZY) return text(speaker, fizzyMessages);
    else throw new UnityException("Unexpected speaker " + speaker.name);
  }

  public Conversation wait(float seconds) {
    actions.Add(new Action.Wait(seconds));
    return this;
  }

  /**
   * Displays messageText like a normal text message using the current speaker, then displays 2-4 options
   * above the message for the player to choose from.
   * When the player clicks on one, the callback is invoked with the text and index of the selected option.
   */
  public Conversation choice(string messageText, string option0, string option1, Action.Choice.OnSelection callback) {
    actions.Add(new Action.Choice(messageText, new string[] { option0, option1 }, callback));
    return this;
  }

  public Conversation choice(string messageText, string option0, string option1, string option2, Action.Choice.OnSelection callback) {
    actions.Add(new Action.Choice(messageText, new string[] { option0, option1, option2 }, callback));
    return this;
  }

  public Conversation choice(string messageText, string option0, string option1, string option2, string option3, Action.Choice.OnSelection callback) {
    actions.Add(new Action.Choice(messageText, new string[] { option0, option1, option2, option3 }, callback));
    return this;
  }

  public Conversation performAction(Callback callback) {
    actions.Add(new Action.PerformAction(callback));
    return this;
  }

  public Conversation speed(Speed speed) {
    actions.Add(new Action.SetSpeed(speed));
    return this;
  }

  // TODO sound
  // TODO set character icon/sprite
  // TODO effect

  public void show() {
    assertAlwaysHaveSpeaker();
    ConversationManager.start(this);
  }

  public void show(Callback callback) {
    assertAlwaysHaveSpeaker();
    this.callback = callback;
    ConversationManager.start(this);
  }

  // Throws an exception if there are any SetText actions without at least one preceding SetSpeaker action.
  private void assertAlwaysHaveSpeaker() {
    foreach (Action action in actions) {
      if (action is Action.SetSpeaker) return;
      if (action is Action.SetText) throw new UnityException("Must set a speaker before setting text.");
    }
  }
}
