﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class onUI_LevelClear : MonoBehaviour {
    [Header("全星積分")]
    public int myScoreGetAllStar;
    public int myScore;
    public Text myScore_Text;
    public float myCountScore;
    public Image myStar_Image;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        myCountScore = (float)myScore / (float)myScoreGetAllStar;
        myStar_Image.fillAmount = myCountScore;
        myScore_Text.text = "得分：" + myScore.ToString();

    }
    public void BTN_BackToBigMap() {
        if (myCountScore >= 1)//3星
        {

        }
        else if (myCountScore <= 1 && myCountScore >= 0.6)//2星
        {

        }
        else if (myCountScore <= 0.6 && myCountScore >= 0.25)//1星
        {

        }
        else if (myCountScore <= 0.25)//0星
        {

        }
        Application.LoadLevel("MainScene");
    }
}
