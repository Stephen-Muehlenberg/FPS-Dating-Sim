using UnityEngine;

public class EnemySpawner : MonoBehaviour {
  [System.Serializable]
  public class EnemySpawn {
    public GameObject prefab;
    public float threat = 1;

    public float spawnChance { get { return 1f / threat; } }
  }

  public int maxMonsters = 100;
  public Transform[] spawnPoints;
  public EnemySpawn[] spawns;

  private const float timeBetweenWaves = 23f;
  private float timeTillNextWave = 18f;

  private float threatInNextWave = 11;
  private float threatRemainingThisWave;
  private float spawnRange = 0f;

  void Start() {
    foreach (EnemySpawn spawn in spawns) spawnRange += spawn.spawnChance;
  }

	void Update () {
    if (Time.timeScale == 0) return; // Game is paused; do nothing

    timeTillNextWave -= Time.deltaTime;

    if (timeTillNextWave <= 0 && MonstersController.monsters.Count < maxMonsters) {
      threatRemainingThisWave = threatInNextWave;
      var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length - 1)];

      while (threatRemainingThisWave > 0) spawn(randomSpawn(), spawnPoint);

      threatInNextWave++;
      timeTillNextWave = timeBetweenWaves;
    }
	}

  private EnemySpawn randomSpawn() {
    var randomSpawnIndex = Random.Range(0f, spawnRange);
    foreach (EnemySpawn spawn in spawns) {
      if (randomSpawnIndex <= spawn.spawnChance) return spawn;
      randomSpawnIndex -= spawn.spawnChance;
    }
    throw new UnityException("Should not be possible to reach this point.");
  }

  private void spawn(EnemySpawn enemy, Transform spawnPoint) {
    var offset = new Vector3(Random.Range(-25f, 25f), 0, Random.Range(-5f, 5f));
    Instantiate(enemy.prefab, spawnPoint.position + offset, Quaternion.identity);
    threatRemainingThisWave -= enemy.threat;
  }
}
