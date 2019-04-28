public class Weapon {
  public string name;
  public string weaponName;
  public int index;
  public bool canEquip;
  public bool equipped;
  public WeaponController controller;
  public Conversation.Speaker conversationSpeaker;
  public CombatDialog.Speaker combatSpeaker;

  public Weapon(string name, string weaponName, int index, bool canEquip, bool equipped, Conversation.Speaker conversationSpeaker, CombatDialog.Speaker combatSpeaker) {
    this.name = name;
    this.weaponName = weaponName;
    this.index = index;
    this.canEquip = canEquip;
    this.equipped = equipped;
    this.conversationSpeaker = conversationSpeaker;
    this.combatSpeaker = combatSpeaker;
  }

  public void equip() { Weapons.setEquipped(this, true, true); }
  public void equip(bool playEffects) { Weapons.setEquipped(this, true, true); }
  public void unequip() { Weapons.setEquipped(this, false, false); }

  // Weapons persist between scene changes, but controllers do not.
  // When new controllers are loaded in, make sure to initialize them
  // to match the weapon's state.
  public void setController(WeaponController controller) {
    this.controller = controller;
    controller.setVisible(equipped);
  }
}
