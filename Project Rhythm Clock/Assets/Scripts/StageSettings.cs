using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSettings : MonoBehaviour
{
    [SerializeField] public float BPM = 120;
    [Header("Cycle Per Bit")]
    [SerializeField] public int CPB = 8;
    [Header("Cycle Per Seconds")]
    [SerializeField] public float CPS;

    public float StartTime = -1.0f;
    private void Awake()   
    {
        CPS = 60 * CPB / BPM;
    }

    public void ChangeBPM(float BPM)
    {
        this.BPM = BPM;
        CPS = 60 * CPB / BPM;
    }
}
