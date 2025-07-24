using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;
using UnityEngine.InputSystem;

public class RoomManager : PunSingleton<RoomManager>
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField]private Vector3[] _spawnOffset; 

    private void Start()
    {
        // 먼저 캐릭터를 고름
        // choosecharacter 등등
        // 고른 후 : 
        // 팀별로 스폰 포인트를 할당
        if (PhotonNetwork.IsMasterClient)
        {
            AssignSpawnPoints();
        }
        Debug.Log($"[RoomManager] Start - PhotonNetwork.IsMasterClient: {PhotonNetwork.IsMasterClient}");
        // 스폰 포인트별로 팀 스폰
        // 자기 고른 캐릭터를 스폰
        StartCoroutine(WaitForSpawnDataAndSpawn());
    }
    
    // 팀을 찾아옴
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

    // 각 팀에 스폰포인트 할당
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

    // 자신 팀에 맞는 스폰포인트를 찾음
    private bool TryGetSpawnPoint(string teamKey, out Vector3 spawnPoint)
    {
        spawnPoint = Vector3.zero;

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(teamKey, out object spawnData))
        {
            string[] split = spawnData.ToString().Split(',');
            if (split.Length == 3 &&
                float.TryParse(split[0], out float x) &&
                float.TryParse(split[1], out float y) &&
                float.TryParse(split[2], out float z))
            {
                spawnPoint = new Vector3(x, y, z);
                return true;
            }
        }

        return false;
    }

    // 자기가 팀에서 몇번째 플레이어인지
    private int TryGetIndexOfTeam(string myTeam)
    {
        var teamMembers = PhotonNetwork.PlayerList
            .Where(p => PhotonServerManager.Instance.GetPlayerTeam(p) == myTeam)
            .OrderBy(p => p.ActorNumber)
            .ToList();

        Debug.Log($"Team members ({myTeam}): {string.Join(", ", teamMembers.Select(p => p.ActorNumber))}");

        int myIndex = teamMembers.IndexOf(PhotonNetwork.LocalPlayer);
        Debug.Log($"My index in team {myTeam}: {myIndex}");

        return myIndex;
    }

    private IEnumerator WaitForSpawnDataAndSpawn()
    {
        Debug.LogWarning("1");
        string myTeam = PhotonServerManager.Instance.GetPlayerTeam(PhotonNetwork.LocalPlayer);
        string key = $"spawn_{myTeam}";

        // 최대 3초 대기
        float timeout = 3f;
        while (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key) && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        if (!TryGetSpawnPoint(key, out Vector3 basePos))
        {
            Debug.LogError($"[Spawn] Invalid or missing spawn data for team {myTeam}");
            yield break;
        }

        int myIndex = TryGetIndexOfTeam(myTeam);
        if (myIndex < 0 || myIndex >= _spawnOffset.Length)
        {
            Debug.LogError($"[Spawn Error] Invalid spawn offset index {myIndex}");
            yield break;
        }

        Vector3 finalPos = basePos + _spawnOffset[myIndex];

        // PlayerType CustomProperty를 받아오기
        ECharacterName character = ECharacterName.Fulfuns;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("character", out object charObj))
        {
            character = (ECharacterName)(int)charObj;
        }
        // 해당 캐릭터 프리팹 Resources에서 로드 (예: Resources/Players/KnightPlayer.prefab)
        string path = $"Players/{character}Player";
        GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab == null)
        {
            Debug.LogWarning($"[Spawn] 캐릭터 프리팹을 찾을 수 없습니다: {path} → 기본 Player로 대체");
            prefab = Resources.Load<GameObject>("Player");
        }

        GameObject player = PhotonNetwork.Instantiate(path, finalPos, Quaternion.identity);
        Debug.Log($"[Spawn] {character}Player 스폰 완료 at {finalPos} (Team {myTeam}, Index {myIndex})");
        // Debug.Log($"[Spawn] Player {PhotonNetwork.LocalPlayer.UserId} at {finalPos} (Team {myTeam}, Index {myIndex})");
    }
}