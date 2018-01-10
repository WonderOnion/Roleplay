using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Client : MonoBehaviour
{
    public bool D = true;
    public int ConnectionTimeOut = 5;
    public bool Creato = false;
    public string ServerIP;
    public int Port = 25565;
    public string Name;
    public Lobby lobby = null;
    public Actions action;
    public Socket Servente;

    public void Run()
    {
        string Mex = null;
        Creato = true;
        action.lobby = lobby;
        SendActions Send = new SendActions();
        Send.BufferSize = action.BufferSize;
        Send.lobby = lobby;

        try
        {
            if (D)Debug.Log("Inizializzazione client");
            IPHostEntry ipHostInfo = Dns.Resolve(ServerIP); //risolvo l'indirizzo del server
            IPAddress ipAddress = ipHostInfo.AddressList[0]; //prendo l'indirizzo del server dalle varie informazioni
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, Port);

            //creo il socket
            Servente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //mi collego all'endpoint
            if (D) Debug.Log( "tentativo di connessione al server: " + ServerIP + ":" + Port);
            var result = Servente.BeginConnect(remoteEP, null, null);

            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(ConnectionTimeOut));

            Servente.NoDelay = true;

            if (!success)
            {
                Debug.LogError("TimeOut Connessione verso il Server");
                Creato = false;
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Errore durante la connessione al server\n" + e);
            Creato = false;
            return;
        }
        Debug.Log("Connessione con il server eseguita: "+ Servente.RemoteEndPoint.ToString());
        try
        {
            if (D) Debug.Log("Invio credenziali: " + Name);
            
            Send.Send_to_One("0#" + Name, Servente, "Errore nella comunicazione del nome");
            Mex = Send.Receive_by_one(Servente, "Client");
            Debug.Log("C_riceve: " + Mex);
            if (Int32.Parse(Mex) == 0)
            {
                Debug.LogError("Ti è stato negato l'accesso al server causa Nome o motivazione");
                Creato = false;
                Servente.Close();
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Errore nel passaggio dell'username\n" + e);
            Creato = false;
            Servente.Close();
            return;
        }


        //richiesta di aggiornamento lobby
        try
        {
            List<int> IDlist = lobby.List_of_ID();
            List<int> OnlineList = new List<int>();
            for (int I = 0; I < IDlist.Count; I++)
                OnlineList.Add(lobby.Check_Online_by_ID(IDlist[I]));
            Send.Send_to_One(Send.Refresh_Lobby(IDlist, OnlineList), Servente, "Errore durante la richiesta di refresh della lobby");
        }
        catch (Exception e)
        {
            Debug.LogError("Errore nella richiesta di aggiornamento della lobby \n" + e);
        }
        while (true)
        {
            try
            {
                Mex = Send.Receive_by_one(Servente, "Client");
                if (D) Debug.Log("C_Ricevuto: " + Mex);
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
            catch (Exception e)
            {
                Debug.LogError("Errore durante la ricezione: " + e);
            }
        }
    }

    
}