using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateSpawn : MonoBehaviour
{
    public GameObject level;
    public WaveController waveController;


    void Awake()
    {
        waveController.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(level);
            waveController.enabled = true;
        }
    }
}
