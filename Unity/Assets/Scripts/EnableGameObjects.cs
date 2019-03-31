using UnityEngine;

public class EnableGameObjects : MonoBehaviour {
  public GameObject[] objectsToEnable;

  public void enableAll() {
    foreach (GameObject obj in objectsToEnable) {
      obj.SetActive(true);
    }
  }
}
