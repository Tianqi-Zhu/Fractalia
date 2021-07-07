using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentManager : MonoBehaviour
{
    public static PersistentManager Instance;

    public static int collectionSize = 7;
    public string[] collectionStrArr = new string[collectionSize];    public bool[] enemyBoolArr = new bool[collectionSize]; // true is enemy is killed    public bool[] combinedBoolArr = new bool[collectionSize];    private void Awake()    {        if(Instance == null)        {            Instance = this;            for (int i = 0; i < collectionSize; i++)            {                collectionStrArr[i] = "EMPTY";            }            DontDestroyOnLoad(gameObject);        } else        {            Destroy(gameObject);        }    }    public void AddToCollection(int serialNo, string text)
    {
        collectionStrArr[serialNo] = text;
    }

    public void CombineChildrenArr(int serialNo)    {        combinedBoolArr[serialNo] = true;        int leftIndex = serialNo * 2 + 1;        collectionStrArr[serialNo] = collectionStrArr[leftIndex] + " " + collectionStrArr[serialNo];        collectionStrArr[serialNo] = collectionStrArr[serialNo] + " " + collectionStrArr[leftIndex + 1];    }

    public void ResetGame()    {        for (int i = 0; i < collectionSize; i++)        {            collectionStrArr[i] = "EMPTY";            enemyBoolArr[i] = false;        }        Debug.Log("Reset");    }
}
