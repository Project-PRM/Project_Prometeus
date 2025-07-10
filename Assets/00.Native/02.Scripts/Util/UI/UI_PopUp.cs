using DG.Tweening;
using UnityEngine;

/// <summary>
/// 팝업형 UI의 공통 트위닝 기능을 제공하는 베이스 클래스입니다.
/// </summary>
public abstract class UI_PopUp : MonoBehaviour
{
    [Header("Tween 애니메이션 시간")]
    [SerializeField] 
    protected float _showDuration = 0.3f;
    protected float _hideDuration = 0.2f;

    private RectTransform _rect;
    private Vector2 _initialPos;
    private Vector2 _offscreenPos;

    protected virtual void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _initialPos = _rect.anchoredPosition;

        float width = _rect.rect.width;
        _offscreenPos = new Vector2(-width, _initialPos.y);

        //gameObject.SetActive(false);
        _rect.anchoredPosition = _initialPos;
    }

    /// <summary>
    /// 팝업을 열 때 트위닝으로 보여줍니다.
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
        _rect.anchoredPosition = _offscreenPos;
        _rect.DOAnchorPos(_initialPos, _showDuration).SetEase(Ease.OutCubic);
    }

    /// <summary>
    /// 팝업을 닫을 때 트위닝으로 숨깁니다.
    /// </summary>
    public virtual void Hide()
    {
        _rect.DOAnchorPos(_offscreenPos, _hideDuration)
             .SetEase(Ease.InCubic)
             .OnComplete(() => gameObject.SetActive(false));
    }
}
