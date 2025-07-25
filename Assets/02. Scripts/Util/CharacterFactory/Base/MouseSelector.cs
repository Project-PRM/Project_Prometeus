using UnityEngine;

public class MouseSelector : Singleton<MouseSelector>
{
    [SerializeField] private LayerMask characterLayer;
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private LayerMask selectableLayerMask;

    [SerializeField] private ISelectable _currentSelected;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, selectableLayerMask))
        {
            var selectable = hit.collider.GetComponentInParent<ISelectable>();

            if (selectable != null)
            {
                if (_currentSelected != selectable)
                {
                    _currentSelected?.SetHighlight(false);


                    _currentSelected = selectable;
                    _currentSelected.SetHighlight(true);
                }
            }
        }
        else
        {
            _currentSelected?.SetHighlight(false);
            _currentSelected = null;
        }
    }

    /// <summary>
    /// 현재 하이라이트된 게임오브젝트 반환
    /// </summary>
    public GameObject GetCurrentSelectedObject()
    {
        return (_currentSelected as MonoBehaviour)?.gameObject;
    }

    /// <summary>
    /// 현재 선택된 ISelectable 반환
    /// </summary>
    public ISelectable GetCurrentSelectable()
    {
        return _currentSelected;
    }

    public Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        return ground.Raycast(ray, out float enter) ? ray.GetPoint(enter) : ray.GetPoint(10f);
    }

    public CharacterBase GetCharacterUnderMouse()
    {
        Vector3 pos = GetMouseWorldPosition();
        Collider[] hits = Physics.OverlapSphere(pos, 0.5f, characterLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<CharacterBehaviour>(out var behaviour))
            {
                return behaviour.GetCharacterBase();
            }
        }

        return null;
    }

    public IPickupable GetPickupableUnderMouse()
    {
        Vector3 pos = GetMouseWorldPosition();
        Collider[] hits = Physics.OverlapSphere(pos, 0.5f, itemLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IPickupable>(out var pickupable))
            {
                return pickupable;
            }
        }

        return null;
    }
}
