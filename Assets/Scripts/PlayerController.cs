using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public GameObject bullet;
    public float bulletSpeed = 1.0f;

    private Rigidbody rigidbodyComponent;

    void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
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
        GameObject bulletGameObject = Instantiate(bullet, transform.position, Quaternion.identity);
        bulletGameObject.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
    }

    void UpdatePosition()
    {
        var inputVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (inputVector.sqrMagnitude >= 1.0f)
        {
            inputVector = inputVector.normalized;
        }
      
        var speedVector = inputVector * movementSpeed;
        rigidbodyComponent.velocity = speedVector;
    }
}
