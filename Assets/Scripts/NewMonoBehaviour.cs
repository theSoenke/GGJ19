using UnityEngine;
using System.Collections;

public class MapSpawnController : MonoBehaviour
{
    public GameObject level;

    private void Update()
    {
        // Check tap
        // place map
        var map = Instantiate(level);
    }
}
