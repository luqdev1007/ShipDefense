using UnityEngine;
using DG.Tweening;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Ballista
{
    public class BallistaView : MonoBehaviour
    {
        [SerializeField] private BallistaController _controller;
        [SerializeField] private Animator _animator;

        [Header("Recoil Settings")]
        [SerializeField] private float _recoilDistance = 0.8f;
        [SerializeField] private float _jumpHeight = 0.4f;
        [SerializeField] private float _maxPitch = 12f;
        [Range(0, 1)][SerializeField] private float _minRecoilPower = 0.35f;

        [Header("Timing Distribution (%)")]
        [Range(0, 1)][SerializeField] private float _backWeight = 0.15f;
        [Range(0, 1)][SerializeField] private float _returnWeight = 0.65f;

        [Header("Jerkiness Control")]
        [SerializeField] private Ease _returnEase = Ease.OutBack;

        private const string ParamName = "ChargeProgress";
        private Vector3 _currentPosOffset;
        private float _currentPitchOffset;

        private void OnEnable() => _controller.OnFired += PlayRelease;
        private void OnDisable() => _controller.OnFired -= PlayRelease;

        private void Update()
        {
            if (_controller.IsCharging) _animator.SetFloat(ParamName, _controller.ChargeProgress);
        }

        public void PlayRelease(float progress)
        {
            float totalTime = _controller.FireCycleDuration;
            float backTime = totalTime * _backWeight;
            float returnTime = totalTime * _returnWeight;

            DOTween.Kill(this);

            // Сила отдачи теперь не падает до нуля при быстром клике
            float power = Mathf.Lerp(_minRecoilPower, 1f, progress);

            Vector3 targetPosOffset = new Vector3(0, _jumpHeight * power, -_recoilDistance * power);
            float targetPitch = -_maxPitch * power;

            Sequence recoilSeq = DOTween.Sequence().SetId(this);

            // Фаза 1: Резкий удар назад/вверх
            recoilSeq.Append(DOTween.To(() => _currentPosOffset, x => _currentPosOffset = x, targetPosOffset, backTime).SetEase(Ease.OutExpo));
            recoilSeq.Join(DOTween.To(() => _currentPitchOffset, x => _currentPitchOffset = x, targetPitch, backTime).SetEase(Ease.OutExpo));

            // Фаза 2: Плавный возврат с амортизацией (OutBack)
            recoilSeq.Append(DOTween.To(() => _currentPosOffset, x => _currentPosOffset = x, Vector3.zero, returnTime).SetEase(_returnEase));
            recoilSeq.Join(DOTween.To(() => _currentPitchOffset, x => _currentPitchOffset = x, 0f, returnTime).SetEase(_returnEase));

            recoilSeq.OnUpdate(() => {
                _controller.SetRecoilOffsets(_currentPosOffset, _currentPitchOffset);
            });

            // Анимация тетивы
            DOTween.To(() => progress, x => _animator.SetFloat(ParamName, x), 0f, backTime).SetId(this).SetEase(Ease.OutQuad);
        }
    }
}