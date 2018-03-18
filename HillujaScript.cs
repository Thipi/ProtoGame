using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HillujaScript : MonoBehaviour {

    public Transform rainPosition;
    public Transform snowPosition;
    public Transform hRainPosition;
    public Transform sandStorm;

    public GameObject Snow;
    public GameObject Rain;
    public GameObject HeavyRain;
    public GameObject SandStorm;

    public float minTime = 2;
    public float maxTime = 15;

    float timer;
    float spawnTime;


    public bool isRaining = false;
    public bool isSnowing = false;
    public bool isSandStorm = false;
    public bool isHeavyRaining = false;
    public bool ExitArea1 = false;
    public bool ExitArea2 = false;
    public bool ExitArea3 = false;
    public bool ExitArea4 = false;

    public Object RainClone { get; private set; }



    // Use this for initialization
    void Start() {
        SetRandomTime();
        timer = minTime;
       
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BasicForest")
        {
            isRaining = true;

        }

        else if (other.tag == "SnowLand")
        {
            isSnowing = true;
        }

        else if (other.tag == "Desert")
        {
            isSandStorm = true;

        }

        else if (other.tag == "Jungle")
        {
            isHeavyRaining = true;

        }
        else return;
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (isRaining && timer >= spawnTime)
        {
            timer = 0;
            GameObject RainClone = Instantiate(Rain, rainPosition.position, rainPosition.rotation) as GameObject;
        }


        else if (sandStorm && timer >= spawnTime)
        {
            timer = 0;
            GameObject SandStormClone = Instantiate(SandStorm, sandStorm.position, sandStorm.rotation) as GameObject;
        }

        else if (isHeavyRaining && timer >= spawnTime)
        {
            timer = 0;
            GameObject HeavyRainClone = Instantiate(HeavyRain, hRainPosition.position, hRainPosition.rotation) as GameObject;
        }

        else if (isSnowing && timer >= spawnTime)
        {
            timer = 0;
            GameObject SnowClone = Instantiate(Snow, snowPosition.position, snowPosition.rotation) as GameObject;
        }
        else return;
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "BasicForest")
        {
            isRaining = false;
            ExitArea1 = true;

        }
        if (other.tag == "Desert")
        {
            isSandStorm = false;
            ExitArea2 = true;
        }
        if (other.tag == "Jungle")
        {
            isHeavyRaining = false;
            ExitArea3 = true;
        }
        if (other.tag == "SnowLand")
        {
            isSnowing = false;
            ExitArea4 = true;
        }
    }

    void Update()
    {

        if (ExitArea1)
        {
            DestroyImmediate(Rain, true);
            ExitArea1 = false;
        }
        if (ExitArea2)
        {
            DestroyImmediate(SandStorm, true);
            ExitArea2 = false;
        }
        if (ExitArea3)
        {
            DestroyImmediate(HeavyRain, true);
            ExitArea3 = false;
        }
        if (ExitArea4)
        {
            DestroyImmediate(Snow, true);
            ExitArea4 = false;
        }


    }


   

    void SetRandomTime()
    {
        spawnTime = Random.Range(minTime, maxTime);
    }

}
