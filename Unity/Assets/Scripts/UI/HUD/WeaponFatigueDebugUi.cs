using UnityEngine;
using UnityEngine.UI;

public class WeaponFatigueDebugUi : MonoBehaviour {
  public Image fatigueFill;
  public Image fatigueBackground;

  public bool showDebugBars = false;

  private GunSwitch gunSwitch;

  void Start () {
    gunSwitch = Player.SINGLETON.GetComponent<GunSwitch>();
    setShowDebug(showDebugBars);
	}
	
	void Update () {
    if (Input.GetKeyDown(KeyCode.F)) setShowDebug(!showDebugBars);

    if (!showDebugBars) return;

    if (Weapons.currentlyEquipped != null) fatigueFill.fillAmount = Weapons.currentlyEquipped.controller.fatigue.getAsFraction();
    else fatigueFill.fillAmount = 0;
  }

  private void setShowDebug(bool showDebug) {
    showDebugBars = showDebug;
    fatigueFill.enabled = showDebug;
    fatigueBackground.enabled = showDebug;
  }
}
