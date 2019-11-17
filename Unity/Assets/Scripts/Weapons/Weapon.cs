public class Weapon {
  public string name;
  public string weaponName;
  public int index;
  public bool inInventory = true; // Does this weapon show up in the weapon select menu?
  public bool canEquip = true; // Can this weapon be equipped?
  public bool equipped = false;
  public WeaponController controller;
  public Character character;
  public SelectionMenu.OptionColors selectionColors; // Weapon select menu colours

  public Weapon(string name, string weaponName, int index, Character character, SelectionMenu.OptionColors selectionColors) {
    this.name = name;
    this.weaponName = weaponName;
    this.index = index;
    this.character = character;
    this.selectionColors = selectionColors;
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
