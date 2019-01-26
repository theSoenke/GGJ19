using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float _damage;

    [SerializeField]
    private float _lifetime;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, _lifetime);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void OnTriggerEnter(Collider other)
    {
        var enemy = other.gameObject.GetComponent<EnemyController>();
        
        if(enemy != null) {
            enemy.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}
