using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public float animationWalkSpeedFactor = 1.0f;

    private Vector3 inputVector;

    private Rigidbody rigidbodyComponent;
    private Animator animator;
    private Vector3 prevInputVector = new Vector3();
    public GameObject bullet;
    public Transform BulletParent;
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
        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (inputVector.sqrMagnitude >= 1.0f)
        {
            inputVector = inputVector.normalized;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        UpdateAnimator();        
        UpdateRotation();
    }

    void FixedUpdate()
    {
        UpdatePosition();
    }

    private void UpdateAnimator()
    {
        var animationWalkSpeed = animationWalkSpeedFactor * rigidbodyComponent.velocity.magnitude;
        animator.SetFloat("WalkSpeed", animationWalkSpeed);
    }

    private void Shoot()
    {
        GameObject bulletGameObject;
        if (BulletParent == null)
        {
            bulletGameObject = Instantiate(bullet, GunMuzzle.position, transform.rotation);
        }
        else
        {
            bulletGameObject = Instantiate(bullet, GunMuzzle.position, transform.rotation, BulletParent);
        }
        bulletGameObject.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
    }

    private void UpdatePosition()
    {       
        var speedVector = inputVector * movementSpeed;
        rigidbodyComponent.velocity = speedVector;
    }

    private void UpdateRotation()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100))
        {
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }
}
