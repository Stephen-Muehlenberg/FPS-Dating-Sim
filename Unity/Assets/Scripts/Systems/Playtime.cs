using System;
using UnityEngine;

/**
 * Keeps track of the amount of time spent in-game.
 */
public class Playtime {
  private static readonly float TICKS_TO_SECONDS = 0.0000001f;
  private static DateTime startTime;
  private static int initialPlaytime;

  /**
   * Start counting playtime, including an initial playtime in seconds.
   */
  public static void startCount(int initialPlaytime) {
    startTime = DateTime.Now;
    Playtime.initialPlaytime = initialPlaytime;
  }

  /**
   * Get the current playtime in ticks (0.1 seconds)
   */
  public static int getCount() {
    long ticks = DateTime.Now.Ticks - startTime.Ticks;
    int seconds = Mathf.RoundToInt(ticks * TICKS_TO_SECONDS);
    return initialPlaytime + seconds;
  }
}
