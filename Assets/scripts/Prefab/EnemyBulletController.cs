using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{    public float damage;

    void OnCollisionEnter(Collision co)
    {
        if (co.gameObject.tag == "Bullet")
        {
            Destroy(gameObject);
        }
        else if (co.gameObject.tag == "Player")
        {
            co.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
