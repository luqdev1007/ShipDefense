using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private KeyCode _upKey = KeyCode.W;
    [SerializeField] private float _speed = 5;

    private void Start()
    {
        transform.localScale *= 3;
    }

    private void Update()
    {
        if (Input.GetKey(_upKey))
        {
            transform.Translate(Vector2.left * Time.deltaTime);
        }
    }
}
