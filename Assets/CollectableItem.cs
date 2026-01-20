using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float jumpDuration = 0.4f;
    [SerializeField] private float jumpHeight = 1f;

    public bool IsTriggered { get; set; }

    private Transform _followTarget;
    private Vector3 _offset;
    private bool _isFollowing;

    public void JumpToStack(Vector3 targetPosition, System.Action onComplete)
    {
        transform.DOJump(targetPosition, jumpHeight, 1, jumpDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void StartFollowing(Transform followTarget, float yOffset)
    {
        _followTarget = followTarget;
        _offset = new Vector3(0f, yOffset, 0f);
        _isFollowing = true;
        StartCoroutine(FollowRoutine());
    }

    private IEnumerator FollowRoutine()
    {
        while (_isFollowing)
        {
            if (_followTarget != null)
            {
                Vector3 targetPos = _followTarget.position + _offset;
                transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
            }

            yield return null;
        }
    }
}
