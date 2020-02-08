using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Quest_BedStore : Quest {
  public static readonly string NAME = "BedStore";
  public override string name => NAME;
  private const string SCENE_BED_STORE = "mission_bed_store";

  private const string KEY_DEF1 = "def1", KEY_DEF2 = "def2", KEY_SCR1 = "scr1", KEY_SCR2 = "scr2";
  private static readonly Vector3 STORE_POS_MC = new Vector3(156, 0, 95.5f),
                                  STORE_POS_A = new Vector3(155, 0.2f, 97),
                                  STORE_POS_B = new Vector3(155.6f, 0.2f, 97.5f),
                                  STORE_POS_C = new Vector3(156.4f, 0.2f, 97.4f),
                                  STORE_POS_D = new Vector3(157, 0.2f, 97);

  private Weapon defender1 = null;
  private Weapon defender2 = null;
  private Weapon scrounger1 = null;
  private Weapon scrounger2 = null;

  protected override void initialise(int state, Hashtable args) {
    int defender1Index = args.getOrDefault(KEY_DEF1, -1);
    int defender2Index = args.getOrDefault(KEY_DEF2, -1);
    int scrounger1Index = args.getOrDefault(KEY_SCR1, -1);
    int scrounger2Index = args.getOrDefault(KEY_SCR2, -1);

    defender1 = defender1Index >= 0 ? Weapons.list[defender1Index] : null;
    defender2 = defender2Index >= 0 ? Weapons.list[defender2Index] : null;
    scrounger1 = scrounger1Index >= 0 ? Weapons.list[scrounger1Index] : null;
    scrounger2 = scrounger2Index >= 0 ? Weapons.list[scrounger2Index] : null;

    Hashtable girlPositions = new Hashtable();
    if (state == 400) {
      girlPositions.Add(defender1, STORE_POS_A);
      girlPositions.Add(defender2, STORE_POS_D);
      girlPositions.Add(scrounger1, STORE_POS_B);
      girlPositions.Add(scrounger2, STORE_POS_C);
    }

    setUpScene(
      state: state,
      scene: (state >= 100 && state < 500) ? SCENE_BED_STORE : "cafe",
      mcPosition: state == 0 || state > 400 ? new Vector3(0, 0, -8.5f)
                : state >= 200 && state <= 400 ? STORE_POS_MC
                : (Vector3?) null,
      mcRotation: state == 200 || state == 400 ? Quaternion.identity 
                : state == 300 ? Quaternion.Euler(0, 180, 0)
                : (Quaternion?) null,
      rosePosition: state == 0 || state == 500 ? new Vector3(-1.23f, 0, -6.918f)
                  : state == 200 ? STORE_POS_A
                  : state == 400 ? (Vector3) girlPositions[Weapons.SHOTGUN]
                  : (Vector3?) null,
      mayPosition: state == 0 || state == 500 ? new Vector3(-0.58908f, 0, -6.104723f)
                 : state == 200 ? STORE_POS_B
                  : state == 400 ? (Vector3) girlPositions[Weapons.MACHINE_GUN]
                 : (Vector3?) null,
      vanessaPosition: state == 0 || state == 500 ? new Vector3(1.156008f, 0, -6.55563f)
                     : state == 200 ? STORE_POS_C
                  : state == 400 ? (Vector3) girlPositions[Weapons.SNIPER_RIFLE]
                     : (Vector3?) null,
      fizzyPosition: state == 0 || state == 500 ? new Vector3(0.378718f, 0, -5.961192f)
                   : state == 200 ? STORE_POS_D
                  : state == 400 ? (Vector3) girlPositions[Weapons.GRENADE_LAUNCHER]
                   : (Vector3?) null,
      moveEnabled: state == 100 || state == 300,
      jumpEnabled: state == 100 || state == 300,
      weaponEquipped: state == 100 ? Weapons.random()
                    : state == 300 ? (Random.value > 0.5 ? defender1 : defender2)
                    : null,
      questMessage: state == 100 ? "Head to the furniture store"
                  : state == 300 ? "Defend the store"
                  : null,
      onTransitionComplete: () => {
        // If the player's passed the initial wave of enemies, remove them.
        if (state >= 200 && state < 500) GameObject.Destroy(GameObject.Find("Monsters"));
      }
    );
  }

  protected override void handleState(int state) {
    this.state = state;
    if (state == 0) s000_startConversation();
    else if (state == 100) s100_setupLevel(); // TODO probably show some sort of text once you load, directing you
    else if (state == 110) s110_setupClearNearbyEnemies();
    else if (state == 200) s200_outsideDialog();
    else if (state == 300) s300_setupDefend();
    else if (state == 400) s400_endMission();
    else if (state == 500) s500_returnToCafe();
    else throw new UnityException("Unhandled quest state " + state);
  }

  protected override void saveArgs(Hashtable args) {
    if (defender2 == null) return; // Unless both defenders have been chosen, don't persist them
    args.Add(KEY_DEF1, defender1.index);
    args.Add(KEY_DEF2, defender2.index);
    args.Add(KEY_SCR1, scrounger1.index);
    args.Add(KEY_SCR2, scrounger2.index);
  }

  public override void stop() {
    LocationMarker.clear();
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
  
  private void s000_startConversation() {
    new Conversation()
      .wait(2f)
      .text(Character.NONE, "<i>Some dialog goes here.</i>")
      .text(Character.VANESSA, "The sun's going down; let's take a moment to eat some food and then sleep for the night.")
      .speed(Conversation.Speed.FAST)
      .text(Character.FIZZY, "I'll grab the food!")
      .speed(Conversation.Speed.NORMAL)
      .text("Raiding fridges is my favorite pasttime.")
      .text(Character.VANESSA, "I can fetch the blankets and such.")
      .text("Where do you keep them, MC? In the back?")
      .speed(Conversation.Speed.SLOW)
      .text(Character.MC, "...Um.")
      .speed(Conversation.Speed.NORMAL)
      .text(Character.VANESSA, "Hmm?")
      .text(Character.MC, "...It's a café. Why would we have blankets?")
      .text(Character.VANESSA, "...Is there not a deluge of homeless people inhabiting your lounge every night?")
      .text(Character.FIZZY, "Uhh. Vanessa, I think you've heard some weird things about city life.")
      .text(Character.VANESSA, "That is entirely possible.")
      .text("This is embarrassing.")
      .text(Character.ROSE, "Wait, there's no blankets? No pillows?")
      .text(Character.MAY, "Is there anything, like, next door? Somewhere we can get that stuff?")
      .text(Character.MC, "Um, there's a home furnishing store like two blocks from here.")
      .text(Character.MAY, "Goddammit. <i>Mission Time's back on!</i>")
      .wait(0.4f)
      .show(() => reloadMission(state: 100));
  }
  
  private void s100_setupLevel() {
    var locationMarkerTransform = new GameObject("Bed Store Marker").transform;
    locationMarkerTransform.position = new Vector3(156, 2.5f, 95);
    LocationMarker.add(locationMarkerTransform);
    CurrentQuestMessage.set("Head to the furniture store");
  }
  
  private void s110_setupClearNearbyEnemies() {
    CurrentQuestMessage.set("Clear the area of monsters");
    HighlightMonsters.highlightNear(new Vector3(156, 2.5f, 95), 50f, () => {
      HighlightMonsters.clearHighlights();
      new CombatDialog()
        .wait(2.5f)
        .message(Character.MAY, "Ok I guess that's safe enough.")
        .show(priority: CombatDialog.Priority.HIGH,
              callback: () => reloadMission(state: 200));
    });
  }
  
  private void s200_outsideDialog() {
    new Conversation()
      .wait(1.5f)
      // TODO have girl who likes you most say "good job", restoring your health
      .text(Character.MAY, "Alright, MC can stay out here and defend the store.")
      .text("But two of us should go in to pick out some stuff and carry it out.")
      .text(Character.MC, "Pick out some stuff? Just get the first five pillows and blankets you find.")
      .text(Character.ROSE, "I'm not using one of those scratchy-ass wool blankets. Those things are bullshit.")
      .text(Character.MC, "Oh, yeah, that kind of stiff shit? That's fair, actually, those are pretty bullshit.")
      .text(Character.FIZZY, "I don't want one of those pillows stuffed with feathers!")
      .text("I just stay up all night picking out the feathers through the pillowcase.")
      .text(Character.ROSE, "I don't want Fizzy to have one of those either, 'cause then I have to listen to it all night.")
      .text(Character.VANESSA, "Yes, yes. Can we resume the mission?")
      .show(() => {
        Player.SINGLETON.setLookDirection(direction: new Vector3(0, 0, 0), duration: 0.5f);
        showChoice();
      });
  }

  private void showChoice() {
    Player.SINGLETON.firstPersonController.look.inputEnabled = false;
    Player.SINGLETON.transform.rotation = Quaternion.identity;

    new Conversation()
      .choice(
        messageText: defender1 == null ? "MC, choose two of us to defend the store with. The other two will scrounge through the store." : "Who else will stay with you?",
        option0: defender1 == Weapons.SHOTGUN ? "[Undo]" : Weapons.SHOTGUN.name,
        option1: defender1 == Weapons.MACHINE_GUN ? "[Undo]" : Weapons.MACHINE_GUN.name,
        option2: defender1 == Weapons.SNIPER_RIFLE ? "[Undo]" : Weapons.SNIPER_RIFLE.name,
        option3: defender1 == Weapons.GRENADE_LAUNCHER ? "[Undo]" : Weapons.GRENADE_LAUNCHER.name,
        callback: (selection, text) => {
          // Select 1st weapon
          if (defender1 == null) {
            defender1 = Weapons.list[selection];
            showChoice();
          }

          // Deselect 1st weapon
          else if (Weapons.list[selection] == defender1) {
            defender1 = null;
            showChoice();
          }

          // Select 2nd weapon
          else {
            defender2 = Weapons.list[selection];

            // Save non-selected weapons as 'scroungers' in a random order
            List<Weapon> scroungers = new List<Weapon>(Weapons.list);
            scroungers.Remove(defender1);
            scroungers.Remove(defender2);
            bool reverseOrder = Random.value > 0.5f;
            scrounger1 = scroungers[reverseOrder ? 1 : 0];
            scrounger2 = scroungers[reverseOrder ? 0 : 1];

            Player.SINGLETON.firstPersonController.look.inputEnabled = true;
            new Conversation()
              .text(Character.MAY, "Ok, see you in a bit!")
              .show(() => reloadMission(300));
          }
        })
      .show();
  }

  private void s300_setupDefend() {
    defender1.canEquip = true;
    defender2.canEquip = true;
    scrounger1.canEquip = false;
    scrounger2.canEquip = false;
    
    GameObject.Find("HordeToggle").GetComponent<EnableGameObjects>().enableAll();

    Monsters.OnMonstersChanged += onMonstersChanged;
  }

  private void onMonstersChanged(Monster monster, bool added, int monstersRemaining) {
    if (monstersRemaining == 0) {
      Monsters.OnMonstersChanged -= onMonstersChanged;

      var dialog = new CombatDialog();
      var weapon = Weapons.currentlyEquipped;

      if (weapon == Weapons.SHOTGUN) {
        dialog = dialog.message(weapon.character, "Bam!")
          .message(weapon.character, "Alright, let's get back to the girls.");
      }
      else if (weapon == Weapons.MACHINE_GUN) {
        dialog = dialog.message(weapon.character, "Woo, gotcha!")
          .message(weapon.character, "Good work, MC. Now back to the store.");
      }
      else if (weapon == Weapons.SNIPER_RIFLE) {
        dialog = dialog.message(weapon.character, "Ah, that's the last of them.")
          .message(weapon.character, "Excellent. Let's rejoin the others.");
      }
      else {
        dialog = dialog.message(weapon.character, "Haha! <i>Boom!<i>")
          .message(weapon.character, "Okey dokey, guess that's the last of 'em.");
      }

      dialog.show(CombatDialog.Priority.MAX, () => reloadMission(400));
    }
  }
  
  private void s400_endMission() {
    var scrounger = Weapons.randomNonEquipableWeapon().character;
    var defender = Weapons.randomEquipableWeapon().character;

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
      .show(() => reloadMission(500));
  }
  
  private void s500_returnToCafe() {
    // TODO choose who says what based on affinity
    Character antagonist = Weapons.random().character;
    Character protagonist;
    do {
      protagonist = Weapons.random().character;
    } while (antagonist == protagonist); // Re-roll until we get two different girls

    new Conversation()
      .wait(1.25f)
      .text(Character.MAY, "Okay, for real now.")
      .text("No more Mission Time.")
      .text("Now it's Sleep Time.")
      .text(Character.FIZZY, "What if we make it our mission to have a good night's sleep?")
      .text(Character.MAY, "I do <i>not</i> have enough energy to play these games, woman.")
      .text(Character.MC, "Okay, so should we set up in front here...?")
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
      .text(Character.MC, "Plus, I have to leave room for all the homeless people.")
      .text(Character.VANESSA, "Oh, very funny.")
      .show(() => SceneTransition.changeTo(scene: "main_menu",
                                            onSceneLoaded: () => TimeUtils.mode = TimeUtils.TimeMode.GAMEPLAY));
  }

  // In some places its simpler to reload the mission, rather than manually making changes
  private void reloadMission(int state) {
    Hashtable args = save();
    args[Quest.KEY_STATE] = state;
    QuestManager.start(quest: new Quest_BedStore(),
                       args: args);
  }

  // -- EVENTS --

  private void eventWrongWay1() {
    new CombatDialog()
      .message(Character.MAY, "Left or right?")
      .message(Character.MC, "Right. I think.")
      .show(CombatDialog.Priority.MEDIUM);
  }

  private void eventWrongWay2() {
    new CombatDialog()
      .message(Character.MC, "Yeah no, definitely the other way.")
      .show(CombatDialog.Priority.MEDIUM);
  }

  private void eventFireComment() {
    new CombatDialog()
      .message(Character.FIZZY, "Woah!")
      .wait(3f)
      .message(Character.ROSE, "Damn, what a mess.")
      .message(Character.VANESSA, "Now I see why you cut through that side street, MC.")
      .message(Character.VANESSA, "The road is completely blocked.")
      .show(CombatDialog.Priority.LOW);
  }

  private void eventNiceCar() {
    new CombatDialog()
      .message(Character.MAY, "That's a nice car.")
      .message(Character.FIZZY, "How can you even tell?")
      .message(Character.FIZZY, "<i>They all look the same!</i>")
      .show(CombatDialog.Priority.TRIVIAL);
  }

  private void eventNicePark() {
    new CombatDialog()
      .message(Character.VANESSA, "This is pleasant.")
      .message(Character.FIZZY, "The park, or the monster killing?")
      .message(Character.VANESSA, "The park.")
      .message(Character.VANESSA, "Well, mostly the park.")
      .show(CombatDialog.Priority.LOW);
  }

  private void eventApproachStore() {
    if (state != 100) return;

    LocationMarker.clear();
    CurrentQuestMessage.clear();

    if (Monsters.monstersNear(new Vector3(156, 0, 96), 35f) > 0) {
      new CombatDialog()
        .message(Character.MC, "This is the place.")
        .message(Character.MAY, "Great, but before we go in, let's finish off any nearby monsters.")
        .show(CombatDialog.Priority.MAX, () => setState(110));
    }
    else {
      new CombatDialog()
        .message(Character.MC, "This is the place.")
        .show(CombatDialog.Priority.MAX, () => reloadMission(state: 200));
    }
  }

  private void eventDontLeaveStore() {
    var dialog = new CombatDialog();

    Weapon randomWeapon = Weapons.randomEquipableWeapon();

    if (randomWeapon == Weapons.SHOTGUN) {
      dialog = dialog.message(Character.ROSE, "Yo, we should probably stay near the store.")
        .message(Character.ROSE, "Can't let the monsters interrupt Pillow Time, can we?");
    }
    else if (randomWeapon == Weapons.MACHINE_GUN) {
      dialog = dialog.message(Character.MAY, "Hold on there, stud.")
        .message(Character.MAY, "Let's keep the killing <i>near</i> the store.")
        .message(Character.MAY, "As fun as this is, we have a job to do.");
    }
    else if (randomWeapon == Weapons.SNIPER_RIFLE) {
      dialog = dialog.message(Character.VANESSA, "I feel I should point out we're rather far from the store.")
        .message(Character.VANESSA, "I'm sure the girls are capable of handling themselves...")
        .message(Character.VANESSA, "...but I suppose we <i>ought</i> to stay close.");
    }
    else {
      dialog = dialog.message(Character.FIZZY, "Hahaa! Hey girls, did you see...")
        .message(Character.FIZZY, "Oh shoot, I forgot, they're totally in the store.")
        .message(Character.FIZZY, "We should probably stay near them.");
    }

    dialog.show(CombatDialog.Priority.HIGH);
  }
}
