﻿using UnityEngine;
using System.Collections;

public class onMQVer3 : MonoBehaviour {
    public int WhatKindOfMQAmI;//0基本,1炸彈,2冰,3殭屍,4未定
    [Header("蚊子滿血血量")]
    public int myFullHP;//血量
    [Header("蚊子血量")]
    public int myHP;//血量
    [Header("蚊子移動速度")]
    public float myMoveSpeed;//蚊子的跑速
    [Header("蚊子攻擊力")]
    public int myAttack;
    [Header("蚊子多久打一下(秒)")]
    public float myAttackTimerTarget;
    [Header("蚊子爆擊率")]
    public int myCritHit;


    public GameObject myTargetPoint;//蚊子咬哪邊
    public bool isAttackTime;//該打怪了嗎？
    public bool isBeHit;
    public bool isHitFlyAway;
    public bool isNeedToMoveToNextPoint;//要咬下個地方嗎？
    public float myScaleControl;//蚊子多大隻
    public float myAttackTimer;
    //--------------------------------------
    public bool isSuperStarTime;
    float isSuperStarTimer;
    public GameObject green;

    public bool isAttackPowerUpTime;
    float isAttackPowerUpTimer;

    public bool isAttackSpeedUpTime;
    float isAttackSpeedUpTimer;

    public bool isCritHitUpTime;
    float isCritHitUpTimer;

    //---------------------------------------
    public int myMQAniMod;
    public float myHitflyAwayTimer;

    public GameObject DeadEffect;
    public GameObject myChildMQ;
    float deadtimer;

