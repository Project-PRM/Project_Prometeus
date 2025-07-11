using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using Unity.Cinemachine;

public class Player : MonoBehaviour
{
    private Dictionary<Type, PlayerActivity> _cache = new();
    private PhotonView _photonView;
    public PhotonView PhotonView => _photonView;
    private Animator _animator;
    public Animator Animator => _animator;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();

        if (_photonView.IsMine)
        {
            SetupCamera();
        }
    }

    public T GetActivity<T>() where T : PlayerActivity
    {
        var type = typeof(T);

        if (_cache.TryGetValue(type, out var activity))
            return (T)activity;

        activity = GetComponent<T>() ?? GetComponentInChildren<T>();
        if (activity != null)
        {
            _cache[type] = activity;
            return (T)activity;
        }

        throw new Exception($"Activity {type.Name} not found on {gameObject.name}.");
    }

    private void SetupCamera()
    {
        CinemachineCamera cam = FindAnyObjectByType<CinemachineCamera>();
        if (cam != null)
        {
            cam.Follow = this.transform; // 예: 머리 위 Empty
            cam.LookAt = this.transform;
        }
    }
}
