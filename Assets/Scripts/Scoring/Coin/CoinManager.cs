using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour {
    private static CoinManager instance;
    public static CoinManager singleton() {
        if (instance == null) instance = new CoinManager();
        return instance;
    }

    public GameObject CoinPrefab;
    public uint targetNum = 1;

    public HillKingScoring scoreManager;
    ArrayList totalSpawnPoints;
    ArrayList availableSpawnPoints;

    ArrayList spawnedCoins;
    // Start is called before the first frame update
    void Start() {
        if (this != instance) instance = this;
        totalSpawnPoints = new ArrayList();
        spawnedCoins = new ArrayList();
        foreach (var spawnObject in GameObject.FindGameObjectsWithTag("CoinSpawn"))
            totalSpawnPoints.Add(spawnObject.transform.position);
        availableSpawnPoints = new ArrayList(totalSpawnPoints);
        print(availableSpawnPoints.Count);
        for (int i = 0; i < targetNum; i++) {
            spawn(selectRandomly(availableSpawnPoints));
        }
    }

    public void collected(GameObject coin, GameObject collector) {
        availableSpawnPoints.Add(coin.transform.position);
        spawnedCoins.Remove(coin);
        Vector3 locationForRespawn = selectRandomly(availableSpawnPoints);
        spawn(locationForRespawn);
        if (!scoreManager) return;
        scoreManager.addPointToPlayer(collector);
    }

    void spawn(Vector3 position) {
        if (spawnedCoins.Count > targetNum) return;
        GameObject spawnedCoin = GameObject.Instantiate(CoinPrefab, position, Quaternion.identity);
        spawnedCoins.Add(spawnedCoin);
        availableSpawnPoints.Remove(position);
    }

    Vector3 selectRandomly(ArrayList list) {
        int index = Random.Range(0, list.Count);
        return (Vector3)list[index];
    }
}
