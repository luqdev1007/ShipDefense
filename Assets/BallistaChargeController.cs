using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class BallistaChargeController : MonoBehaviour
{
    [Header("Views")]
    [SerializeField] private Transform currentView; // То, что видит игрок (активная баллиста)
    [SerializeField] private Transform startState;  // Префаб ShootStarted
    [SerializeField] private Transform maxState;    // Префаб ShootMaximized

    [Header("Settings")]
    [SerializeField] private float chargeDuration = 2f;
    [SerializeField] private Ease chargeEase = Ease.OutQuad;

    private Sequence _chargeSequence;

    // Метод для запуска зарядки
    public void StartChargeAttackAnimation()
    {
        // Если старая анимация еще идет — убиваем её
        _chargeSequence?.Kill();
        _chargeSequence = DOTween.Sequence();

        // Рекурсивно проходим по всем дочерним объектам
        AnimateHierarchy(currentView, maxState);
    }

    private void AnimateHierarchy(Transform current, Transform target)
    {
        // Анимируем текущий объект (Position, Rotation, Scale)
        _chargeSequence.Join(current.DOLocalMove(target.localPosition, chargeDuration).SetEase(chargeEase));
        _chargeSequence.Join(current.DOLocalRotateQuaternion(target.localRotation, chargeDuration).SetEase(chargeEase));
        _chargeSequence.Join(current.DOScale(target.localScale, chargeDuration).SetEase(chargeEase));

        // Идем вглубь по иерархии
        for (int i = 0; i < current.childCount; i++)
        {
            // Важно: структуры дочерних объектов в обоих префабах должны быть идентичны!
            if (i < target.childCount)
            {
                AnimateHierarchy(current.GetChild(i), target.GetChild(i));
            }
        }
    }

    // Метод для мгновенного сброса в начальное состояние (например, после выстрела)
    [ContextMenu("Reset Ballista")]
    public void ResetToStart()
    {
        _chargeSequence?.Kill();
        SetHierarchyState(currentView, startState);
        StartChargeAttackAnimation();
    }

    private void SetHierarchyState(Transform current, Transform target)
    {
        current.localPosition = target.localPosition;
        current.localRotation = target.localRotation;
        current.localScale = target.localScale;

        for (int i = 0; i < current.childCount; i++)
        {
            if (i < target.childCount)
            {
                SetHierarchyState(current.GetChild(i), target.GetChild(i));
            }
        }
    }
}