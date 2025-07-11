using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class RoomManager : PunSingleton<RoomManager>
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private string[] teamNames = { "A", "B", "C", "D", "E" };

    private Dictionary<string, Transform> _teamSpawnDict;

    private void Start()
    {
        // 팀별로 스폰 포인트를 할당
        AssignSpawnPoints();
        // 스폰 포인트별로 팀 스폰
        PhotonServerManager.Instance.OnGameStarted += GeneratePlayer;
    }

    private void AssignSpawnPoints()
    {
        // 팀 A ~ E를 각각 스폰포인트에 할당
        // 1. 스폰포인트 셔플
        List<Transform> shuffled = new List<Transform>(spawnPoints);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int randIndex = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[randIndex]) = (shuffled[randIndex], shuffled[i]);
        }

        // 2. 팀 이름에 따라 5개 선택해서 매핑
        for (int i = 0; i < Mathf.Min(teamNames.Length, shuffled.Count); i++)
        {
            _teamSpawnDict[teamNames[i]] = shuffled[i];
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

        // 팀 이름에 맞는 스폰포인트에 스폰
        GameObject player = PhotonNetwork.Instantiate("Player", spawnPoint.position, spawnPoint.rotation);
        Debug.Log($"Spawned player for team {team} at {spawnPoint.position}");
    }

}