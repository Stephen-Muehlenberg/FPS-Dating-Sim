using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_Competition : Quest {
  public static string NAME = "Competition";
  public override string name => NAME;

  protected override string getSceneForState(int state) {
    if (state == 0) return "cafe";
    else return "mission_competition";
  }

  protected override void handleState(int state) {
    if (state == 0) s000_openingConversation();
    else if (state == 100) s100_park();
    else throw new UnityException("Unknown state " + state);
  }

  private void s000_openingConversation() {
    Character.setPositions(mcPos: new Vector3(0, 0, -3),
                           mcRot: Quaternion.identity,
                           rosePos: new Vector3(-1, 0, -2),
                           mayPos: new Vector3(-0.35f, 0, -1.7f),
                           vanessaPos: new Vector3(0.35f, 0, -1.7f),
                           fizzyPos: new Vector3(1, 0, -2));

    new Conversation()
      .text(Character.MAY, "Ok, let's get back to saving the world!")
      .text(Character.VANESSA, "Thank you for your help, MC. Maybe we'll visit again later? In the meantime, stay safe.")
      .text(Character.FIZZY, "Wait, he should totes come with us. Much safer with magic-gun-ladies than alone.")
      .text(Character.ROSE, "Yeah I was gonna say. He's a surprisingly good shot. Like, better than the rest of you. We should keep him.")
      .text(Character.VANESSA, "Fair points, though it's up to MC.")
      .text(Character.MC, "Ok, let's get back to saving the world!")
      .text(Character.MC_NARRATION, "The girls gave a cheer, and we all left the cafe.")
      .text("It soon became clear we'd killed every monster in the vicinity. We ended up wondering over to a nearby park.")
      .show(() => { SceneTransition.fadeTo("mission_competition", () => { setState(100); }); });
  }

  private void s100_park() {

  }
}
