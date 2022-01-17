using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutManager : MonoBehaviour
{
    public string[] sentences;
    public Text dialogueText;
    public GameObject panel; 
    private PersistentManager manager;
    int count = 0; 

    void Start()
    {
        manager =  GameObject.FindWithTag("PersistentManager").GetComponent<PersistentManager>();
        if (! manager.tutFinished) {
            panel.SetActive(true); 
            dialogueText.text = sentences[count];
            count++;
        } else {
            panel.SetActive(false);
        }
    }

    public void NextImage()
    {
        if (count < sentences.Length) {
            dialogueText.text = sentences[count];
            count++;
        } else {
            panel.SetActive(false);
            manager.tutFinished = true; 
        }
    }
}