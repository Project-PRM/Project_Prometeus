using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class EnemyDataManager : Singleton<EnemyDataManager>
{
    private EnemyDataRepository _enemyDataRepository;

    public Dictionary<string, EnemyData> Enemies => _enemyDataRepository.Enemies;

    protected async override void Awake()
    {
        _enemyDataRepository = new EnemyDataRepository();
        await _enemyDataRepository.InitializeAsync();
    }

    public bool TryGetEnemyData(string enemyName, out EnemyData enemyData)
    {
        if (_enemyDataRepository.Enemies.TryGetValue(enemyName, out var original))
        {
            enemyData = new EnemyData(original); // 깊은 복사된 객체 반환
            return true;
        }
        enemyData = null;
        return false;
    }
}
