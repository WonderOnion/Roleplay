using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    public void SwitchMenu (string Temp)
    {
        string Actual = Temp.Split(',')[0];
        string NewOne = Temp.Split(',')[1];
        Debug.Log(Actual + " -> " + NewOne);
    }
}