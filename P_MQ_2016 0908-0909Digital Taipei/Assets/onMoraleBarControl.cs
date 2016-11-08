﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class onMoraleBarControl : MonoBehaviour {
    //public 
    public Image myUI_MoraleBar_MQ;
    public Image myUI_MoraleBar_Monster;
    public Image myUI_LocalMQ_Amount;
    // Use this for initialization
    void Start () {
        myUI_LocalMQ_Amount = transform.GetChild(0).GetComponent<Image>();
        myUI_MoraleBar_MQ = transform.GetChild(1).GetComponent<Image>();
        myUI_MoraleBar_Monster = transform.GetChild(2).GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
        myUI_MoraleBar_MQ.fillAmount = 1 - myUI_MoraleBar_Monster.fillAmount;
        myUI_LocalMQ_Amount.fillAmount = (float)GameObject.Find("Canvas").GetComponent<onCanvasForUIControll>().myLocalMQ_Amount / (float)GameObject.Find("Canvas").GetComponent<onCanvasForUIControll>().myLocalMQ_AmountFull;
    }
}
