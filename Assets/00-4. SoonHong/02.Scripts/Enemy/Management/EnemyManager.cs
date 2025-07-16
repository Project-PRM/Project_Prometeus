using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class EnemyManager : PunSingleton<EnemyManager>
{
    [Header("씬에 배치된 Enemy 목록")]
    [SerializeField] private List<EnemyBase> _presetEnemies;

    private readonly List<EnemyBase> _allEnemies = new List<EnemyBase>();

    private void Awake()
    {
        // 수동 지정된 적들 등록
        foreach (var enemy in _presetEnemies)
        {
            if (enemy != null)
                Register(enemy);
        }
    }

    public void Register(EnemyBase enemy)
    {
        if (!_allEnemies.Contains(enemy))
        {
            _allEnemies.Add(enemy);
            Debug.Log($"[EnemyManager] 등록됨: {enemy.name}");
        }
    }

    public void Unregister(EnemyBase enemy)
    {
        if (_allEnemies.Contains(enemy))
        {
            _allEnemies.Remove(enemy);
            Debug.Log($"[EnemyManager] 해제됨: {enemy.name}");
        }
    }

    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var enemy in _allEnemies)
            {
                enemy.UpdateMasterFlag();
            }
        }
    }
}