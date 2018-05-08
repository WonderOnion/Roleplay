using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;
public class NetworkHandler : MonoBehaviour
{
    private Settings settings;
    public bool NetworkHandlerDebug = false;


    [HideInInspector] public string IP = "";      //Ip a cui si collega
    [HideInInspector] public int Port = 0;        //Porta in cui hosta oppure Porta della persona a cui si collega
    [HideInInspector] public Lobby lobby;
    [HideInInspector] public Server server;
    [HideInInspector] public Client client;

    public int BufferSize;                          //dimensione del buffer d'invio


    public bool ServerDebug = false;                //alla creazione del server la variabile prenderà questo valore
    [SerializeField]private GameObject ServerPortOBJ;

    public bool ClientDebug = false;
    public string ClientName;
    [SerializeField] private GameObject ClientIPOBJ;
    [SerializeField] private GameObject ClientPortOBJ;
    
    [SerializeField] public List<Thread> ThreadList = new List<Thread>();


    public void Start()
    {
        settings = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Settings>();
        server = gameObject.GetComponent<Server>();
        client = gameObject.GetComponent<Client>();
        lobby = gameObject.GetComponent<Lobby>();
    }


    public void Create_Server(string Temp)
    {

        /*     La creazione del server avviene in due fasi:
               1) Creazione del server
               2) Creazione del Client con collegamento al server
        */



        // Creazione Server

        //controllo se la richiesta avviene da console o da menu
        Int32.TryParse(Temp, out Port);
        if (Port != 0)
            goto PortCheck;         // se avviene da console vado dierttamente a portCheck, altrimenti prendo i dati che mi interessano
        //vado a prendere la porta dall'input
        if (ServerPortOBJ.activeSelf)
            Int32.TryParse(ServerPortOBJ.GetComponent<TMP_InputField>().text, out Port);
        else
            settings.Error_Profiler("D001", 0, "stai cercando di avviare il server senza che l'input box sia raggiungibile (NetworkHandler => CreateServer())", 2, false);

        


        //controllo la genuinità della porta
        PortCheck:
        if (Port > 1023 && Port < 49152)
        {
            if (NetworkHandlerDebug) settings.Console_Write("Inizializzazione server, porta richiesta: " + Port, true);
        }
        else
        {
            settings.Error_Profiler("N001", 0, "Port Range error (NetworkHandler => CreateServer) Port: " + Port, 2, true);
            return;
        }

        //COntrollo se il server è già stato creato
        lock (server)
        {
            if (server.Creato)
            {
                //nel caso sia già in esecuzione creo un errore e faccio return
                settings.Error_Profiler("N003", 0, "(NetworkHandler => CreateServer)", 2, true);
                return;
                
            }
            else
                server.Creato = true;   
        }

        GameObject.FindGameObjectWithTag("Canvas").GetComponent<MenuHandler>().CallPopUPByName("PopUpDirectHost,2");

        //Server non creato, creazione in corso
        try
        {
            server.Port = Port;
            server.lobby = lobby;
            server.Host = server;
            server.settings = settings;
            server.Buffersize = BufferSize;
            server.D = ServerDebug;

            Thread TempThr = new Thread(() => server.Run("nothing"));
            TempThr.Name = "Server";
            lock (ThreadList)
            {
                ThreadList.Add(TempThr);
                ThreadList[ThreadList.Count-1].Start();
                if (ServerDebug)
                {
                    if (ThreadList[ThreadList.Count - 1].IsAlive && ThreadList[ThreadList.Count - 1].Name.Equals("Server"))
                        settings.Console_Write("Thread Server Avviato e vivo", true);
                    else
                        settings.Console_Write("Thread Server Avviato e morto (NetworkHandler => CreateServer)", true);
                }
            }
        }
        catch(Exception e)
        {
            settings.Error_Profiler("N002", 0, "NetworkHandler: " + e, 2, true);
        }



        //Creazione Client account Locale
        Create_Client("127.0.0.1:" + Port);
        
    }

