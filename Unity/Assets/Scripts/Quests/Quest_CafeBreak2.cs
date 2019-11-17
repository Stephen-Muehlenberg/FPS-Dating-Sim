using System.Collections;

public class Quest_CafeBreak2 : Quest {
  public static string NAME = "CafeBreak2";
  public override string name => NAME;

  protected override string getSceneForState(int state) {
    return "cafe";
  }

  protected override void handleState(int state) {
    if (state == 0) conversation1();
  }

  private void conversation1() {
    // TODO set lighting
    // TODO add sleeping bags and stuff to cafe back room
    // TODO have some conversation / dialog
  }
}

/*

  private void openingConversationPart1() {
    SceneTransition.fadeTo("Cafe", () => {
      // Set initial player position and clear girl positions
      Character.setPositions(OPENING_MC_POS, Quaternion.identity);
      // Lock player in place during conversation
      Player.SINGLETON.setInConversation(true);

      new Conversation()
        .wait(2f)
        .text(Character.MC_NARRATION, "<i>I stagger back into the café with four guns that each hop off and transform back into women.</i>")
        .wait(0.3f)
        .performAction(() => {
          Character.FIZZY.getProp()
            .setPosition(OPENING_FIZZY_POS)
            .GetComponent<ParticleSystem>().Play();
        })
        .wait(0.4f)
        .performAction(() => {
          Character.ROSE.getProp()
            .setPosition(OPENING_ROSE_POS)
            .GetComponent<ParticleSystem>().Play();
        })
        .wait(0.4f)
        .performAction(() => {
          Character.MAY.getProp()
            .setPosition(OPENING_MAY_POS)
            .GetComponent<ParticleSystem>().Play();
        })
        .wait(0.4f)
        .performAction(() => {
          Character.VANESSA.getProp()
            .setPosition(OPENING_VANESSA_POS)
            .GetComponent<ParticleSystem>().Play();
        })
        .wait(0.9f)
        .text("<i>Because apparently that is my life now.</i>")
        .wait(0.2f)
        .text(Character.MAY, "Ugh. I'm glad Mission Time is done with.")
        .text(Character.MC, "Mission Time, eh? So that was our first official mission?")
        .speed(Conversation.Speed.FAST)
        .speaker(Character.FIZZY)
        .performAction(() => {
          Player.SINGLETON.firstPersonController.look.inputEnabled = false;
          Player.SINGLETON.setLookDirection(Vector3.forward, 0.6f);
        })
        .choice("Sure was! Did you have fun? I had fun!", CHOICE_ENJOY_0, CHOICE_ENJOY_1, CHOICE_ENJOY_2, CHOICE_ENJOY_3, (choice, text) => {
          choiceEnjoy = choice;
          // TODO choose who responds to you and how based on your choice and maybe performance?
        })
        .dynamicallyInsert(() => {
          string selectedText = choiceEnjoy == 0 ? CHOICE_ENJOY_0 :
                                choiceEnjoy == 1 ? CHOICE_ENJOY_1 :
                                choiceEnjoy == 2 ? CHOICE_ENJOY_2 :
                                                   CHOICE_ENJOY_3;
          return new Conversation().text(Character.MC, selectedText);
        })
      .text(Character.VANESSA, "I did not enjoy it, particularly. But what's done is done.")
      .text("Let's have that break was so rudely interupted, hmm?")
      .text(Character.MC_NARRATION, "<i>Despite all the craziness of the last half hour, my waiter instincts reassert themselves.</i>")
      .text(Character.MC, "So can I get anyone something to drink?")
      .text(Character.MC_NARRATION, "<i>Or maybe it's</i> because <i> of all the craziness of the last half hour? Like my brain's trying to find something normal to latch on to.</i>")
      .text(Character.MAY, "I'm cool with whatever.")
      .text(Character.MC, "Every barista's favourite order. Rose?")
      .text(Character.ROSE, "Something girly. Any judgment will be balled up and returned to sender via your throat.")
      .text(Character.MC, "I don't know what that means. Fizzy? Something fizzy?")
        .speed(Conversation.Speed.FAST)
      .text(Character.FIZZY, "Cherry Coke, please! With the silliest straw you have!")
      .text(Character.FIZZY, "Ooh! What if you gave me <i>eight</i> straws? That'd be super-silly. Like it was made for an octopus!")
        .speed(Conversation.Speed.NORMAL)
      .text(Character.ROSE, "...What?")
      .text(Character.FIZZY, "What?")
      .text(Character.ROSE, "Why would an octopus want that? He still only has one mouth. Like—it doesn't—")
      .text(Character.VANESSA, "Let's move on. I will take—")
      .text(Character.ROSE, "No! It—it doesn't make any sense! I have four limbs, are you gonna give me four straws?")
        .speed(Conversation.Speed.FAST)
      .text(Character.FIZZY, "You don't hold straws with your feet, silly!")
        .speed(Conversation.Speed.NORMAL)
      .text(Character.ROSE, "<i>That's not the—</i>")
      .text(Character.MC, "Vanessa, please save me.")
      .text(Character.VANESSA, "I will take a lemonade, mixed with soda water and a splash of grenadine. Oh, and please make sure it's chilled.")
      .text(Character.MC, "That's not saving me. That's not saving me at all.")
      .show(() => { setState(20); });
    });
  }

  private void openingConversationPart3() {
    SceneTransition.fadeTo("Cafe", () => {
      Character.setPositions(INTERLUDE_MC_POS, Quaternion.Euler(0, 270, 0), INTERLUDE_ROSE_POS, INTERLUDE_MAY_POS, INTERLUDE_VANESSA_POS, INTERLUDE_FIZZY_POS);
      Player.SINGLETON.setInConversation(false, true);

      if (!deliveredRosesDrink) {
        Character.ROSE.getProp().setInteraction("Give drink", (roseTarget) => {
          Player.SINGLETON.setInConversation(true);
          deliveredRosesDrink = true;

          Conversation conversation = new Conversation()
            .text(Character.MC, "Here we go: one girly-ass drink.")
            .text(Character.ROSE, "You're thinking about judging me. I can tell.")
            .text(Character.MC, "I can promise you, I don't do a lot of thinking. Like, when it—")
            .text(Character.ROSE, "You know what? I picked up on that.")
            .text(Character.MC, "I <i>meant</i> when it comes to judging customers. It all blends together after all the years on the job. I do not have room in my brain to care.")
            .text(Character.MC_NARRATION, "<i>Rose raises her fruity glass to me.</i>")
            .text(Character.ROSE, "Here's to capitalism! Sucking the souls of the population one minimum-wage worker at a time.");

          int drinks = drinksRemaining;

          if (drinks > 1) conversation = conversation.text(Character.MC_NARRATION, "<i>After a pause, I grab a random glass off the tray to toast with her.</i>");
          else if (drinks == 1) conversation = conversation.text(Character.MC_NARRATION, "<i>After a pause, I grab the last glass off the tray to toast with her.</i>");
          else conversation = conversation
            .text(Character.MC, "...I don't have a glass to toast with.")
            .text(Character.ROSE, "See? That's because of the one percent. I'm telling you, it's fucked up.");

          conversation
            .text(Character.MC_NARRATION, "<i>I walk away from this strange anarchist ritual, not totally sure if she's serious or not.</i>")
            .text(Character.MC_NARRATION, "<i>Still. I feel an odd kinship.</i>")
            .show(onDrinkConversationEnded);
        });
      }

      if (!deliveredMaysDrink) {
        Character.MAY.getProp().setInteraction("Give drink", (mayTarget) => {
          Player.SINGLETON.setInConversation(true);
          deliveredMaysDrink = true;

          new Conversation()
          .text(Character.MAY, "Wait I have dialog in this demo? Sweeeet.")
          .show(onDrinkConversationEnded);
        });
      }

      if (!deliveredVanessasDrink) {
        Character.VANESSA.getProp().setInteraction("Give drink", (vanessaTarget) => {
          Player.SINGLETON.setInConversation(true);
          deliveredVanessasDrink = true;

          Conversation conversation = new Conversation()
          .text(Character.MC, "Annnnd here you go!")
          .text(Character.VANESSA, "Chilled?")
          .text(Character.MC, "Yup.")
          .text(Character.VANESSA, "Mixed with soda water?")
          .text(Character.MC, "Mm-hm.")
          .text(Character.VANESSA, "A splash of grenadine?")
          .text(Character.MC, "You got it.")
          .text(Character.MC_NARRATION, "Vanessa tentatively takes a sip.")
          .text(Character.VANESSA, "...This is none of those things.")
          .text(Character.MC, "Yeah, 'cause I was pretty sure you were just testing me to see how good I was at following instructions. Like Van Halen and their brown M&M thing.")
          .text(Character.VANESSA, "Hmm. Oddly perceptive.")
          .text(Character.MC, "We're also out of all of those things.")
          .text(Character.MC, "Not because of the apocalypse. This place was already shitty and poorly stocked.")
          .text(Character.VANESSA, "Who was responsible for doing inventory at this establishment?")
          .text(Character.MC, "Me.")
          .text(Character.VANESSA, "...")
          .text(Character.MC, "...")
          .text(Character.VANESSA, "...")
          .text(Character.MC, "Aaannnyway, I've got more drinks to hand out, sooo...");

          if (drinksRemaining == 0) {
            conversation = conversation
              .text(Character.VANESSA, "No, you don't.")
              .text(Character.MC, "And yet, I am still walking away.");
          }

          conversation.show(onDrinkConversationEnded);
        });
      }

      if (!deliveredFizzysDrink) {
        Character.FIZZY.getProp().setInteraction("Give drink", (fizzyTarget) => {
          Player.SINGLETON.setInConversation(true);
          deliveredFizzysDrink = true;

          new Conversation()
          .text(Character.MC, "For you, a Cherry Coke with the silliest straw we have.")
          .speed(Conversation.Speed.FAST)
          .text(Character.FIZZY, "Aw, it's just a normal straw! It's a serious straw, is what it is.")
          .speed(Conversation.Speed.NORMAL)
          .text(Character.MC, "It's a bendy straw! It bends.")
          .speed(Conversation.Speed.FAST)
          .text(Character.FIZZY, "Oh. Wait, the bend is at the bottom—did you put it in upside-down?")
          .speed(Conversation.Speed.NORMAL)
          .text(Character.MC, "Yeah. I thought that was <i>kind</i> of silly.")
          .speed(Conversation.Speed.FAST)
          .text(Character.FIZZY, "Aww, geez. I appreciate you doing your best to make up for your serious straws! I'll leave you a good Yelp review.")
          .show(onDrinkConversationEnded);
        });
      }
    });
  }

  private void onDrinkConversationEnded() {
    if (drinksRemaining > 0) Player.SINGLETON.setInConversation(false, true);
    else QuestManager.start(new Quest_BedStore());
  }
}

 */
