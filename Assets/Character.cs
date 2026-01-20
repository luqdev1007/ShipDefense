using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 200f;

    [Header("Ранец")]
    [SerializeField] private Transform _backpackPoint;
    [SerializeField] private float _cubeHeight = 0.3f;
    [SerializeField] private Transform _stackRoot; // родитель для качающейся башни

    private CharacterController _controller;
    private List<Transform> _stackedCubes = new List<Transform>();
    private bool _isMoving = false;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CollectableItem item) && !item.IsTriggered)
        {
            item.IsTriggered = true;
            AddToStack(item);
        }
    }

    private void AddToStack(CollectableItem item)
    {
        Transform targetFollow = _stackedCubes.Count == 0 ? _backpackPoint : _stackedCubes[_stackedCubes.Count - 1];
        float yOffset = _stackedCubes.Count * _cubeHeight;

        Vector3 targetWorldPos = new Vector3(
            targetFollow.position.x,
            _backpackPoint.position.y + yOffset,
            targetFollow.position.z
        );

        item.transform.SetParent(null); // чтобы не следовал сразу
        item.JumpToStack(targetWorldPos, () =>
        {
            item.StartFollowing(targetFollow, _cubeHeight);
            _stackedCubes.Add(item.transform);
        });
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        Vector3 move = transform.forward * moveInput * moveSpeed;
        _controller.SimpleMove(move);
        transform.Rotate(Vector3.up, turnInput * rotationSpeed * Time.deltaTime);

        bool isCurrentlyMoving = moveInput != 0 || turnInput != 0;

        if (isCurrentlyMoving && !_isMoving)
        {
            _isMoving = true;
            ShakeTower();
        }
        else if (!isCurrentlyMoving && _isMoving)
        {
            _isMoving = false;
        }
    }

    private void ShakeTower()
    {
        _stackRoot.DOKill();
        _stackRoot.DOShakeRotation(0.3f, new Vector3(0, 0, 5f), 10, 90, false, ShakeRandomnessMode.Harmonic)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
