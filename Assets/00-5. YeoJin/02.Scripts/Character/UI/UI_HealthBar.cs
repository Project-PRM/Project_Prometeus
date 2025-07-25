using UnityEngine;
using Photon.Pun;

public class UI_HealthBar : MonoBehaviour
{
    [SerializeField] private RectTransform Fill;

    private void Start()
    {
        Fill.localScale = new Vector3(5, 0.2f, 1);
    }

    public void SetValue(float value)
    {
        Vector3 scale = Fill.localScale;
        scale.x = value * 5f;
        Debug.Log(value);
        Fill.localScale = scale;
    }

    private void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}
