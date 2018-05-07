using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

/*
Una volta richiamato questo file afvrà la funzione di creare ilò primo collegamento con il server e di rimanere in ascolto di eventuali messaggi da
parte del server.
Quando uno di questi messaggi arriva lo sgancia in un thread per essere subito pronto per il prossimo comando evitando il delay die secuzione
*/

public class Client : MonoBehaviour
{
    public int ConnectionTimeOut = 5;
    public bool Creato = false;
    public string ServerIP;
    public int Port;

    [HideInInspector]
    public bool D = false;
    [HideInInspector]
    public string Name;
    [HideInInspector]
    public Lobby lobby = null;
    [HideInInspector]
    public Actions action;
    [HideInInspector]
    public SendActions Send = new SendActions();
    [HideInInspector]
    public Socket Servente;
    [HideInInspector]
    public Settings settings;
    [HideInInspector]
    private bool Shutdown = false;
    [HideInInspector]
    public int BufferSize;



    public void Run()
    {
        try
        { 
        if (D) settings.Console_Write("Inizializzazione client");
        action.lobby = lobby;
        Send.lobby = lobby;
        Send.BufferSize = action.BufferSize;

        if (!Shutdown)
            Inizialize_Client();
        if (!Shutdown)
            FirstContact();
        if (!Shutdown)
            Comunicazione();
        }
        catch (ThreadAbortException e)
        {
            settings.Error_Profiler("N008", 0, "Error during the Client execution by SocketException." + e, 5);
        }


        if (D) settings.Console_Write("Client chiuso");
        Shutdown = false;
        Creato = false;
    }

    public void Shutdown_Client(string Caller)
    {
        //notifico dell'avvio di chiusura del client e imposto la variabile di controllo su true 
        if (D) settings.Console_Write(Caller + " call to close Client (" + Caller + " => Client => Shutdown_Client");
        Shutdown = true;

        //ora che tutte le istanze della classe sanno che l'applicazione si deve chiudere forzo la chiusura del socket
        try
        {
            Servente.Shutdown(SocketShutdown.Both);
        }
        //Tale eccezzione verrà generata sicuramente poichèho bloccato i canali di comunicazione
        catch (SocketException e)
        {
            if (D && !Shutdown) settings.Error_Profiler("N005", 0, "Errore nella chiusura del Client (" + Caller + " => Client => Shudown_Client):" + e, 2);
        }

        //ora che il socket è stato bloccato e ho chiuso tutte le comunicazioni in modo forzato provvedo a chiudere del tutto il socket
        Servente.Close();
        if (D) settings.Console_Write("Socket Client chiuso");
    }

    public void Inizialize_Client()
    {
        try
        {
            IPHostEntry ipHostInfo = Dns.Resolve(ServerIP); //risolvo l'indirizzo del server
            IPAddress ipAddress = ipHostInfo.AddressList[0]; //prendo l'indirizzo del server dalle varie informazioni
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, Port);


            //creo il socket
            Servente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);



            //mi collego all'endpoint
            if (D) settings.Console_Write("tentativo di connessione al server: " + ServerIP + ":" + Port);
            var result = Servente.BeginConnect(remoteEP, null, null);

            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(ConnectionTimeOut));

            Servente.NoDelay = true;

            if (!success)
            {
                settings.Console_Write("<color=\"red\">TimeOut Connessione verso il Server");
                Creato = false;
                return;
            }
        }
        catch (Exception e)
        {
            settings.Console_Write("<color=\"red\">Errore durante la connessione al server\n" + e);
            Creato = false;
            return;
        }
        if (D) settings.Console_Write("Connessione con il server eseguita: " + Servente.RemoteEndPoint.ToString());
    }

    void FirstContact()
    {
        string Mex;
        try
        {
            Send.Send_to_One(Name, "0#" + Name, Servente, "Errore nella comunicazione del nome");
            Mex = "";
            while (Mex.Length != 1)
                Mex = Send.Receive_by_one(Servente, Name);

            if (Int32.Parse(Mex) == 0)
            {
                Debug.LogError("Ti è stato negato l'accesso al server causa Nome o motivazione");
                Shutdown_Client("Client => FirstContact");
            }
        }
        catch (Exception e)
        {
            if (Shutdown)
            {
                if (D) settings.Console_Write("Client non più in comunicazione (Username)");
            }
            else
            {
                settings.Error_Profiler("N010",0,"Errore nel passaggio dell'username\n" + e,2);
                Shutdown_Client("Client => FirstContact");
            }
        }

        if (!Shutdown)
        {
            //richiesta di aggiornamento lobby
            try
            {
                List<int> IDlist = lobby.List_of_ID();
                List<int> OnlineList = new List<int>();
                for (int I = 0; I < IDlist.Count; I++)
                    OnlineList.Add(lobby.Check_Online_by_ID(IDlist[I]));
                Send.Send_to_One(Name, Send.Refresh_Lobby(IDlist, OnlineList), Servente, "Errore durante la richiesta di refresh della lobby");
            }
            catch (Exception e)
            {
                if (Shutdown)
                {
                    if (D) settings.Console_Write("Client non più in comunicazione (Aggiornamento Lobby)");
                }
                else
                {
                    settings.Error_Profiler("D001",0,"Errore nella richiesta di aggiornamento della lobby \n" + e,2);
                    Shutdown_Client("Client => FirstContact");
                }
            }
        }
    }

    void Comunicazione ()
    {
        try
        {
            string Mex;
            while (Creato)
            {
                try
                {
                    Mex = Send.Receive_by_one(Servente, "Client");

                    Actions TempActions = new Actions
                    {
                        User = Servente,
                        Ricevuto = Mex,
                        contesto = 0,
                        lobby = lobby,
                    };
                    Thread newThread = new Thread(new ParameterizedThreadStart(TempActions.Run));
                    newThread.Start(Servente);
                }
                catch (SocketException Se)
                {
                    Debug.LogError("Client: Socket Exception durante ascolto " + Se.ErrorCode);
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogError("Errore durante la ricezione: " + e);

                    return;
                }
            }
        }
        catch (Exception e)
        {
            if (Shutdown)
            {
                if (D) settings.Console_Write("Socket Client non più in accettazione");
            }
            else
            {
                settings.Error_Profiler("N011", 0, "Chiusura in corso (Client => Comunicazione)", 5);
                Shutdown_Client("Client => Comunicazione");
            }
        }
    }

    public bool ChiudiContatti()
    {

        try
        {
            if (D) Debug.Log("Tentativo di chiusura client : creato" + Creato);
            if (Creato)
            {
                Creato = false;
                Send.Send_to_One(Name,Send.Client_Player_Logout(), Servente, "Errore durante la comunicazione dell'offline");
                Servente.Shutdown(SocketShutdown.Both);
                Servente.Close();
            }
        }
        catch (SocketException Se)
        {
            Debug.LogError("SocketException Errore durante la chiusura del client " + Se.ErrorCode);
            if (!Send.Send_to_One(Name,Send.Lobby_Ping(), Servente, ""))
                Creato = false;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Errore durante chiusura socket Client " + e);
            return false;
        }
        if (D) Debug.Log("Chiusura connessioni client riuscita");
        return true;
    }
}