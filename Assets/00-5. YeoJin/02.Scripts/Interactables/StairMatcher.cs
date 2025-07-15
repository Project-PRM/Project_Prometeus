using System.Collections.Generic;
using UnityEngine;

public class StairMatcher : MonoBehaviour
{
    [SerializeField] private GameObject[] _stairs;
    [SerializeField] private int _matchCount = 3;


    private void Start()
    {
        if (_stairs.Length < _matchCount)
        {
            Debug.LogError($"Need at least {_matchCount} stairs");
            return;
        }

        // stairs, points 복사 후 셔플
        GameObject[] shuffledStairs = (GameObject[])_stairs.Clone();

        Shuffle(shuffledStairs);

        // 앞에서부터 매칭
        for (int i = 0; i < _matchCount; i++)
        {
            GameObject stair = shuffledStairs[i];
            // 계단 키기
        }
    }

    private void Shuffle(GameObject[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}
