using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Server : MonoBehaviour {

    public int Port = 25565;
    public bool Creato = false;

    public void Run()
    {
        Thread.Sleep(2000);
        Debug.Log("Dormito 2 secondi");
        Creato = false;
    }
}
