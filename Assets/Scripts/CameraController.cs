using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float MoveSpeed = 20f;
    public float XScale = 1f;
    public float YScale = 1f;

    private Vector3 offset;


    void Start()
    {
        if(player != null)
        {
            offset = transform.position - player.transform.position;
        }
    }

    void Update()
    {
        if(player != null)
        {
            
            transform.position = player.position + offset;
        }
        else
        {
            transform.position = transform.position + new Vector3(Input.GetAxis("Horizontal") * XScale, 0, Input.GetAxis("Vertical") * YScale) * MoveSpeed * Time.deltaTime;
        }
    }
}
