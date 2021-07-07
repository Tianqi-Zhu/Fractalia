using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTrigger : MonoBehaviour
{
    public int SceneNumber;

    void OnTriggerEnter(Collider other)    {        if (other.gameObject.tag == "Player")        {            SceneManager.LoadScene(SceneNumber);        }    }
}
