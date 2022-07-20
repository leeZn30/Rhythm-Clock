using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HourHandMoving : MonoBehaviour
{
    [SerializeField] private StageSettings stageSettings;
    private static float BPM = 0;
    private static int CPB = 0;
    private static bool isStart = false;
    private static float RotateSpeed = 0;
    void Start()
    {
        SetBPM();
        print(BPM);
    }
    
    void Update()
    {
        if (isStart)
        {
            transform.Rotate(Vector3.back, RotateSpeed * Time.deltaTime);
        }
    }
    
    public void SetBPM()
    {
        BPM = stageSettings.BPM;
        CPB = stageSettings.CPB;
        RotateSpeed = BPM * 6 / CPB; 
    }

    public void HourHandStart()
    {
        isStart = true;
    }
    
    public void HourHandStop()
    {
        isStart = false;
    }
}