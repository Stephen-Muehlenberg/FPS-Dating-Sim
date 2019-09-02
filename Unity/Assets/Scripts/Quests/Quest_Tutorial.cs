using System.Collections;
using UnityEngine;

public class Quest_Tutorial : Quest {
  public static string NAME = "Tutorial";
  public override string name => NAME;

  public override void start(Hashtable args) {
    state = (int) args.getOrDefault(Quest.KEY_STATE, 0);

    setState(state);
  }

  public override Hashtable save() {
    return new Hashtable {
      { Quest.KEY_STATE, state }
    };
  }

  private void setState(int state) {
    if (state == 0) setupScene();
    else throw new UnityException("Unknown state " + state);
  }

  private void setupScene() {
    state = 0;

    SceneTransition.fadeTo("mission_tutorial",
      () => {
        Player.SINGLETON.firstPersonController.move.inputEnabled = false;
        Player.SINGLETON.firstPersonController.jump.inputEnabled = false;
        Player.SINGLETON.GetComponent<GunSwitch>().enabled = false;
        MainCanvas.transform.Find("Crosshair").GetComponent<CanvasGroup>().alpha = 0;
        MainCanvas.transform.Find("Health Bar").GetComponent<CanvasGroup>().alpha = 0;
      },
      () => {
        new Conversation()
          .text(Character.MC_NARRATION, "So here I am, sneaking out the back door, loaded up with guns who also happen to be girls.")
          .text(Character.MC_NARRATION, "And I was hoping this morning would get <i>less</i> weird.")
          .show(() => {
            CurrentQuestMessage.set("Escape through the back alleys");
            Player.SINGLETON.firstPersonController.move.inputEnabled = true;
            Player.SINGLETON.firstPersonController.jump.inputEnabled = true;
            Player.SINGLETON.StartCoroutine(MainCanvas.transform.Find("Crosshair").GetComponent<CanvasGroup>().fade(1, 1.5f));
            Player.SINGLETON.StartCoroutine(MainCanvas.transform.Find("Health Bar").GetComponent<CanvasGroup>().fade(1, 1.5f));
          });
      }
    );
  }

  public override void handleEvent(string eventName, GameObject trigger) {
    if (eventName == "1st enemy trigger") trigger1stEnemyGroup();
    else if (eventName == "2nd enemy trigger") trigger2ndEnemyGroup();
    else if (eventName == "3rd enemy trigger") trigger3rdEnemyGroup();
    else if (eventName == "End tutorial") endTutorial();
  }

  private void trigger1stEnemyGroup() {
    Player.SINGLETON.StartCoroutine(encounterFirstEnemiesEvent());
  }

  private IEnumerator encounterFirstEnemiesEvent() {
    var monsters = new Monster[] {
      MonstersController.findByName("Group 1 Torch 1"),
      MonstersController.findByName("Group 1 Torch 2")
    };
    foreach (Monster monster in monsters) {
      monster.GetComponent<FollowPlayer>().enabled = true;
    }

    // TODO play monster roar

    yield return new WaitForSeconds(1.2f);

    new CombatDialog()
      .message(Character.MC, "So much for escaping undetected.\nLadies? Little help?")
      .performAction(() => {
        Weapons.MACHINE_GUN.equip();
        Player.SINGLETON.StartCoroutine(showObjectiveWithDelay());
      })
      .message(Character.MAY, "Gotcha covered. Just point and shoot.")
      .setPriority(CombatDialog.Priority.HIGH)
      .message(Character.MAY, "Primary fire's a burst. Alt fire's full auto.")
      .message(Character.MAY, "Try not to use that one too much?")
      .message(Character.MAY, "It's, uh. Tiring.")
      .show(CombatDialog.Priority.MAX);
  }

  // Wait a bit before updating the objective, to make it coincide with the dialog
  private IEnumerator showObjectiveWithDelay() {
    float time = 0f;
    while (time < 1.6f) {
      if (!TimeUtils.gameplayPaused) time += Time.deltaTime;
      yield return null;
    }

    CurrentQuestMessage.set("[Left mouse]: Primary fire, [Right mouse]: Alt fire");
  }

  private void trigger2ndEnemyGroup() {
    var monsters = new Monster[] {
      MonstersController.findByName("Group 2 Torch 1"),
      MonstersController.findByName("Group 2 Torch 2"),
      MonstersController.findByName("Group 2 Torch 3"),
      MonstersController.findByName("Group 2 Torch 4"),
      MonstersController.findByName("Group 2 Torch 5")
    };
    foreach (Monster monster in monsters) {
      monster.GetComponent<Awareness>().enabled = true;
      monster.GetComponent<IdleBehaviour>().enabled = true;
    }
  }

  private void trigger3rdEnemyGroup() {
    var monsters = new Monster[] {
      MonstersController.findByName("Group 3 Hellhound 1"),
  //    MonstersController.findByName("Group 3 Hellhound 2"),
     // MonstersController.findByName("Group 3 Hellhound 3"),
    //  MonstersController.findByName("Group 3 Hellhound 4"),
      MonstersController.findByName("Group 3 Torch 1"),
      MonstersController.findByName("Group 3 Torch 2"),
      MonstersController.findByName("Group 3 Torch 3"),
      MonstersController.findByName("Group 3 Torch 4")
    };
    foreach (Monster monster in monsters) {
      monster.GetComponent<Awareness>().enabled = true;
      monster.GetComponent<IdleBehaviour>().enabled = true;
    }
    
    new CombatDialog()
      .performAction(() => {
        Weapons.SHOTGUN.equip();
      })
      .message(Character.ROSE, "Yo, I want some of that action.")
      .setPriority(CombatDialog.Priority.HIGH)
      .message(Character.ROSE, "Primary fire will mince anything at close range.")
      .message(Character.ROSE, "And if there's something you want <i>extra</i> dead, use alt fire.")
      .show(CombatDialog.Priority.MAX);
  }

  private void endTutorial() {
    new CombatDialog()
      .message(Character.MC, "That's as far as this tutorial mission goes.")
      .message(Character.MC, "Now returning to the cafe...")
      .show(CombatDialog.Priority.MAX, () => {
        QuestManager.start(Quest_CafeBreak1.NAME);
      });
  }
}
