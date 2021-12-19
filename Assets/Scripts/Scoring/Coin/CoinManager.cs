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
    GameObject[] prefabPool;
    public uint targetNum = 1;

    public HillKingScoring scoreManager;
    ArrayList totalSpawnPoints;
    ArrayList availableSpawnPoints;
    
    ArrayList spawnedCoins;
    // Start is called before the first frame update


    public void collected(GameObject coin, GameObject collector) {
        // reset coin and adjust lists
        availableSpawnPoints.Add(coin.transform.position);
        coin.GetComponent<Animator>().Play(stateName: "Entry");
        coin.SetActive(false);
        spawnedCoins.Remove(coin);

        Vector3 locationForRespawn = selectRandomly(availableSpawnPoints);
        spawn(locationForRespawn);
        if (!scoreManager) return;
        scoreManager.addPointToPlayer(collector);

    }


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

    void initPool(){
        prefabPool = new GameObject[targetNum];
        for (int i = 0; i < prefabPool.Length; i++){
            prefabPool[i] = Instantiate(CoinPrefab, Vector3.zero, Quaternion.identity);
            prefabPool[i].SetActive(false);
        }
    }

    GameObject firstInactive(){
        foreach (var prefab in prefabPool){
            if (prefab.activeSelf == false) return prefab;
        }
        return null;
    }

    void spawn(Vector3 position) {
        if (spawnedCoins.Count > targetNum) return;
        var spawnedCoin = firstInactive();
        spawnedCoins.Add(spawnedCoin);
        availableSpawnPoints.Remove(position);
    }

    Vector3 selectRandomly(ArrayList list) {
        int index = Random.Range(0, list.Count);
        return (Vector3)list[index];
    }
}
