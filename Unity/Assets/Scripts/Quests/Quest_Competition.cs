using System.Collections;
using UnityEngine;

public class Quest_Competition : Quest {
  public static readonly string NAME = "Competition";
  public override string name => NAME;
  private static readonly string SCENE = "mission_competition";

  private static readonly string OBJECTIVE_CLEAR_PLAZA = "Kill all the monsters in the plaza";

  protected override void initialise(int state, Hashtable args) {
    setUpScene(
      state: state,
      scene: state == 0 ? "cafe" : SCENE,
      mcPosition: state == 0 ? new Vector3(0, 0, -3) // Cafe
                             : new Vector3(34, 0, 34), // Mission start
      mcRotation: state == 0 ? Quaternion.identity
                             : Quaternion.Euler(0, -135, 0),
      rosePosition: state == 0 ? new Vector3(-1, 0, -2)
                               : null as Vector3?,
      mayPosition: state == 0 ? new Vector3(-0.35f, 0, -1.7f)
                              : null as Vector3?,
      vanessaPosition: state == 0 ? new Vector3(0.35f, 0, -1.7f)
                                  : null as Vector3?,
      fizzyPosition: state == 0 ? new Vector3(1, 0, -2)
                                : null as Vector3?,
      lookEnabled: true,
      moveEnabled: state > 0,
      jumpEnabled: state > 0,
      weaponEquipped: state == 0 ? null : Weapons.SHOTGUN,
      questMessage: state == 0 ? null : OBJECTIVE_CLEAR_PLAZA,
      animateQuestMessageIn: true
    );
  }

  protected override void handleState(int state) {
    if (state == 0) s000_openingConversation();
    else if (state == 100) s100_park();
    else if (state == 200) s200_convo1();
    else throw new UnityException("Unknown state " + state);
  }

  private void s000_openingConversation() {
    new Conversation()
      .text(Character.MAY, "Ok, let's get back to saving the world!")
      .text(Character.VANESSA, "Thank you for your help, MC. Maybe we'll visit again later? In the meantime, stay safe.")
      .text(Character.FIZZY, "Wait, he should totes come with us. Much safer with magic-gun-ladies than alone.")
      .text(Character.ROSE, "Yeah I was gonna say. He's a surprisingly good shot. Like, better than the rest of you. We should keep him.")
      .text(Character.VANESSA, "Fair points, though it's up to MC.")
      .text(Character.MC, "Ok, let's get back to saving the world!")
      .text(Character.MC_NARRATION, "The girls gave a cheer, and we all left the cafe.")
      .text("It soon became clear we'd killed every monster in the vicinity. We ended up wondering over to the nearby shopping center.")
      .show(() => { SceneTransition.changeTo(scene: "mission_competition", onSceneLoaded: () => {
        Weapons.equip(Weapons.SHOTGUN);
        setState(100);
      }); });
  }

  private void s100_park() {
    CurrentQuestMessage.set(OBJECTIVE_CLEAR_PLAZA);
    MonstersController.OnMonstersChanged += (Monster monster, bool added, int monstersRemaining) => {
      if (monstersRemaining == 0) {
        CurrentQuestMessage.clear();
        setState(200);
      }
    };
  }

  private void s200_convo1() {
    new Conversation()
      .text(Character.NONE, "[The rest of the level hasn't been added yet.]")
      .text("[Skipping to next level...]")
      .show(() => { QuestManager.start(new Quest_BedStore()); });
  }
}
