using TMPro;
using UnityEngine;

public class UI_NicknameIngame : MonoBehaviour
{
    public Transform TargetTransform { get; set; }
    public RectTransform RectTransform => (RectTransform)transform;

    [SerializeField] private TextMeshPro _nameText;

    public void SetName(string name)
    {
        _nameText.text = name;
    }

    private void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                     Camera.main.transform.rotation * Vector3.up);
    }
}
