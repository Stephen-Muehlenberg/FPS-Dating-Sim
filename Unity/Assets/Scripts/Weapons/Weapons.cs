using UnityEngine;

public class Weapons {
  public static Weapon SHOTGUN = new Weapon("Rose", "Shotgun", 0, true, false, Conversation.Speaker.ROSE, CombatDialog.Speaker.ROSE);
  public static Weapon MACHINE_GUN = new Weapon("May", "Machine Gun", 1, true, false, Conversation.Speaker.MAY, CombatDialog.Speaker.MAY);
  public static Weapon SNIPER_RIFLE = new Weapon("Vanessa", "Sniper Rifle", 2, true, false, Conversation.Speaker.VANESSA, CombatDialog.Speaker.VANESSA);
  public static Weapon GRENADE_LAUNCHER = new Weapon("Fizzy", "Grenade Launcher", 3, true, false, Conversation.Speaker.FIZZY, CombatDialog.Speaker.FIZZY);
  public static Weapon[] array = new Weapon[] { SHOTGUN, MACHINE_GUN, SNIPER_RIFLE, GRENADE_LAUNCHER };
  public static Weapon currentlyEquipped;

  public static Weapon randomWeapon() { return array[Random.Range(0, 4)]; }
  public static Weapon randomEquipableWeapon() { return randomWeaponWithEquipStatus(true); }
  public static Weapon randomNonEquipableWeapon() { return randomWeaponWithEquipStatus(false); }

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
}
