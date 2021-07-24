using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{    private PersistentManager Manager;

    void Start()
    {
        Manager = GameObject.FindWithTag("PersistentManager").GetComponent<PersistentManager>();
    }    private void OnCollisionEnter(Collision other)
    {        if (other.gameObject.tag == "Player")        {            Manager.healthPotionNo += 1;            Destroy(gameObject);        }    }
}
