public class Weapon {
  public string name;
  public string weaponName;
  public int index;
  public bool canEquip;
  public bool equipped;
  public WeaponController controller;
  public Character character;

  public Weapon(string name, string weaponName, int index, bool canEquip, bool equipped, Character character) {
    this.name = name;
    this.weaponName = weaponName;
    this.index = index;
    this.canEquip = canEquip;
    this.equipped = equipped;
    this.character = character;
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
