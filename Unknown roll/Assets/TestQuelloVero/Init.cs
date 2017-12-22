using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Init : MonoBehaviour {

    private static string IP = "79.10.254.193";
    private static string Port = "25565";
    private static Client Cliente = new Client();
    private static Server Host = new Server();
         //----------------------Server-------------------------
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 100),"diventa l'host"))
        {
            if (Host.Creato == false)
            {
                Host.Creato = true;
                try
                {
                    Host.Port = Int32.Parse(Port);
                }catch(Exception e) { Debug.LogError("Errore nella conversione della porta in INT\n" + e); Host.Creato = false; return; }
                Debug.Log("Controllo Porta eseguito");
                Thread Hos = new Thread(() => Host.Run());
                Hos.Start();
                Debug.Log("Thread Host creato");
            }
            else
            {
                Debug.LogError("Esiste già un thread del Server\nCreazione Server");
            }
        }


        //----------------------Client-------------------------

        if (GUI.Button(new Rect(120, 10, 100, 100), "Partecipa"))
        {
            if (Cliente.Creato == false)
            {
                Cliente.Creato = true;
                try
                {
                    Cliente.Port = Int32.Parse(Port);
                    Cliente.ServerIP = IP;
                }
                catch (Exception e) { Debug.LogError("Errore nella conversione della porta in INT\n" + e ); Cliente.Creato = false; return; }
                Debug.Log("Controllo porta e assegnazione IP eseguita");
                Thread Cli = new Thread(() => Cliente.Run());
                Cli.Start();
                Debug.Log("Thread Client creato");
            }
            else
            {
                Debug.LogError("Esiste Già un thread Cliente\nCreazione Client");
            }

        }


        //----------------------IP/Port-------------------------
        IP = GUI.TextField(new Rect(10, 120, 140, 30),IP,30);
        Port = GUI.TextField(new Rect(150, 120, 70, 30), Port, 30);
        GUI.Label(new Rect(10, 160, 210, 30), IP);
    }

}