    Vector3 myTargetPointRandom;
    // Use this for initialization
    void Start()
    {
        myHP = myFullHP;
        //DeadEffect.SetActive(false);
        //green.SetActive(false);
        transform.localScale = new Vector3(myScaleControl, myScaleControl, myScaleControl);
        myTargetPoint = GameObject.Find("MainCamera").GetComponent<OnCameraLookAt>().HotPointList[GameObject.Find("MainCamera").GetComponent<OnCameraLookAt>().cameraMod];
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        myTargetPointRandom = myTargetPoint.transform.position;
        /*myTargetPointRandom.x = Random.Range(myTargetPoint.transform.position.x - 0.05f, myTargetPoint.transform.position.x + 0.05f);
        myTargetPointRandom.y = Random.Range(myTargetPoint.transform.position.y - 0.05f, myTargetPoint.transform.position.y + 0.05f);
        */
        //transform.rotation = GameObject.Find("MainCamera").transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (myHP < 1)//被打三下就得死
        {
            if (deadtimer >= 1.5f)
            {
                deadtimer = 0;
                GameObject.Find("MainCamera").GetComponent<OnCameraForShootMQ>().myHowManyMQOnScene--;
                Destroy(gameObject);
            }
            else {
                myChildMQ.SetActive(false);
                DeadEffect.SetActive(true);
                deadtimer += Time.deltaTime;
            }
        }
        else if (isHitFlyAway) { myBeHitThenFlyAway(); }
        else if (myTargetPoint.GetComponent<OnLookAtPoint>().myHP <= 0)
        {
            if (myTargetPoint.name == "hitpoint-2" || myTargetPoint.name == "hitpoint-3") { }
            else {
                isAttackTime = false;
                isNeedToMoveToNextPoint = true;
            }
        }
        
        myMQModController();
        //mydisAtoBis = Vector3.Distance(gameObject.transform.position, myTargetPoint.transform.position);

        //myMQAnimController();
        MQBuff();
    }
    public void MQBuff() {
        if (isSuperStarTime)
        {
            if (isSuperStarTimer >= 5){isSuperStarTimer = 0;isSuperStarTime = false;}
            else {isSuperStarTimer += Time.deltaTime;}
        }
        if (isAttackPowerUpTime) {
            if (isAttackPowerUpTimer >= 5) { isAttackPowerUpTimer = 0; isAttackPowerUpTime = false; }
            else { isAttackPowerUpTimer += Time.deltaTime; }
        }
        if (isAttackSpeedUpTime) {
            if (isAttackSpeedUpTimer >= 5) { isAttackSpeedUpTimer = 0; isAttackSpeedUpTime = false; }
            else { isAttackSpeedUpTimer += Time.deltaTime; }
        }
        if (isCritHitUpTime) {
            if (isCritHitUpTimer >= 5) { isCritHitUpTimer = 0; isCritHitUpTime = false; }
            else { isCritHitUpTimer += Time.deltaTime; }
        }

    }
    public void myMQModController()
    {
        if (isBeHit) { myMQAniMod = 3; }
        else if (isAttackTime)
        {
            //print("夠近拉，可以打人了");
            if (myAttackTimer >= myAttackTimerTarget)
            {
                myMQAniMod = 0;
            }
            else {
                myMQAniMod = 1;
                myAttackTimer += Time.deltaTime;
            }
        }
        else {
            if (isNeedToMoveToNextPoint)
            {//判斷是否要飛到下一個點
                myMQAniMod = 2;
                myMoveToNextPoint();
            }
            else {
                myMQAniMod = 2;
                myFlyToTarget();
            }
        }
    }
    public void myFlyToTarget() {
        gameObject.transform.LookAt(myTargetPoint.transform);
        myTargetPointRandom = myTargetPoint.transform.position;
        transform.position = Vector3.Lerp(transform.position, myTargetPointRandom, Time.deltaTime * myMoveSpeed);
        //print("move to target");
    }
    public void myBeHitThenFlyAway() {
        if (myHitflyAwayTimer >= 1)
        {
            myHitflyAwayTimer = 0;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            isHitFlyAway = false;
            myMoveSpeed = 1.5f;
        }
        else {
            myHitflyAwayTimer += Time.deltaTime;
            gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward*-8;
            isAttackTime = false;
            transform.parent = null;
        }
    }
    public void myMoveToNextPoint() {
        //讓蚊子飛到下一個點
        myMoveSpeed = 1;
        if (myTargetPoint.name == "hitpoint-1") {
            //print("if (myTargetPoint.name == hitpoint - 1) {");
            if (GameObject.Find("hitpoint-4").GetComponent<OnLookAtPoint>().myHP >= 0|| GameObject.Find("hitpoint-5").GetComponent<OnLookAtPoint>().myHP >= 0) {
                int a = Random.Range(0, 2);
                if (a == 0) { myTargetPoint = GameObject.Find("hitpoint-4"); }
                else { myTargetPoint = GameObject.Find("hitpoint-5"); }
                //print(GameObject.Find("hitpoint-4").GetComponent<OnLookAtPoint>().myName);
            }
            else {
                //print("elee");
                //print(GameObject.Find("hitpoint-4").GetComponent<OnLookAtPoint>().myName);
                int a = Random.Range(0, 2);
                if (a == 0) { myTargetPoint = GameObject.Find("hitpoint-2"); }
                else { myTargetPoint = GameObject.Find("hitpoint-3"); }
            }
        }
        else if (myTargetPoint.name == "hitpoint-4" || myTargetPoint.name == "hitpoint-5") {
            if (GameObject.Find("hitpoint-1").GetComponent<OnLookAtPoint>().myHP <= 0) {
                int a = Random.Range(0, 2);
                if (a == 0) { myTargetPoint = GameObject.Find("hitpoint-2"); }
                else { myTargetPoint = GameObject.Find("hitpoint-3"); }
            }
            else {myTargetPoint = GameObject.Find("hitpoint-1"); }
        }
        myFlyToTarget();
    }
    public void myMQSkill() {
        if (GameObject.Find("MainCamera").GetComponent<OnCameraForShootMQ>().myTeamBTNClick == WhatKindOfMQAmI) {
            switch (WhatKindOfMQAmI)
            {
                case 1:
                    print("onMQVer3 Team A mq skill be call");
                    break;
                case 2:
                    print("onMQVer3 Team B mq skill be call");
                    break;
                case 3:
                    print("onMQVer3 Team C mq skill be call");
                    break;
                case 4:
                    print("onMQVer3 Team D mq skill be call");
                    break;
                case 5:
                    print("onMQVer3 Team E mq skill be call");
                    break;
                default:
                    break;
            }
        }
    }
}
/* if (Input.GetKeyDown(",")){
     if (myMQAniMod < 0) { myMQAniMod = 4; }
     else { myMQAniMod--; }
 }
 if (Input.GetKeyDown("."))
 {
     if (myMQAniMod > 4) { myMQAniMod = 0; }
     else { myMQAniMod++; }
 }*/