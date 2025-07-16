using UnityEngine;

public class UI_HealthBar : MonoBehaviour
{
    void Start()
    {
        
    }

    private void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}
