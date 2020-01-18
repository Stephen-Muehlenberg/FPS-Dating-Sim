using UnityEngine;

public class Weapons {
  public delegate void EquipEvent(Weapon selection);

  public static Weapon SHOTGUN = new Weapon(
    name: "Rose",
    weaponName: "Shotgun",
    index: 0,
    character: Character.ROSE,
    selectionColors: new SelectionMenu.OptionColors(
      normal: new Color(1, 0.7137255f, 0.7137255f, 0.7686275f),
      selected: new Color(1, 0.4103774f, 0.4103774f))
    );

  public static Weapon MACHINE_GUN = new Weapon(
    name: "May",
    weaponName: "Machine Gun",
    index: 1,
    character: Character.MAY,
    selectionColors: new SelectionMenu.OptionColors(
      normal: new Color(0.7215686f, 1, 0.6117647f, 0.7686275f),
      selected: new Color(0.2700704f, 0.9433962f, 0))
    );

  public static Weapon SNIPER_RIFLE = new Weapon(
    name: "Vanessa",
    weaponName: "Sniper Rifle",
    index: 2,
    character: Character.VANESSA,
    selectionColors: new SelectionMenu.OptionColors(
      normal: new Color(0.6980392f, 0.9372549f, 1, 0.7686275f),
      selected: new Color(0, 0.9158051f, 1))
    );

  public static Weapon GRENADE_LAUNCHER = new Weapon(
    name: "Fizzy", 
    weaponName: "Grenade Launcher",
    index: 3,
    character: Character.FIZZY,
    selectionColors: new SelectionMenu.OptionColors(
      normal: new Color(1, 0.7843137f, 0.5137255f, 0.7686275f),
      selected: new Color(1, 0.629899f, 0))
    );

  public static Weapon[] array = new Weapon[] { SHOTGUN, MACHINE_GUN, SNIPER_RIFLE, GRENADE_LAUNCHER };
  public static Weapon currentlyEquipped;
  public static event EquipEvent equipEvents;

  public static Weapon randomWeapon() { return array[Random.Range(0, 4)]; }
  public static Weapon randomEquipableWeapon() { return randomWeaponWithEquipStatus(true); }
  public static Weapon randomNonEquipableWeapon() { return randomWeaponWithEquipStatus(false); }

  public static SelectionMenu.Option[] equipableOptions() {
    SelectionMenu.Option[] options = new SelectionMenu.Option[4];

    for (int i = 0; i < 4; i++) {
      if (!array[i].inInventory) continue;
      // TODO handle weapon fatigue colour change
      if (array[i].canEquip) options[i] = new SelectionMenu.Option(array[i].name, array[i].selectionColors);
      else options[i] = new SelectionMenu.Option(array[i].name, SelectionMenu.OptionColors.Unselectable); 
    }

    return options;
  }

  private static Weapon randomWeaponWithEquipStatus(bool canEquip) {
    int selectableCount = 0;
    foreach (Weapon weapon in array) { if (weapon.canEquip == canEquip) selectableCount++; }

    if (selectableCount == 0) throw new UnityException("No weapons are equipable");

    int randomIndex = Random.Range(0, selectableCount);
    for (int i = 0; i < 4; i++) {
      if (array[i].canEquip == canEquip) {
        if (randomIndex == 0) return array[i];
        else randomIndex--;
      }
    }

    throw new UnityException("Should be unreachable");
  }

  public static void equip(Weapon weapon, bool playEquipEffects = true) { setEquipped(weapon, true, playEquipEffects); }
  public static void unequip() { setEquipped(currentlyEquipped, false, false); }

  private static void setEquipped(Weapon weapon, bool equipped, bool playEquipEffects) {
    if (weapon == null && currentlyEquipped == null || weapon.equipped == equipped) return; // No change
    if (equipped && !weapon.canEquip) throw new UnityException("Can't equip " + weapon.name + " while canEquip == false.");

    if (equipped) {
      if (currentlyEquipped != null) setEquipped(currentlyEquipped, false, false);
      currentlyEquipped = weapon;
    }
    else {
      currentlyEquipped = null;
    }

    weapon.equipped = equipped;
    weapon.controller.setEquipped(equipped, playEquipEffects);

    if (equipEvents != null) equipEvents.Invoke(currentlyEquipped);
  }
}
