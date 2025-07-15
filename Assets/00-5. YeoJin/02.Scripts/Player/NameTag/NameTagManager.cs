using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NameTagManager : Singleton<NameTagManager>
{
    [SerializeField] private GameObject nameTagPrefab;
    [SerializeField] private RectTransform nameTagRoot;

    private Dictionary<Photon.Realtime.PhotonPlayer, NameTag> _nameTags = new();

    private void LateUpdate()
    {
        foreach (var entry in _nameTags)
        {
            if (entry.Value.TargetTransform == null) continue;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(entry.Value.TargetTransform.position + Vector3.up * 4f);
            entry.Value.RectTransform.position = screenPos;
        }
    }

    public void CreateNameTag(Photon.Realtime.PhotonPlayer player, Transform targetTransform)
    {
        GameObject obj = Instantiate(nameTagPrefab, nameTagRoot);
        var nameTag = obj.GetComponent<NameTag>();

        // 이름 표시
        nameTag.SetName(player.NickName);

        nameTag.TargetTransform = targetTransform;
        _nameTags[player] = nameTag;
        
    }
}
