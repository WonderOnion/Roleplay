using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBar_Animation : MonoBehaviour {
    public float Velocity;
    public int Dimention;
    public int Elements;
	void Update ()
    {
        if (gameObject.transform.position.y >= -(Dimention))
    		gameObject.transform.Translate(0, -Velocity, 0);
        else
            gameObject.transform.Translate(0, Dimention * Elements - Velocity, 0);
    }
}
