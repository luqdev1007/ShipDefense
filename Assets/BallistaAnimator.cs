using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BallistaAnimator : MonoBehaviour
{
    [SerializeField] private Transform _backMechRoll;
    [SerializeField] private Transform[] _ropeParts;
    [SerializeField] private Transform _rotateBody;

    [SerializeField] private float _maxAngleX = -70;
    [SerializeField] private float _totalDuration = 2;

    private Vector3 _finalRotateBackMechRoll;
    private Vector3 _finalRotateBody;

    private const float FullAngle = 360;
    private const int Direction = -1;
    
    public void AnimateRopeDisappearance()
    {
        Sequence ropeSequence = DOTween.Sequence().
            OnKill(() => _ropeParts.ToList().ForEach(i => i.gameObject.transform.localScale = Vector3.one * 0.1f));

        int count = _ropeParts.Length;
        float individualShrinkDuration = 0.3f;

        for (int i = count - 1; i >= 0; i--)
        {
            Transform part = _ropeParts[i];

            float progress = (float)i / (count > 1 ? count - 1 : 1);

            float easedProgress = 1f - (1f - progress) * (1f - progress);

            float maxDelay = _totalDuration - individualShrinkDuration;
            float delay = easedProgress * maxDelay;

            ropeSequence.Insert(delay,
                part.DOScale(Vector3.zero, individualShrinkDuration)
                    .SetEase(Ease.OutQuad)
            );
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayAnimation();
        }
    }

    public void PlayAnimation()
    {
        AnimateRopeDisappearance();

        _finalRotateBackMechRoll = new Vector3(FullAngle * Direction, 0, 0);
        _finalRotateBody = new Vector3(_maxAngleX, 0, 0);

        _rotateBody.transform.eulerAngles = new Vector3(-90, 0, 0);

        _backMechRoll.DOLocalRotate(_finalRotateBackMechRoll, _totalDuration, RotateMode.FastBeyond360)
            .SetRelative(true)
            .SetEase(Ease.OutQuart)
            .From(_backMechRoll.localEulerAngles);

        _rotateBody.DOLocalRotate(_finalRotateBody, _totalDuration)
            .SetRelative(true)
            .SetEase(Ease.OutQuart)
            .From(_rotateBody.localEulerAngles);
    }
}
