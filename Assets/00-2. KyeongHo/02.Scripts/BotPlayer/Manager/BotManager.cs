using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using Photon.Realtime; 
using UnityEngine;

public class BotManager : PunSingleton<BotManager>
{
    private const int PLAYERS_PER_TEAM = 3;
    private const string TEAM_PROPERTY_KEY = "team";

    // 모든 플레이어가 스폰된 후 MasterClient가 호출
    public void TrySpawnBots()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        Debug.Log("🤖 MasterClient가 부족한 팀 인원을 확인하고 Bot 스폰을 시작합니다.");
        
        // 1. 현재 씬의 모든 플레이어를 팀별로 그룹화합니다.
        var teams = new Dictionary<string, List<PhotonPlayer>>();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(TEAM_PROPERTY_KEY, out object teamNameObj))
            {
                string teamName = teamNameObj.ToString();
                if (!teams.ContainsKey(teamName))
                {
                    teams[teamName] = new List<PhotonPlayer>();
                }
                teams[teamName].Add(player);
            }
        }
        
        // 2. 각 팀을 순회하며 인원이 부족하면 Bot을 스폰합니다.
        foreach (var team in teams)
        {
            string teamName = team.Key;
            List<PhotonPlayer> teamMembers = team.Value;
            
            int botsNeeded = PLAYERS_PER_TEAM - teamMembers.Count;

            if (botsNeeded > 0)
            {
                Debug.Log($"팀 '{teamName}'에 {botsNeeded}명의 Bot이 필요합니다. 스폰을 시작합니다.");

                // Bot이 따라갈 타겟 플레이어를 찾습니다. (해당 팀의 아무나)
                PhotonPlayer targetPlayer = teamMembers.FirstOrDefault();
                if (targetPlayer == null) continue; // 혹시 모를 예외 처리

                for (int i = 0; i < botsNeeded; i++)
                {
                    // Bot 스폰 위치 (타겟 플레이어 근처 등)
                    Vector3 spawnPosition = GetRandomSpawnPointNear(targetPlayer);

                    // Bot에게 타겟 플레이어의 ActorNumber를 데이터로 넘겨줍니다.
                    object[] instantiationData = new object[] { targetPlayer.ActorNumber };

                    // "BotPrefab"이라는 이름의 리소스를 포톤으로 스폰합니다.
                    PhotonNetwork.Instantiate("Bot_DummyPlayer", spawnPosition, Quaternion.identity, 0, instantiationData);
                }
            }
        }
    }

    // 예시: 타겟 플레이어 근처에 랜덤한 위치를 반환하는 함수
    private Vector3 GetRandomSpawnPointNear(PhotonPlayer targetPlayer)
    {
        // 실제 게임에서는 맵의 스폰 포인트를 사용해야 합니다.
        // 여기서는 간단하게 플레이어의 캐릭터 위치를 찾아 그 주변에 스폰합니다.
        PhotonView targetView = PhotonView.Find(targetPlayer.ActorNumber); // 이 방법은 비효율적일 수 있습니다.
        // 더 좋은 방법은 플레이어 캐릭터가 스폰될 때 자신의 PhotonView와 ActorNumber를 딕셔너리에 등록하는 것입니다.
        if(targetView != null)
        {
             return targetView.transform.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        }
        return Vector3.zero; // 타겟을 못찾으면 원점
    }
}