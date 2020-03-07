using System.Collections.Generic;
using UnityEngine;
using static EventManager;
using static Character;
using static CombatDialog;

/**
 * For optimization purposes, rules MUST be sorted by more criteria -> fewer criteria.
 */
public static class EventsDb {
  public struct Criteria {
    public string name;
    public int min;
    public int max;

    public Criteria(string name, int min, int max) {
      this.name = name;
      this.min = min;
      this.max = max;
    }

    public Criteria(string name, int value) {
      this.name = name;
      this.min = value;
      this.max = value;
    }
  }

  public class Event {
    public List<Criteria> rule;
    public Callback response;
    public int score { get { return rule.Count; } }

    private static int value; // Cached for re-use.

    public Event(List<Criteria> rule, Callback response) {
      if (rule.Count == 0) throw new UnityException("rule must have at least 1 Criteria.");
      this.rule = rule;
      this.response = response;
    }

    public bool matches(Dictionary<string, int> args) {
      foreach (Criteria criteria in rule) {
        if (!args.ContainsKey(criteria.name)) return false;
        value = args[criteria.name];
        if (value < criteria.min || value > criteria.max) return false;
      }
      return true;
    }

    // This is just for debugging, so optimization is not important.
    public string ruleToString() {
      string s = "";
      foreach (Criteria criteria in rule) s += criteria.name + (criteria.min == criteria.max ? ("=" + criteria.min) : (" " + criteria.min + "-" + criteria.max)) + ", ";
      return s.Substring(0, s.Length - 2);
    }

    public void logRules() {
      foreach (Criteria criteria in rule) Debug.Log("- " + criteria.name + (criteria.min == criteria.max ? (" = " + criteria.min) : (" " + criteria.min + " - " + criteria.max)));
    }
  }

  // Events are grouped by Context to make searching through them all faster.
  public static List<Event> eventsFor(int context) {
    switch(context) {
      case Context.PLAYER_HIT: return PLAYER_HIT_EVENTS;
      case Context.WEAPON_MISSED: return WEAPON_MISSED_EVENTS;
      case Context.WEAPON_FATIGUED: return WEAPON_FATIGUED_EVENTS;
      case Context.WEAPON_EXHAUSTED: return WEAPON_EXHAUSTED_EVENTS;
      case Context.WEAPON_RECOVERED: return WEAPON_RECOVERED_EVENTS;
      case Context.ENEMY_KILLED: return ENEMY_KILLED_EVENTS;
    }
    Debug.Log("Unhandled context " + context);
    return new List<Event>();
  }

