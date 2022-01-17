using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{     
    public Sprite[] bgList;
    public GameObject imagePanel;
    public string[] sentences;
    public Text dialogueText;
    int count = 0; 

    void Start()
    {
        dialogueText.text = sentences[count];
        imagePanel.GetComponent<Image>().sprite = bgList[count];
        count++;
    }

    public void NextImage()
    {   
        if (count < bgList.Length) 
        {
            dialogueText.text = sentences[count];
            imagePanel.GetComponent<Image>().sprite = bgList[count];
            count++;
        } else {
            SceneManager.LoadScene(1);
        }
    }
}
