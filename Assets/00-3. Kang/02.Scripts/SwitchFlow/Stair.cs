using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class Stair : MonoBehaviour
{
    [SerializeField] private Transform _movePoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerInput>().enabled = false;
            Fade.Instance.OnFadeInComplete += () => { UpFlow(other.transform); }; ;
            Fade.Instance.FadeInAndOut();
        }
    }
    private void UpFlow(Transform player)
    {
        player.transform.position = _movePoint.position;
        player.gameObject.GetComponent<PlayerInput>().enabled = true;
        Fade.Instance.OnFadeInComplete -= () => { UpFlow(player); }; ;
    }
}
