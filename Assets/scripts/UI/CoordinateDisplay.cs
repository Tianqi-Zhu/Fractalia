using System;
using UnityEngine;
using UnityEngine.UI;

public class CoordinateDisplay : MonoBehaviour
{
    public int terminalNo;
    public Transform player;    public GameObject[] enemyArr;
    public Text playerCoord;
    public Text[] terminalCoordArr;    private static Color calculateColor(float r, float g, float b)    {        return new Color(r / 255, b / 255, g / 255);    }    private Color collectColor = calculateColor(83, 92, 104);    private Color combineColor = calculateColor(72, 52, 212);    //private string terminalCoordStr;    private PersistentManager Manager;

    void Start()    {        Manager = GameObject.FindWithTag("PersistentManager").GetComponent<PersistentManager>();        for (int i = 0; i < terminalNo; i++)        {            Transform terminalTransform = enemyArr[i].GetComponent<Transform>();            int terminalSerialNo = enemyArr[i].GetComponent<EnemyController>().serialNo;            terminalCoordArr[i].text = "Terminal " + terminalSerialNo + ": ("                                + terminalTransform.position.x.ToString("0") + ", "                                + terminalTransform.position.y.ToString("0") + ", "                                + terminalTransform.position.z.ToString("0") + ")";            terminalCoordArr[i].name = "TerminalCoord" + terminalSerialNo;            if (!String.Equals(Manager.collectionStrArr[terminalSerialNo], "EMPTY"))            {                terminalCoordArr[i].color = collectColor;            }            if (Manager.combinedBoolArr[terminalSerialNo])            {                terminalCoordArr[i].color = combineColor;            }        }        /*        foreach (GameObject enemy in enemyArr)        {            Transform terminalTransform = enemy.GetComponent<Transform>();            terminalCoordStr += "Terminal " + enemy.GetComponent<EnemyController>().serialNo + ": ("                                + terminalTransform.position.x.ToString("0") + ", "                                + terminalTransform.position.y.ToString("0") + ", "                                + terminalTransform.position.z.ToString("0") + ")"                                + "\n";        }        terminalCoord.text = terminalCoordStr;        */    }

    void Update()
    {
        playerCoord.text = "Player: (" + player.position.x.ToString("0") + ", " + player.position.y.ToString("0") + ", " + player.position.z.ToString("0") + ")";
    }

    public void HighlightCollect(int serialNo)    {        GameObject.Find("TerminalCoord" + serialNo).GetComponent<Text>().color = collectColor;    }

    public void HighlightCombine(int serialNo)    {        GameObject.Find("TerminalCoord" + serialNo).GetComponent<Text>().color = combineColor;    }
}
