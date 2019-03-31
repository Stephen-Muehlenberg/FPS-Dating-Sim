﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Quest_BedStore : Quest {
  private Vector3 playerConversationPosition = new Vector3(156, 0, 95.5f);
  private Vector3 girlConversationPositionA = new Vector3(155, 0.2f, 97);
  private Vector3 girlConversationPositionB = new Vector3(155.6f, 0.2f, 97.5f);
  private Vector3 girlConversationPositionC = new Vector3(156.4f, 0.2f, 97.4f);
  private Vector3 girlConversationPositionD = new Vector3(157, 0.2f, 97);

  public static string NAME = "BedStore";
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

  public override void stop() {
    LocationMarker.clear();
  }

  public void setState(int state) {
    this.state = state;
    if (state == 0) SceneTransition.fadeTo("cafe", null, () => { startConversation(); });
    else if (state == 10) SceneTransition.fadeTo("mission_bed_store", () => { setupLevel(); }); // TODO probably show some sort of text once you load, directing you
    else if (state == 20) setupClearNearbyEnemies();
    else if (state == 30) setupOutsideDialog();
    else if (state == 40) setupDefend();
    else if (state == 50) endMission();
    else if (state == 60) returnToCafe();
    else throw new UnityException("Unhandled quest state " + state);
  }

  public override void handleEvent(string eventName, GameObject trigger) {
    if (eventName == "Wrong way 1") eventWrongWay1();
    else if (eventName == "Wrong way 2") eventWrongWay2();
    else if (eventName == "Fire") eventFireComment();
    else if (eventName == "Nice park") eventNicePark();
    else if (eventName == "Nice car") eventNiceCar();
    else if (eventName == "Approach store") eventApproachStore();
    else if (eventName == "Dont leave") eventDontLeaveStore();
  }

  // -- 0 --
  private void startConversation() {
    // Disable player movement
    var firstPersonController = Player.SINGLETON.GetComponent<FirstPersonModule.FirstPersonController>();
    firstPersonController.move.inputDisabled = true;
    firstPersonController.jump.inputDisabled = true;

    new Conversation()
      .wait(1.25f)
      .text(Conversation.Speaker.MC_NARRATION, "<i>I stagger back into the café with four guns that each hop off and transform back into women.</i>")
      .wait(0.3f)
      .performAction(() => {
        Actors.getFizzy().setPosition(new Vector3(0.3787175f, 0, -5.961192f))
        .GetComponent<ParticleSystem>().Play();
      })
      .wait(0.4f)
      .performAction(() => {
        Actors.getRose().setPosition(new Vector3(-1.23f, 0, -6.918f))
        .GetComponent<ParticleSystem>().Play();
      })
      .wait(0.4f)
      .performAction(() => {
        Actors.getMay().setPosition(new Vector3(-0.5890785f, 0, -6.104723f))
.GetComponent<ParticleSystem>().Play();
      })
      .wait(0.4f)
      .performAction(() => {
        Actors.getVanessa().setPosition(new Vector3(1.156008f, 0, -6.55563f))
.GetComponent<ParticleSystem>().Play();
      })
      .wait(0.9f)
      .text("<i>Because apparently that is my life now.</i>")
      .wait(0.2f)
      .text(Conversation.Speaker.MAY, "Ugh. I'm glad Mission Time is done with.")
      .text(Conversation.Speaker.MC, "Mission Time, eh? So that was our first official mission?")
      .text(Conversation.Speaker.FIZZY, "Sure was! Did you have fun? I had fun!")
      .text(Conversation.Speaker.VANESSA, "I did not, particularly. But what's done is done.")
      .text("The sun's going down; let's take a moment to eat some food and then sleep for the night.")
      .speed(Conversation.Speed.FAST)
      .text(Conversation.Speaker.FIZZY, "I'll grab the food!")
      .speed(Conversation.Speed.NORMAL)
      .text("Raiding fridges is my favorite pasttime.")
      .text(Conversation.Speaker.VANESSA, "I can fetch the blankets and such.")
      .text("Where do you keep them, MC? In the back?")
      .speed(Conversation.Speed.SLOW)
      .text(Conversation.Speaker.MC, "...Um.")
      .speed(Conversation.Speed.NORMAL)
      .text(Conversation.Speaker.VANESSA, "Hmm?")
      .text(Conversation.Speaker.MC, "...It's a café. Why would we have blankets?")
      .text(Conversation.Speaker.VANESSA, "...Is there not a deluge of homeless people inhabiting your lounge every night?")
      .text(Conversation.Speaker.FIZZY, "Uhh. Vanessa, I think you've heard some weird things about city life.")
      .text(Conversation.Speaker.VANESSA, "That is entirely possible.")
      .text("This is embarrassing.")
      .text(Conversation.Speaker.ROSE, "Wait, there's no blankets? No pillows?")
      .text(Conversation.Speaker.MAY, "Is there anything, like, next door? Somewhere we can get that stuff?")
      .text(Conversation.Speaker.MC, "Um, there's a home furnishing store like two blocks from here.")
      .text(Conversation.Speaker.MAY, "Goddammit. <i>Mission Time's back on!</i>")
      .wait(0.4f)
      .show(() => setState(10));
  }

  // -- 10 --
  private void setupLevel() {
    var locationMarkerTransform = new GameObject("Bed Store Marker").transform;
    locationMarkerTransform.position = new Vector3(156, 2.5f, 95);
    LocationMarker.add(locationMarkerTransform);
    CurrentQuestMessage.set("Head to the furniture store");
    Weapons.MACHINE_GUN.setEquipped(true);
  }

  // -- 20 --
  private void setupClearNearbyEnemies() {
    CurrentQuestMessage.set("Clear the area of monsters");
    HighlightMonsters.highlightNear(new Vector3(156, 2.5f, 95), 50f, () => {
      HighlightMonsters.clearHighlights();
      new CombatDialog()
        .wait(2.5f)
        .message(CombatDialog.Speaker.MAY, "Ok I guess that's safe enough.")
        .show(CombatDialog.Priority.HIGH, () => { setState(30); });
    });
  }

  // -- 30 --
  private void setupOutsideDialog() {
    // Make player invincible so they don't die during fade, e.g. from stray projectiles
    Player.SINGLETON.GetComponent<PlayerHealth>().setGodMode(true);

    CurrentQuestMessage.clear();

    ScreenFade.fadeOut(() => {
      // Disable game stuff, enter conversation mode
      MonstersController.killAll();
      foreach (GameObject projectile in GameObject.FindGameObjectsWithTag("Projectile")) { Object.Destroy(projectile); }
      CombatDialogManager.clearAllMessages();
      Weapons.currentlyEquipped.setEquipped(false);
      
      // Position actors
      Actors.getRose().setPosition(girlConversationPositionA);
      Actors.getMay().setPosition(girlConversationPositionB);
      Actors.getVanessa().setPosition(girlConversationPositionC);
      Actors.getFizzy().setPosition(girlConversationPositionD);

      // Reposition, reset, and restrict the player
      var player = Player.SINGLETON;
      player.transform.position = playerConversationPosition;
      player.transform.rotation = Quaternion.identity; // Identity just happens to be facing the direction we want
      var firstPersonController = player.GetComponent<FirstPersonModule.FirstPersonController>();
      firstPersonController.reset();
      firstPersonController.move.inputDisabled = true;
      firstPersonController.jump.inputDisabled = true;

      ScreenFade.fadeIn(() => {
        new Conversation()
          .wait(1.5f)
          // TODO have girl who likes you most say "good job", restoring your health
          .text(Conversation.Speaker.MAY, "Alright, MC can stay out here and defend the store.")
          .text("But two of us should go in to pick out some stuff and carry it out.")
          .text(Conversation.Speaker.MC, "Pick out some stuff? Just get the first five pillows and blankets you find.")
          .text(Conversation.Speaker.ROSE, "I'm not using one of those scratchy-ass wool blankets. Those things are bullshit.")
          .text(Conversation.Speaker.MC, "Oh, yeah, that kind of stiff shit? That's fair, actually, those are pretty bullshit.")
          .text(Conversation.Speaker.FIZZY, "I don't want one of those pillows stuffed with feathers!")
          .text("I just stay up all night picking out the feathers through the pillowcase.")
          .text(Conversation.Speaker.ROSE, "I don't want Fizzy to have one of those either, 'cause then I have to listen to it all night.")
          .text(Conversation.Speaker.VANESSA, "Yes, yes. Can we resume the mission?")
          .text("MC, choose two of us to defend the store with, and the other two will scrounge through the store.")
          .show(() => {
            // TODO fluidly rotate it to face the girls.
            firstPersonController.look.inputDisabled = true;
            player.transform.rotation = Quaternion.identity;

            WeaponSelectMenu.select(2, 2, "Choose 2 girls to help defend the store", (selection) => {
              firstPersonController.look.inputDisabled = false;

              foreach (Weapon weapon in Weapons.array) {
                weapon.canEquip = false;
              }
              selection[0].canEquip = true;
              selection[1].canEquip = true;

              new Conversation()
                .text(Conversation.Speaker.MAY, "Ok, see you in a bit!")
                .show(() => { setState(40); });
            });
          });
      });
    });
  }

  // -- 40 --

  private void setupDefend() {
    ScreenFade.fadeOut(() => {
      Player.SINGLETON.GetComponent<FirstPersonModule.FirstPersonController>().enableAllInput();
      Player.SINGLETON.GetComponent<PlayerHealth>().setGodMode(false);
      Player.SINGLETON.GetComponent<GunSwitch>().equip(Weapons.randomEquipableWeapon());
      GameObject.Destroy(Actors.getRose().gameObject);
      GameObject.Destroy(Actors.getMay().gameObject);
      GameObject.Destroy(Actors.getVanessa().gameObject);
      GameObject.Destroy(Actors.getFizzy().gameObject);
      GameObject.Find("HordeToggle").GetComponent<EnableGameObjects>().enableAll();
      CurrentQuestMessage.set("Defend the store");

      ScreenFade.fadeIn(() => {
        MonstersController.OnMonstersChanged += onMonstersChanged;
      });
    });
  }

  private void onMonstersChanged(Monster monster, bool added, int monstersRemaining) {
    if (monstersRemaining == 0) {
      MonstersController.OnMonstersChanged -= onMonstersChanged;

      var dialog = new CombatDialog();
      var weapon = Weapons.currentlyEquipped;

      if (weapon == Weapons.SHOTGUN) {
        dialog = dialog.message(weapon.combatSpeaker, "Bam!")
          .message(weapon.combatSpeaker, "Alright, let's get back to the girls.");
      }
      else if (weapon == Weapons.MACHINE_GUN) {
        dialog = dialog.message(weapon.combatSpeaker, "Woo, gotcha!")
          .message(weapon.combatSpeaker, "Good work, MC. Now back to the store.");
      }
      else if (weapon == Weapons.SNIPER_RIFLE) {
        dialog = dialog.message(weapon.combatSpeaker, "Ah, that's the last of them.")
          .message(weapon.combatSpeaker, "Excellent. Let's rejoin the others.");
      }
      else {
        dialog = dialog.message(weapon.combatSpeaker, "Haha! <i>Boom!<i>")
          .message(weapon.combatSpeaker, "Okey dokey, guess that's the last of 'em.");
      }

      dialog.show(CombatDialog.Priority.MAX, () => { setState(50); });
    }
  }

  // -- 50 --

  private void endMission() {
    // Make player invincible so they don't die during fade, e.g. from stray projectiles
    Player.SINGLETON.GetComponent<PlayerHealth>().setGodMode(true);

    ScreenFade.fadeOut(() => {
      // Disable game stuff, enter conversation mode
      foreach (GameObject projectile in GameObject.FindGameObjectsWithTag("Projectile")) { Object.Destroy(projectile); }
      CombatDialogManager.clearAllMessages();
      Weapons.currentlyEquipped.setEquipped(false);

      // Position actors based on whether they fought or scrounged
      var defenders = new List<Actor>();
      var scroungers = new List<Actor>();
      if (Weapons.SHOTGUN.canEquip) { defenders.Add(Actors.getRose()); } else { scroungers.Add(Actors.getRose()); }
      if (Weapons.MACHINE_GUN.canEquip) { defenders.Add(Actors.getMay()); } else { scroungers.Add(Actors.getMay()); }
      if (Weapons.SNIPER_RIFLE.canEquip) { defenders.Add(Actors.getVanessa()); } else { scroungers.Add(Actors.getVanessa()); }
      if (Weapons.GRENADE_LAUNCHER.canEquip) { defenders.Add(Actors.getFizzy()); } else { scroungers.Add(Actors.getFizzy()); }

      var firstDefenderOnLeft = Random.Range(0, 2) == 1;
      defenders[0].setPosition(firstDefenderOnLeft ? girlConversationPositionA : girlConversationPositionD);
      defenders[1].setPosition(firstDefenderOnLeft ? girlConversationPositionD : girlConversationPositionA);
      var firstScroungerOnLeft = Random.Range(0, 2) == 1;
      scroungers[0].setPosition(firstScroungerOnLeft ? girlConversationPositionB : girlConversationPositionC);
      scroungers[1].setPosition(firstScroungerOnLeft ? girlConversationPositionC : girlConversationPositionB);

      // Reposition, reset, and restrict the player
      var player = Player.SINGLETON;
      player.transform.position = playerConversationPosition;
      player.transform.rotation = Quaternion.identity; // Identity just happens to be facing the direction we want
      var firstPersonController = player.GetComponent<FirstPersonModule.FirstPersonController>();
      firstPersonController.reset();
      firstPersonController.move.inputDisabled = true;
      firstPersonController.jump.inputDisabled = true;

      ScreenFade.fadeIn(() => {
        var weaponA = Weapons.randomNonEquipableWeapon();
        var weaponB = Weapons.randomEquipableWeapon();
        var scrounger = Weapons.randomNonEquipableWeapon().conversationSpeaker;
        var defender = Weapons.randomEquipableWeapon().conversationSpeaker;

        new Conversation()
          .wait(1.5f)
          .variableText(
            scrounger,
            new string[] {"Alright, we've got enough shit.", "Now I guess you've gotta shoot everything between here and homebase for us."},
            new string[] {"Alright, we've got a trolley full of sleepytime stuff.", "Mission Time, part three! Back to base!"},
            new string[] {"We've gathered enough. Our task is complete.", "Shall we fight our way back to the café?"},
            new string[] {"We're back! And we got the <b>best</b> blankets and stuff.", "Time to blast our way back home!"})
          .variableText(defender,
            new string[] {"I'm not sure what you thought we were doing out here, but everything's dead now.", "We can just walk back."},
            new string[] {"Yeeeaahh, we <i>kinda</i> killed all the monsters already.", "Surprise?"},
            new string[] {"Actually, I do believe we've killed everything that could be a threat in the vicinity."},
            new string[] {"Actually, I'm pretty sure everything around here is super dead.", "No need to thank us!"})
          .variableText(scrounger,
            new string[] {"Oh.", "Cool."},
            new string[] {"Oh.", "Sweet!"},
            new string[] {"Ah.", "Let's carry on then."},
            new string[] {"Ooh, nice!", "Alright, then let's go!"})
          .show(() => { setState(60); });
      });
    });
  }

  // -- 60 --

  private void returnToCafe() {
    SceneTransition.fadeTo("cafe",
      () => {
        var firstPersonController = Player.SINGLETON.GetComponent<FirstPersonModule.FirstPersonController>();
        firstPersonController.move.inputDisabled = true;
        firstPersonController.jump.inputDisabled = true;
        Actors.getFizzy().setPosition(new Vector3(0.3787175f, 0, -5.961192f));
        Actors.getRose().setPosition(new Vector3(-1.23f, 0, -6.918f));
        Actors.getMay().setPosition(new Vector3(-0.5890785f, 0, -6.104723f));
        Actors.getVanessa().setPosition(new Vector3(1.156008f, 0, -6.55563f));
      },
      () => {
        // TODO choose who says what based on affinity
        Conversation.Speaker antagonist = Weapons.randomWeapon().conversationSpeaker;
        Conversation.Speaker protagonist;
        do {
          protagonist = Weapons.randomWeapon().conversationSpeaker;
        } while (antagonist == protagonist); // Re-roll until we get two different girls

        new Conversation()
          .wait(1.25f)
          .text(Conversation.Speaker.MAY, "Okay, for real now.")
          .text("No more Mission Time.")
          .text("Now it's Sleep Time.")
          .text(Conversation.Speaker.FIZZY, "What if we make it our mission to have a good night's sleep?")
          .text(Conversation.Speaker.MAY, "I do <i>not</i> have enough energy to play these games, woman.")
          .text(Conversation.Speaker.MC, "Okay, so should we set up in front here...?")
          .variableText(antagonist,
            new string[] { "Excuse me? 'We'?", "How about <i>you</i> take the front and <i>we'll</i> hang out in the back?" },
            new string[] { "Haah, <i>wait</i> a second.", "We just met, and all.", "No offense, but maybe you should sleep in the front and we'll take the back?" },
            new string[] { "Mmn, I'm not sure that would be proper.", "Perhaps you should sleep in the front while the rest of us are in the back of the shop?" },
            new string[] { "Wooooaaah there!", "All my sleepovers are strictly Girl Time. I don't want your cooties.", "Howzabout you take the front and we'll take the back?" }
          )
          .variableText(protagonist,
            new string[] { "Hey, how about we <i>not</i> be dicks?", "Don't leave him to the fuckin' wolves. If something attacks, he needs to be able to defend himself." },
            new string[] { "And let him get eaten alive when something breaks in? C'mon.", "Let's all take the back room, if only so he has something to defend himself with." },
            new string[] { "Come now, " + antagonist.name + ", there's no need for that.", "We can all sleep in the back.", "Last thing we need is for something to attack him while he's unable to defend himself." },
            new string[] { "Wha--?", "We just kicked <i>so</i> much butt with him and you want to kick him to the curb?", "Or... closer to the curb than us?", "Anyway. C'mon! If something busts in, he'll have nothing to defend himself!" }
          )
          .variableText(antagonist,
            new string[] { "Man... I guess.", "Whatever." },
            new string[] { "That's... totally true, yeah.", "Damnit, now I made things all awkward.", "Sorry about that." },
            new string[] { "I suppose that's true.", "Apologies, MC." },
            new string[] { "That is one hundred percent fair.", "Sorry, MC! I wasn't really thinking." }
          )
          .text(Conversation.Speaker.MC, "Plus, I have to leave room for all the homeless people.")
          .text(Conversation.Speaker.VANESSA, "Oh, very funny.")
          .show(() => { SceneTransition.fadeTo("main_menu"); });
      });
  }

  // -- EVENTS --

  private void eventWrongWay1() {
    new CombatDialog()
      .message(CombatDialog.Speaker.MAY, "Left or right?")
      .message(CombatDialog.Speaker.MC, "Right. I think.")
      .show(CombatDialog.Priority.MEDIUM);
  }

  private void eventWrongWay2() {
    new CombatDialog()
      .message(CombatDialog.Speaker.MC, "Yeah no, definitely the other way.")
      .show(CombatDialog.Priority.MEDIUM);
  }

  private void eventFireComment() {
    new CombatDialog()
      .message(CombatDialog.Speaker.FIZZY, "Woah!")
      .wait(3f)
      .message(CombatDialog.Speaker.ROSE, "Damn, what a mess.")
      .message(CombatDialog.Speaker.VANESSA, "Now I see why you cut through that side street, MC.")
      .message(CombatDialog.Speaker.VANESSA, "The road is completely blocked.")
      .show(CombatDialog.Priority.LOW);
  }

  private void eventNiceCar() {
    new CombatDialog()
      .message(CombatDialog.Speaker.MAY, "That's a nice car.")
      .message(CombatDialog.Speaker.FIZZY, "How can you even tell?")
      .message(CombatDialog.Speaker.FIZZY, "<i>They all look the same!</i>")
      .show(CombatDialog.Priority.TRIVIAL);
  }

  private void eventNicePark() {
    new CombatDialog()
      .message(CombatDialog.Speaker.VANESSA, "This is pleasant.")
      .message(CombatDialog.Speaker.FIZZY, "The park, or the monster killing?")
      .message(CombatDialog.Speaker.VANESSA, "The park.")
      .message(CombatDialog.Speaker.VANESSA, "Well, mostly the park.")
      .show(CombatDialog.Priority.LOW);
  }

  private void eventApproachStore() {
    LocationMarker.clear();
    CurrentQuestMessage.clear();

    if (state == 10) {
      if (MonstersController.monstersNear(new Vector3(156, 0, 96), 35f) > 0) {
        new CombatDialog()
          .message(CombatDialog.Speaker.MC, "This is the place.")
          .message(CombatDialog.Speaker.MAY, "Great, but before we go in, let's finish off any nearby monsters.")
          .show(CombatDialog.Priority.MAX, () => { setState(20); });
      }
      else {
        new CombatDialog()
          .message(CombatDialog.Speaker.MC, "This is the place.")
          .show(CombatDialog.Priority.MAX, () => { setState(30); });
      }
    }
  }

  private void eventDontLeaveStore() {
    var dialog = new CombatDialog();

    Weapon randomWeapon = Weapons.randomEquipableWeapon();

    if (randomWeapon == Weapons.SHOTGUN) {
      dialog = dialog.message(CombatDialog.Speaker.ROSE, "Yo, we should probably stay near the store.")
        .message(CombatDialog.Speaker.ROSE, "Can't let the monsters interrupt Pillow Time, can we?");
    }
    else if (randomWeapon == Weapons.MACHINE_GUN) {
      dialog = dialog.message(CombatDialog.Speaker.MAY, "Hold on there, stud.")
        .message(CombatDialog.Speaker.MAY, "Let's keep the killing <i>near</i> the store.")
        .message(CombatDialog.Speaker.MAY, "As fun as this is, we have a job to do.");
    }
    else if (randomWeapon == Weapons.SNIPER_RIFLE) {
      dialog = dialog.message(CombatDialog.Speaker.VANESSA, "I feel I should point out we're rather far from the store.")
        .message(CombatDialog.Speaker.VANESSA, "I'm sure the girls are capable of handling themselves...")
        .message(CombatDialog.Speaker.VANESSA, "...but I suppose we <i>ought</i> to stay close.");
    }
    else {
      dialog = dialog.message(CombatDialog.Speaker.FIZZY, "Hahaa! Hey girls, did you see...")
        .message(CombatDialog.Speaker.FIZZY, "Oh shoot, I forgot, they're totally in the store.")
        .message(CombatDialog.Speaker.FIZZY, "We should probably stay near them.");
    }

    dialog.show(CombatDialog.Priority.HIGH);
  }
}