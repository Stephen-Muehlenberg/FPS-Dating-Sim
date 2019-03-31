using UnityEngine;

public class PlayerHealth : Health {
  private bool godMode = false;

  public void Update() {
    if (Input.GetKeyDown(KeyCode.G) && Time.timeScale > 0) {
      setGodMode(!godMode);
    }
  }

  override public void takeDamage(Damage damage) {
    if (remaining <= 0) return;

    DamageDirectionIndicator.show(transform, damage.origin);

    if (godMode) return;

    remaining -= damage.amount;
    if (remaining <= 0) SendMessage("die");
  }

  public void setGodMode(bool enabled) {
    godMode = enabled;
    Debug.Log("God mode " + (godMode ? "on" : "off"));
  }
}
