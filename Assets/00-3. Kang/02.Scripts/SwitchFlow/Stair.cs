using UnityEngine;
using System;

public class Stair : MonoBehaviour
{
    [SerializeField] private Transform _movePoint;
    private Transform _moveTarget;

    private void Start()
    {
        Fade.Instance.OnFadeInComplete += () =>
        { UpFlow(_moveTarget, _movePoint); };
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _moveTarget = other.transform;
            Fade.Instance.FadeInAndOut();
        }
    }
    private void UpFlow(Transform player, Transform movePoint)
    {
        if(player == null) throw new ArgumentNullException("이동시킬 대상 없듬");
        if(movePoint == null) throw new ArgumentNullException("이동시킬 위치 없듬");

        player.transform.position = movePoint.position;
    }
}
