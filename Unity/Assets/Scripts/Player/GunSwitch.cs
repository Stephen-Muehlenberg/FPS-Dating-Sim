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
    else if (Input.GetButtonUp("SelectWeapon")) endWeaponSelection();

    else if (Input.GetKeyUp(KeyCode.Alpha1)) equip(Weapons.SHOTGUN, true);
    else if (Input.GetKeyUp(KeyCode.Alpha2)) equip(Weapons.MACHINE_GUN, true);
    else if (Input.GetKeyUp(KeyCode.Alpha3)) equip(Weapons.SNIPER_RIFLE, true);
    else if (Input.GetKeyUp(KeyCode.Alpha4)) equip(Weapons.GRENADE_LAUNCHER, true);
  }

  private void startWeaponSelection() {
    TimeUtils.setTimeScale(SLOW_TIMESCALE);
  //  postProcessing.depthOfField.enabled = true;
    Player.SINGLETON.GetComponent<FirstPersonController>().look.inputDisabled = true;

    WeaponSelectMenu.startHighlight(Weapons.currentlyEquipped);
  }

  private void endWeaponSelection() {
    TimeUtils.clearTimeScale();
//    postProcessing.depthOfField.enabled = false;
    Player.SINGLETON.GetComponent<FirstPersonController>().look.inputDisabled = false;

    var selectedWeapon = WeaponSelectMenu.endHighlight();
    equip(selectedWeapon, true);
  }

  public void equip(Weapon newWeapon) { equip(newWeapon, false); }
  public void equip(Weapon newWeapon, bool playEffects) {
    if (Weapons.currentlyEquipped == newWeapon) return; // No change
    newWeapon.setEquipped(true);
    if (playEffects) audioSource.Play();
  }
}
