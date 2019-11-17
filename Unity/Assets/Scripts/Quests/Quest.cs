using System.Collections;
using UnityEngine;

public abstract class Quest {
  public static string KEY_STATE = "state";
  protected static string KEY_PLAYER = "player";
  // TODO automatically save / load girl states, too

  public abstract string name { get; }

  /** 
   * Represents the current state of the Quest.
   * 
   * 0 is the initial state, indicating no progress. Higher numbers generally correspond
   * to further progress.
   * 
   * States which are multiples of 100 represent unique save points, while non-multiples
   * represent non-save states. E.g. saving at state 300 then loading will return to 
   * exactly state 300, while saving at state 350 will return to state 300. Use the former
   * for states which count as checkpoints, and the latter for intermediary states which 
   * aren't important enough to warrant checkpoints.
   */
  public int state = 0;

  /**
   * Starts the Quest from scratch.
   */
  public void start() {
    string scene = getSceneForState(0);
    if (scene == null) setState(0);
    else SceneTransition.fadeTo(scene, () => { setState(0); });
  }

  /**
   * Starts the Quest from a previously saved state.
   */
  public void resume(Hashtable previousState) {
    state = (int) previousState[KEY_STATE];

    string scene = getSceneForState(state);

    if (scene == null) {
      if (previousState.ContainsKey(KEY_PLAYER)) {
        Player.SINGLETON.enabled = true;
        Player.SINGLETON.setState(previousState[KEY_PLAYER] as PlayerState);
      }
      setupState(state, previousState);
      handleState(state);
    }
    else SceneTransition.fadeTo(scene, () => {
      if (previousState.ContainsKey(KEY_PLAYER)) {
        Player.SINGLETON.enabled = true;
        Player.SINGLETON.setState(previousState[KEY_PLAYER] as PlayerState);
      }
      // TODO automatically load:
      // - conversation line (maybe)
      // - girl positions & states
      // - weapon / inventory states (e.g. can be equipped)

      setupState(state, previousState);
      handleState(state);
    });
  }

  /**
   * Returns the scene name required for the current state, or null if the subclass should handle the transition.
   */
  protected abstract string getSceneForState(int state);

  /**
   * Override to provide custom setup logic before resuming a state.
   * Called after loading the correct scene, but before fading in.
   */
  protected virtual void setupState(int state, Hashtable args) {}
  
  /**
   * Updates the overall state of the Quest.
   */
  protected void setState(int state) {
    this.state = state;
    handleState(state);
  }
  
  protected abstract void handleState(int state);

  public Hashtable save() {
    var args = new Hashtable {
      { Quest.KEY_STATE, Mathf.FloorToInt(state / 100f) * 100 } // Digits and tens are for states that should be repeated on load.
    };

    if (Player.SINGLETON != null) {
      args.Add(KEY_PLAYER, Player.SINGLETON.getState());
    }

    saveArgs(args);

    return args;
  }

  protected virtual void saveArgs(Hashtable args) {}

  public virtual void stop() {}

  public virtual void handleEvent(string eventName, GameObject trigger) {}
}
