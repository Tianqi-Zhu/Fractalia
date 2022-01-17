using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MapSelector : MonoBehaviour
{
    public void Map1()
    {
        SceneManager.LoadScene(1);    
    }

    public void Map2()
    {
        SceneManager.LoadScene(4);    
    }

    public void Map3()
    {
        SceneManager.LoadScene(7);    
    }
}
