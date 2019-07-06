using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LocationMarker : MonoBehaviour {
  private static GameObject prefab;
  private static List<LocationMarker> markers = new List<LocationMarker>();
  private static Vector3 offsetPosition; // Cached to avoid re-allocation

  public Transform location;
  public float verticalOffset;
  public Image image;

  private static void loadPrefab() {
    prefab = Resources.Load<GameObject>("UI/LocationMarker");
  }

  public static LocationMarker add(Transform location) { return add(location, 0f); }

  public static LocationMarker add(Transform location, float verticalOffset) {
    if (prefab == null) loadPrefab();
    var markerObj = Instantiate(prefab) as GameObject;
    markerObj.transform.SetParent(MainCanvas.transform);
    var marker = markerObj.GetComponent<LocationMarker>();
    marker.location = location;
    marker.verticalOffset = verticalOffset;
    markers.Add(marker);
    return marker;
  }

  public static void remove(LocationMarker marker) {
    for (int i = 0; i < markers.Count; i++) {
      if (markers[i] == marker) {
        markers.RemoveAt(i);
        return;
      }
    }
    throw new UnityException("Couldn't find marker " + marker + " to remove.");
  }

  public static void remove(Transform location) {
    for (int i = 0; i < markers.Count; i++) {
      if (markers[i].location == location) {
        markers.RemoveAt(i);
        return;
      }
    }
    throw new UnityException("Couldn't find marker with location " + location + " to remove.");
  }

  public static void clear() {
    foreach (LocationMarker marker in markers) {
      if (marker != null) Destroy(marker.gameObject);
    }
    markers.Clear();
  }

  void Update() {
    if (location == null) {
      Debug.LogWarning("LocationMarker self destructed because associated Transform was null!");
      markers.Remove(this);
      Destroy(gameObject);
      return;
    }

    offsetPosition = location.position + (Vector3.up * verticalOffset);

    // WorldToScreenPoint() will return a screen position even if location is behind player, so got to check if location is actually visible
    if (Vector3.Dot(Player.SINGLETON.camera.transform.forward, offsetPosition - Player.SINGLETON.transform.position) >= 0) {
      image.enabled = true;
      transform.position = Player.SINGLETON.camera.WorldToScreenPoint(offsetPosition);
    }
    else image.enabled = false;
  }
}
