using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombineButton : MonoBehaviour
{    private PersistentManager Manager;
    private Button button;    private CoordinateDisplay coordDisplay;
    public int serialNo;

    void Start()
    {
        Manager = GameObject.FindWithTag("PersistentManager").GetComponent<PersistentManager>();
        button = GetComponent<Button>();        coordDisplay = GameObject.FindWithTag("DisplayCanvas").GetComponent<CoordinateDisplay>();
    }    public void CombineChildrenDisplay()
    {
        GameObject canvas = GameObject.FindWithTag("StoryCanvas");

        int leftIndex = serialNo * 2 + 1;

        Manager.CombineChildrenArr(serialNo);        coordDisplay.HighlightCombine(serialNo);

        Text BodyLeft = canvas.transform.Find("StoryWindow/BodyLeft").GetComponent<Text>();
        Text BodyRight = canvas.transform.Find("StoryWindow/BodyRight").GetComponent<Text>();

        BodyLeft.text = Manager.collectionStrArr[leftIndex];
        BodyRight.text = Manager.collectionStrArr[leftIndex + 1];

        button.interactable = false;

    }
}
