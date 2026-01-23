using DG.Tweening;
using System.Collections;
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
    private const int InfinityLoops = -1;


    public IEnumerator AnimateRopeDisappearance()
    {
        while (true)
        {
            Sequence ropeSequence = DOTween.Sequence();
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

            yield return new WaitUntil(() => ropeSequence.IsComplete());

            ropeSequence.Kill();
        }
    }


    public void Start()
    {
        _finalRotateBackMechRoll = new Vector3(FullAngle * Direction, 0, 0);
        _finalRotateBody = new Vector3(_maxAngleX, 0, 0);

        _backMechRoll.DOLocalRotate(_finalRotateBackMechRoll, _totalDuration, RotateMode.FastBeyond360)
            .SetRelative(true)
            .SetEase(Ease.OutQuart)
            .SetLoops(InfinityLoops)
            .From(_backMechRoll.localEulerAngles);

        _rotateBody.DOLocalRotate(_finalRotateBody, _totalDuration)
            .SetRelative(true)
            .SetEase(Ease.OutQuart)
            .SetLoops(InfinityLoops)
            .From(_rotateBody.localEulerAngles);

        StartCoroutine(AnimateRopeDisappearance());      
    }
}
