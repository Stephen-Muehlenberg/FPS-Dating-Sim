using UnityEngine;
using System.Collections.Generic;

public class Affinity {
  public static float Rose = 0;
  public static float May = 0;
  public static float Vanessa = 0;
  public static float Fizzy = 0;

  /**
   * Returns the Character with the highest affinity. In the case of a draw, one of the winners is 
   * selected at random.
   */
  public static Character highest() {
    List<Character> highestChars = new List<Character>();
    if (Rose >= May && Rose >= Vanessa && Rose >= Fizzy) highestChars.Add(Character.ROSE);
    if (May >= Rose && May >= Vanessa && May >= Fizzy) highestChars.Add(Character.MAY);
    if (Vanessa >= Rose && Vanessa >= May && Vanessa >= Fizzy) highestChars.Add(Character.VANESSA);
    if (Fizzy >= Rose && Fizzy >= May && Fizzy >= Vanessa) highestChars.Add(Character.FIZZY);
    int randomIndex = Random.Range(0, highestChars.Count);
    return highestChars[randomIndex];
  }

  public static void log() {
    Debug.Log("AFFINITY: Rose = " + Rose + ", May = " + May + ", Vanessa = " + Vanessa + ", Fizzy = " + Fizzy);
  }
}
