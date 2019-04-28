using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.UI;

public class Quest_Introduction : Quest {
  private const string KEY_CHOICE_GREETING = "greeting";
  private const string GREETING_0 = "Hey there! My name's MC. Please don't eat me.";
  private const string GREETING_1 = "Hi. Don't freak out. I'm a store worker who got caught up in the chaos. I'm trying my very best not to pop out and scare you, but it was hard to know when to… you know. Say hi.";
  private const string GREETING_2 = "Welcome! What can I getcha?";
  private const string GREETING_3 = "BOO!";
  private const string PROFICIENCY_0 = "I know how to use a paintball gun. That counts, right?";
  private const string PROFICIENCY_1 = "No, but I've played a bunch of first person shooters.";
  private const string PROFICIENCY_2 = "I've been down to the firing range a few times, yeah.";
  private const string PROFICIENCY_3 = "Not really, but did any of you before this morning?";

  public static string NAME = "Introduction";
  public override string name => NAME;

  private int greetingSelection = -1;
  private int proficiencySelection = -1;

  public override void start(Hashtable args) {
    state = (int) args.getOrDefault(Quest.KEY_STATE, 0);
    greetingSelection = (int) args.getOrDefault(KEY_CHOICE_GREETING, -1);

    setState(state);
  }

  public override Hashtable save() {
    return new Hashtable {
      { Quest.KEY_STATE, state }
    };
  }

  private void setState(int state) {
    if (state == 0) setupInitialMonologue();
    else if (state == 10) lookTutorial();
    else if (state == 20) moveTutorial();
    else if (state == 30) interactTutorial();
    else if (state == 40) secondMonologue();
    else if (state == 50) greetRose();
    else throw new UnityException("Unknown state " + state);
  }

