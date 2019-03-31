using UnityEngine;

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

  public void setEquipped(bool equipped) { setEquipped(equipped, false); }
  public void setEquipped(bool equipped, bool playEffects) {
    if (this.equipped == equipped) return; // No change // TODO i reckon this is the problem
    if (!canEquip) throw new UnityException("Can't equip " + name + " while canEquip == false.");

    if (equipped) {
      if (Weapons.currentlyEquipped != null) Weapons.currentlyEquipped.setEquipped(false);
      Weapons.currentlyEquipped = this;
    }
    else Weapons.currentlyEquipped = null;

    this.equipped = equipped;

    controller.setEquipped(equipped, playEffects);
  }

  // Weapons persist between scene changes, but controllers do not.
  // When new controllers are loaded in, make sure to initialize them
  // to match the weapon's state.
  public void setController(WeaponController controller) {
    this.controller = controller;
    controller.setVisible(equipped);
  }
}
