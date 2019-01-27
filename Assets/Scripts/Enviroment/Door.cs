using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    [SerializeField]
    private GameObject _door;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerController>() != null)
        {
            _door.transform.Rotate(Vector3.up * 90f);
        }
    }


    public void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerController>() != null)
        {
            _door.transform.Rotate(- Vector3.up * 90f);
        }
    }
}
