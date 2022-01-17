using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalController : MonoBehaviour
{    public int serialNo;    public string title;    public string centerText;    public string leftText;    public string rightText;    public Canvas storyCanvas;    private bool isCollected = false;    private PersistentManager Manager;    private CoordinateDisplay coordDisplay;    void Start()    {        Manager = GameObject.FindWithTag("PersistentManager").GetComponent<PersistentManager>();        coordDisplay = GameObject.FindWithTag("DisplayCanvas").GetComponent<CoordinateDisplay>();    }    private void OnCollisionEnter(Collision other)
    {        GameObject canvas = GameObject.FindWithTag("StoryCanvas");        if (other.gameObject.tag == "Player" && canvas == null)        {            PopUpWindow();            if (!isCollected)            {                AddTextToCollection();            }        }    }

    private void PopUpWindow()    {        Canvas canvas = Instantiate(storyCanvas);        Text Title = canvas.transform.Find("StoryWindow/Title").GetComponent<Text>();        Text BodyCenter = canvas.transform.Find("StoryWindow/BodyCenter").GetComponent<Text>();        Text BodyLeft = canvas.transform.Find("StoryWindow/BodyLeft").GetComponent<Text>();        Text BodyRight = canvas.transform.Find("StoryWindow/BodyRight").GetComponent<Text>();        Title.text = title;        BodyCenter.text = centerText;        BodyLeft.text = leftText;        BodyRight.text = rightText;        Button combineButton = canvas.transform.Find("StoryWindow/CombineButton").GetComponent<Button>();        combineButton.GetComponent<CombineButton>().serialNo = serialNo;        int leftIndex = serialNo * 2 + 1;        // If not leaf, and both children must be collected, and both children must be combined        if (leftIndex < PersistentManager.collectionSize
            && !String.Equals(Manager.collectionStrArr[leftIndex], "EMPTY")
            && !String.Equals(Manager.collectionStrArr[leftIndex + 1], "EMPTY")            && Manager.combinableBoolArr[leftIndex]            && Manager.combinableBoolArr[leftIndex + 1])        {
            combineButton.interactable = true;
        }        else
        {
            // Deactivate button
            combineButton.interactable = false;
        }    }

    private void AddTextToCollection()    {        isCollected = true;
        Manager.AddToCollection(serialNo, centerText);        coordDisplay.HighlightCollect(serialNo);
    }
}
