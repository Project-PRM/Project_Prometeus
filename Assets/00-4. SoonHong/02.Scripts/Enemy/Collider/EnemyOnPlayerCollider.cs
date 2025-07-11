using UnityEngine;

public class EnemyOnPlayerCollider : MonoBehaviour
{
    private EnemyBTBase _enemyBTBase;
    private void Awake()
    {
        _enemyBTBase = GetComponentInParent<EnemyBTBase>();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        _enemyBTBase.TargetSetup(other.gameObject);
    }
}
