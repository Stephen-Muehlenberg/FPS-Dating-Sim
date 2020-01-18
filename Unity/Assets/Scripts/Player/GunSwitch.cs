using UnityEngine;
using FirstPersonModule;
//using UnityEngine.Rendering.PostProcessing;

public class GunSwitch : MonoBehaviour {
  private const float NORMAL_LOOK_SENSITIVITY = 5f;
  private const float SLOW_TIMESCALE = 0.05f;
  
  // TODO re-enable post processing once the PostProcessing library is no longer buggy
 // public PostProcessProfile postProcessing;
  public AudioSource audioSource;

  public bool equipRandomWeaponOnStart = true;

  void Start() {
    //   postProcessing.depthOfField.enabled = false;
    if (equipRandomWeaponOnStart) equip(Weapons.randomEquipableWeapon(), false);
  }

  void Update () {
    if (TimeUtils.gameplayPaused) return;

    if (Input.GetButtonDown("SelectWeapon")) startWeaponSelection();

    else if (Input.GetKeyUp(KeyCode.Alpha1)) equip(Weapons.SHOTGUN, true);
    else if (Input.GetKeyUp(KeyCode.Alpha2)) equip(Weapons.MACHINE_GUN, true);
    else if (Input.GetKeyUp(KeyCode.Alpha3)) equip(Weapons.SNIPER_RIFLE, true);
    else if (Input.GetKeyUp(KeyCode.Alpha4)) equip(Weapons.GRENADE_LAUNCHER, true);
  }

  private void startWeaponSelection() {
    TimeUtils.startBulletTime(SLOW_TIMESCALE);
  //  postProcessing.depthOfField.enabled = true;
    Player.SINGLETON.GetComponent<FirstPersonController>().look.inputEnabled = false;

    SelectionMenu.showWeaponChoice(
      options: Weapons.equipableOptions(),
      initialSelection: Weapons.currentlyEquipped == null ? -1 : Weapons.currentlyEquipped.index,
      callback: endWeaponSelection);
  }

  private void endWeaponSelection(int selection, string _) {
    TimeUtils.stopBulletTime();
//    postProcessing.depthOfField.enabled = false;
    Player.SINGLETON.GetComponent<FirstPersonController>().look.inputEnabled = true;
    equip(weapon: selection >= 0 ? Weapons.array[selection] : null,
          playEffects: true);
  }

  public void equip(Weapon weapon, bool playEffects = false) {
    if (Weapons.currentlyEquipped == weapon) return; // No change
    else {
      Weapons.equip(weapon, playEffects);
      if (playEffects) audioSource.Play();
    }
  }
}
