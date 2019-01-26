using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public float animationWalkSpeedFactor = 1.0f;

    private Rigidbody rigidbodyComponent;
    private Animator animator;
    private Vector3 prevInputVector = new Vector3();
    public GameObject bullet;
    public float bulletSpeed = 1.0f;

    public Transform GunMuzzle;

    
    private const float SMALL_FLOAT = 0.0001f;

    private void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        prevInputVector = new Vector3(1.0f, 0.0f, 0.0f);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        UpdateAnimator();
        UpdatePosition();
        UpdateRotation();
    }

    private void UpdateAnimator()
    {
        var animationWalkSpeed = animationWalkSpeedFactor * rigidbodyComponent.velocity.magnitude;
        animator.SetFloat("WalkSpeed", animationWalkSpeed);
    }

    private void Shoot()
    {
        GameObject bulletGameObject = Instantiate(bullet, GunMuzzle.position, transform.rotation);
        bulletGameObject.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
    }

    private void UpdatePosition()
    {
        var inputVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (inputVector.sqrMagnitude >= 1.0f)
        {
            inputVector = inputVector.normalized;
        }
        var speedVector = inputVector * movementSpeed;
        rigidbodyComponent.velocity = speedVector;
    }

    private void UpdateRotation()
    {
        if(rigidbodyComponent.velocity.magnitude >= SMALL_FLOAT) {
            var forwardsVector = rigidbodyComponent.velocity.normalized;
            transform.rotation = Quaternion.LookRotation(forwardsVector);
        }
    }
}
