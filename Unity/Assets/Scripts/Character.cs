using UnityEngine;

public class Character {
  public string name;
  public Weapon weapon;
  public Color dialogTextOutline; // Out of combat text colours
  public Color dialogTextFill;    // Ditto
  public Color combatTextOutline; // In combat text colours
  public bool hasProp; // True if the character has a sprite body (e.g. May), false if not (e.g. Player)
  private Actor prop;

  public string portraitResource { get { return "Textures/Portrait" + name; } }

  private Character(string name, Weapon weapon, Color dialogTextOutline, Color dialogTextFill, Color combatTextOutline, bool hasProp) {
    this.name = name;
    this.weapon = weapon;
    this.dialogTextOutline = dialogTextOutline;
    this.dialogTextFill = dialogTextFill;
    this.combatTextOutline = combatTextOutline;
    this.hasProp = hasProp;
    this.prop = null;
  }

//  public static bool operator ==(Character a, Character b) { return a == null ? b == null : a.Equals(b); }
  //public static bool operator !=(Character a, Character b) { return a == null ? b != null : !a.Equals(b); }

  public static Character MC = new Character("MC", null, new Color(0.25f, 0.25f, 0.25f), Color.white, new Color(0.42f, 0.42f, 0.42f), false);
  public static Character MC_NARRATION = new Character("MC", null, new Color(0.35f, 0.35f, 0.35f), new Color(0.8f, 0.8f, 0.8f), Color.yellow, false);
  // NONE is used for text not originating from any characers, e.g. onomatopoeia or non-diagetic text.
  public static Character NONE = new Character("", null, new Color(0.35f, 0.35f, 0.35f), new Color(0.8f, 0.8f, 0.8f), Color.white, false);
  public static Character ROSE = new Character("Rose", Weapons.SHOTGUN, new Color(1, 0, 0, 0.5f), new Color(1, 0.9333f, 0.9333f, 1), new Color(1, 0, 0), true);
  public static Character MAY = new Character("May", Weapons.MACHINE_GUN, new Color(0, 0.5f, 0, 0.5f), new Color(0.9333f, 1, 0.9333f, 1), new Color(0, 0.72f, 0.06f), true);
  public static Character VANESSA = new Character("Vanessa", Weapons.SNIPER_RIFLE, new Color(0, 0.5f, 1, 0.5f), new Color(0.9333f, 0.9333f, 1, 1), new Color(0.5f, 0.5f, 1), true);
  public static Character FIZZY = new Character("Fizzy", Weapons.GRENADE_LAUNCHER, new Color(0.85f, 0.508f, 0.11f, 0.5f), new Color(1, 1, 0.9333f, 1), new Color(0.87f, 0.63f, 0.24f), true);
  
  public static void setPositions(Vector3? mcPos, Quaternion? mcRot) { setPositions(mcPos, mcRot, null, null, null, null); }
  public static void setPositions(Vector3? rosePos, Vector3? mayPos, Vector3? vanessaPos, Vector3? fizzyPos) { setPositions(null, null, rosePos, mayPos, vanessaPos, fizzyPos); }
  /**
   * Sets the position and rotation of MC (the player), and the positions of the girls.
   * If MC's position or rotation is null, that value will not be set.
   * If a girl's position is null, that girl will not be in the scene.
   */
  public static void setPositions(Vector3? mcPos, Quaternion? mcRot, Vector3? rosePos, Vector3? mayPos, Vector3? vanessaPos, Vector3? fizzyPos) {
    if (mcPos.HasValue) Player.SINGLETON.transform.position = mcPos.Value;
    if (mcRot.HasValue) Player.SINGLETON.transform.rotation = mcRot.Value;

    if (rosePos.HasValue) ROSE.getProp().transform.position = rosePos.Value;
    else if (ROSE.prop != null) GameObject.Destroy(ROSE.prop);

    if (mayPos.HasValue) MAY.getProp().transform.position = mayPos.Value;
    else if (MAY.prop != null) GameObject.Destroy(MAY.prop);

    if (vanessaPos.HasValue) VANESSA.getProp().transform.position = vanessaPos.Value;
    else if (VANESSA.prop != null) GameObject.Destroy(VANESSA.prop);

    if (fizzyPos.HasValue) FIZZY.getProp().transform.position = fizzyPos.Value;
    else if (FIZZY.prop != null) GameObject.Destroy(FIZZY.prop);
  }

  public Actor getProp() {
    if (!hasProp) throw new UnityException("hasProp = false for character " + this);

    if (prop == null) prop = GameObject.Instantiate(Resources.Load<GameObject>("Actors/" + name)).GetComponent<Actor>();
    return prop;
  }
}
