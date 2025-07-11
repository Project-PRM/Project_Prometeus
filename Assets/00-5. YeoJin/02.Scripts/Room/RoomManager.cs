using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : PunSingleton<RoomManager>
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private string[] teamNames = { "A", "B", "C", "D", "E" };

    private Dictionary<string, Transform> _teamSpawnDict;

    private void Start()
    {
        // 팀별로 스폰 포인트를 할당
        if (PhotonNetwork.IsMasterClient)
        {
            AssignSpawnPoints();
        }
        // 스폰 포인트별로 팀 스폰
        // GeneratePlayer(myTeam);
        StartCoroutine(WaitForSpawnDataAndSpawn());
    }

    private void AssignSpawnPoints()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        List<Transform> shuffled = new List<Transform>(spawnPoints);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int randIndex = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[randIndex]) = (shuffled[randIndex], shuffled[i]);
        }

        // Room CustomProperties에 저장할 해시테이블
        Hashtable spawnTable = new Hashtable();

        for (int i = 0; i < Mathf.Min(teamNames.Length, shuffled.Count); i++)
        {
            Vector3 pos = shuffled[i].position;
            spawnTable[$"spawn_{teamNames[i]}"] = $"{pos.x},{pos.y},{pos.z}";
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(spawnTable);
        Debug.Log("Spawn points saved to room properties.");
    }

    private IEnumerator WaitForSpawnDataAndSpawn()
    {
        string myTeam = PhotonServerManager.Instance.GetPlayerTeam(PhotonNetwork.LocalPlayer);
        string key = $"spawn_{myTeam}";

        // 최대 3초까지 기다리기
        float timeout = 3f;
        while (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key) && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(key, out object spawnData))
        {
            string[] split = spawnData.ToString().Split(',');
            if (split.Length == 3 &&
                float.TryParse(split[0], out float x) &&
                float.TryParse(split[1], out float y) &&
                float.TryParse(split[2], out float z))
            {
                Vector3 basePos = new Vector3(x, y, z);
                Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * 1.5f;
                Vector3 finalPos = basePos + offset;

                GameObject player = PhotonNetwork.Instantiate("Player", finalPos, Quaternion.identity);
                Debug.Log($"Spawned player at {finalPos} for team {myTeam}");
            }
            else
            {
                Debug.LogError("Invalid spawn data format.");
            }
        }
        else
        {
            Debug.LogError($"No spawn data found for team {myTeam}");
        }
    }

    private void GeneratePlayer(string team)
    {
        if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom) return;

        // 팀 이름을 찾음
        if (!_teamSpawnDict.ContainsKey(team))
        {
            Debug.LogError($"No spawn point defined for team '{team}'");
            return;
        }

        Transform spawnPoint = _teamSpawnDict[team];

        // 오프셋 주기
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * 1.5f;

        Vector3 spawnPos = spawnPoint.position + offset;
        Quaternion spawnRot = spawnPoint.rotation;

        // 팀 이름에 맞는 스폰포인트에 스폰
        GameObject player = PhotonNetwork.Instantiate("Player", spawnPos, spawnRot);
        Debug.Log($"Spawned player for team {team} at {spawnPos}");
    }
}