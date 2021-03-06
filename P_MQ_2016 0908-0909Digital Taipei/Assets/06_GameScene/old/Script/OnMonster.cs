﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class OnMonster : MonoBehaviour {
    public UnityEngine.AI.NavMeshAgent agent;
    public Collider[] attackTarget;
    [Header("頭上的字串")]
    public string mytitle;
    [Header("血量")]
    public int myHP;
    [Header("攻擊力")]
    public int myAttack;
    [Header("移動速度")]
    public float mymovespeed;
    [Header("幾秒攻擊一次")]
    public float myAttackTimer;
    [Header("索敵角度")]
    public float viewAngle = 135.0f;
    [Header("所敵距離")]
    public float viewDistance = 20.0f;

    [Header("怪物狀態")]
    public int myMod;//1=待機 2=巡邏 3=追擊 4=戰鬥 5=死掉
    public bool willBeAttack;
    public bool isInjuredFinish;
    [System.Serializable]
    public struct Pathfindingdetalstuff
    {
        [Header("是否需要待機")]
        public bool IsNeedIdle;
        [Header("要待機幾秒")]
        public float IdleTimer;
        [Header("路徑點")]
        public Transform way;
    }
    [System.Serializable]
    public struct Pathfinding
    {
        [Header("永遠被動")]
        public bool idleforever;
        [Header("可倒退走ABCBA")]
        public bool cangoback;
        [Header("4點亂亂跑")]
        public bool pointrandom;//Random.Range(1, 5);
        [Header("巡邏點1")]
        public Pathfindingdetalstuff way1;
        [Header("巡邏點2")]
        public Pathfindingdetalstuff way2;
        [Header("巡邏點3")]
        public Pathfindingdetalstuff way3;
        [Header("巡邏點4")]
        public Pathfindingdetalstuff way4;
    }
    [Header("找路系統相關設定")]
    public Pathfinding myPathfindingsetting;
    public Transform TargetObject;
    private int random = 1;
    public int startpoint = 1;
    private float movetime = 4;
    public float time;
    public bool isidletime = false;
    public float myidletimer;
    public float myidletimer_now;
    public int mygoback = 0;//0=go 1=back
    //---------------------
    [Header("怪物搜敵")]
    //public float mydis;
    public Transform fireCube;
    public Transform[] targetCube;
    public Vector3[] targetDir;
    public float[] angle;

    private float basicViewDistance;
    RaycastHit hit;//檢查是否進入可視距離
    public GameObject myChaseTarget;

    public bool isChasePlayer=false;
    //---------------------
    //---------------------
    public float myTimer;
    
    public bool isAttackFinish;
    //---------------------
    //---------------------
    public GameObject[] myHitEffect;
    public Text myHPText;
    //---------------------
    //---------------------
    public int myModelMod;
    //---------------------
    //---------------------
    //---------------------
    // Use this for initialization
    void Start () {
        basicViewDistance = viewDistance + 1;
       // setTargetCube();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (myPathfindingsetting.idleforever)
        {
            myMod = 1;
        }
        else {
            myMod = 2;
        }
    }
    public void setTargetCube() {
        for (int a = 0; a < targetCube.Length; a++) {
            if (a == 0) {
                targetCube[a] = GameObject.Find("Player").transform;
                attackTarget[a] = targetCube[a].GetComponent<Collider>();
            }
            else {
                GameObject[] b = GameObject.FindGameObjectsWithTag("MQ");
                int c;
                c = a - 1;
                targetCube[a] = b[c].transform;
                attackTarget[a] = targetCube[a].GetComponent<Collider>();
            }
        }
    }
	// Update is called once per frame
	void Update () {
        
        myMonsterModController();
        //forUpdateViewDistance();
        if (isAttackFinish) {
            isAttackFinish = false;
        }
    }
    public void myMonsterModController()
    {
        switch (myMod)
        {
            case 1://idle
                forMod1();
                break;
            case 2://lookaround;
                forMod2();
                break;
            case 3://chase
                forMod3();
                break;
            case 4://fight
                forMod4();
                break;
            case 5://GG
                forMod5();
                break;
        }
    }
    //idle
    public void forMod1() {
        for (int a = 0; a < targetCube.Length; a++)
        {
            if (targetCube[a] == null || targetDir[a]==null || angle[a]==null) { }
            else {
                targetDir[a] = targetCube[a].transform.position - fireCube.transform.position;
                angle[a] = Vector3.Angle(targetDir[a], fireCube.transform.forward);
                if (targetDir[a].magnitude < viewDistance && angle[a] < viewAngle / 2)
                { // 目標進入敵人可視角
                    if (Physics.Raycast(fireCube.transform.position, targetDir[a], out hit))
                    {
                        // 目標看的到囉!
                        if (hit.transform.name == targetCube[a].transform.name)
                        {
                            //print("hit.transform.name == targetCube[a].transform.name");
                            //print("monster serching");
                            if (hit.transform.name == "Player")
                            {
                                print("is player!!!!!!!!!!");
                                myChaseTarget = targetCube[a].transform.gameObject;
                                targetCube[a].GetComponent<OnPlayer>().myMod = 2;
                                targetCube[a].GetComponent<OnPlayer>().whoIsChaseMe = gameObject;
                                myMod = 3;
                                isChasePlayer = true;
                            }
                            else {
                                //print("else 333");
                                myChaseTarget = targetCube[a].transform.gameObject;
                                targetCube[a].GetComponent<OnMQ>().myMod = 5;
                                targetCube[a].GetComponent<OnMQ>().whoIsChaseMe = gameObject;
                                myMod = 3;
                            }
                        }
                        Debug.DrawRay(hit.point, hit.normal * 1, Color.blue);
                        //Debug.DrawRay (hit.point, inDirection*100, Color.magenta);	
                        //Debug.Log("Object name: " + hit.transform.name);
                        Debug.DrawRay(fireCube.transform.position, targetDir[a], Color.green);
                        /*if (hit.transform == targetCube[a])
                        {
                            GameObject playerPos = GameObject.FindGameObjectWithTag("Player");
                            print("My target is " + playerPos);
                        }*/
                    }
                }
            }//else
            


        }//for
    }
    //lookaround
    public void forMod2() {
        if (myPathfindingsetting.pointrandom)
        {
            NavRandoccmMove();
            forMod1();
        }
        else {
            pointmove();
            forMod1();
        }
    }
    //chase
    public void forMod3() {
        //print("mod3 be call");
        if (myChaseTarget == null) { }
        else {
            if (isChasePlayer)
            {
                myChaseTarget = GameObject.Find("Player");
                //print("isPlayer");
                if (Vector3.Distance(transform.position, myChaseTarget.transform.position) <= 1.5)
                {
                    //print("in if mod3 be call");
                    agent.speed = 0;
                    print("抓到你囉！");
                    myMod = 4;
                }
                else {
                    //print("in else mod3 be call");
                    agent.speed = mymovespeed * 1.1f;
                    agent.destination = GameObject.Find("Player").transform.position;
                }
            }
            else {
                if (Vector3.Distance(transform.position, myChaseTarget.transform.position) <= 1.5)
                {
                    print("in if mod3 be call");
                    agent.speed = 0;
                    print("抓到你囉！");
                    myMod = 4;
                }
                else {
                    forMod1();
                    //print("in else mod3 be call");
                    agent.speed = mymovespeed * 1.1f;
                    agent.destination = myChaseTarget.transform.position;
                }
            }
        }
        
        
    }
    //fight
    public void forMod4() {
        //agent.speed = 0;
        /*
        if (myChaseTarget == null) { }
        else {
            gameObject.transform.LookAt(myChaseTarget.transform);
            if (Vector3.Distance(transform.position, myChaseTarget.transform.position) <= 1.5)
            {
                agent.speed = 0;
                myTimer += Time.deltaTime;
                if (myTimer >= myAttackTimer)
                {
                    myTimer = 0;
                    for (int a = 0; a < attackTarget.Length; a++) {
                        print("in for");
                        if (attackTarget[a] == null) {
                            print("attacktarget null");
                        }
                        else {
                            if (attackTarget[a].tag == "Player")
                            {
                                print("attacktarget player");
                                attackTarget[a].GetComponent<OnPlayer>().myMod = 3;
                                if (attackTarget[a].GetComponent<OnPlayer>().willBeAttack) {
                                    forHitEffect(0, myAttack.ToString(), "R");
                                    attackTarget[a].GetComponent<OnPlayer>().myHP -= myAttack;
                                    if (attackTarget[a].GetComponent<OnPlayer>().myHP <= 0)
                                    {
                                        attackTarget[a].GetComponent<OnPlayer>().myMod = 4;
                                        attackTarget[a] = null;
                                        myMod = 1;
                                    }
                                }
                            }
                            else {
                                if(attackTarget[a].GetComponent<OnMQ>().willBeAttack){
                                    print("怪物打對象一夏");
                                    forHitEffect(0, myAttack.ToString(), "R");
                                    attackTarget[a].GetComponent<OnMQ>().myHP -= myAttack;
                                    if (attackTarget[a].GetComponent<OnMQ>().myHP <= 0)
                                    {
                                        print("MQ is GG");
                                        attackTarget[a].GetComponent<OnMQ>().myMod = 7;
                                        attackTarget[a] = null;
                                        myMod = 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else {
                agent.speed = mymovespeed * 1.1f;
                agent.destination = myChaseTarget.transform.position;
            }
        }//else
        */
        
    }
    public void forMod5() {
        print("monster gg");
        Destroy(gameObject);
    }
    //injured
    public void forMod6() {
        //受傷惹
        print(gameObject.name + " get hurt");
        if (isInjuredFinish) {
            myMod = 4;
            isInjuredFinish = false;
        }
    }
    //看看前面有沒有障礙物，有的話更新所敵距離
    public void forUpdateViewDistance()
    {
        Ray ray = new Ray(fireCube.transform.position, fireCube.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50))
        {
            float mydis = Vector3.Distance(hit.point, fireCube.transform.position);
            if (mydis < viewDistance) { viewDistance = mydis; }
            else { viewDistance = basicViewDistance; }
            //print("my blue ray hit = " + hit.transform.name);
            Debug.DrawLine(ray.origin, hit.point, Color.blue);
        }
    }
    void pointmove()
    {
        agent.speed = mymovespeed;
        if (startpoint == 1)
        {
            TargetObject = myPathfindingsetting.way1.way;
            if (TargetObject != null)
            {
                agent.destination = TargetObject.position;
                if (Vector3.Distance(gameObject.transform.position, TargetObject.position) < 1.5f)
                {
                    //print(gameObject.name+" is on point 1");
                    if (myPathfindingsetting.way1.IsNeedIdle)
                    {
                        if (time < myPathfindingsetting.way1.IdleTimer)
                        {
                            myidletimer_now = myPathfindingsetting.way1.IdleTimer;
                            time += Time.deltaTime;
                            myMod = 1;//idle 
                        }
                        else {
                            myMod = 0;//walk ;
                            time = 0;
                            myidletimer_now = 0;
                            if (myPathfindingsetting.cangoback)
                            {
                                if (mygoback == 0)
                                {
                                    startpoint++;
                                }
                                else {
                                    startpoint++;
                                    mygoback = 0;
                                }
                            }
                            else {
                                if (myPathfindingsetting.pointrandom) { startpoint = Random.Range(1, 5); }
                                else { startpoint++; }

                            }

                        }
                    }
                    else {
                        if (myPathfindingsetting.cangoback)
                        {
                            if (mygoback == 0)
                            {
                                startpoint++;
                            }
                            else {
                                startpoint++;
                                mygoback = 0;
                            }
                        }
                        else {
                            if (myPathfindingsetting.pointrandom) { startpoint = Random.Range(1, 5); }
                            else { startpoint++; }
                        }

                    }
                }
            }
        }
        else if (startpoint == 2)
        {
            //print("Point 2 TargetObject = "+ TargetObject.name);

            TargetObject = myPathfindingsetting.way2.way;
            if (TargetObject != null)
            {
                //print("Point 2");
                agent.destination = TargetObject.position;
                if (Vector3.Distance(gameObject.transform.position, TargetObject.position) < 1.5f)
                {
                    if (myPathfindingsetting.way2.IsNeedIdle)
                    {
                        if (time < myPathfindingsetting.way2.IdleTimer)
                        {
                            myidletimer_now = myPathfindingsetting.way2.IdleTimer;
                            time += Time.deltaTime;
                            myMod = 1;//idle 
                        }
                        else {
                            myMod = 0;//walk ;
                            time = 0;
                            myidletimer_now = 0;
                            if (myPathfindingsetting.cangoback)
                            {
                                if (mygoback == 0) { startpoint++; }
                                else { startpoint--; }
                            }
                            else {
                                if (myPathfindingsetting.pointrandom) { startpoint = Random.Range(1, 5); }
                                else { startpoint++; }
                            }
                        }
                    }
                    else {
                        if (myPathfindingsetting.cangoback)
                        {
                            if (mygoback == 0) { startpoint++; }
                            else { startpoint--; }
                        }
                        else {
                            if (myPathfindingsetting.pointrandom) { startpoint = Random.Range(1, 5); }
                            else { startpoint++; }
                        }
                    }
                }
            }
        }//startpoint==2
        else if (startpoint == 3)
        {
            TargetObject = myPathfindingsetting.way3.way;
            if (TargetObject != null)
            {
                agent.destination = TargetObject.position;
                if (Vector3.Distance(gameObject.transform.position, TargetObject.position) < 1.5f)
                {
                    if (myPathfindingsetting.way3.IsNeedIdle)
                    {
                        if (time < myPathfindingsetting.way3.IdleTimer)
                        {
                            myidletimer_now = myPathfindingsetting.way3.IdleTimer;
                            time += Time.deltaTime;
                            myMod = 1;//idle 
                        }
                        else {
                            print("navv myMod=0");
                            //myMod = 0;//walk ;
                            time = 0;
                            myidletimer_now = 0;
                            if (myPathfindingsetting.cangoback)
                            {
                                if (mygoback == 0) { startpoint++; }
                                else { startpoint--; }
                            }
                            else {
                                if (myPathfindingsetting.pointrandom) { startpoint = Random.Range(1, 5); }
                                else { startpoint++; }
                            }
                        }
                    }
                    else {
                        if (myPathfindingsetting.cangoback)
                        {
                            if (mygoback == 0) { startpoint++; }
                            else { startpoint--; }
                        }
                        else {
                            if (myPathfindingsetting.pointrandom) { startpoint = Random.Range(1, 5); }
                            else { startpoint++; }
                        }
                    }
                }
            }
        }//startpoint==3
        else if (startpoint == 4)
        {
            TargetObject = myPathfindingsetting.way4.way;
            if (TargetObject != null)
            {
                agent.destination = TargetObject.position;
                if (Vector3.Distance(gameObject.transform.position, TargetObject.position) < 1.5f)
                {
                    print("point 4 in 1.5f");
                    if (myPathfindingsetting.way4.IsNeedIdle)
                    {
                        print("point4 isneedidle");
                        if (time < myPathfindingsetting.way4.IdleTimer)
                        {
                            myidletimer_now = myPathfindingsetting.way4.IdleTimer;
                            time += Time.deltaTime;
                            myMod = 1;//idle 
                        }
                        else {
                            myMod = 0;//walk ;
                            time = 0;
                            myidletimer_now = 0;
                            if (myPathfindingsetting.cangoback)//是否原路返回
                            {
                                startpoint--;
                                mygoback = 1;//
                            }
                            else {
                                if (myPathfindingsetting.pointrandom) { startpoint = Random.Range(1, 5); }
                                else { startpoint = 1; }
                            }
                        }
                    }
                    else {
                        if (myPathfindingsetting.cangoback)
                        {
                            startpoint--;
                            mygoback = 1;//
                        }
                        else {
                            if (myPathfindingsetting.pointrandom) { startpoint = Random.Range(1, 5); }
                            else { startpoint = 1; }
                        }
                    }
                }
            }
        }//startpoint==4
    }
    void NavRandoccmMove()
    {
        if (movetime > 0)
        {
            if (random == 1)
            {
                TargetObject = myPathfindingsetting.way1.way;
                if (TargetObject != null)
                {
                    agent.destination = TargetObject.position;
                }
                random = Random.Range(1, 5);
            }
            if (random == 2)
            {
                TargetObject = myPathfindingsetting.way2.way;
                if (TargetObject != null)
                {
                    agent.destination = TargetObject.position;
                }
                random = Random.Range(1, 5);
            }
            if (random == 3)
            {
                TargetObject = myPathfindingsetting.way3.way;
                if (TargetObject != null)
                {
                    agent.destination = TargetObject.position;
                }
                random = Random.Range(1, 5);
            }
            if (random == 4)
            {
                TargetObject = myPathfindingsetting.way4.way;
                if (TargetObject != null)
                {
                    agent.destination = TargetObject.position;
                }
                random = Random.Range(1, 5);
            }

        }
    }

    public void forHitEffect(int isBigHit, string hurt, string RGB)
    {
        print(gameObject.name + "forhiteffect");
        GameObject hiteffect = Instantiate(myHitEffect[isBigHit], Vector3.zero, Quaternion.identity) as GameObject;
        hiteffect.transform.parent = GameObject.Find("Canvas").transform;
        Vector2 a = myHPText.GetComponent<RectTransform>().anchoredPosition;

        hiteffect.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(a.x - 50, a.x + 50), Random.Range(a.y - 10, a.y + 100));
        hiteffect.GetComponentInChildren<Text>().text = hurt;
        switch (RGB)
        {
            case "R":
                Color c = hiteffect.GetComponentInChildren<Text>().color;
                c.r = 255;
                c.g = c.b = 0;
                hiteffect.GetComponentInChildren<Text>().color = c;
                break;
            case "B":
                Color cc = hiteffect.GetComponentInChildren<Text>().color;
                cc.b = 255;
                cc.r = cc.g = 0;
                hiteffect.GetComponentInChildren<Text>().color = cc;
                break;
        }
    }

}
