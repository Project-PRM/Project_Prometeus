using System;
using UnityEngine;

public enum Selectorder
{
    First,
    Second,
    Third,
}

public class SelectCharacter : Singleton<SelectCharacter>
{
    [SerializeField] private float _selectTime = 5f;
    [SerializeField] private GameObject _block;
    [SerializeField] private Selectorder _myOrder;

    public Selectorder CurrentOrder { get; private set; } = Selectorder.First;
    public float SelectTimer { get; private set; } = 0f;

    private void Start()
    {
        SelectTimer = _selectTime;
        Debug.Log($"현재순서 : {CurrentOrder}");
        OnOffBlock();
    }

    private void Update()
    {
        if (SelectTimer > 0)
        {
            SelectTimer -= Time.deltaTime;
        }
        else
        {
            SwitchOrder();
        }
    }
    private void SwitchOrder()
    {
        if (CurrentOrder < Selectorder.Third)
        {
            SelectTimer = _selectTime;
            CurrentOrder++;
            Debug.Log($"현재순서 : {CurrentOrder}");
        }
        else
        {
            Debug.Log("게임시작");
        }
        OnOffBlock();
    }
    private void OnOffBlock()
    {
        _block.SetActive(CurrentOrder != _myOrder);
    }
    public void SetOrder(Selectorder order)
    {
        _myOrder = order;
    }
}
