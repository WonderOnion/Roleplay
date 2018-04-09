using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBar_Animation : MonoBehaviour {
    public float Velocity;
    public int Dimention;
	void Update ()
    {
        if (gameObject.transform.position.y >= -(Dimention/2))
    		gameObject.transform.Translate(0, -Velocity, 0);
        else
            gameObject.transform.Translate(0, Dimention*2 - Velocity, 0);
    }
}
