using UnityEngine;
using DG.Tweening;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Ballista
{
    // нужно будет добавить эффект появления стрелы,чтобы смазать переход между пропсом и реальным прожектайлом

    public class BallistaView : MonoBehaviour
    {
        [SerializeField] private BallistaController _controller;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _visualRoot;

        [SerializeField] private float _recoilDistance = 0.8f;
        [SerializeField] private float _recoilBackDuration = 0.05f;
        [SerializeField] private float _recoilReturnDuration = 0.4f;
        [SerializeField] private Ease _returnEase = Ease.OutElastic;

        private const string ParamName = "ChargeProgress";
        private Vector3 _initialLocalPos;

        private void Awake()
        {
            _initialLocalPos = _visualRoot.localPosition;
        }

        private void OnEnable()
        {
            _controller.OnFired += PlayRelease;
        }

        private void OnDisable()
        {
            _controller.OnFired -= PlayRelease;
        }

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

            _visualRoot.DOLocalMoveZ(_initialLocalPos.z - (_recoilDistance * progress), _recoilBackDuration)
                .SetEase(Ease.OutExpo)
                .OnComplete(() =>
                {
                    _visualRoot.DOLocalMoveZ(_initialLocalPos.z, _recoilReturnDuration)
                        .SetEase(_returnEase);
                });

            DOTween.To(() => progress, x => _animator.SetFloat(ParamName, x), 0f, 0.2f)
                .SetEase(Ease.InExpo);
        }
    }
}