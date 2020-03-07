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
   * Starts the Quest.
   * If a previousState is provided then it resumes the quest from that point.
   */
  public void start(Hashtable previousState = null) {
    int state = 0;

    if (previousState == null) previousState = new Hashtable(); // Make hashtable non-null so children don't have to check if it's null
    else if (previousState.Contains(KEY_STATE)) state = (int) previousState[KEY_STATE];

    initialise(state, previousState);
  }

  /**
   * Called on quest start or load to handle initialisation.
   * In most cases, child classes should rely on initialiseScene() to set everything up.
   */
  protected abstract void initialise(int state, Hashtable args);
  
  protected void setUpScene(int state,
                            string scene,
                            bool autoFadeIn = true,
                            bool gameplayInitiallyPaused = false,
                            Vector3? mcPosition = null,
                            Quaternion? mcRotation = null,
                            Vector3? rosePosition = null,
                            Vector3? mayPosition = null,
                            Vector3? vanessaPosition = null,
                            Vector3? fizzyPosition = null,
                            bool lookEnabled = true,
                            bool moveEnabled = true,
                            bool jumpEnabled = true,
                            bool combatHudEnabled = false,
                            Weapon weaponEquipped = null,
                            string questMessage = null,
                            bool animateQuestMessageIn = false,
                            Callback onTransitionComplete = null,
                            Callback onFadeComplete = null) {
    // Pause until everything is loaded
    TimeUtils.mode = TimeUtils.TimeMode.PAUSED;

    SceneTransition.changeTo(
      scene: scene,
      fadeOut: true, // Quest should only be changed via menu which handles fade out for us
      fadeIn: autoFadeIn,
      onSceneLoaded: () => {
        Character.setPositions(mcPos: mcPosition,
                               mcRot: mcRotation,
                               rosePos: rosePosition,
                               mayPos: mayPosition,
                               vanessaPos: vanessaPosition,
                               fizzyPos: fizzyPosition);
        Player.SINGLETON.enabled = true;
        Player.SINGLETON.firstPersonController.setState(lookEnabled: lookEnabled,
                                                        moveEnabled: moveEnabled,
                                                        jumpEnabled: jumpEnabled);
        Player.SINGLETON.gunSwitch.equip(weapon: weaponEquipped, playEffects: false);
        Weapons.list.ForEach(weapon => weapon.canEquip = true);
        CombatDialogManager.clearAllMessages();
        CurrentQuestMessage.set(message: questMessage, animateIn: animateQuestMessageIn);
        HighlightMonsters.clearHighlights();
        // TODO toggle combat HUD

        onTransitionComplete?.Invoke();
        TimeUtils.mode = /*gameplayInitiallyPaused ? TimeUtils.TimeMode.DIALOG :*/ TimeUtils.TimeMode.GAMEPLAY;
      },
      onFadeComplete: () => {
        onFadeComplete?.Invoke();
        setState(state);
      }
    );
  }

  /**
   * Updates the overall state of the Quest.
   */
  protected void setState(int state) {
    this.state = state;
    // TODO asynchronously autosave if state is a checkpoint (multiple of 100)
    handleState(state);
  }
  
  protected abstract void handleState(int state);

  public Hashtable save() {
    // Only states which are multiples of 100 are checkpoints. 
    int saveState = Mathf.FloorToInt(state / 100f) * 100; // Round down to the nearest multiple of 100

    var args = new Hashtable {
      { Quest.KEY_STATE, saveState }
    };

    if (Player.SINGLETON != null) {
      args.Add(KEY_PLAYER, Player.SINGLETON.getState());
    }

    saveArgs(args); // Quest-specific arguments

    return args;
  }

  protected virtual void saveArgs(Hashtable args) {}

  /**
   * Called just before the Quest is destroyed. Use this callback for any teardown.
   */
  public virtual void stop() {}

  public virtual void handleEvent(string eventName, GameObject trigger) {}
}
