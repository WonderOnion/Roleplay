using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NetworkHandler : MonoBehaviour
{
    private Settings settings;
    public bool NetworkHandlerDebug = false;

    public string IP = "";
    public int Port = 0;
    public int BufferSize;
    public Lobby lobby;

    public bool ServerDebug = false;
    public Server server;
    public GameObject ServerPortOBJ;

    public bool ClientDebug = false;
    public Client client;
    public GameObject ClientIPOBJ;
    public GameObject ClientPortOBJ;

    public List<Thread> ThreadList = new List<Thread>();
    
    public void Start()
    {
        settings = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Settings>();
        server = gameObject.GetComponent<Server>();
        client = gameObject.GetComponent<Client>();
        lobby = gameObject.GetComponent<Lobby>();
    }


    public void CreateServer(string Temp)
    {
        Int32.TryParse(Temp, out Port);
        if (Port != 0)
            goto PortCheck;
        //vado a prendere la porta dall'input
        if (ServerPortOBJ.activeSelf)
            Int32.TryParse(ServerPortOBJ.GetComponent<TMP_InputField>().text, out Port);
        else
            settings.Error_Profiler("D001", 0, "stai cercando di avviare il server senza che l'input box sia raggiungibile (NetworkHandler => CreateServer())", 2);



        //controllo la genuinità della porta
        PortCheck:
        if (Port > 1023 && Port < 49152)
        {
            if (NetworkHandlerDebug) settings.Console_Write("Inizializzazione server, porta richiesta: " + Port);
        }
        else
        {
            settings.Error_Profiler("N001", 0, "Port Range error (NetworkHandler => CreateServer) Port: " + Port, 2);
            return;
        }

        try
        {
            server.Port = Port;
            server.lobby = lobby;
            server.action = new Actions();
            server.action.BufferSize = BufferSize;
            server.Host = server;
            Thread TempThr = new Thread(() => server.Run("nothing"));
            lock (ThreadList)
            {
                ThreadList.Add(TempThr);
                ThreadList[ThreadList.Count-1].Start();
                if (ThreadList[ThreadList.Count - 1].IsAlive)
                    settings.Console_Write("è vivo ;/");
                else
                    settings.Console_Write("è morto ;/////");
            }
            
        }
        catch(Exception e)
        {
            settings.Error_Profiler("N002", 0, "NetworkHandler: " + e, 2);
        }
    }


    public void KillThreads()
    {
        lock (ThreadList)
        {
            foreach (Thread T in ThreadList)
            {
                if (T.IsAlive)
                {
                    settings.Console_Write(T.Name + ": I'm alive");
                    T.Abort();
                }
                else
                {
                    settings.Console_Write(T.Name + ": I'm already dead");
                }
            }
        }
    }

}
