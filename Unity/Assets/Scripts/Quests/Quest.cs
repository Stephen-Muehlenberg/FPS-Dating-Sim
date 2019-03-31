using System;
using UnityEngine;
using System.Collections;

public abstract class Quest {
  public static string KEY_STATE = "state";

  public abstract string name { get; }
  public int state = 0;

  public void start() { start(new Hashtable()); }
  public abstract void start(Hashtable args);

  public abstract Hashtable save();

  public virtual void stop() {}

  public virtual void handleEvent(string eventName, GameObject trigger) {}
}