    public void Create_Client(string Temp)
    {
        if (Temp.Split(':').Length == 2)
        {
            IP = Temp.Split(':')[0];
            Int32.TryParse(Temp.Split(':')[1], out Port);
            if (Port != 0)
                goto PortCheck;         // se avviene da console vado dierttamente a portCheck, altrimenti prendo i dati che mi interessano
        }

        //vado a prendere la porta dall'input
        if (ClientPortOBJ.activeSelf)
            Int32.TryParse(ClientPortOBJ.GetComponent<TMP_InputField>().text, out Port);
        else
            settings.Error_Profiler("D001", 0, "stai cercando di avviare il Client senza che l'input box sia raggiungibile (NetworkHandler => CreateCLient => Port)", 2, true);

        //vado a prendere l'IP dall'input
        if (ClientIPOBJ.activeSelf )
            IP = ClientIPOBJ.GetComponent<TMP_InputField>().text;
        else
            settings.Error_Profiler("D001", 0, "stai cercando di avviare il Client senza che l'input box sia raggiungibile (NetworkHandler => CreateCLient => IP)", 2, true);



        //controllo la genuinità della porta
        PortCheck:
        if (!(Port > 1023) && !(Port < 49152))
        {
            settings.Error_Profiler("N001", 0, "Port Range error (NetworkHandler => CreateClient) IP: " + Temp,2, true);
            return;
        }

        //controllo Genuinità IP
        if (IP.Length > 2)
        {
            if (NetworkHandlerDebug) settings.Console_Write("Inizializzazione Client, IP richiesto: " + IP + ":" + Port, true);
        }
        else
        {
            settings.Error_Profiler("N007", 0, "IP error (NetworkHandler => CreateClient) IP: " + Temp, 2, true);
            return;
        }


        //COntrollo se il Client è già stato creato
        lock (client)
        {
            if (client.Creato)
            {
                //nel caso sia già in esecuzione creo un errore e faccio return
                settings.Error_Profiler("N006", 0, "(NetworkHandler => CreateCLient)", 2, true);
                return;

            }
            else
                client.Creato = true;
        }



        GameObject.FindGameObjectWithTag("Canvas").GetComponent<MenuHandler>().CallPopUPByName("PopUpDirectHost,2");
        GameObject.FindGameObjectWithTag("Canvas").GetComponent<MenuHandler>().SwitchMenu("All,Lobby");


        //Client non creato, creazione in corso
        try
        {
            client.D = ClientDebug;
            client.Port = Port;
            client.ServerIP = IP;
            client.Name = ClientName;
            client.BufferSize = BufferSize;




            Thread ThreadClient = new Thread(() => client.Run("nothing"));
            ThreadClient.Name = "Client";

            lock (ThreadList)
            {
                ThreadList.Add(ThreadClient);
                ThreadList[ThreadList.Count - 1].Start();
                if (ClientDebug)
                {
                    if (ThreadList[ThreadList.Count - 1].IsAlive && ThreadList[ThreadList.Count - 1].Name.Equals("Client"))
                        settings.Console_Write("Thread Client Avviato e vivo", true);
                    else
                    {
                        settings.Console_Write("<color=\"red\">Thread Client Avviato e morto (NetworkHandler => CreateClient)", true);
                        client.Shutdown_Client("NetworkHandler => CreateClient");
                    }
                }
            }
        }
        catch (Exception e)
        {
            settings.Error_Profiler("N004", 0, "NetworkHandler: " + e, 2, true);
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
                    settings.Console_Write(T.Name + ": I'm alive", true);
                    switch(T.Name)
                    {
                        case "Server":
                            server.Shutdown_Server("NetworkHandler");
                            break;
                        case "Client":
                            client.Shutdown_Client("NetworkHandler");
                            break;
                        default:
                            settings.Error_Profiler("D001", 0, "Tentando di chiudere una sezione di network non profilata (NetworkHandler => KillThreads) (Nome:" + T.Name + ")", 2, true);
                            break;
                    }
                }
                else
                {
                    settings.Console_Write(T.Name + ": I'm already dead", true);
                }
            }

            int I = 0;
            while (I < ThreadList.Count)
            {
                if (!ThreadList[I].IsAlive)
                    ThreadList.RemoveAt(I);
                else
                    I++;
            }

            settings.Console_Write("Sono rimasti: " + ThreadList.Count + " Elementi attivi.", true);
        }
    }

}

