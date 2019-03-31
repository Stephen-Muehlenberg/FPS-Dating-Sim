using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actors {
  private static Actor rose;
  private static Actor may;
  private static Actor vanessa;
  private static Actor fizzy;

  public static Actor getRose() {
    if (rose == null) rose = GameObject.Instantiate(Resources.Load<GameObject>("Actors/Rose")).GetComponent<Actor>();
    return rose;
  }

  public static Actor getMay() {
    if (may == null) may = GameObject.Instantiate(Resources.Load<GameObject>("Actors/May")).GetComponent<Actor>();
    return may;
  }

  public static Actor getVanessa() {
    if (vanessa == null) vanessa = GameObject.Instantiate(Resources.Load<GameObject>("Actors/Vanessa")).GetComponent<Actor>();
    return vanessa;
  }

  public static Actor getFizzy() {
    if (fizzy == null) fizzy = GameObject.Instantiate(Resources.Load<GameObject>("Actors/Fizzy")).GetComponent<Actor>();
    return fizzy;
  }
}
