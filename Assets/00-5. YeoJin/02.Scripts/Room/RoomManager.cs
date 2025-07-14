using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class RoomManager : PunSingleton<RoomManager>
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField]private Vector3[] _spawnOffset; 

    private void Start()
    {
        // 팀별로 스폰 포인트를 할당
        if (PhotonNetwork.IsMasterClient)
        {
            AssignSpawnPoints();
        }
        // 스폰 포인트별로 팀 스폰
        StartCoroutine(WaitForSpawnDataAndSpawn());
    }

    private HashSet<string> GetActiveTeams()
    {
        HashSet<string> teamSet = new HashSet<string>();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("team", out object teamValue))
            {
                string teamName = teamValue as string;
                if (!string.IsNullOrEmpty(teamName))
                {
                    teamSet.Add(teamName);
                }
            }
        }

        return teamSet;
    }

    private void AssignSpawnPoints()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        HashSet<string> activeTeams = GetActiveTeams();

        List<Transform> shuffled = new List<Transform>(spawnPoints);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int randIndex = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[randIndex]) = (shuffled[randIndex], shuffled[i]);
        }

        Hashtable spawnTable = new Hashtable();
        int j = 0;

        foreach (string team in activeTeams)
        {
            if (j >= shuffled.Count)
            {
                Debug.LogWarning("스폰포인트가 부족합니다.");
                break;
            }

            Vector3 pos = shuffled[j].position;
            spawnTable[$"spawn_{team}"] = $"{pos.x},{pos.y},{pos.z}";
            j++;
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(spawnTable);
        Debug.Log("스폰포인트 할당 및 저장완료.");
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
            if (split.Length == 3 && float.TryParse(split[0], out float x) && float.TryParse(split[1], out float y) && float.TryParse(split[2], out float z))
            {
                // 기본 스폰포인트 위치
                Vector3 basePos = new Vector3(x, y, z);

                // 현재 팀의 모든 멤버 수집 및 정렬
                List<Photon.Realtime.PhotonPlayer> teamMembers = PhotonNetwork.PlayerList.Where(p => PhotonServerManager.Instance.GetPlayerTeam(p) == myTeam).ToList();
                Debug.Log($"teammembers are {teamMembers[0].ActorNumber}, {teamMembers[1].ActorNumber}, {teamMembers[2].ActorNumber}");
                teamMembers = teamMembers.OrderBy(p => p.ActorNumber).ToList(); // StringComparer.Ordinal을 사용하여 안정적인 정렬 보장

                int myIndex = teamMembers.IndexOf(PhotonNetwork.LocalPlayer);
                Debug.Log($"my index is {myIndex}");

                // 각각 위치에 스폰하기
                if (myIndex >= 0 && myIndex < _spawnOffset.Length)
                {
                    Vector3 finalPos = basePos + _spawnOffset[myIndex];
                    GameObject player = PhotonNetwork.Instantiate("Player", finalPos, Quaternion.identity);
                    Debug.Log($"[Spawn] Player {PhotonNetwork.LocalPlayer.UserId} at {finalPos} (Team {myTeam}, Index {myIndex})");
                }
                else
                {
                    Debug.LogError($"[Spawn Error] Invalid spawn offset index {myIndex} (Team size: {teamMembers.Count})");
                }
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
}