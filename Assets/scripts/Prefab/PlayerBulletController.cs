using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletController : MonoBehaviour
{
    public float damage;

    void OnCollisionEnter(Collision co)    {        if (co.gameObject.tag == "Bullet")        {            Destroy(gameObject);        }
        else if (co.gameObject.tag == "Enemy")        {            co.gameObject.GetComponent<EnemyController>().TakeDamage(damage);            Destroy(gameObject);        }    }
}
