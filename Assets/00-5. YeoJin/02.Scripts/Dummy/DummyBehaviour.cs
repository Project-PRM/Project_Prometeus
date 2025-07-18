using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Animator))]
public class DummyBehaviour : MonoBehaviour, IDamageable
{
    [SerializeField] private ECharacterName _characterName;

    private DummyBase _character;
    public DummyBase GetDummyBase() => _character;

    [Header("# Components")]
    public Animator Animator { get; private set; }
    public PhotonView PhotonView { get; private set; }

    [Header("Dummy Attributes")]
    [SerializeField] private string _team;
    [SerializeField] private string _nickname;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        PhotonView = GetComponent<PhotonView>();
    }

    private async void Start()
    {
        if (!CharacterManager.Instance.IsInitialized)
        {
            await CharacterManager.Instance.Init();
        }
        var skills = await CharacterManager.Instance.GetCharacterMetaDataAsync(_characterName);

        _character = new DummyBase(
            this,
            _characterName.ToString(),
            CharacterManager.Instance.CharacterStats
        );
    }

    public void SetDummy(string team, string nickname)
    {
        _team = team;
        _nickname = nickname;
    }

    public void TakeDamage(float Damage)
    {
        _character.TakeDamage(Damage);
    }

    public void Heal(float Amount)
    {
        _character.Heal(Amount);
    }
}