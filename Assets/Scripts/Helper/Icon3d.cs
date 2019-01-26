using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Icon3d : MonoBehaviour
{
    public event Action OnIconClicked;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var cameraDirection = Camera.main.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(cameraDirection);
    }

    public void OnMouseDown() 
    {
        if(OnIconClicked != null) {
            OnIconClicked();
            gameObject.SetActive(false);
        }
    }
}
