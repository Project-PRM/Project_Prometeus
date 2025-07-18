using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;

public class LobbyCharacterManager : PunSingleton<LobbyCharacterManager>
{
    [Header("캐릭터 스폰 위치")]
    public Transform centerPos; // 내 캐릭터 위치
    public Transform leftPos;   // 팀원1 위치
    public Transform rightPos;  // 팀원2 위치

    // 생성된 캐릭터들을 관리하기 위한 딕셔너리
    private Dictionary<int, GameObject> spawnedCharacters = new Dictionary<int, GameObject>();

    /// <summary>
    /// 현재 네트워크 상태에 맞춰 캐릭터 디스플레이를 업데이트하는 메인 함수
    /// </summary>
    public void UpdateCharacterDisplay()
    {
        ClearAllCharacters(); // 일단 모든 캐릭터 삭제
        string myTeam = PhotonServerManager.Instance.GetPlayerTeam(PhotonNetwork.LocalPlayer);
        
        if (!PhotonNetwork.InRoom || // 룸에 들어가 있지 않다면 내 캐릭터만 표시  
            string.IsNullOrEmpty(myTeam) ||  
            myTeam == "None") // 룸에 있지만 아직 팀이 없다면 내 캐릭터만 표시
        {
            SpawnCharacter(PhotonNetwork.LocalPlayer, centerPos);
            return;
        }
        
        // 팀이 정해졌다면, 우리 팀원들을 모두 찾아서 표시
        var myTeammates = PhotonNetwork.PlayerList
            .Where(p => PhotonServerManager.Instance.GetPlayerTeam(p) == myTeam)
            .ToList();

        // 나를 중앙에 먼저 배치
        SpawnCharacter(PhotonNetwork.LocalPlayer, centerPos);

        // 나를 제외한 팀원들을 배치
        var others = myTeammates.Where(p => !p.IsLocal).ToList();
        if (others.Count > 0) SpawnCharacter(others[0], leftPos);
        if (others.Count > 1) SpawnCharacter(others[1], rightPos);
    }

    /// <summary>
    /// 지정된 위치에 플레이어의 캐릭터를 생성
    /// 잠시 빼둠 - 테스트용
    /// </summary>
    /*private void SpawnCharacter(PhotonPlayer player, Transform spawnPos)
    {
        if (player == null) return;
        
        // TODO: AccountManager 등에서 플레이어의 캐릭터 프리팹 정보를 가져와야 함
        // 예시: GameObject characterPrefab = GetPrefabFromPlayer(player);
        GameObject characterPrefab = Resources.Load<GameObject>("LobbyPlayer"); // 임시 프리팹

        GameObject character = Instantiate(characterPrefab, spawnPos.position, spawnPos.rotation);
        spawnedCharacters[player.ActorNumber] = character;
        
        // TODO: 캐릭터 외형 커스터마이징 적용
    }*/ 
    // 테스트용
    private void SpawnCharacter(PhotonPlayer player, Transform spawnPos)
    {
        ECharacterName character = ECharacterName.Dummy; // 기본값

        if (player.CustomProperties.TryGetValue("character", out object charObj))
        {
            character = (ECharacterName)(int)charObj;
        }

        // 해당 캐릭터 프리팹 로드 (Resources/LobbyCharacters/ 아래에 있어야 함)
        string path = $"LobbyPlayers/{character}Player";
        GameObject characterPrefab = Resources.Load<GameObject>(path);

        if (characterPrefab == null)
        {
            Debug.LogWarning($"[LobbyCharacterManager] 캐릭터 프리팹 로드 실패: {path}");
            characterPrefab = Resources.Load<GameObject>("Player");
        }

        GameObject characterInstance = Instantiate(characterPrefab, spawnPos.position, spawnPos.rotation);
        spawnedCharacters[player.ActorNumber] = characterInstance;

        Debug.Log($"[LobbyCharacterManager] {player.NickName} → {character} 생성");
    }

    /// <summary>
    /// 현재 생성된 모든 캐릭터를 지움
    /// </summary>
    private void ClearAllCharacters()
    {
        foreach (var character in spawnedCharacters.Values)
        {
            Destroy(character);
        }
        spawnedCharacters.Clear();
    }
    
    // Pun Callbacks
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    } 
    protected override void OnDestroy()
    {
        ClearAllCharacters();
    }

    public override void OnPlayerPropertiesUpdate(PhotonPlayer targetPlayer, Hashtable changedProps)
    {
        // 테스트용 추가
        if (changedProps.ContainsKey("character"))
        {
            UpdateCharacterDisplay();
            return;
        }
        // "Team" 프로퍼티가 변경되었을 때만 캐릭터 디스플레이를 업데이트
        if (changedProps.ContainsKey("team")) // "Team"은 실제 사용하는 프로퍼티 키로 변경해야 합니다.
        {
            UpdateCharacterDisplay();
        }
    }
    public override void OnJoinedLobby()
    {
        UpdateCharacterDisplay();
        
    }
    public override void OnJoinedRoom()
    {
        UpdateCharacterDisplay();
    }
    public override void OnPlayerEnteredRoom(PhotonPlayer newPlayer)
    {
        UpdateCharacterDisplay();
    }

    public override void OnPlayerLeftRoom(PhotonPlayer otherPlayer)
    {
        // 약간의 지연을 주어 안정성을 높일 수 있습니다.
        Invoke(nameof(UpdateCharacterDisplay), 0.1f);
    }
    public override void OnLeftRoom()
    {
        UpdateCharacterDisplay();
    }

}