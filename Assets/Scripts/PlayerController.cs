using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 1.0f;

    private Rigidbody rigidbodyComponent;
    private Vector3 prevInputVector = new Vector3();

    private void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
        prevInputVector = new Vector3(1.0f, 0.0f, 0.0f);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        UpdatePosition();
    }

    private void Shoot()
    {
        Debug.Log("Shoot");
    }

    void UpdatePosition()
    {
        var inputVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (inputVector.sqrMagnitude >= 1.0f)
        {
            inputVector = inputVector.normalized;
        }
        if (inputVector.sqrMagnitude >= 0.0001)
        {
            prevInputVector = inputVector.normalized;
        }

        var speedVector = inputVector * movementSpeed;
        rigidbodyComponent.velocity = speedVector;
    }
}
