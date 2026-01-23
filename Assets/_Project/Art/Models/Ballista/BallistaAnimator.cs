using UnityEngine;
using DG.Tweening;

public class BallistaAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _chargeDuration = 2f;
    [SerializeField] private float _reverseSpeedMultiplier = 2f;
    private Tween _chargeTween;
    private float _currentProgress = 0f;
    private bool _isReversing = false;

    private const string ParamName = "ChargeProgress";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !_isReversing)
        {
            StartCharge();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            StartReverse();
        }
    }

    private void StartCharge()
    {
        _chargeTween?.Kill();

        float remainingTime = _chargeDuration * (1f - _currentProgress);

        _chargeTween = DOTween.To(() => _currentProgress, x => _currentProgress = x, 1f, remainingTime)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                _animator.SetFloat(ParamName, _currentProgress);
            });
    }

    private void StartReverse()
    {
        _chargeTween?.Kill();
        _isReversing = true;

        float reverseDuration = (Mathf.Sqrt(_chargeDuration) / _reverseSpeedMultiplier) * _currentProgress;

        _chargeTween = DOTween.To(() => _currentProgress, x => _currentProgress = x, 0f, reverseDuration)
            .SetEase(Ease.OutBack)
            .OnUpdate(() =>
            {
                _animator.SetFloat(ParamName, _currentProgress);
            })
            .OnComplete(() =>
            {
                _isReversing = false;
            });
    }
}
