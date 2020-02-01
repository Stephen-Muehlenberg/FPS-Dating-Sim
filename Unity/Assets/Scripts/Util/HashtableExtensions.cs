using System.Collections;

public static class HashtableExtensions {
  public static T getOrDefault<T>(this Hashtable hashtable, object key, T defaultValue) {
    if (hashtable.ContainsKey(key)) return (T) hashtable[key];
    else return defaultValue;
  }
}