  private void setupInitialMonologue() {
    state = 0;

    ScreenFade.fadeOut(() => {
      SceneTransition.swapTo("cafe", () => {
        var player = Player.SINGLETON;
        player.transform.position = new Vector3(0.3375727f, 0.0799998f, -9.60523f);
        player.transform.rotation = Quaternion.Euler(-1, -157.117f, 0);
        player.camera.fieldOfView = 6f;
        player.camera.transform.localRotation = Quaternion.Euler(-1, 0, 0);
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
            .speaker(Conversation.Speaker.MC_NARRATION, "")
            .text("So this morning's been eventful.")
            .wait(0.5f)
            .show(() => {
              player.StartCoroutine(fadeGraphic(fillImage, 0, 4, true));

              var slowZoomCoroutine = zoomOutSlowly(player.camera);
              player.StartCoroutine(slowZoomCoroutine);
              player.StartCoroutine(fadeOutAmbienceBasedOnZoom(player.camera, sirens, alarms));

              new Conversation()
                .speaker(Conversation.Speaker.MC_NARRATION, "")
                .wait(1.5f)
                .text("Seems to be an apocalypse. Monsters, or demons, or undead... maybe all of the above?")
        /*        .text("It's been pretty bad.")
                .text("They've been rampaging through the streets, emptying out the whole town pretty quickly.")
                .text("Everyone who hasn't been cut down is running for their lives.")
                .wait(0.5f)
                .text("And yet, I came to work.")
                .wait(1.5f)
                .text("I've already seen the fires of hell. My manager rained them down on me the last time I missed a shift.")
                .text("Well, fine, Steve. The world's ending, but here I am. You happy?")
                .wait(2f)
                .text("...I already saw his body on the way over. I don't know why I'm still mentally yelling at him.")
                .text("Pretty sure he bled on the welcome mat, though. Which, honestly, is just like him. I've never seen him make <i>coffee</i> without leaving something for me to clean up.")
                .wait(2.5f)
       */         .text("<i>Sigh.</i> I should probably find some detergent or something.")
                .text("Does detergent work on blood? I don't have a lot of experience with this.")
                .show(() => {
                  // If slow zoom hasn't finished, stop slow zoom and quickly finish zooming out
                  if (player.camera.fieldOfView < 75) {
                    player.StopCoroutine(slowZoomCoroutine);
                    player.StartCoroutine(zoomOutQuickly(player.camera));
                  }
                  setState(10);
                });
            });
        });
      });
    });
  }

  private void lookTutorial() {
    state = 10;

    var player = Player.SINGLETON;
    var fpController = player.GetComponent<FirstPersonModule.FirstPersonController>();
    fpController.enableAllInput();
    fpController.move.inputDisabled = true;
    fpController.jump.inputDisabled = true;
    CurrentQuestMessage.set("Use the mouse to look around");
    player.StartCoroutine(waitForPlayerToLookAround(player.transform));
  }

  private void moveTutorial() {
    state = 20;

    var player = Player.SINGLETON;
    player.GetComponent<FirstPersonModule.FirstPersonController>().move.inputDisabled = false;
    CurrentQuestMessage.set("Use WASD to move");
    player.StartCoroutine(waitForPlayerToMoveAround(player.transform));
  }

  private void interactTutorial() {
    state = 30;

    var player = Player.SINGLETON;
    player.GetComponent<FirstPersonModule.FirstPersonController>().move.inputDisabled = false;
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
      .speaker(Conversation.Speaker.MC_NARRATION, "")
      .text("…")
      .wait(0.5f)
      .show(() => { setState(40); });
    });
  }

  private void secondMonologue() {
    state = 40;

    var player = Player.SINGLETON;
    var fpController = player.GetComponent<FirstPersonModule.FirstPersonController>();
    fpController.look.inputDisabled = true;
    fpController.move.inputDisabled = true;

    // TOOD fix this
    player.setCrouchHeight(0.7f, 1.5f);
    player.smoothMove(new Vector3(2.93f, player.transform.position.y, -3), 0.5f);
//    player.StartCoroutine(smoothMove(player.transform, new Vector3(2.93f, player.transform.position.y, -3), 0.5f));
    player.setLookDirection(new Vector3(5, 90, 0), 1.5f);

    new Conversation()
      .wait(1.5f)
      .speaker(Conversation.Speaker.MC_NARRATION, "")
      .text("I'm not <i>totally</i> sure why I'm here.")
 /*     .wait(0.5f)
      .text("Maybe because I've been gunning for Employee of the Month for a while now, and I can't stop <i>now</i>.")
      .text("This kind of dedication would look pretty good to any secret shoppers that showed up.")
      .wait(0.5f)
      .text("Maybe because I don't want to give corporate any excuse to keep my next paystub from me.")
      .text("I wonder if apocalypses are recognized federal holidays? Maybe I can get time and a half for this.")
      .wait(1.5f)
      .text("Maybe...")
      .wait(1)
      .text("Maybe I just like the routine. If I'm going to get devoured by aliens, I might as well keep doing my job until it happens.")
 */     .text("Not like I have much else going on.")
      .wait(1.5f)
      .text(Conversation.Speaker.NONE, "*<i>Doorbell rings</i>*")
      .text("*<i>Footsteps</i>*")
      .performAction(() => {
        player.setLookDirection(new Vector3(-5, 90, 0), 0.5f);
        Actors.getRose().transform.position = new Vector3(1.35f, 0, -6.45f);
        Actors.getVanessa().transform.position = new Vector3(0.668f, 0, -7.6f);
        Actors.getFizzy().transform.position = new Vector3(0.78f, 0, -8.67f);
        Actors.getMay().transform.position = new Vector3(0.95f, 0, -10.03f);
      })
      .wait(0.5f)
      .speaker(Conversation.Speaker.MC_NARRATION, "")
      .text("Wow. Actual customers?")
      .text("…Or aliens. Probably aliens.")
      .text("Maybe I can offer a complimentary muffin to go with my brains.")
      .performAction(() => {
        // Make the girls walk into place during the conversation
        player.StartCoroutine(walk(Actors.getRose().transform, new Vector3(1.35f, 0, -3.45f), 5));
        player.StartCoroutine(walk(Actors.getVanessa().transform, new Vector3(0.668f, 0, -4.6f), 5.4f));
        player.StartCoroutine(walk(Actors.getFizzy().transform, new Vector3(0.78f, 0, -5.67f), 5.75f));
        player.StartCoroutine(walk(Actors.getMay().transform, new Vector3(0.95f, 0, -7.03f), 6.05f));
        
        player.setCrouchHeight(0.95f, 1f);
        player.setLookDirection(new Vector3(-10, 250, 0), 1f);
      })
      .wait(1.3f)
      .text("They're not aliens, surprisingly.")
      .text("Well, they could be. I don't know enough about aliens to say they don't all look like college-aged women covered in blood.")
      .text("Looks like they're in the middle of a conversation...")
      .speaker(Conversation.Speaker.ROSE, "???")
      .text("'Cause I want a friggin' muffin, that's why.")
      .speaker(Conversation.Speaker.VANESSA, "???")
      .text("I certainly wouldn't say no to a break. How many was that? Forty, fifty kills?")
      .speaker(Conversation.Speaker.FIZZY, "???")
      .text("I'm pretty sure that was, like, ten.")
      .speaker(Conversation.Speaker.VANESSA, "???")
      .text("Goodness. I suppose I assumed the rest of you were carrying your weight.")
      .speaker(Conversation.Speaker.FIZZY, "???")
      .text("Ruuude!")
      .speaker(Conversation.Speaker.MAY, "???")
      .text("Alright, alright. Get your muffins and take a load off, but we should get back out there pretty quick.")
      .text("We're about the only people that can fight back right now. We have to do our best to make some kind of difference.")
      .speaker(Conversation.Speaker.MC_NARRATION, "")
      .text("They haven't noticed me yet, but the one up front is getting closer. Probably getting ready to hop up onto the counter and grab the pastries.")
      .choice("Well, I've gotta say <i>something</i>…",
        GREETING_0, GREETING_1, GREETING_2, GREETING_3,
        (selection, text) => {
          greetingSelection = selection;
          greetRose();
        }
      )
      .show();
  }

  private void greetRose() {
    state = 50;

    string greetingText;
    if (greetingSelection == 0) greetingText = "Hey th—";
    else if (greetingSelection == 1) greetingText = "Hi—";
    else if (greetingSelection == 2) greetingText = "Welco—";
    else if (greetingSelection == 3) greetingText = GREETING_3;
    else throw new UnityException("greetingSelection must have been set between 0 and 3 before this point.");

    Player player = Player.SINGLETON;

    var convo = new Conversation()
      .performAction(() => {
        player.setCrouchHeight(1.7f, 0.3f);
        player.setLookDirection(new Vector3(2, 240, 0), 0.3f);
      })
      .speaker(Conversation.Speaker.MC)
      .text(greetingText)
      .speaker(Conversation.Speaker.ROSE, "???")
      .text("WOAH GOD FUCK")
      .performAction(() => {
        var shotgunRes = Resources.Load("Props/Shotgun");
        var shotgun = GameObject.Instantiate(shotgunRes, Actors.getRose().transform.position + Vector3.up * 1.379f, Quaternion.Euler(-57f, 0f, 0f)) as GameObject;
        shotgun.name = "SHOTGUN_PROP";
        shotgun.GetComponent<ParticleSystem>().Play();
        GameObject.Destroy(Actors.getRose().gameObject);
        // TODO drop shotgun onto bench
      })
      .speaker(Conversation.Speaker.NONE)
      .text("*<i>Fwoosh!</i>*");

    if (greetingSelection == 3) {
      convo = convo.speaker(Conversation.Speaker.MC_NARRATION)
        .wait(0.75f)
        .text("I was right to go on the offensive. These are definitely aliens.")
        .text("The one in the back has doubled over laughing.")
        .performAction(() => {
          GameObject.Destroy(GameObject.Find("SHOTGUN_PROP")); // TODO there's probably a more efficient way to keep a reference to this prop
          Actors.getRose().transform.position = new Vector3(1.35f, 0, -3.45f);
          Actors.getRose().GetComponent<ParticleSystem>().Play();
        })
        .speaker(Conversation.Speaker.ROSE, "???")
        .text("Fun fact, May: Not actually that funny!")
        .speaker(Conversation.Speaker.MAY)
        .text("I'm sorry, it's—ppfffhahahaha—")
        .text("It's just like, we're out there literally killing demons, nearly dying—")
        .text("—After all that carnage, we—we run into a building for a break, looking for food—")
        .text("—And some dude just, fuckin', pops out like \"BOO!\" Like, what is happening? Is this real life?")
        .text("I don't know why that was so funny to me. I'm going a little insane right now.");
    }
    else {
      convo = convo.speaker(Conversation.Speaker.MC_NARRATION)
        .wait(0.75f)
        .text("Crap. It <i>is</i> aliens.")
        .text("The other three girls-slash-aliens are tense, but their eyes are on me, so whatever <i>that</i> was seems to be normal for them.")
        .text("Unable to contain my curiosity, I look down to see…");
    }

    convo = convo.speaker(Conversation.Speaker.MC)
        .text("…Did she just turn into a shotgun?")
        .speaker(Conversation.Speaker.FIZZY, "???")
        .text("Yeeeeaaahhh…. It's a long story.")
        .speaker(Conversation.Speaker.VANESSA, "???")
        .text("Well, no. It's a short one. It just doesn't have a beginning, middle, or end: A bunch of hell portals opened up all over the place, we were zapped with mysterious light, and now we can do… that.")
        .speaker(Conversation.Speaker.MC)
        .text("…All of you? You can all do this? How does that… work?");

    if (greetingSelection != 3) convo = convo.performAction(() => {
      GameObject.Destroy(GameObject.Find("SHOTGUN_PROP")); // TODO there's probably a more efficient way to keep a reference to this prop
      Actors.getRose().transform.position = new Vector3(1.35f, 0, -3.45f);
      Actors.getRose().GetComponent<ParticleSystem>().Play();
    });

    convo.speaker(Conversation.Speaker.ROSE, "???")
      .text("Well, it works better when <i>someone picks me up and shoots me</i>, I'll tell you that much.")
      .speaker(Conversation.Speaker.VANESSA, "???")
      .text("And then this nice young man would be very much dead, and don't you think you'd be a little regretful?")
      .speaker(Conversation.Speaker.ROSE, "???")
      .text("I mean—Yeah. I'm just saying, like…")
      .speaker(Conversation.Speaker.FIZZY, "???")
      .text("Rose's just trying to tell this guy that she knows she looked <i>really</i> silly turning into a gun and then just falling on the counter, and she wants to explain what she was going for! Ain't no thang.")
      .performAction(() => {
        // TODO
        // This bubbly girl steps towards me and leans on the counter with both elbows, her hands interlocking and creating a bridge for her chin to rest.
      })
      .speaker(Conversation.Speaker.FIZZY, "???")
      .text("Hi! I'm Felicity. But my friends call me Fizzy, and since you're one of the only living things not wanting to kill us around here, that includes you by default!")
      .speaker(Conversation.Speaker.MAY, greetingSelection == 3 ? null : "???")
      .text("Are you… open? Like, you came into work and everything?")
      .speaker(Conversation.Speaker.MC)
      .text("I, uh… yeah. I guess I didn't like this day starting so weird, so I stubbornly tried to turn it into a normal one.")
      .speaker(Conversation.Speaker.ROSE)
      .text("Oh, good! A crazy person.")
      .speaker(Conversation.Speaker.MAY, greetingSelection == 3 ? null : "???")
      .text("Eh, I can see where you're coming from. ...I'm May, by the way. Nice to meet you.")
      .speaker(Conversation.Speaker.VANESSA, "???")
      .text("And my name is Vanessa. A pleasure.")
      .wait(1f)
      .text(Conversation.Speaker.ROSE, "...What? You already told him. I'm Rose. Yo. Hi. Gimmee a muffin.")
      .text(Conversation.Speaker.MC_NARRATION, "Dutifully, I slide open the glass case and grab one to offer to her.")
      .text(Conversation.Speaker.MC, "That'll be… uh, never mind.")
      .text(Conversation.Speaker.ROSE, "I was gonna say.")
      .text(Conversation.Speaker.MC_NARRATION, "She snatches it up and tears into it. I empty the rest of the case for the other women.")
      .wait(0.5f)
      .text(Conversation.Speaker.MC, "So… you were just out there shooting monsters? With… each other?")
      .text(Conversation.Speaker.MAY, "That about covers it. Fucked up, right?")
      .text(Conversation.Speaker.MC, "Yeah, I'm not gonna lie. It's kinda hard to wrap my head around.")
      .text(Conversation.Speaker.FIZZY, "Well, it's like this.")
      .performAction(() => {
        // She turns to Vanessa leaps towards her, glowing green as she goes.
        // TODO fizzy jumps towards Vanessa, transforms midair. Vanessa catches her, taking half a step back in the process.
        // TODO maybe make a generic function on all the girls to have them transform to/from their gun forms?
      })
      .text("Catch me!")
      .text(Conversation.Speaker.MC_NARRATION, "With a flash, a grenade launcher appears out of the dust marking where Fizzy once existed. Vanessa, clearly caught off - guard, fumbles with the weapon. The one that's also her friend. Apparently.")
      .text(Conversation.Speaker.FIZZY, "See? Just like that. Now she can blow stuff up!")
      // The grenade launcher lights up almost imperceptibly with every word of Fizzy's that emanates from it.
      .text(Conversation.Speaker.VANESSA, "...I'd rather not.")
      .text(Conversation.Speaker.FIZZY, "You're never gonna cut loose, are you? Lame.")
      // TODO Without warning, she shifts into human form. Vanessa, now carrying an adult woman, falls over very quickly.
      .text(Conversation.Speaker.VANESSA, "Erf—! Fizzy—!")
      // TODO animate the two girls back up, like a second after the following message starts displaying.
      .text(Conversation.Speaker.MC_NARRATION, "The two climb to their feet after a bit of rolling around on the ground.")
      .text("I look between Vanessa and May.")
      .text(Conversation.Speaker.MC, "Is it rude to ask? I don't know the social protocol for this. But I gotta know what you turn into.")
      .text(Conversation.Speaker.MAY, "Oh, yeah, sure.")
      // May falls towards Fizzy as she transforms with an orange hue.Fizzy quickly grabs her by the barrel and hoists her into firing position, revealing her to be an assault rifle.
      // Fizzy wastes no time in firing wildly through the windows and door with a wide grin on her face, shattering the glass entirely.Guess I don't have to clean it anymore. 
      .text(Conversation.Speaker.VANESSA, "Could we not attract more attention than we need to? I swore we were in here specifically to stop fighting for a spell.")
      .text(Conversation.Speaker.ROSE, "Yeahg, Ihm mnot domm mmy mmffn.")
      .text(Conversation.Speaker.FIZZY, "Yeah, yeah. C'mon, your turn, Vanessa!")
      .text(Conversation.Speaker.MC_NARRATION, "The roar of a monster fills the air. Then more roars, closer, join in.")
      .text(Conversation.Speaker.VANESSA, "I'm not sure we have time for that.")
      .text(Conversation.Speaker.MAY, "Preeetty sure you just got the attention of every monster in the neighborhood, Fiz.")
      .text(Conversation.Speaker.FIZZY, "My bad.")
      .text(Conversation.Speaker.MAY, "<i>Sigh.</i> We'd better get moving. This place isn't really defensible.")
      .text(Conversation.Speaker.ROSE, "Cmhm fhef dmf— Urfh…")
      .text(Conversation.Speaker.MC_NARRATION, "Rose quickly scarfs down the remainder of her muffin.")
      .text(Conversation.Speaker.ROSE, "Come on, sis, don't be stupid. We've been running around all morning; you guys can barely stand.")
      .text(Conversation.Speaker.FIZZY, "Even <i>I'm</i> tired of running.")
      .text(Conversation.Speaker.VANESSA, "I'm afraid she's right. We've no choice but to make a stand here, defensible or not.")
      .text(Conversation.Speaker.MAY, "Shit. I'm sorry guys, I should have found somewhere safer for us to rest.")
      .speaker(Conversation.Speaker.FIZZY)
      .choice("If only there was some random helpful person willing to carry us poor, exhausted, attractive girls around. And possibly use us to slay demons.",
        "Demon slaying? Awesome, sign me up!",
        "Oooh! Me! I'm a random helpful person willing to something something attractive girls.",
        "Subtle <i>and</i> modest. How can I refuse such charm?",
        "Well, I guess I'd be a lot safer with a bunch of sentient gun ladies by my side.",
        (selection, _) => {
          // TODO
        }
      )
      .text(Conversation.Speaker.MC, "I should also mention there's a back door, leads onto some side streets away from the main roads. Might have a better chance of escaping monsters back there.")
      .text(Conversation.Speaker.MAY, "I mean, ok, that all <i>sounds</i> pretty good, but—")
      .speaker(Conversation.Speaker.ROSE)
      .choice("Do you even know how to <i>use</i> a gun?", PROFICIENCY_0, PROFICIENCY_1, PROFICIENCY_2, PROFICIENCY_3, (selection, _) => {
        proficiencySelection = selection;
      })
      // TODO this isn't working; looks like PROF_3 is always being selected.
      .text(Conversation.Speaker.MC, 
        proficiencySelection == 0 ? PROFICIENCY_0 :
        proficiencySelection == 1 ? PROFICIENCY_1 :
        proficiencySelection == 2 ? PROFICIENCY_2 :
        PROFICIENCY_3
      )
      .wait(proficiencySelection == 0 ? 0f : 1.5f)
      .text(Conversation.Speaker.FIZZY, "Good enough for me!")
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

  private IEnumerator zoomOutSlowly(Camera camera) {
    while (camera.fieldOfView < 75) {
      camera.fieldOfView += Time.deltaTime * (0.2f + 4.5f * (camera.fieldOfView / 75));
      yield return null;
    }
    camera.fieldOfView = 75;
  }

  private IEnumerator zoomOutQuickly(Camera camera) {
    float velocity = 0;
    while (camera.fieldOfView < 75) {
      camera.fieldOfView = Mathf.SmoothDamp(camera.fieldOfView, 75, ref velocity, 1.8f);
      yield return null;
    }
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
    setState(20);
  }

  private IEnumerator waitForPlayerToMoveAround(Transform playerTransform) {
    while (playerTransform.position.z < -3) yield return null;
    setState(30);
  }

  // TODO move this to some utils file
  private IEnumerator smoothMove(Transform transform, Vector3 destination, float duration) { return smoothMove(transform, destination, duration, null); }
  private IEnumerator smoothMove(Transform transform, Vector3 destination, float duration, Callback callback) {
    Vector3 velocity = Vector3.zero;
    while (transform.position != destination) {
      yield return null;
      transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, duration);
    }
    callback?.Invoke();
  }

  // Move the transform at a constant speed to the destination, with slight vertical headbob.
  private IEnumerator walk(Transform transform, Vector3 destination, float duration) {
    Vector3 origin = transform.position;
    float t = 0f; // Realtime spent walking
    float progress = 0; // Fraction completed
    while (transform.position != destination) {
    //  if (TimeUtils.dialogPaused) yield return null;

   //   t += Time.deltaTime;
      progress += Time.deltaTime / duration;
      Vector3.Lerp(origin, destination, progress);
 //     transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(t), transform.position.z);
      yield return null;
    }
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
}
