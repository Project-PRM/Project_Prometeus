using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using Unity.Cinemachine;
using FOW;

public class Player : MonoBehaviour
{
    private Dictionary<Type, PlayerActivity> _cache = new();
    private PhotonView _photonView;
    public PhotonView PhotonView => _photonView;
    private Animator _animator;
    public Animator Animator => _animator;

    private CharacterBase _character;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();
        // 호출을 늦게 해야함
        //_character = GetComponent<CharacterBehaviour>().GetCharacterBase();

        if (_photonView.IsMine)
        {
            SetupCamera();
        }

        TurnOnRevealer();
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

    public void TakeDamage(float Damage)
    {
        // 대미지 받기 구현
    }

    public void TurnOnRevealer()
    {
        if (_photonView.IsMine)
        {
            var revealer = GetComponent<FogOfWarRevealer3D>();
            if (revealer != null) revealer.enabled = true;
            return;
        }

        var myTeam = PhotonServerManager.Instance.GetPlayerTeam(PhotonNetwork.LocalPlayer);
        var team = PhotonServerManager.Instance.GetPlayerTeam(_photonView.Owner);

        if(myTeam == team)
        {
            var revealer = GetComponent<FogOfWarRevealer3D>();
            if (revealer != null) revealer.enabled = true;
        }
    }
}