  private static List<Event> PLAYER_HIT_EVENTS = new List<Event> {
    newEvent(PLAYER_HP.isLessThan(300),
             MC.sayLine("Ow, fuck fuck FUCK!", Priority.MEDIUM)),
    newEvent(PLAYER_HP.isLessThan(300),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Don't go dying on me, buddy.", Priority.MEDIUM)),
    newEvent(PLAYER_HP.isLessThan(300),
             MAY_CAN_TALK.isTrue(),
             MAY.sayLine("Get out of there, MC!", Priority.MEDIUM)),
    newEvent(PLAYER_HP.isLessThan(300),
             VANESSA_CAN_TALK.isTrue(),
             VANESSA.sayLine("Girls, MC doesn't look to good...", Priority.MEDIUM)),
    newEvent(PLAYER_HP.isLessThan(300),
             FIZZY_CAN_TALK.isTrue(),
             FIZZY.sayLine("MC nooo!", Priority.MEDIUM)),
    newEvent(PLAYER_HP.isBetween(300, 1000),
             MC.sayLine("Ouch.", Priority.LOW)),
    newEvent(PLAYER_HP.isBetween(300, 1000),
             MC.sayLine("Oof.", Priority.LOW)),
    newEvent(PLAYER_HP.isBetween(300, 1000),
             MC.sayLine("Gah!", Priority.LOW)),
    newEvent(CURRENT_WEAPON.equals(ROSE),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("You got this, man.", Priority.LOW)),
    newEvent(CURRENT_WEAPON.equals(ROSE),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Work through the pain!", Priority.LOW)),
    newEvent(CURRENT_WEAPON.equals(MAY),
             MAY_CAN_TALK.isTrue(),
             MAY.sayLine("You ok?", Priority.LOW)),
    newEvent(CURRENT_WEAPON.equals(MAY),
             MAY_CAN_TALK.isTrue(),
             MAY.sayLine("Ouch, that looked painful.", Priority.LOW)),
    newEvent(CURRENT_WEAPON.equals(VANESSA),
             VANESSA_CAN_TALK.isTrue(),
             VANESSA.sayLine("Oh my!", Priority.LOW)),
    newEvent(CURRENT_WEAPON.equals(VANESSA),
             VANESSA_CAN_TALK.isTrue(),
             VANESSA.sayLine("Do be careful, MC.", Priority.LOW)),
    newEvent(CURRENT_WEAPON.equals(FIZZY),
             FIZZY_CAN_TALK.isTrue(),
             FIZZY.sayLine("Ouchies!", Priority.LOW)),
    newEvent(CURRENT_WEAPON.equals(FIZZY),
             FIZZY_CAN_TALK.isTrue(),
             FIZZY.sayLine("Stop picking on MC, meanie!", Priority.LOW))
  };

  private static List<Event> WEAPON_MISSED_EVENTS = new List<Event> {
    newEvent(TRIGGERED_BY.equals(ROSE),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Damnit.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(ROSE),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Grr...", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(ROSE),
             FIZZY_CAN_TALK.isTrue(),
             FIZZY.sayLine("Ha ha, you missed horribly.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(MAY),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Get good, sis.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(MAY),
             MAY_CAN_TALK.isTrue(),
             MAY.sayLine("Damni- er, whoops.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(MAY),
             MAY_CAN_TALK.isTrue(),
             MAY.sayLine("Ack! Sorry, MC", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(VANESSA),
             VANESSA_CAN_TALK.isTrue(),
             VANESSA.sayLine("Drat.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(VANESSA),
             VANESSA_CAN_TALK.isTrue(),
             VANESSA.sayLine("Hmph.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(VANESSA),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Haha!", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(FIZZY),
             FIZZY_CAN_TALK.isTrue(),
             FIZZY.sayLine("Oopsie.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(FIZZY),
             FIZZY_CAN_TALK.isTrue(),
             FIZZY.sayLine("Haha, I completely missed...", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(FIZZY),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("How do you miss with a huge-ass explosion?", Priority.TRIVIAL))
  };

  private static List<Event> WEAPON_FATIGUED_EVENTS = new List<Event> {
    newEvent(TRIGGERED_BY.equals(ROSE),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Phew, this is a good work out.", Priority.HIGH)),
    newEvent(TRIGGERED_BY.equals(ROSE),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Definitely feeling the burn, MC.", Priority.HIGH)),
    newEvent(TRIGGERED_BY.equals(ROSE),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("I can, hah, do this all day.", Priority.HIGH)),
    newEvent(TRIGGERED_BY.equals(ROSE),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Getting kinda thirsty here.", Priority.HIGH)),
    newEvent(TRIGGERED_BY.equals(MAY),
             MAY_CAN_TALK.isTrue(),
             MAY.sayLine("Hah, s-slow down there, hah, MC.", Priority.HIGH)),
    newEvent(TRIGGERED_BY.equals(VANESSA),
             VANESSA_CAN_TALK.isTrue(),
             VANESSA.sayLine("Do guns sweat? I think I'm sweating.", Priority.HIGH)),
    newEvent(TRIGGERED_BY.equals(VANESSA),
             VANESSA_CAN_TALK.isTrue(),
             VANESSA.sayLine("Goodness, this is... <i>huff</i>... tiring.", Priority.HIGH)),
    newEvent(TRIGGERED_BY.equals(FIZZY),
             MAY_CAN_TALK.isTrue(),
             MAY.sayLine("Fizz? I think you should take a break.", Priority.HIGH)),
    newEvent(TRIGGERED_BY.equals(FIZZY),
             VANESSA_CAN_TALK.isTrue(),
             VANESSA.sayLine("Careful you don't overexert yourself, dear.", Priority.HIGH))
  };

  private static List<Event> WEAPON_EXHAUSTED_EVENTS = new List<Event> {
    newEvent(TRIGGERED_BY.equals(ROSE),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Gimme... <i>huff</i>... a sec...", Priority.HIGH)),
    newEvent(TRIGGERED_BY.equals(ROSE),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Fuck... <i>huff</i>... that.", Priority.HIGH)),
    newEvent(TRIGGERED_BY.equals(MAY),
             MAY_CAN_TALK.isTrue(),
             MAY.sayLine("Can't... <i>wheeze</i>... shoot...", Priority.HIGH)),
    newEvent(TRIGGERED_BY.equals(MAY),
             MAY_CAN_TALK.isTrue(),
             MAY.sayLine("Just... just wait a, a sec...", Priority.HIGH)),
    newEvent(TRIGGERED_BY.equals(VANESSA),
             VANESSA_CAN_TALK.isTrue(),
             VANESSA.sayLine("Just... <i>huff... puff...</i> a moment.", Priority.HIGH)),
    newEvent(TRIGGERED_BY.equals(VANESSA),
             VANESSA_CAN_TALK.isTrue(),
             VANESSA.sayLine("Please, I'm <i>huff</i> not meant for <i>puff</i> rapid fire.", Priority.HIGH)),
    newEvent(TRIGGERED_BY.equals(VANESSA),
             VANESSA_CAN_TALK.isTrue(),
             VANESSA.sayLine("S-slow down, please...", Priority.HIGH)),
    newEvent(TRIGGERED_BY.equals(FIZZY),
             FIZZY_CAN_TALK.isTrue(),
             FIZZY.sayLine("So... <i>huff</i>... many... <i>puff</i>... explosions.", Priority.HIGH)),
    newEvent(TRIGGERED_BY.equals(FIZZY),
             FIZZY_CAN_TALK.isTrue(),
             FIZZY.sayLine("I think I... need a nap...", Priority.HIGH)),
    newEvent(TRIGGERED_BY.equals(FIZZY),
             FIZZY_CAN_TALK.isTrue(),
             FIZZY.sayLine("My barrel hurts :(", Priority.HIGH))
  };

  private static List<Event> WEAPON_RECOVERED_EVENTS = new List<Event> {
    newEvent(TRIGGERED_BY.equals(ROSE),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Alright MC, I'm good to go.", Priority.MEDIUM))
  };

  private static List<Event> ENEMY_KILLED_EVENTS = new List<Event> {
    newEvent(TRIGGERED_BY.equals(ROSE),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Gotcha.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(ROSE),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Blam!", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(ROSE),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Die!", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(ROSE),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Mmm, giblets...", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(ROSE),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("That's how it's done.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(ROSE),
             MAY_CAN_TALK.isTrue(),
             MAY.sayLine("Great work, you two.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(ROSE),
             VANESSA_CAN_TALK.isTrue(),
             VANESSA.sayLine("My, what a mess.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(MAY),
             MAY_CAN_TALK.isTrue(),
             MAY.sayLine("Pew pew!", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(MAY),
             MAY_CAN_TALK.isTrue(),
             MAY.sayLine("Monster's dead.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(MAY),
             VANESSA_CAN_TALK.isTrue(),
             VANESSA.sayLine("Nicely done.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(MAY),
             FIZZY_CAN_TALK.isTrue(),
             FIZZY.sayLine("You sure filled him with holes.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(VANESSA),
             VANESSA_CAN_TALK.isTrue(),
             VANESSA.sayLine("Excellent.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(VANESSA),
             VANESSA_CAN_TALK.isTrue(),
             VANESSA.sayLine("Nice and neat.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(VANESSA),
             VANESSA_CAN_TALK.isTrue(),
             VANESSA.sayLine("Good shot, MC.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(VANESSA),
             FIZZY_CAN_TALK.isTrue(),
             FIZZY.sayLine("Nice shooting, Ness!", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(VANESSA),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Not too shabby, girl.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(FIZZY),
             FIZZY_CAN_TALK.isTrue(),
             FIZZY.sayLine("Ka-boom!", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(FIZZY),
             FIZZY_CAN_TALK.isTrue(),
             FIZZY.sayLine("Wahahaa!", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(FIZZY),
             FIZZY_CAN_TALK.isTrue(),
             FIZZY.sayLine("Isn't this just so much fun?", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(FIZZY),
             FIZZY_CAN_TALK.isTrue(),
             FIZZY.sayLine("I love explosions.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(FIZZY),
             ROSE_CAN_TALK.isTrue(),
             ROSE.sayLine("Haha, you guys are crazy.", Priority.TRIVIAL)),
    newEvent(TRIGGERED_BY.equals(FIZZY),
             VANESSA_CAN_TALK.isTrue(),
             VANESSA.sayLine("There's something strangely beautiful about monsters exploding.", Priority.TRIVIAL))
  };

  private static Event newEvent(Criteria c1, Callback response) { return new Event(rule: new List<Criteria> { c1 }, response: response); }
  private static Event newEvent(Criteria c1, Criteria c2, Callback response) { return new Event(rule: new List<Criteria> { c1, c2 }, response: response); }
  private static Event newEvent(Criteria c1, Criteria c2, Criteria c3, Callback response) { return new Event(rule: new List<Criteria> { c1, c2, c3 }, response: response); }
  private static Event newEvent(Criteria c1, Criteria c2, Criteria c3, Criteria c4, Callback response) { return new Event(rule: new List<Criteria> { c1, c2, c3, c4 }, response: response); }
  private static Event newEvent(Criteria c1, Criteria c2, Criteria c3, Criteria c4, Criteria c5, Callback response) { return new Event(rule: new List<Criteria> { c1, c2, c3, c4, c5 }, response: response); }
  private static Event newEvent(Criteria c1, Criteria c2, Criteria c3, Criteria c4, Criteria c5, Criteria c6, Callback response) { return new Event(rule: new List<Criteria> { c1, c2, c3, c4, c5, c6 }, response: response); }
  private static Event newEvent(Criteria c1, Criteria c2, Criteria c3, Criteria c4, Criteria c5, Criteria c6, Criteria c7, Callback response) { return new Event(rule: new List<Criteria> { c1, c2, c3, c4, c5, c6, c7 }, response: response); }

  private static Criteria equals(this string name, int value) { return new Criteria(name, value, value); }
  private static Criteria equals(this string name, Character character) { return new Criteria(name, character.id, character.id); }
  private static Criteria isLessThan(this string name, int value) { return new Criteria(name, int.MinValue, value - 1); }
  private static Criteria isLessThanOrEquals(this string name, int value) { return new Criteria(name, int.MinValue, value); }
  private static Criteria isMoreThan(this string name, int value) { return new Criteria(name, value + 1, int.MaxValue); }
  private static Criteria isMoreThanOrEquals(this string name, int value) { return new Criteria(name, value, int.MaxValue); }
  private static Criteria isBetween(this string name, int min, int max) { return new Criteria(name, min, max); }
  private static Criteria isTrue(this string name) { return new Criteria(name, 1, 1); }
  private static Criteria isFalse(this string name) { return new Criteria(name, 0, 0); }

  public static Callback sayLine(this Character speaker, string text, Priority priority = Priority.MEDIUM) {
    return () => {
      new CombatDialog().message(speaker, text).show(priority); };
  }
}
