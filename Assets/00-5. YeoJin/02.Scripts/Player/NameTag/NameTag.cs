using TMPro;
using UnityEngine;

public class NameTag : MonoBehaviour
{
    public Transform TargetTransform { get; set; }
    public RectTransform RectTransform => (RectTransform)transform;

    [SerializeField] private TextMeshProUGUI nameText;

    public void SetName(string name)
    {
        nameText.text = name;
    }
}
