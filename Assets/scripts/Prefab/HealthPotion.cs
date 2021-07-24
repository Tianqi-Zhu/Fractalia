using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{    private PersistentManager Manager;    private Collectibles collectibles;

    void Start()
    {
        Manager = GameObject.FindWithTag("PersistentManager").GetComponent<PersistentManager>();        collectibles = GameObject.FindWithTag("Collectibles").GetComponent<Collectibles>();
    }    private void OnCollisionEnter(Collision other)
    {        if (other.gameObject.tag == "Player")        {            Manager.healthPotionNo += 1;            collectibles.setHealthPotionText(Manager.healthPotionNo);            Destroy(gameObject);        }    }
}
