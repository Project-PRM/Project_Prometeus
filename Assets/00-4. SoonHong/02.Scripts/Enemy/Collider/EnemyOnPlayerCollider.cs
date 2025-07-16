using System;
using Unity.VisualScripting;
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
        if(other.gameObject.CompareTag("Player"))
        {
            _enemyBTBase.TargetSetup(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            _enemyBTBase.TargetSetup(null);
        }
    }
}
