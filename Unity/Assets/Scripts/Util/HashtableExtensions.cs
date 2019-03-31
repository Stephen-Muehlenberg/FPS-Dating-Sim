using System.Collections;

public static class HashtableExtensions {
  public static object getOrDefault(this Hashtable hashtable, object key, object defaultValue) {
    if (hashtable.ContainsKey(key)) return hashtable[key];
    else return defaultValue;
  }
}
