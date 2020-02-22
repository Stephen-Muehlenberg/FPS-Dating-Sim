using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EventManager;
using static Character;
using static CombatDialog;

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

  /**
    * For optimization purposes, rules MUST be sorted by more criteria -> fewer criteria.
    */
  public static List<Event> COMMON_EVENTS = new List<Event> {
    newEvent(CONTEXT.equals(Context.PLAYER_HIT),
             CURRENT_WEAPON.equals(ROSE),
             ROSE.sayLine("You got this, man.")),
    newEvent(CONTEXT.equals(Context.PLAYER_HIT),
             CURRENT_WEAPON.equals(ROSE),
             ROSE.sayLine("Work through the pain!")),
    newEvent(CONTEXT.equals(Context.PLAYER_HIT),
             CURRENT_WEAPON.equals(MAY),
             MAY.sayLine("You ok?")),
    newEvent(CONTEXT.equals(Context.PLAYER_HIT),
             CURRENT_WEAPON.equals(MAY),
             MAY.sayLine("Ouch, that looked painful.")),
    newEvent(CONTEXT.equals(Context.PLAYER_HIT),
             CURRENT_WEAPON.equals(VANESSA),
             VANESSA.sayLine("Oh my!")),
    newEvent(CONTEXT.equals(Context.PLAYER_HIT),
             CURRENT_WEAPON.equals(VANESSA),
             VANESSA.sayLine("Do be careful, MC.")),
    newEvent(CONTEXT.equals(Context.PLAYER_HIT),
             CURRENT_WEAPON.equals(FIZZY),
             FIZZY.sayLine("Ouchies!")),
    newEvent(CONTEXT.equals(Context.PLAYER_HIT),
             CURRENT_WEAPON.equals(FIZZY),
             FIZZY.sayLine("Stop picking on MC, meanie!"))
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

  public static Callback sayLine(this Character speaker, string text, Priority priority = Priority.MEDIUM) {
    return () => {
      Debug.Log("sayLine: char = " + speaker.name + " (" + speaker.id + ")");
      new CombatDialog().message(speaker, text).show(priority); };
  }
}
