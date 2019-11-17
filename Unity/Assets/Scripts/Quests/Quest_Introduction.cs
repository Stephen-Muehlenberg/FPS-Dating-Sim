using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Quest_Introduction : Quest {
//  private const string KEY_PLAYER_POSITION 
  private const string KEY_CHOICE_GREETING = "greeting";
  private const string KEY_CHOICE_PROFICIENCY = "proficiency";
  private const string KEY_CHOICE_HELP = "help";

  // TODO move this to some other scene
  private const string PROFICIENCY_0 = "[Challenging]\nI've used a paintball gun. That counts, right?";
  private const string PROFICIENCY_1 = "[Challenging]\nNo, but I've played a bunch of first person shooters.";
  private const string PROFICIENCY_2 = "[Difficult]\nI spend some time on the firing range, yeah.";
  private const string PROFICIENCY_3 = "[Easy]\nNot really, but did any of you before this morning?";

  private readonly PlayerState PLAYER_START = new PlayerState(0.3375727f, 0, -9.60523f, -1, -157.117f, false, false, false);
  private const float ROSE_X = 1.3f,      ROSE_START_Z = -7.35f,   ROSE_END_Z = -2.55f,   ROSE_WALK_TIME = 8.7f;
  private const float VANESSA_X = 0.668f, VANESSA_START_Z = -8.2f, VANESSA_END_Z = -3.5f, VANESSA_WALK_TIME = 9.1f;
  private const float FIZZY_X = 0.78f,    FIZZY_START_Z = -9.07f,  FIZZY_END_Z = -4.47f,  FIZZY_WALK_TIME = 9.45f,  FIZZY_BOB_HEIGHT = 0.25f, FIZZY_BOB_FREQUENCY = 4.5f;
  private const float MAY_X = 0.95f,      MAY_START_Z = -10.03f,   MAY_END_Z = -5.53f,    MAY_WALK_TIME = 9.75f;

  public static string NAME = "Introduction";
  public override string name => NAME;

  private int dialogChoice_greeting = -1;
  private int dialogChoice_proficiency = -1;
  private int dialogChoice_help = -1;

  protected override string getSceneForState(int state) {
    return state == 0 ? null : "cafe"; // Handle state 0's transition manually.
  }

  protected override void setupState(int state, Hashtable args) {
    dialogChoice_greeting = (int) args[KEY_CHOICE_GREETING];
    dialogChoice_proficiency = (int) args[KEY_CHOICE_PROFICIENCY];
    dialogChoice_help = (int) args[KEY_CHOICE_HELP];

    if (state == 100) Player.SINGLETON.setState(PLAYER_START);
  }

  protected override void saveArgs(Hashtable args) {
    args.Add(KEY_CHOICE_GREETING, dialogChoice_greeting);
    args.Add(KEY_CHOICE_PROFICIENCY, dialogChoice_proficiency);
    args.Add(KEY_CHOICE_HELP, dialogChoice_help);
  }

  protected override void handleState(int state) {
    if (state == 0) s0_initialMonologue();
    else if (state == 100) s100_lookTutorial();
    else if (state == 110) s110_moveTutorial();
    else if (state == 120) s120_interactTutorial();
    else if (state == 300) s300_secondMonologue();
    else if (state == 310) s310_greetRose();
    else throw new UnityException("Unknown state " + state);
  }

  private void s0_initialMonologue() {
    ScreenFade.fadeOut(() => {
      SceneTransition.swapTo("cafe", () => {
        var player = Player.SINGLETON;
        player.setState(PLAYER_START);
        player.camera.fieldOfView = 6f;
        player.GetComponent<FirstPersonModule.FirstPersonController>().disableAllInput();

        // Add a black screen fill BEHIND the conversation UI (the fade fill is always in front of everything).
        // This lets us show conversations before fading in.
        // TODO this is probably a useful util - maybe move it elsewhere?
        var screenFill = new GameObject("Screen fill");
        var fillImage = screenFill.AddComponent<Image>();
        fillImage.color = Color.black;
        screenFill.transform.SetParent(MainCanvas.transform);
        var rectTransform = screenFill.transform as RectTransform;
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = new Vector2(0, 0);
        rectTransform.offsetMax = new Vector2(0, 0);

        // Fade in background noises
        // TODO play more and varied sounds - screams, explosions, monster roars, thunder
        var mixer = Resources.Load<AudioMixer>("Audio/MainMixer");
        AudioSource sirens = setupAmbientAudioSource("Ambience - sirens", "Audio/Environment/Sirens", mixer);
        AudioSource alarms = setupAmbientAudioSource("Ambience - alarms", "Audio/Environment/Alarm", mixer);
        player.StartCoroutine(fadeVolume(1, 4, sirens, alarms));

        ScreenFade.fadeIn(() => {
          new Conversation()
            .wait(3f)
            .speaker(Character.MC_NARRATION, "")
            .text("So this morning's been eventful.")
            .wait(0.5f)
            .show(() => {
              player.StartCoroutine(fadeGraphic(fillImage, 0, 4, true));

              var slowZoomCoroutine = player.camera.zoomProportional(75, 30);
              player.StartCoroutine(slowZoomCoroutine);
              player.StartCoroutine(fadeOutAmbienceBasedOnZoom(player.camera, sirens, alarms));

              new Conversation()
                .speaker(Character.MC_NARRATION, "")
                .wait(1.5f)
                .text("Seems to be an apocalypse. Monsters, or demons, or undead... maybe all of the above?")
                .text("It's been pretty bad.")
                .text("They've been rampaging through the streets, emptying out the whole town pretty quickly.")
                .text("Everyone who hasn't been cut down is running for their lives.")
                .wait(0.5f)
                .text("And yet, I came to work.")
                .wait(1.5f)
                .text("I've already seen the fires of hell. My manager rained them down on me the last time I missed a shift.")
                .text("Well, fine, Steve. The world's ending, but here I am. You happy?")
                .wait(2f)
                .text("...He's probably dead. I don't know why I'm still mentally yelling at him.")
                .text("Pretty sure that was his blood smeared all over the window. Which, honestly, is just like him. I've never seen him make <i>coffee</i> without leaving something for me to clean up.")
                .wait(2.5f)
                .text("<i>Sigh.</i> I should probably find some detergent or something.")
                .text("Does detergent work on blood? I don't have a lot of experience with this.")
                .show(() => {
                  // If slow zoom hasn't finished, stop slow zoom and quickly finish zooming out
                  if (player.camera.fieldOfView < 75) {
                    player.StopCoroutine(slowZoomCoroutine);
                    player.StartCoroutine(player.camera.zoomDamp(75, 1));
                  }
                  setState(100);
                });
            });
        });
      });
    });
  }

  private void s100_lookTutorial() {
    var player = Player.SINGLETON;
    player.firstPersonController.setState(lookEnabled: true, moveEnabled: false, jumpEnabled: false);
    CurrentQuestMessage.set("Use the mouse to look around");
    player.StartCoroutine(waitForPlayerToLookAround(player.transform));
  }

  private void s110_moveTutorial() {
    var player = Player.SINGLETON;
    player.firstPersonController.setState(lookEnabled: true, moveEnabled: true, jumpEnabled: false);
    CurrentQuestMessage.set("Use WASD to move");
    player.StartCoroutine(waitForPlayerToMoveAround(player.transform));
  }

  private void s120_interactTutorial() {
    CurrentQuestMessage.set("Find some detergent or something");

    var resource = Resources.Load("Props/Cleaning Bottle");
    var bottle = GameObject.Instantiate(resource, new Vector3(3.327f, -0.013f, -3.646f), Quaternion.Euler(0, 285.267f, 0)) as GameObject;
    LocationMarker.add(bottle.transform, 0.4f);

    var bottleLookTarget = bottle.GetComponentInChildren<LookTarget>();
    bottleLookTarget.interactionRange = 2.15f;
    bottleLookTarget.setInteraction("Pick up\n[Left mouse]", (_) => {
      CurrentQuestMessage.clear();
      LocationMarker.clear();

      new Conversation()
      .speaker(Character.MC_NARRATION, "")
      .text("…")
      .wait(0.5f)
      .show(() => { setState(300); });
    });
  }

  private void s300_secondMonologue() {
    var player = Player.SINGLETON;
    player.firstPersonController.look.inputEnabled = false;
    player.firstPersonController.move.inputEnabled = false;

    // TOOD fix this
    player.setCrouchHeight(0.7f, 1.5f);
    player.smoothMove(new Vector3(2.93f, player.transform.position.y, -3), 1.5f);
    player.setLookDirection(new Vector3(5, 90, 0), 1.5f);

    new Conversation()
      .wait(1.5f)
      .performAction(() => {
        player.firstPersonController.look.inputEnabled = true;
        player.firstPersonController.look.restrictCameraToDirection(Vector3.zero, 0); // TODO
      })
      .speaker(Character.MC_NARRATION, "")
      .text("I'm not <i>totally</i> sure why I'm here.")
        .wait(0.5f)
        .text("Maybe because I've been gunning for Employee of the Month for a while, and I can't stop <i>now</i>.")
        .text("This kind of dedication would look pretty good to any secret shoppers that showed up.")
        .wait(0.5f)
        .text("Maybe because I don't want to give corporate any excuse to keep my next paystub from me.")
        .text("I wonder if apocalypses are recognized federal holidays? Maybe I can get time and a half for this.")
        .wait(1.5f)
        .text("Maybe...")
        .wait(1)
        .text("Maybe I just like the routine. If I'm going to get devoured by aliens, I might as well keep doing my job until it happens.")
        .text("Not like I have much else going on.")
      .wait(1.5f)
      .text(Character.NONE, "*<i>Door chime rings</i>*")
      .text("*<i>Footsteps</i>*")
      .performAction(() => {
        Character.ROSE.getProp().transform.position = new Vector3(ROSE_X, 0, ROSE_START_Z);
        Character.VANESSA.getProp().transform.position = new Vector3(VANESSA_X, 0, VANESSA_START_Z);
        Character.FIZZY.getProp().transform.position = new Vector3(FIZZY_X, 0, FIZZY_START_Z);
        Character.MAY.getProp().transform.position = new Vector3(MAY_X, 0, MAY_START_Z);
      })
      .wait(0.5f)
      .speaker(Character.MC_NARRATION, "")
      .text("Wow. Actual customers?")
      .text("…Or aliens. Probably aliens.")
      .text("Maybe I can offer a complimentary muffin to go with my brains.")
      .performAction(() => {
        Character.ROSE.getProp().walk(new Vector3(ROSE_X, 0, ROSE_END_Z), ROSE_WALK_TIME);
        Character.VANESSA.getProp().walk(new Vector3(VANESSA_X, 0, VANESSA_END_Z), VANESSA_WALK_TIME);
        Character.FIZZY.getProp().walk(new Vector3(FIZZY_X, 0, FIZZY_END_Z), FIZZY_WALK_TIME, FIZZY_BOB_HEIGHT, FIZZY_BOB_FREQUENCY);
        Character.MAY.getProp().walk(new Vector3(MAY_X, 0, MAY_END_Z), MAY_WALK_TIME);
        
        player.setCrouchHeight(0.95f, 1f);
        player.firstPersonController.look.inputEnabled = false;
        player.setLookDirection(new Vector3(-10, 235, 0), 1f);
      })
      .wait(1.3f)
      .performAction(() => {
        player.firstPersonController.look.inputEnabled = true;
      })
      .text("They're not aliens, surprisingly.")
      .text("Well, they could be. I don't know enough about aliens to say they don't all look like college-aged women covered in blood.")
      .text("Looks like they're in the middle of a conversation...")
      .speaker(Character.ROSE, "???")
      .text("'Cause I want a friggin' muffin, that's why.")
      .speaker(Character.VANESSA, "???")
      .text("I certainly wouldn't say no to a break. How many was that? Forty, fifty kills?")
      .speaker(Character.FIZZY, "???")
      .text("I'm pretty sure that was, like, ten.")
      .speaker(Character.VANESSA, "???")
      .text("Goodness. I suppose I assumed the rest of you were carrying your weight.")
      .speaker(Character.FIZZY, "???")
      .text("Ruuude!")
      .speaker(Character.MAY, "???")
      .text("Alright, alright. Get your muffins and take a load off, but we should get back out there pretty quick.")
      .text("Seems like we're the only ones that can fight back right now. We have to do our best to make some kind of difference.")
      .speaker(Character.MC_NARRATION, "")
      .text("They haven't noticed me yet, but the one up front is getting closer. Probably getting ready to hop up onto the counter and grab the pastries.")
      .performAction(() => {
        player.firstPersonController.look.inputEnabled = false;
        player.setLookDirection(new Vector3(-14, 255, 0), 1f);
      })
      .choice("Well, I've gotta say <i>something</i>…",
              "Hey there! My name's MC. Please don't eat me.",
              "Hi. Don't freak out. I'm trying my best not to pop out and scare you, but it was hard to know when to… you know. Say hi.",
              "Welcome! What can I getcha?",
              "BOO!",
        (selection, text) => {
          dialogChoice_greeting = selection;
          setState(310);
        }
      )
      .show();
  }

  private void s310_greetRose() {
    string greetingText;
    if (dialogChoice_greeting == 0) greetingText = "Hey th—";
    else if (dialogChoice_greeting == 1) greetingText = "Hi—";
    else if (dialogChoice_greeting == 2) greetingText = "Welco—";
    else if (dialogChoice_greeting == 3) greetingText = "BOO!";
    else throw new UnityException("greetingSelection must have been set between 0 and 3 before this point.");

    Player player = Player.SINGLETON;

    GameObject shotgunProp = null;

    var convo = new Conversation()
      .performAction(() => {
        player.setCrouchHeight(1.7f, 0.3f);
        player.setLookDirection(new Vector3(2, 255, 0), 0.3f);
        player.firstPersonController.look.inputEnabled = true;
      })
      .speaker(Character.MC)
      .text(greetingText)
      .speaker(Character.ROSE, "???")
      .text("WOAH GOD FUCK")
      .performAction(() => {
        var shotgunRes = Resources.Load("Props/Shotgun");
        shotgunProp = GameObject.Instantiate(shotgunRes, Character.ROSE.getProp().transform.position + Vector3.up * 1.379f, Quaternion.Euler(-57f, 0f, 0f)) as GameObject;
        shotgunProp.GetComponent<ParticleSystem>().Play();
        GameObject.Destroy(Character.ROSE.getProp().gameObject);
        // TODO drop shotgun onto bench
      })
      .speaker(Character.NONE)
      .text("*<i>Fwoosh!</i>*");

    if (dialogChoice_greeting == 3) {
      convo = convo.speaker(Character.MC_NARRATION)
        .wait(0.75f)
        .text("I was right to go on the offensive. These are definitely aliens.")
        .text("The one in the back has doubled over laughing.")
        .performAction(() => {
          GameObject.Destroy(shotgunProp);
          Character.ROSE.getProp().transform.position = new Vector3(ROSE_X, 0, ROSE_END_Z);
          Character.ROSE.getProp().GetComponent<ParticleSystem>().Play();
        })
        .speaker(Character.ROSE, "???")
        .text("Fun fact, May: Not actually that funny!")
        .speaker(Character.MAY)
        .text("I'm sorry, it's—ppfffhahahaha—")
        .text("It's just like, we're out there literally killing demons, nearly dying—")
        .text("—After all that carnage, we—we run into a building for a break, looking for food—")
        .text("—And some dude just, fuckin', pops out like \"BOO!\" Like, what is happening? Is this real life?")
        .text("I don't know why that was so funny to me. I'm going a little insane right now.");
    }
    else {
      convo = convo.speaker(Character.MC_NARRATION)
        .wait(0.75f)
        .text("Crap. It <i>is</i> aliens.")
        .text("The other three girls-slash-aliens are tense, but their eyes are on me, so whatever <i>that</i> was seems to be normal for them.")
        .text("Unable to contain my curiosity, I look down to see…");
    }

    convo = convo.speaker(Character.MC)
        .text("…Did she just turn into a shotgun?")
        .speaker(Character.FIZZY, "???")
        .text("Yeeeeaaahhh…. It's a long story.")
        .speaker(Character.VANESSA, "???")
        .text("Well, no. It's a short one. It just doesn't have a beginning, middle, or end: A bunch of hell portals opened up all over the place, we were zapped with mysterious light, and now we can do… that.")
        .speaker(Character.MC)
        .text("…All of you? You can all do this? How does that… work?");

    if (dialogChoice_greeting != 3) convo = convo.performAction(() => {
      GameObject.Destroy(shotgunProp);
      Character.ROSE.getProp().transform.position = new Vector3(ROSE_X, 0, ROSE_END_Z);
      Character.ROSE.getProp().GetComponent<ParticleSystem>().Play();
    });

    convo.speaker(Character.ROSE, "???")
      .text("Well, it works better when <i>someone picks me up and shoots me</i>, I'll tell you that much.")
      .speaker(Character.VANESSA, "???")
      .text("And then this nice young man would be very much dead, and don't you think you'd be a little regretful?")
      .speaker(Character.ROSE, "???")
      .text("I mean—Yeah. I'm just saying, like…")
      .speaker(Character.FIZZY, "???")
      .text("Rose's just trying to tell this guy that she knows she looked <i>really</i> silly turning into a gun and then just falling on the counter, and she wants to explain what she was going for! Ain't no thang.")
      .performAction(() => {
        Character.FIZZY.getProp().walk(new Vector3(FIZZY_X + 0.35f, 0, FIZZY_END_Z + 0.15f), 0.35f, FIZZY_BOB_HEIGHT);
      })
      .speaker(Character.FIZZY, "???")
      .text("Hi! I'm Felicity. But my friends call me Fizzy, and since you're one of the only living things around here not wanting to kill us, that includes you by default!")
      .text(Character.MC, "Hi. I'm MC.")
      .speaker(Character.MAY, dialogChoice_greeting == 3 ? null : "???")
      .text("Are you… open? Like, you came into work and everything?")
      .speaker(Character.MC)
      .text("I, uh… yeah. I guess I didn't like this day starting so weird, so I stubbornly tried to turn it into a normal one.")
      .speaker(Character.ROSE)
      .text("Oh, good! A crazy person.")
      .speaker(Character.MAY, dialogChoice_greeting == 3 ? null : "???")
      .text("Eh, I can see where you're coming from. ...I'm May, by the way. Nice to meet you.")
      .speaker(Character.VANESSA, "???")
      .text("And my name is Vanessa. A pleasure.")
      .wait(1f)
      .text(Character.ROSE, "...What? You already told him. I'm Rose. Yo. Hi. Gimmee a muffin.")
      .text(Character.MC_NARRATION, "Dutifully, I slide open the glass case and grab one to offer to her.")
      .text(Character.MC, "That'll be… uh, never mind.")
      .text(Character.ROSE, "I was gonna say.")
      .text(Character.MC_NARRATION, "She snatches it up and tears into it. I empty the rest of the case for the other women.")
      .wait(0.5f)
      .text(Character.MC, "So… you were just out there shooting monsters? With… each other?")
      .text(Character.MAY, "That about covers it. Fucked up, right?")
      .text(Character.MC, "Yeah, I'm not gonna lie. It's kinda hard to wrap my head around.")
      .text(Character.FIZZY, "Well, it's like this.")
      .performAction(() => {
        // TODO maybe make a generic function on all the girls to have them transform to/from their gun forms?
      })
      .text("Catch me!")
      .text(Character.MC_NARRATION, "Fizzy suddenly leaps towards Vanessa. With a flash, a grenade launcher appears out of the dust marking where Fizzy once existed.")
      .text("Vanessa, clearly caught off - guard, fumbles with the weapon. The one that's also her friend. Apparently.")
      .text(Character.FIZZY, "See? Just like that. Now she can blow stuff up!")
      // The grenade launcher lights up almost imperceptibly with every word of Fizzy's that emanates from it.
      .text(Character.VANESSA, "...I'd rather not.")
      .text(Character.FIZZY, "You're never gonna cut loose, are you? Lame.")
      // TODO Without warning, she shifts into human form. Vanessa, now carrying an adult woman, falls over very quickly.
      .text(Character.MC_NARRATION, "Without warning, Fizzy shifts back into human form. Vanessa, now carrying an adult woman, falls over very quickly.")
      .text(Character.VANESSA, "Erf—! Fizzy—!")
      // TODO animate the two girls back up, like a second after the following message starts displaying.
      .text(Character.MC_NARRATION, "The two climb to their feet after a bit of rolling around on the ground.")
      .text("I look between Vanessa and May.")
      .text(Character.MC, "Is it rude to ask? I don't know the social protocol for this. But I gotta know what you turn into.")
      .text(Character.MAY, "Oh, yeah, sure.")
      // TODO May falls towards Fizzy as she transforms with an orange hue.Fizzy quickly grabs her by the barrel and hoists her into firing position, revealing her to be an assault rifle.
      // Fizzy wastes no time in firing wildly through the windows and door with a wide grin on her face, shattering the glass entirely.Guess I don't have to clean it anymore. 
      .text(Character.MC_NARRATION, "May falls towards Fizzy as she transforms with a green hue. Fizzy quickly snatches the glowing assault rifle out of the air and hoists her into firing poisition.")
      .text("She wastes no time firing a burst through the nearby window, wide grin across her face as the glass shatters entirely. Guess I don't have to clean it anymore.")
      .text(Character.VANESSA, "Could we not attract more attention than we need to? I swore we were in here specifically to stop fighting for a spell.")
      .text(Character.ROSE, "Yeahg, Ihm mnot domm mmy mmffn.")
      .text(Character.FIZZY, "Yeah, yeah. C'mon, your turn, Vanessa!")
      .text(Character.MC_NARRATION, "Fizzy places the assault rifle on the ground, where May quickly reappears. Fizzy then gestures towards Vanessa.")
      .text(Character.MC_NARRATION, "The distant roar of a monster fills the air. Then more roars, closer, join in.")
      .wait(0.5f)
      .text(Character.VANESSA, "I'm not sure we have time for that.")
      .text(Character.MAY, "Preeetty sure you just got the attention of every monster in the neighborhood, Fiz.")
      .text(Character.FIZZY, "My bad.")
      .text(Character.MAY, "<i>Sigh.</i> We'd better get moving. This place isn't really defensible.")
      .text(Character.ROSE, "Cmhm fhef dmf— Urfh…")
      .text(Character.MC_NARRATION, "Rose quickly scarfs down the remainder of her muffin.")
      .text(Character.ROSE, "Come on, sis, don't be stupid. We've been on the move all morning. You guys can barely stand.")
      .text(Character.FIZZY, "Yeah, even <i>I'm</i> getting tired.")
      .text(Character.VANESSA, "I'm afraid she's right. We've no choice but to make a stand here, defensible or not.")
      .text(Character.MAY, "Shit. I'm sorry guys, I should have found somewhere safer for us to rest.")
      .performAction(() => {
        player.firstPersonController.look.inputEnabled = false;
        player.setLookDirection(new Vector3(2, 255, 0), 0.3f);
      })
      .speaker(Character.FIZZY)
      .choice("If only there was some random helpful bystander willing to carry us poor, exhausted, attractive girls around. And possibly use us to slay demons.",
        "Demon slaying? Awesome, sign me up!",
        "Oooh! Me! I'm a random helpful bystander something something attractive girls.",
        "Subtle <i>and</i> modest. How can I refuse such charm?",
        "Well, I guess I'd be a lot safer with a bunch of sentient gun ladies.",
        (selection, _) => {
          dialogChoice_help = selection;
          // TODO it was determined that this approach DOESNT WORK. The problem being that variableText
          // is compiled at the start of the dialog, and changes to the value don't effect the compiled 
          // dialog. Need to change state, save the response value, and start a new dialog.
        }
      )
      .performAction(() => {
        player.firstPersonController.look.inputEnabled = true;
      })
      .dynamicallyInsert(() => {
        switch(dialogChoice_help) {
          case 0:
            return new Conversation().text(Character.ROSE, "Heck yeah it's awesome!");
          case 1:
            return new Conversation().text(Character.FIZZY, "Oh, you're actually game? Woo!");
          case 2:
            return new Conversation().text(Character.VANESSA, "<i>Giggle.</i> Careful Fizzy, this gentlemen appears to have a wit to match your own.");
          case 3:
            return new Conversation().text(Character.MAY, "Exactly, safety in numbers and all that.", "But we should really get moving.");
          default:
            throw new UnityException("dialogChoice_help was " + dialogChoice_help + ", which is invalid.");
        }
      })
      .text(Character.MC, "Alright, let's go. There's a back door through the kitchen, leads onto some side streets away from the main road. Might have a better chance of escaping undetected through there.")
      .dynamicallyInsert(() => {
        switch(dialogChoice_help) {
          case 0:
            return new Conversation().text(Character.ROSE, "Let's do this.");
          case 1:
            return new Conversation().text(Character.FIZZY, "This'll be fun!");
          case 2:
            return new Conversation().text(Character.VANESSA, "After you, MC.");
          case 3:
            return new Conversation().text(Character.MAY, "Lead the way.");
          default:
            throw new UnityException("dialogChoice_help was " + dialogChoice_help + ", which is invalid.");
        }
      })
      .show(() => { QuestManager.start(new Quest_Tutorial()); });
  }

  // ---------- UTILS -----------

  // TODO this might be a useful generic util
  private IEnumerator fadeGraphic(Graphic image, float alpha, float fadeDuration, bool destroyImageOnComplete) {
    while (image.color.a != alpha) {
      image.CrossFadeAlpha(alpha, fadeDuration, false);
      yield return null;
    }
    if (destroyImageOnComplete) GameObject.Destroy(image.gameObject);
  }

  private AudioSource setupAmbientAudioSource(string name, string resource, AudioMixer mixer) {
    GameObject obj = new GameObject("Ambience: " + resource);
    AudioSource source = obj.AddComponent<AudioSource>();
    source.clip = Resources.Load<AudioClip>(resource);
    source.outputAudioMixerGroup = mixer.outputAudioMixerGroup;
    source.volume = 0;
    source.loop = true;
    source.Play();
    return source;
  }

  private IEnumerator fadeOutAmbienceBasedOnZoom(Camera camera, params AudioSource[] audioSources) {
    while (camera.fieldOfView < 75) {
      foreach (AudioSource source in audioSources) {
        source.volume = 1f - ((camera.fieldOfView - 6) / 69f);
      }
      yield return null;
    }

    for (int i = audioSources.Length - 1; i >= 0; i--) {
      GameObject.Destroy(audioSources[i].gameObject);
    }
  }

  private IEnumerator waitForPlayerToLookAround(Transform playerTransform) {
    while (playerTransform.rotation.eulerAngles.x < -25 || playerTransform.rotation.eulerAngles.x > 25
      || (playerTransform.rotation.eulerAngles.y < 330 && playerTransform.rotation.eulerAngles.y > 30)) {
      yield return null;
    }
    setState(110);
  }

  private IEnumerator waitForPlayerToMoveAround(Transform playerTransform) {
    while (playerTransform.position.z < -3) yield return null;
    setState(120);
  }

  private IEnumerator fadeVolume(float targetVolume, float fadeDuration, params AudioSource[] audioSources) {
    float t = 0;
    float volumeDeltaPerSecond = (targetVolume - audioSources[0].volume) / fadeDuration;

    while (t < fadeDuration) {
      foreach (AudioSource audioSource in audioSources) {
        audioSource.volume += volumeDeltaPerSecond * Time.deltaTime;
      }
      t += Time.deltaTime;
      yield return null;
    }

    foreach (AudioSource audioSource in audioSources) {
      audioSource.volume = targetVolume;
    }
  }
}
