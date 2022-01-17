using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersistentManager : MonoBehaviour
{
    public static PersistentManager Instance;

    public static int collectionSize = 7;
    public string[] collectionStrArr = new string[collectionSize];
    public bool[] enemyBoolArr = new bool[collectionSize]; // true is enemy is killed
    public bool[] combinedBoolArr = new bool[collectionSize];
    public bool[] combinableBoolArr = new bool[collectionSize];

    public int addHealthAmount;
    public float shiedDuration;
    public int healthPotionNo = 0;
    public int shieldNo = 0;

    public bool tutFinished = false; 

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            for (int i = 0; i < collectionSize; i++)
            {
                collectionStrArr[i] = "EMPTY";
            }
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    void Start()    {        int leftmostChild = collectionSize / 2;        for (int i = leftmostChild; i < collectionSize; i++)        {            combinableBoolArr[i] = true;        }    }
    public void AddToCollection(int serialNo, string text)
    {
        collectionStrArr[serialNo] = text;
    }

    public void CombineChildrenArr(int serialNo)
    {
        combinedBoolArr[serialNo] = true;
        combinableBoolArr[serialNo] = true;

        int leftIndex = serialNo * 2 + 1;
        collectionStrArr[serialNo] = collectionStrArr[leftIndex] + " " + collectionStrArr[serialNo];
        collectionStrArr[serialNo] = collectionStrArr[serialNo] + " " + collectionStrArr[leftIndex + 1];

        // If root is combined
        if (serialNo == 0)        {            levelComplete();        }

    }

    public void ResetGame()
    {
        for (int i = 0; i < collectionSize; i++)
        {
            collectionStrArr[i] = "EMPTY";
            combinableBoolArr[i] = false;
            combinedBoolArr[i] = false;
            enemyBoolArr[i] = false;
        }

        healthPotionNo = 0;
        shieldNo = 0;
        Debug.Log("Reset");
    }

    public void levelComplete()    {        GameObject storyCanvas = GameObject.FindWithTag("StoryCanvas");        storyCanvas.transform.Find("StoryWindow/Title").GetComponent<Text>().text = "Congratulations!";        GameObject.FindWithTag("StoryCombineButton").SetActive(false);        GameObject.FindWithTag("StoryCloseButton").SetActive(false);        Button button = GameObject.FindWithTag("ReturnToMenuButton").GetComponent<Button>();        button.interactable = true;        button.transform.Find("Text").GetComponent<Text>().color = new Color(50f / 255, 50f / 255, 50f / 255);    }
}