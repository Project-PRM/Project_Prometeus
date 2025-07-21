using DG.Tweening;
using UnityEngine;

public class UI_PopUp : MonoBehaviour
{
    [Header("Tween 애니메이션 시간")]
    [SerializeField] 
    protected float _showDuration = 0.3f;
    protected float _hideDuration = 0.2f;

    private RectTransform _rect;
    private Vector2 _initialPos;
    private Vector2 _offscreenPos;

    private Tween _currentTween;

    protected virtual void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _initialPos = _rect.anchoredPosition;

        float width = _rect.rect.width;
        _offscreenPos = new Vector2(-width, _initialPos.y);

        _rect.anchoredPosition = _initialPos;
    }

    public virtual void Show()
    {
        if (_currentTween != null && _currentTween.IsActive())
            _currentTween.Kill(); // 이전 트윈 제거

        gameObject.SetActive(true);
        _rect.anchoredPosition = _offscreenPos;

        _currentTween = _rect.DOAnchorPos(_initialPos, _showDuration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true); // 타임스케일 무시 여부 필요시
    }

    public virtual void Hide()
    {
        if (_currentTween != null && _currentTween.IsActive())
            _currentTween.Kill(); // 이전 트윈 제거

        _currentTween = _rect.DOAnchorPos(_offscreenPos, _hideDuration)
            .SetEase(Ease.InCubic)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }
}