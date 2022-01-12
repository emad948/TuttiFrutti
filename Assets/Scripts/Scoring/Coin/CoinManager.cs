using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Mirror;

public class CoinManager : NetworkBehaviour
{
    private static CoinManager instance;

    public static CoinManager singleton()
    {
        if (instance == null) instance = new CoinManager();
        return instance;
    }

    public GameObject CoinPrefab;
    GameObject[] prefabPool;
    public uint targetNum = 1;

    public HillKingScoring scoreManager;
    ArrayList totalSpawnPoints;
    ArrayList availableSpawnPoints;

    [SyncVar] ArrayList spawnedCoins;
    // Start is called before the first frame update


    void Start()
    {
        if (this != instance) instance = this;
        totalSpawnPoints = new ArrayList();
        spawnedCoins = new ArrayList();
        foreach (var spawnObject in GameObject.FindGameObjectsWithTag("CoinSpawn"))
            totalSpawnPoints.Add(spawnObject.transform.position);
        availableSpawnPoints = new ArrayList(totalSpawnPoints);
        //print(availableSpawnPoints.Count);
        initPool();
        for (int i = 0; i < targetNum; i++)
        {
            spawn(selectRandomly(availableSpawnPoints));
        }
    }

    public void collected(GameObject coin, GameObject collector)
    {
        IEnumerator coroutine()
        {
            yield return new WaitForSeconds(2);
            availableSpawnPoints.Add(coin.transform.position);
            coin.BroadcastMessage("Reset");

            coin.SetActive(false);
            spawnedCoins.Remove(coin);

            Vector3 locationForRespawn = selectRandomly(availableSpawnPoints);
            spawn(locationForRespawn);
            if (!scoreManager) yield return 1;
            scoreManager.addPointToPlayer(collector);
        }

        StartCoroutine(coroutine());
    }

    void initPool()
    {
        prefabPool = new GameObject[targetNum];
        for (int i = 0; i < prefabPool.Length; i++)
        {
            prefabPool[i] = Instantiate(CoinPrefab, Vector3.zero, Quaternion.identity);
            prefabPool[i].SetActive(false);
        }
    }

    GameObject firstInactive()
    {
        foreach (var prefab in prefabPool)
        {
            if (prefab.activeSelf == false) return prefab;
        }

        return null;
    }

    void spawn(Vector3 position)
    {
        if (spawnedCoins.Count > targetNum) return;
        var spawnedCoin = firstInactive();
        //print(spawnedCoin);
        spawnedCoins.Add(spawnedCoin);
        availableSpawnPoints.Remove(position);
        spawnedCoin.transform.position = position;
        spawnedCoin.SetActive(true);
    }

    Vector3 selectRandomly(ArrayList list)
    {
        int index = Random.Range(0, list.Count);
        return (Vector3) list[index];
    }
}