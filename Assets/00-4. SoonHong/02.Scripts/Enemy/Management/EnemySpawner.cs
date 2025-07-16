using UnityEngine;
using Photon.Pun;
using System.Collections;

public class EnemySpawner : MonoBehaviourPun
{
    public GameObject EnemyPrefab;
    public Transform[] SpawnPoints;
    public float SpawnInterval = 10f; 
    private float _firstSpawnTimer = 300f;

    private void Start()
    {
        // 3분 뒤 10초(SpawnInterval)간격으로 Spawn 추후 수정
        StartCoroutine(FirstSpawnTimer_Coroutine());
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnLoop_Coroutine());
        }
    }

    private IEnumerator FirstSpawnTimer_Coroutine()
    {
        yield return new WaitForSeconds(_firstSpawnTimer);
    }
    private IEnumerator SpawnLoop_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(SpawnInterval);
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, SpawnPoints.Length);
        Vector3 pos = SpawnPoints[randomIndex].position;
        Quaternion rot = Quaternion.identity;

        GameObject enemyGO = PhotonNetwork.Instantiate(EnemyPrefab.name, pos, rot);
        EnemyBase enemy = enemyGO.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            EnemyManager.Instance.Register(enemy);
        }

        EnemyBTBase bt = enemyGO.GetComponent<EnemyBTBase>();
        bt?.Init();
    }
}