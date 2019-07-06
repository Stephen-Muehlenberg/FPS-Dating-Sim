using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
        Player.SINGLETON.firstPersonController.move.inputDisabled = true;
        Player.SINGLETON.firstPersonController.jump.inputDisabled = true;
        Player.SINGLETON.GetComponent<GunSwitch>().enabled = false;
        MainCanvas.transform.Find("Crosshair").GetComponent<CanvasGroup>().alpha = 0;
        MainCanvas.transform.Find("Health Bar").GetComponent<CanvasGroup>().alpha = 0;
      },
      () => {
        new Conversation()
          .text(Conversation.Speaker.MC_NARRATION, "So here I am, sneaking out the back door, loaded up with guns who also happen to be girls.")
          .text(Conversation.Speaker.MC_NARRATION, "And I was hoping this morning would get <i>less</i> weird.")
          .show(() => {
            CurrentQuestMessage.set("Escape through the back alleys");
            Player.SINGLETON.firstPersonController.move.inputDisabled = false;
            Player.SINGLETON.StartCoroutine(MainCanvas.transform.Find("Crosshair").GetComponent<CanvasGroup>().fade(1, 1.5f));
            Player.SINGLETON.StartCoroutine(MainCanvas.transform.Find("Health Bar").GetComponent<CanvasGroup>().fade(1, 1.5f));
          });
      }
    );
  }

  private void encounterFirstEnemies() {
    Player.SINGLETON.StartCoroutine(encounterFirstEnemiesEvent());
  }

  private IEnumerator encounterFirstEnemiesEvent() {
    // TODO play monster roar

    // TODO all this find game object is disgusting, improve it
    GameObject torch1, torch2, torch3, torch4;
    torch1 = GameObject.Find("Torch (1)");
    torch2 = GameObject.Find("Torch (2)");
    torch3 = GameObject.Find("Torch (3)");
    torch4 = GameObject.Find("Torch (4)");

    torch1.GetComponent<FollowPlayer>().enabled = true;
    torch2.GetComponent<FollowPlayer>().enabled = true;
    torch3.GetComponent<FollowPlayer>().enabled = true;
    torch4.GetComponent<FollowPlayer>().enabled = true;

    yield return new WaitForSeconds(1.2f);

    torch1.GetComponent<NavMeshAgent>().isStopped = true;
    torch2.GetComponent<NavMeshAgent>().isStopped = true;
    torch3.GetComponent<NavMeshAgent>().isStopped = true;
    torch4.GetComponent<NavMeshAgent>().isStopped = true;

    TimeUtils.gameplayPaused = true;
    Player.SINGLETON.firstPersonController.move.inputDisabled = true;
    CurrentQuestMessage.clear();

    new Conversation()
      .text(Conversation.Speaker.MC, "So much for escaping undetected. Uhh, ladies? Little help.")
      .text(Conversation.Speaker.MAY, "Monsters already? Darnit, that was quick.")
      .text(Conversation.Speaker.MAY, "Ok, I need you to grab me and spray me all over!")
      .wait(0.75f)
      .text(Conversation.Speaker.MC, "Uhhhh…")
      .text(Conversation.Speaker.ROSE, "Pfff, what?")
      .text(Conversation.Speaker.MAY, "Bullets! Spray bullets! All over the monsters!")
      .text(Conversation.Speaker.MAY, "Just grab me, aim, and shoot. Easy peasy.")
      .text(Conversation.Speaker.MAY, "And stop snickering, Rose!")
      .show(() => {
        CurrentQuestMessage.set("Hold [Middle mouse] or [Q] to open the weapon menu\nHighlight a weapon to select it");
        Weapons.MACHINE_GUN.canEquip = true;
        Weapons.SHOTGUN.canEquip = false;
        Weapons.SNIPER_RIFLE.canEquip = false;
        Weapons.GRENADE_LAUNCHER.canEquip = false;
        Player.SINGLETON.GetComponent<GunSwitch>().enabled = true;
        Weapons.equipEvents += onWeaponEquipped;
      });
  }

  private void onWeaponEquipped(Weapon weapon) {
    if (weapon == Weapons.MACHINE_GUN) {
      Weapons.equipEvents -= onWeaponEquipped;

      TimeUtils.gameplayPaused = false;
      Player.SINGLETON.firstPersonController.move.inputDisabled = false;

      GameObject.Find("Torch (1)").GetComponent<NavMeshAgent>().isStopped = false;
      GameObject.Find("Torch (2)").GetComponent<NavMeshAgent>().isStopped = false;
      GameObject.Find("Torch (3)").GetComponent<NavMeshAgent>().isStopped = false;
      GameObject.Find("Torch (4)").GetComponent<NavMeshAgent>().isStopped = false;

      CurrentQuestMessage.clear();

      new CombatDialog()
        .message(CombatDialog.Speaker.MAY, "[Left mouse] to fire a burst, and [Right mouse] for full auto fire.")
        .show(CombatDialog.Priority.HIGH, () => {
          CurrentQuestMessage.set("Fight your way through the back alleys");
        });
    }
  }

  private void triggerThirdEnemies() {
    GameObject.Find("Torch (7)").GetComponent<FollowPlayer>().enabled = true;
    GameObject.Find("Torch (6)").GetComponent<FollowPlayer>().enabled = true;
    new CombatDialog()
      // TODO wait until the monster is killed
//      .message(CombatDialog.Speaker.MAY, "Urgh, sorry, I'm no good with these blind corners.")
  //    .message(CombatDialog.Speaker.ROSE, "Yo, MC, lemme handle this.")
      .message(CombatDialog.Speaker.MC, "<i>TO DO: FINISH OFF THIS TUTORIAL</i>")
      .message(CombatDialog.Speaker.MC, "<i>NOW SKIPPING TO THE NEXT MISSION...</i>")
      .performAction(() => {
        //    Player.SINGLETON.firstPersonController.move.inputDisabled = true;

        //     CurrentQuestMessage.set("Hold [Middle mouse] or [Q] to select weapon");
        // TODO have the player equip rose
        //        Weapons.SHOTGUN.setEquipped(true, true);
        //        CurrentQuestMessage.set("[Left mouse] : Single shot\n[Right mouse] : double shot");
        QuestManager.start(Quest_BedStore.NAME);
      })
  //    .message(CombatDialog.Speaker.ROSE, "Let's go get 'em, MC.")
      .show(CombatDialog.Priority.MAX);
  }

  public override void handleEvent(string eventName, GameObject trigger) {
    if (eventName == "First enemy trigger") encounterFirstEnemies();
    else if (eventName == "Third enemy trigger") triggerThirdEnemies();
  }
}
