using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;

[FirestoreData]
public class ItemData
{
    [FirestoreProperty] public string Name { get; set; }
    [FirestoreProperty] public string Description { get; set; }
    [FirestoreProperty] public EItemRarity Rarity { get; set; }
    [FirestoreProperty] public EItemType ItemType { get; set; }
    [FirestoreProperty] public Dictionary<string, float> AdditiveStats { get; set; } = new();
    [FirestoreProperty] public Dictionary<string, float> MultiplierStats { get; set; } = new();
    public Sprite IconSprite { get; private set; }

    public ItemData() { }

    public Color GetRarityColor()
    {
        return Rarity switch
        {
            EItemRarity.Common => new Color32(200, 200, 200, 255),     // 회색
            EItemRarity.Rare => new Color32(50, 150, 255, 255),        // 파랑
            EItemRarity.Epic => new Color32(180, 70, 255, 255),        // 보라
            EItemRarity.Legendary => new Color32(255, 160, 20, 255),   // 주황
            _ => Color.white
        };
    }
}