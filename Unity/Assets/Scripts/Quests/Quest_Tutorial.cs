using System.Collections;
using System.Collections.Generic;
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
      // TODO
    };
  }

  private void setState(int state) {
    if (state == 0) setupScene();
    else throw new UnityException("Unknown state " + state);
  }

  private void setupScene() {
    state = 0;

    ScreenFade.fadeOut(() => {
      SceneTransition.swapTo("mission_backstreets", () => {
      });
    });
  }
}
