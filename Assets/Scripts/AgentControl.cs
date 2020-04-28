using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentControl : MonoBehaviour
{
    public GameObject agentObjPrefab;
    public GameObject agentsObj;

    public GameObject Ball;

    public const int AGENTSnum = 100;
    public const float MAPSIZE = 100f;
    public const float SearchRadi = 12.0f;
    public const float SearchRadiMin = 3.0f;
    public const float DestRadiMin = 8.0f;
    public static bool isToThrow;
    public static List<int> duller;

    private bool whether;
    private bool isSwitched;
    private Vector3 tempGoal;

    private Vector3[] Goals;

    public struct AgentInfo
    {
        //int ID;
        public int color;
        public GameObject g;
        public float BallSec;
    }

    public static List<AgentInfo> AllAgentsInfo;

    void Awake()
    {

        AllAgentsInfo = new List<AgentInfo>();

        for (int i = 0; i < AGENTSnum * 2; i++)
        {

            float xTemp, yTemp;
            if (i < AGENTSnum)
            {
                xTemp = Random.Range(0f, 50f);
                yTemp = Random.Range(-50f, 50f);
            }
            else
            {
                xTemp = Random.Range(-50f, 0f);
                yTemp = Random.Range(-50f, 50f);
            }


            GameObject g = Instantiate(agentObjPrefab, agentsObj.transform);
            g.transform.position = new Vector3(xTemp, 0.0f, yTemp);

            AgentInfo tempInfo;


            if (i < AGENTSnum)
            {
                tempInfo.color = 0; //red
                g.GetComponent<Renderer>().material.color = Color.red;
            }
            else
            {
                tempInfo.color = 1; // blue
                g.GetComponent<Renderer>().material.color = Color.blue;
            }
            tempInfo.g = g;
            tempInfo.BallSec = 0.0f;
            
            AllAgentsInfo.Add(tempInfo);
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        duller = new List<int>();
        Goals = new Vector3[2] { new Vector3(-MAPSIZE, 0f, 0f), new Vector3(MAPSIZE, 0f, 0f) };
        whether = false;
        isSwitched = false;
        isToThrow = false;
    }

    // Update is called once per frame
    void Update()
    {

        float ballVelo = Mathf.Sqrt((BallEtc.theBallVelo.x) * (BallEtc.theBallVelo.x) + (BallEtc.theBallVelo.y) * (BallEtc.theBallVelo.y) + (BallEtc.theBallVelo.z) * (BallEtc.theBallVelo.z));
        Debug.Log(ballVelo);

            if (Input.GetKey(KeyCode.Space))
        {
            tempGoal.x = Random.Range(0f, 50f);
            tempGoal.z = Random.Range(-50f, 50f);
            //Debug.Log(tempGoal.x);
        }

        if (BallEtc.theBallPos.y < 6f)
        {
            tempGoal.x = BallEtc.theBallPos.x;
            tempGoal.y = BallEtc.theBallPos.z;
        }


        //働かないやつ
        duller = new List<int>();
        int end = AllAgentsInfo.Count;
        float rate = 0.5f;
        int count = (int)((float)end * (1f - rate));
        for (int i = 1; i <= end; i++)
            duller.Add(i);
        while (count-- >= 0)
        {
            int index = Random.Range(0, duller.Count);
            int ransu = duller[index];
            duller.RemoveAt(index);
        }
        if (BallEtc.theBallPos.y >= 25 && !isSwitched)
        {
            whether = !whether;
            isSwitched = true;
        }
        if (BallEtc.theBallPos.y < 25)
            isSwitched = false;
        //働かないやつ

        //死滅用
        List<int> tempRemove = new List<int>();
        //死滅用

        for (int i = 0; i < AllAgentsInfo.Count; i++)
        {

            /*if (AllAgentsInfo[i].color == 0)
                AllAgentsInfo[i].g.GetComponent<Renderer>().material.color = Color.red;
            if (AllAgentsInfo[i].color == 1)
                AllAgentsInfo[i].g.GetComponent<Renderer>().material.color = Color.blue;*/

            if (duller.Contains(i))
                if (whether)
                {
                    if (AllAgentsInfo[i].color == 0)
                        continue;
                }
                else
                {
                    if (AllAgentsInfo[i].color == 1)
                        continue;
                }

                    int neighbor = 0;
            List<int> tempNei = new List<int>();
            for (int j = 0; j < AllAgentsInfo.Count; j++)
            {
                if (tempRemove.Contains(j))
                    continue;

                if (i != j && AllAgentsInfo[i].color == AllAgentsInfo[j].color)
                {
                    if (Mathf.Abs(AllAgentsInfo[i].g.transform.position.x - AllAgentsInfo[j].g.transform.position.x) < SearchRadi)
                    {
                        float evTmp = (AllAgentsInfo[i].g.transform.position.x - AllAgentsInfo[j].g.transform.position.x) * (AllAgentsInfo[i].g.transform.position.x - AllAgentsInfo[j].g.transform.position.x) + (AllAgentsInfo[i].g.transform.position.z - AllAgentsInfo[j].g.transform.position.z) * (AllAgentsInfo[i].g.transform.position.z - AllAgentsInfo[j].g.transform.position.z);
                        
                        if ( evTmp < SearchRadi * SearchRadi)// && evTmp >= SearchRadiMin * SearchRadiMin)
                        {
                            neighbor++;
                            tempNei.Add(j);

                            float tempAtan;
                            float guideForce = 10.0f;
                            tempAtan = Mathf.Atan2(AllAgentsInfo[j].g.transform.position.z - AllAgentsInfo[i].g.transform.position.z, AllAgentsInfo[j].g.transform.position.x - AllAgentsInfo[i].g.transform.position.x);
                            AllAgentsInfo[i].g.GetComponent<Rigidbody>().AddForce(guideForce * Mathf.Cos(tempAtan), 0.0f, guideForce * Mathf.Sin(tempAtan));
                            AllAgentsInfo[j].g.GetComponent<Rigidbody>().AddForce(-guideForce * Mathf.Cos(tempAtan), 0.0f, -guideForce * Mathf.Sin(tempAtan));
                        }
                        if (evTmp < SearchRadiMin * SearchRadiMin)
                        {

                            float tempAtan;
                            float guideForce = -30.0f;

                            float evTemp2 = (tempGoal.y - AllAgentsInfo[i].g.transform.position.z) * (tempGoal.y - AllAgentsInfo[i].g.transform.position.z) + (tempGoal.x - AllAgentsInfo[i].g.transform.position.x) * (tempGoal.x - AllAgentsInfo[i].g.transform.position.x);
                            if (evTemp2 <= 30 * 30)
                            {
                                guideForce = -20.0f;
                            }

                            tempAtan = Mathf.Atan2(AllAgentsInfo[j].g.transform.position.z - AllAgentsInfo[i].g.transform.position.z, AllAgentsInfo[j].g.transform.position.x - AllAgentsInfo[i].g.transform.position.x);
                            AllAgentsInfo[i].g.GetComponent<Rigidbody>().AddForce(guideForce * Mathf.Cos(tempAtan), 0.0f, guideForce * Mathf.Sin(tempAtan));
                            AllAgentsInfo[j].g.GetComponent<Rigidbody>().AddForce(-guideForce * Mathf.Cos(tempAtan), 0.0f, -guideForce * Mathf.Sin(tempAtan));
                        }
                    }
                }
                if (i != j && AllAgentsInfo[i].color != AllAgentsInfo[j].color)
                {
                    if (Mathf.Abs(AllAgentsInfo[i].g.transform.position.x - AllAgentsInfo[j].g.transform.position.x) < SearchRadi)
                    {
                        float evTmp = (AllAgentsInfo[i].g.transform.position.x - AllAgentsInfo[j].g.transform.position.x) * (AllAgentsInfo[i].g.transform.position.x - AllAgentsInfo[j].g.transform.position.x) + (AllAgentsInfo[i].g.transform.position.z - AllAgentsInfo[j].g.transform.position.z) * (AllAgentsInfo[i].g.transform.position.z - AllAgentsInfo[j].g.transform.position.z);
                        float evTemp2 = (tempGoal.y - AllAgentsInfo[i].g.transform.position.z) * (tempGoal.y - AllAgentsInfo[i].g.transform.position.z) + (tempGoal.x - AllAgentsInfo[i].g.transform.position.x) * (tempGoal.x - AllAgentsInfo[i].g.transform.position.x);
                        if ((evTmp < SearchRadi * SearchRadi && evTmp >= SearchRadiMin * SearchRadiMin))// || (evTmp < SearchRadiMin * SearchRadiMin && evTemp2 >= 20f*20f) )
                        {
                            float tempAtan;
                            float guideForce = -20.0f;
                            tempAtan = Mathf.Atan2(AllAgentsInfo[j].g.transform.position.z - AllAgentsInfo[i].g.transform.position.z, AllAgentsInfo[j].g.transform.position.x - AllAgentsInfo[i].g.transform.position.x);
                            AllAgentsInfo[i].g.GetComponent<Rigidbody>().AddForce(guideForce * Mathf.Cos(tempAtan), 0.0f, guideForce * Mathf.Sin(tempAtan));
                            AllAgentsInfo[j].g.GetComponent<Rigidbody>().AddForce(guideForce * Mathf.Cos(tempAtan + Mathf.PI), 0.0f, guideForce * Mathf.Sin(tempAtan + Mathf.PI));
                        }
                        else if (evTmp < SearchRadiMin * SearchRadiMin)
                        {

                            AllAgentsInfo[i].g.GetComponent<Renderer>().material.color = Color.white;

                            float tempAtan;
                            float guideForce = -30.0f;
                            tempAtan = Mathf.Atan2(AllAgentsInfo[j].g.transform.position.z - AllAgentsInfo[i].g.transform.position.z, AllAgentsInfo[j].g.transform.position.x - AllAgentsInfo[i].g.transform.position.x);
                            AllAgentsInfo[i].g.GetComponent<Rigidbody>().AddForce(guideForce * Mathf.Cos(tempAtan), 0.0f, guideForce * Mathf.Sin(tempAtan));
                            AllAgentsInfo[j].g.GetComponent<Rigidbody>().AddForce(guideForce * Mathf.Cos(tempAtan + Mathf.PI), 0.0f, guideForce * Mathf.Sin(tempAtan + Mathf.PI));
                        }
                    }
                }
            }
            if (neighbor >= 10)// && neighbor < 10)
            {
                float tempAtan;
                float guideForce = 30.0f;
                tempAtan = Mathf.Atan2(tempGoal.y - AllAgentsInfo[i].g.transform.position.z, tempGoal.x - AllAgentsInfo[i].g.transform.position.x);
                float evTemp2 = (tempGoal.y - AllAgentsInfo[i].g.transform.position.z) * (tempGoal.y - AllAgentsInfo[i].g.transform.position.z) + (tempGoal.x - AllAgentsInfo[i].g.transform.position.x) * (tempGoal.x - AllAgentsInfo[i].g.transform.position.x);
                if (evTemp2 > 50f * 50f)
                {
                    guideForce = 2.0f;
                }
                else if (evTemp2 < 20f * 20f && evTemp2 >= SearchRadiMin * SearchRadiMin)
                {
                    guideForce = 10.0f;
                }
                else if (evTemp2 < SearchRadiMin * SearchRadiMin)
                {
                    guideForce = 3.0f;
                }

                if (ballVelo >= 22f)
                {
                    if (guideForce >= 10f)
                        guideForce = 10f;
                    guideForce *= -0.05f;
                }


                AllAgentsInfo[i].g.GetComponent<Rigidbody>().AddForce((float)neighbor * guideForce * Mathf.Cos(tempAtan + Random.Range(-20f, 20f) / 180f), 0.0f, (float)neighbor * guideForce * Mathf.Sin(tempAtan + Random.Range(-20f, 20f) / 180f));

            }
            else  // 味方がいないと撤退する
            {
                float tempAtan;
                float guideForce;
                tempAtan = Mathf.Atan2(tempGoal.y - AllAgentsInfo[i].g.transform.position.z, tempGoal.x - AllAgentsInfo[i].g.transform.position.x);
                float evTemp2 = (tempGoal.y - AllAgentsInfo[i].g.transform.position.z) * (tempGoal.y - AllAgentsInfo[i].g.transform.position.z) + (tempGoal.x - AllAgentsInfo[i].g.transform.position.x) * (tempGoal.x - AllAgentsInfo[i].g.transform.position.x);
                
                if (evTemp2 < 50 * 50)
                {
                    guideForce = -30.0f * (50 * 50) / (evTemp2 + 0.5f);
                    AllAgentsInfo[i].g.GetComponent<Rigidbody>().AddForce(guideForce * Mathf.Cos(tempAtan), 0.0f, guideForce * Mathf.Sin(tempAtan));
                    //AllAgentsInfo[i].g.GetComponent<Renderer>().material.color = Color.grey;
                }

            }

            //表示の色
            float nodo = 1f;
            if(neighbor < 5)
                nodo = 1f / (float)((5 - neighbor)* (5 - neighbor));
 
            //Debug.Log(nodo);
            if (AllAgentsInfo[i].color == 0)
            {
                AllAgentsInfo[i].g.GetComponent<Renderer>().material.color = new Color32(255, 0, 0, (byte)(255 * nodo));
            }
            else
            {
                AllAgentsInfo[i].g.GetComponent<Renderer>().material.color = new Color32(0, 0, 255, (byte)(255 * nodo));
            }




            //ブラウン運動
            AllAgentsInfo[i].g.GetComponent<Rigidbody>().AddForce(20f * Mathf.Cos(Random.Range(0f, Mathf.PI)), 0.0f, 20f * Mathf.Sin(Random.Range(0f, Mathf.PI)));



            //ずっとボールのそばにいると死ぬ
            float evTemp3 = (tempGoal.y - AllAgentsInfo[i].g.transform.position.z) * (tempGoal.y - AllAgentsInfo[i].g.transform.position.z) + (tempGoal.x - AllAgentsInfo[i].g.transform.position.x) * (tempGoal.x - AllAgentsInfo[i].g.transform.position.x);
            AgentInfo tempInfo;
            if (evTemp3 <= DestRadiMin * DestRadiMin)
                tempInfo.BallSec = AllAgentsInfo[i].BallSec + Time.deltaTime * Random.Range(0.8f, 1.2f);
            else
                tempInfo.BallSec = 0.0f;
            //Debug.Log(i.ToString() + "|" + tempInfo.BallSec.ToString() + "|" + evTemp3.ToString());
            tempInfo.g = AllAgentsInfo[i].g;
            tempInfo.color = AllAgentsInfo[i].color;
            AllAgentsInfo[i] = tempInfo;

            if (AllAgentsInfo[i].BallSec >= 0.4f)
            {
                tempRemove.Add(i);
                Debug.Log("removing " + AllAgentsInfo[i].color.ToString());
            }
            //ずっとボールのそばにいると死ぬ

        }



        //死滅処理
        int[] toRelife = { 0, 0 };
        for (int i = 0; i < tempRemove.Count; i++)
        {
            Destroy(AllAgentsInfo[tempRemove[i]].g);
            toRelife[AllAgentsInfo[tempRemove[i]].color] = toRelife[AllAgentsInfo[tempRemove[i]].color] + 1;
            //Debug.Log("dead " + AllAgentsInfo[tempRemove[i]].color.ToString());

        }
        for (int i = 0; i < tempRemove.Count; i++)
        {
            AllAgentsInfo.RemoveAt(tempRemove[i]-i);

        }
        //死滅処理


        //復活処理
        //Debug.Log("kon" + toRelife[0].ToString() + "|" + toRelife[1].ToString());
        for (int i = 0; i < toRelife[0] + toRelife[1]; i++)
        {
            
            float xTemp, yTemp;
            if (i < toRelife[0])
            {
                xTemp = Random.Range(0f, MAPSIZE);
                yTemp = Random.Range(-MAPSIZE, MAPSIZE);
            }
            else
            {
                xTemp = Random.Range(-MAPSIZE, 0f);
                yTemp = Random.Range(-MAPSIZE, MAPSIZE);
            }


            GameObject g = Instantiate(agentObjPrefab, agentsObj.transform);
            g.transform.position = new Vector3(xTemp, 0.0f, yTemp);

            AgentInfo tempInfo;


            if (i < toRelife[0])
            {
                tempInfo.color = 0; //red
                g.GetComponent<Renderer>().material.color = new Color32(255, 0, 0, 0);
            }
            else
            {
                tempInfo.color = 1; // blue
                g.GetComponent<Renderer>().material.color = new Color32(0, 0, 255, 0);
            }
            tempInfo.g = g;
            tempInfo.BallSec = 0.0f;

            AllAgentsInfo.Add(tempInfo);
        }
        //復活処理


        if (tempRemove.Count > 0)  // 敵が多い場所に投げるようにしよう
        {
            if (BallEtc.theBallPos.y < 4.001f)
                isToThrow = true;
            /*for (int k = 0; k < 2; k++)
            {
                // ボールを相手の陣地へ
                if (true)//BallEtc.theBallPos.y < 4.1f)// && evTemp3 <= DestRadiMin * DestRadiMi))
                {
                    float tempAtanG;
                    float guideForceG = 5000f * (float)toRelife[k];
                    Debug.Log("shoot" + k.ToString() + "|" + guideForceG.ToString());
                    //tempAtanG = Mathf.Atan2(Goals[k].z - BallEtc.theBallPos.z, Goals[k].x - BallEtc.theBallPos.x);
                    tempAtanG = 0.1f * Random.Range(-Mathf.PI, Mathf.PI) + Mathf.Atan2(Goals[k].z - BallEtc.theBallPos.z, Goals[k].x - BallEtc.theBallPos.x);
                    Ball.GetComponent<Rigidbody>().AddForce(guideForceG * Mathf.Cos((tempAtanG)), 0.0f, guideForceG * Mathf.Sin((tempAtanG)));

                }

            }*/

        }




    }
}
