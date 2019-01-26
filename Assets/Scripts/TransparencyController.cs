using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class TransparencyController : MonoBehaviour
{
    public float transparency = 1.0f;

    private Renderer _renderer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        _renderer.material.SetFloat("_Transparency", transparency);
    }
}
