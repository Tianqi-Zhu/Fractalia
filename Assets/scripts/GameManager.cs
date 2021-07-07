using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject completeLevelUI;

    public void nextIteration()
    {   
        Debug.Log("GG");
        completeLevelUI.SetActive(true);
    }
}
