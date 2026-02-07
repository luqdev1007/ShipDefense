using UnityEngine;
using DG.Tweening;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Ballista
{
    public class BallistaView : MonoBehaviour
    {
        [SerializeField] private BallistaController _controller;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _visualRoot;

        [Header("Recoil Settings")]
        [SerializeField] private float _recoilDistance = 0.8f;
        [SerializeField] private float _jumpHeight = 0.4f; // Высота подскока
        [SerializeField] private float _recoilBackDuration = 0.07f; // Чуть увеличили, чтобы не было чересчур резко
        [SerializeField] private float _recoilReturnDuration = 0.6f; // Увеличили для плавности

        [Header("Jerkiness Control")]
        [SerializeField] private Ease _returnEase = Ease.OutBack; // OutBack дает приятное мягкое возвращение

        private const string ParamName = "ChargeProgress";
        private Vector3 _initialLocalPos;

        private void Awake()
        {
            _initialLocalPos = _visualRoot.localPosition;
        }

        private void OnEnable() => _controller.OnFired += PlayRelease;
        private void OnDisable() => _controller.OnFired -= PlayRelease;

        private void Update()
        {
            if (_controller.IsCharging)
            {
                _animator.SetFloat(ParamName, _controller.ChargeProgress);
            }
        }

        public void PlayRelease(float progress)
        {
            _visualRoot.DOKill();

            // Рассчитываем целевую точку: назад по Z и вверх по Y
            Vector3 recoilTarget = new Vector3(
                _initialLocalPos.x,
                _initialLocalPos.y + (_jumpHeight * progress),
                _initialLocalPos.z - (_recoilDistance * progress)
            );

            // 1. Мгновенный мощный откат назад и вверх
            _visualRoot.DOLocalMove(recoilTarget, _recoilBackDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    // 2. Плавное возвращение в исходную точку
                    // Используем OutBack, чтобы в конце была легкая амортизация
                    _visualRoot.DOLocalMove(_initialLocalPos, _recoilReturnDuration)
                        .SetEase(_returnEase);
                });

            // 3. Дополнительно: легкое покачивание (вращение) для веса
            // Баллиста как бы "задирает нос" при выстреле
            _visualRoot.DOPunchRotation(new Vector3(-10f * progress, 0, 0), _recoilReturnDuration, 2, 0.5f);

            // Анимация затухания тетивы в аниматоре
            DOTween.To(() => progress, x => _animator.SetFloat(ParamName, x), 0f, 0.25f)
                .SetEase(Ease.OutCubic);
        }
    }
}