using System.Collections;
using UnityEngine;

public class MonsterSpawnEffect : MonoBehaviour {
  public GameObject chargingEffect;

  public void initialise(GameObject monsterPrafab, Quaternion rotation, bool awareOfPlayer) {
    StartCoroutine(waitToSpawn(monsterPrafab, rotation, awareOfPlayer));
  }

  private IEnumerator waitToSpawn(GameObject monsterPrefab, Quaternion rotation, bool awareOfPlayer) {
    yield return new WaitForSeconds(3);
    Destroy(chargingEffect);
    GameObject monster = Instantiate(monsterPrefab, transform.position, rotation);
    if (awareOfPlayer) monster.GetComponent<Awareness>().becomeAlert();
  }
}
