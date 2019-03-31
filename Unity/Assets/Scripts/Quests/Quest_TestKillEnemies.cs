using UnityEngine;
using System.Collections;

public class Quest_TestKillEnemies : Quest {
  public static string NAME = "TestKillEnemies";
  public override string name => NAME;

  private Vector3 rosePosition = new Vector3(3.33f, 0, -2.62f);
  private Vector3 mayPosition = new Vector3(-3.598f, 0, -9.048f);
  private Vector3 fizzyPosition = new Vector3(-3.464782f, 0, 11.11054f);
  private Vector3 vanessaPosition = new Vector3(-2.403352f, 0, 11.92565f);

  private int monstersKilled = 0;

  public override void start(Hashtable args) {
    state = (int) args.getOrDefault(Quest.KEY_STATE, 0);
    SceneTransition.fadeTo(sceneForState(state), () => { setState(state); });
  }

  public override Hashtable save() {
    return new Hashtable {
      { Quest.KEY_STATE, state }
    };
  }

  public void setState(int state) {
    this.state = state;
    if (state == 0) setupScene();
    else if (state == 10) onQuestObtained();
    else if (state == 20) setupSuburbia();
    else if (state == 100) failureReturnToCafe();
    else if (state == 200) successfullyReturnToCafe();
  }

  private string sceneForState(int sceneState) {
    return sceneState == 20 ? "suburbia" : "cafe";
  }

  private void setupOtherActors() {
    Actors.getMay()
      .setPosition(mayPosition)
      .setInteraction("Talk", (mayTarget) => {
        if (state == 0) {
          new Conversation()
          .text(Conversation.Speaker.MAY, "Hey MC, how's it going?")
          .text("I <i>think</i> Rose was looking for you. She's probably antsy to go kill some more monsters.")
          .show();
        }
        else if (state == 10) {
          new Conversation()
          .text(Conversation.Speaker.MAY, "Time to shoot some more bad guys?")
          .show();
        }
        else if (state == 100) {
          new Conversation()
          .text(Conversation.Speaker.MAY, "Sorry about that.")
          .text("We'll shoot harder next time to keep you safe, I promise.")
          .show();
        }
        else if (state == 200) {
          new Conversation()
          .text(Conversation.Speaker.MAY, "Good work, MC. You didn't die horribly or anything!")
          .show();
        }
      });

    Actors.getFizzy()
      .setPosition(fizzyPosition)
      .setInteraction("Talk", (fizzyTarget) => {
        new Conversation()
        .speed(Conversation.Speed.FAST)
        .text(Conversation.Speaker.FIZZY, "Hey <b>hey!</b>")
        .text("It's my favourite monster-killing lady-toting cafe guy!")
        .show();
      });

    Actors.getVanessa()
      .setPosition(vanessaPosition)
      .setInteraction("Talk", (vanessaTarget) => {
        new Conversation()
        .text(Conversation.Speaker.VANESSA, "Hmm? Oh, hello MC.")
        .text("We were just discussing the lack of decor in the back half of this establishment.")
        .text("Why <i><b>is</b></i> it so sparse back here?")
        .show();
      });
  }

  // -- 0 --
  public void setupScene() {
    Actors.getRose()
      .setPosition(rosePosition)
      .setInteraction("Talk", (roseTarget) => {
        new Conversation()
        .text(Conversation.Speaker.ROSE, "Hey.")
        .text("There's like fifty monsters out the front causing a ruckus, and it's giving me a <i>headache</i>.")
        .text("Wanna go kill 'em?")
        .show(() => { setState(10); });
      });

    setupOtherActors();
  }

  // -- 10 --
  private void onQuestObtained() {
    GameObject.Find("Front Door").GetComponent<LookTarget>()
      .setInteraction("Exit Cafe", (_) => {
        SceneTransition.fadeTo("suburbia", () => { setState(20); });
      });

    Actors.getRose()
      .setPosition(rosePosition)
      .setInteraction("Talk", (roseTarget) => {
        new Conversation()
        .text(Conversation.Speaker.ROSE, "What?")
        .text("Those monsters aren't gonna kill themselves.")
        .text("Go on, get!")
        .show();
      });

    setupOtherActors();
  }

  // -- 20 --
  private void setupSuburbia() {
    // TODO display monster kill counter
    MonstersController.OnMonstersChanged += onMonsterChangedListener;
    PlayerDeath.onDeath += onPlayerDeath;
  }

  private void onPlayerDeath() {
    PlayerDeath.onDeath -= onPlayerDeath;
    SceneTransition.fadeTo("cafe", () => { setState(100); });
  }

  private void onMonsterChangedListener(Monster monster, bool added, int monsterCount) {
    if (added) return;

    monstersKilled++;

    if (monstersKilled == 50) {
      MonstersController.OnMonstersChanged -= onMonsterChangedListener;
      SceneTransition.fadeTo("cafe", () => { setState(200); });
    }
  }

  // -- 100 --
  private void failureReturnToCafe() {
    Actors.getRose()
      .setPosition(rosePosition)
      .setInteraction("Talk", (_) => {
        new Conversation()
        .text(Conversation.Speaker.ROSE, "Uhhh... are you ok?")
        .text("That looked kinda painful.")
        .text("Maybe we should take a break for today. You look pretty banged up.")
        .text("If you <i>really</i> wanna try again, go back to the main menu and start a new game, 'kay?")
        .show();
      });

    setupOtherActors();
  }

  // -- 200 --
  private void successfullyReturnToCafe() {
    Actors.getRose()
      .setPosition(rosePosition)
      .setInteraction("Talk", (_) => {
        new Conversation()
        .text(Conversation.Speaker.ROSE, "Phew, that was fun!")
        .text("We should do that again sometime.")
        .text("But, uh, not today. Gotta start a new game if you wanna go again.")
        .show();
      });

    setupOtherActors();
  }
}
