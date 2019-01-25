using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float MoveSpeed = 20f;
    public float XScale = 1f;
    public float YScale = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + new Vector3(Input.GetAxis("Horizontal") * XScale, 0, Input.GetAxis("Vertical") * YScale) * MoveSpeed * Time.deltaTime;
    }
}
