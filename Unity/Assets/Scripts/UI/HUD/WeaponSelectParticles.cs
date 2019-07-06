using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectParticles : MonoBehaviour {
  private static WeaponSelectParticles singleton;

  public Camera particleCamera;
  public ParticleSystem[] weaponParticles;

  public void Awake() {
    singleton = this;
  }

  public static void initialize() {
    if (singleton == null) {
      var prefab = Resources.Load("UI/UI Particles") as GameObject;
      var instance = Instantiate(prefab);
      singleton = instance.GetComponent<WeaponSelectParticles>();
    }

    singleton.particleCamera.enabled = true;
  }

  public static void stop() {
    if (singleton == null) throw new UnityException("Must call initialize() before stop().");

    singleton.particleCamera.enabled = false;
    foreach (ParticleSystem particles in singleton.weaponParticles) {
      particles.Stop();
      particles.Clear();
    }
  }

  public static void setEnabled(Weapon weapon, bool enabled) {
    if (singleton == null) throw new UnityException("Must call initialize() before setEnabled().");

    if (enabled) singleton.weaponParticles[weapon.index].Play();
    else singleton.weaponParticles[weapon.index].Stop();
  }
}
