using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeDots : MonoBehaviour
{
    public Image[] livesImgArr;

    private int currentArrIndex;

    void Start()    {        currentArrIndex = PlayerController.maxLives - PlayerController.currentLives;        for (int i = 0; i < PlayerController.maxLives - 1; i++)        {            if (i < currentArrIndex)            {                livesImgArr[i].color = Color.black;            } else            {                livesImgArr[i].color = Color.white;            }        }    }

    public void DarkenDot()    {        currentArrIndex = PlayerController.maxLives - PlayerController.currentLives;        livesImgArr[currentArrIndex].color = Color.black;    }
}
