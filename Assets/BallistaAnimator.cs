using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BallistaAnimator : MonoBehaviour
{
    [SerializeField] private Transform _backMechRoll;
    [SerializeField] private Transform[] _ropeParts;
    [SerializeField] private Transform _rotateBody;
    [SerializeField] private Transform _leftRopePivot;
    [SerializeField] private Transform _rightRopePivot;

    [SerializeField] private float _maxAngleX = -70;
    [SerializeField] private float _totalDuration = 2;

    private Vector3 _finalRotateBackMechRoll;
    private Vector3 _finalRotateBody;

    private const float FullAngle = 360;
    private const int Direction = -1;

    public void AnimateRopeDisappearance()
    {
        Sequence ropeSequence = DOTween.Sequence();

        int count = _ropeParts.Length;
        float totalRopeTime = _totalDuration * 0.75f;

        for (int i = 0; i < count; i++) 
        {
            Transform part = _ropeParts[i];
            part.localScale = Vector3.one * 0.1f;

            float progress = (float)i / (count > 1 ? count - 1 : 1);

            float easedProgress = progress * progress;

            // Если хочешь еще более резкое замедление в конце, используй InCubic:
            // float easedProgress = progress * progress * progress;

            float delay = easedProgress * (totalRopeTime - 0.2f);

            float shrinkTime = Mathf.Lerp(2, 0.05f, progress);

            ropeSequence.Insert(delay,
                part.DOScale(Vector3.zero, shrinkTime)
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
        _leftRopePivot.transform.eulerAngles = new Vector3(0, 0, -37.5f);
        _rightRopePivot.transform.eulerAngles = new Vector3(0, 0, 0);

        _backMechRoll.DOLocalRotate(_finalRotateBackMechRoll, _totalDuration, RotateMode.FastBeyond360)
            .SetRelative(true)
            .SetEase(Ease.OutQuart)
            .From(_backMechRoll.localEulerAngles);

        _rotateBody.DOLocalRotate(_finalRotateBody, _totalDuration)
            .SetRelative(true)
            .SetEase(Ease.OutQuart)
            .From(_rotateBody.localEulerAngles);

        _leftRopePivot.DOLocalRotate(Vector3.zero, _totalDuration)
            .SetEase(Ease.OutQuart);

        _rightRopePivot.DOLocalRotate(new Vector3(0, 0, -37.5f), _totalDuration)
            .SetEase(Ease.OutQuart);
    }
}
