using Firebase.Firestore;
using System;
using System.Collections.Generic;
using UnityEngine;

[FirestoreData]
public class ItemData
{
    [FirestoreProperty] public string Name { get; set; }
    [FirestoreProperty] public string Description { get; set; }
    [FirestoreProperty] public string IconName { get; set; }
    [FirestoreProperty] public EItemRarity Rarity { get; set; }
    [FirestoreProperty] public EItemType ItemType { get; set; }
    [FirestoreProperty] public Dictionary<string, float> AdditiveStats { get; set; } = new();
    [FirestoreProperty] public Dictionary<string, float> MultiplierStats { get; set; } = new();
    private Sprite _iconSprite;
    public Sprite IconSprite
    {
        get
        {
            if (_iconSprite == null && !string.IsNullOrEmpty(IconName))
            {
                _iconSprite = Resources.Load<Sprite>($"Icons/{IconName}");
            }
            return _iconSprite;
        }
    }

    public ItemData() { }

    public ItemData(ItemData other)
    {
        Name = other.Name;
        Description = other.Description;
        IconName = other.IconName;
        Rarity = other.Rarity;
        ItemType = other.ItemType;

        AdditiveStats = new Dictionary<string, float>(other.AdditiveStats);
        MultiplierStats = new Dictionary<string, float>(other.MultiplierStats);
    }

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

    public StatModifier ToStatModifier()
    {
        var mod = new StatModifier();

        foreach (var kvp in AdditiveStats)
        {
            if (Enum.TryParse<EStatType>(kvp.Key, out var statType))
            {
                mod.Add(statType, kvp.Value);
            }
        }

        foreach (var kvp in MultiplierStats)
        {
            if (Enum.TryParse<EStatType>(kvp.Key, out var statType))
            {
                mod.Multiply(statType, kvp.Value);
            }
        }

        return mod;
    }
}