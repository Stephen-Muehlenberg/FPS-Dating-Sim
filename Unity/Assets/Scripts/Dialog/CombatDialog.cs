﻿using System.Collections.Generic;
using UnityEngine;

public class CombatDialog {
  public enum Priority {
    TRIVIAL = 1, // Filler dialog, only if there's nothing more interesting happening
    LOW = 2,     // Banter, maybe minor character building
    MEDIUM = 3,  // Hints and soft directions
    HIGH = 4,    // Important gameplay feedback, e.g. weapon getting tired
    MAX = 5      // Quest updates
  }

  public Priority priority;
  public List<Action> actions = new List<Action>();

  // ==== ACTIONS ====

  public abstract class Action {
    public class ShowMessage : Action {
      public Character speaker;
      public string message;
      public Conversation.Speed speed;
      public ShowMessage(Character speaker, string message, Conversation.Speed speed) {
        this.speaker = speaker;
        this.message = message;
        this.speed = speed;
      }
    }

    public class SetPriority : Action {
      public Priority priority;
      public SetPriority(Priority priority) { this.priority = priority; }
    }

    public class Wait : Action {
      public float duration;
      public Wait(float duration) { this.duration = duration; }
    }

    public class PerformAction : Action {
      public Callback callback;
      public PerformAction(Callback callback) { this.callback = callback; }
    }
  }

  // ==== ENUMS ====

  public struct Speaker {
    public string resourceName;
    public Color outlineColour;

    public Speaker(string resourceName, Color outlineColour) {
      this.resourceName = resourceName;
      this.outlineColour = outlineColour;
    }

    public static Speaker MC = new Speaker("PortraitMC", new Color(0.42f, 0.42f, 0.42f));
    public static Speaker ROSE = new Speaker("PortraitRose", new Color(1, 0, 0));
    public static Speaker MAY = new Speaker("PortraitMay", new Color(0, 0.72f, 0.06f));
    public static Speaker VANESSA = new Speaker("PortraitVanessa", new Color(0.5f, 0.5f, 1));
    public static Speaker FIZZY = new Speaker("PortraitFizzy", new Color(0.87f, 0.63f, 0.24f));
  }

  // ==== BUILDER ====

  public CombatDialog message(Character speaker, string message) {
    actions.Add(new Action.ShowMessage(speaker, message, Conversation.Speed.NORMAL));
    return this;
  }

  public CombatDialog message(Character speaker, string message, Conversation.Speed speed) {
    actions.Add(new Action.ShowMessage(speaker, message, speed));
    return this;
  }

  /**
   * Changes the dialog's priority once it reaches this point.
   * Useful if e.g. the opening lines are important, but the later lines can be interrupted.
   */
  public CombatDialog setPriority(Priority priority) {
    actions.Add(new Action.SetPriority(priority));
    return this;
  }

  public CombatDialog wait(float duration) {
    actions.Add(new Action.Wait(duration));
    return this;
  }

  public CombatDialog performAction(Callback callback) {
    actions.Add(new Action.PerformAction(callback));
    return this;
  }

  // TODO handle different priority messages? Allow higher priority to interrupt lower priority?
  public void show(Priority priority) {
    this.priority = priority;
    CombatDialogManager.show(this, null);
  }

  public void show(Priority priority, Callback callback) {
    this.priority = priority;
    CombatDialogManager.show(this, callback);
  }
}
