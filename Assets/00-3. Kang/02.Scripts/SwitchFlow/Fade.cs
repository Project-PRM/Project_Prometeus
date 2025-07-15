using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Fade : Singleton<Fade>
{
    private CanvasGroup _canvasGroup;

    [SerializeField] private float _fadeInDuration = 0.2f;
    [SerializeField] private float _fadeWaitDuration = 0.5f;
    [SerializeField] private float _fadeOutDuration = 1f;

    public event Action OnFadeInComplete;

    protected override void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    public void FadeInAndOut()
    {
        _canvasGroup.DOFade(1, _fadeInDuration).OnComplete(() =>
        {
            OnFadeInComplete?.Invoke();
            StartCoroutine(FadeOutCoroutine());
        });
    }
    private IEnumerator FadeOutCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _canvasGroup.DOFade(0, _fadeOutDuration);
    }
}
