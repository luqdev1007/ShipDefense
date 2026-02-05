using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] private Rigidbody _rigidbody;

    private bool _inFlight = false;

    public void Launch(float power)
    {
        transform.SetParent(null);

        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        _rigidbody.AddForce(transform.forward * power, ForceMode.Impulse);

        _inFlight = true;
    }

    private void FixedUpdate()
    {
        if (!_inFlight) 
            return;

        if (_rigidbody.linearVelocity.sqrMagnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(_rigidbody.linearVelocity);
        }
    }
}