using Photon.Pun;
using UnityEngine;

public abstract class PlayerActivity : MonoBehaviour
{
    protected Player _owner { get; private set; }
    protected PhotonView _photonView { get; private set; }

    protected virtual void Start()
    {
        _owner = GetComponent<Player>();

        _photonView = _owner.PhotonView;
    }
}
