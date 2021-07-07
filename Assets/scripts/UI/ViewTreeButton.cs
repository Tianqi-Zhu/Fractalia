using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewTreeButton : MonoBehaviour
{
    public Canvas treeCanvas;
    private PersistentManager Manager;
    private int collectionSize;
    private Text[] DisplayTextArr;
    public Button self; 

    void Start()
    {
        Manager = GameObject.FindWithTag("PersistentManager").GetComponent<PersistentManager>();
        collectionSize = PersistentManager.collectionSize;
        DisplayTextArr = new Text[collectionSize];
    }

    public void PopTreeWindow()
    {
        Canvas canvas = Instantiate(treeCanvas);
        for (int i = 0; i < collectionSize; i++)
        {
            DisplayTextArr[i] = canvas.transform.Find("TreeWindow/Verse" + i).GetComponent<Text>();
            if(!String.Equals(Manager.collectionStrArr[i], "EMPTY")) {
                DisplayTextArr[i].color = new Color(72f/255, 52f/255, 212f/255);
            }
        }
        self.interactable = false; 
    }
}