using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class CloseButton : MonoBehaviour
{   
    public void CloseTerminalWindow()
    {
        GameObject canvas = GameObject.FindWithTag("StoryCanvas");
        Destroy(canvas, 0.1f);
    }

    public void CloseTreeWindow()
    {
        GameObject canvas = GameObject.FindWithTag("TreeCanvas");
        Destroy(canvas, 0.1f);
        Button ViewTreeButton = GameObject.FindWithTag("ViewTreeButton").GetComponent<Button>();
        if (ViewTreeButton == null) {
            Debug.Log("Cannot find ViewTreeButton");
        }
        ViewTreeButton.interactable = true; 
    }
}
