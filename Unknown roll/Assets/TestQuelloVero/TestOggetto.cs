using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class TestOggetto : MonoBehaviour
{
    Lobby asd = new Lobby();

    private void Start()
    {
        GameObject network = GameObject.Find("Network");
        
        asd = network.GetComponent<Lobby>();
    }
    void Update ()
    {
		if (Input.GetKeyDown(KeyCode.W))
        {
            try
            {

                //asd.AggRim("Premuto W",false);
                Debug.Log("inserito nella lista");
            }
            catch (Exception e)
            {
                Debug.LogError("Errore nel caricamento dell'azione nella lista.1n" + e );
            }
        }
	}
}
