using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEtc : MonoBehaviour
{
    public static Vector3 theBallPos;
    public static Vector3 theBallVelo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        theBallPos = gameObject.transform.position;
        theBallVelo = gameObject.GetComponent<Rigidbody>().velocity;



        if (AgentControl.isToThrow)
        {
            AgentControl.isToThrow = false;
            float throwAngle1;
            float throwAngle2;
            float throwSpeed;
            throwAngle1 = -Random.Range(5f, 25f);
            throwAngle2 = 0;// Random.Range(-180f, 180f);

            if (theBallPos.x >= 0 && theBallPos.z >= 0)
            {
                Debug.Log("1");
                throwAngle2 = Random.Range(180f, 270f);
            }
            else if (theBallPos.x < 0 && theBallPos.z >= 0)
            {
                Debug.Log("2");
                throwAngle2 = Random.Range(90f, 180f);
            }
            else if (theBallPos.x < 0 && theBallPos.z < 0)
            {
                Debug.Log("3");
                throwAngle2 = Random.Range(0f, 90f);
            }
            else
            {
                Debug.Log("4");
                throwAngle2 = Random.Range(270f, 360f);

            }

            throwSpeed = 35.0f + Random.Range(-10f, 10f);
            transform.eulerAngles = new Vector3(throwAngle1, throwAngle2, 0.0f);
            Rigidbody rb = this.transform.GetComponent<Rigidbody>();
            rb.velocity = transform.forward * throwSpeed;

        }

    }
}
