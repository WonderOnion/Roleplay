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
    public bool D = false;
    public string ServerIP;
    public int Port = 25565;
    public string Name;
    public bool Creato = false;
    public int ConnectionTimeOut = 5;
    public Lobby lobby;
    public Actions action = new Actions();
    public Socket client;

    public void Run()
    {
        Creato = true;
        string Mex = null;
        action.lobby = lobby;

        try
        {
            if (D)Debug.Log("Inizializzazione client");
            IPHostEntry ipHostInfo = Dns.Resolve(ServerIP); //risolvo l'indirizzo del server
            IPAddress ipAddress = ipHostInfo.AddressList[0]; //prendo l'indirizzo del server dalle varie informazioni
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, Port);

            //creo il socket
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //mi collego all'endpoint
            if (D) Debug.Log( "tentativo di connessione al server: " + ServerIP + ":" + Port);
            var result = client.BeginConnect(remoteEP, null, null);

            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(ConnectionTimeOut));

            client.NoDelay = true;

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
        Debug.Log("Connessione con il server eseguita: "+ client.RemoteEndPoint.ToString());
        try
        {
            if (D) Debug.Log("Invio credenziali: " + Name);

            action.Send_to_One(Name, client, "Errore nella comunicazione del nome");
            Mex = action.Receive_by_one(client, "Client");
            Debug.Log("C_riceve: " + Mex);
            if (Int32.Parse(Mex) == 0)
            {
                Debug.LogError("esiste già un client connesso con il nome: " + Name);
                Creato = false;
                client.Close();
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Errore nel passaggio dell'username\n" + e);
            Creato = false;
            client.Close();
            return;
        }


        //richiesta di aggiornamento lobby
        try
        {
            Mex = "RefL";
            List<int> IDlist = lobby.List_of_ID();
            for (int I = 0; I < IDlist.Count; I++)
                Mex = Mex + IDlist[I] + "#";
            if (IDlist.Count == 0)
                Mex = Mex + "#";
            action.Send_to_One(Mex,client,"Errore nella comunicazione della richiesta di aggiornamento lobby");
        }
        catch (Exception e)
        {
            Debug.LogError("Errore nella richiesta di aggiornamento della lobby");
        }
        while (true)
        {
            //bytesRec = client.Receive(data);
            //Mex = Encoding.ASCII.GetString(data,0, bytesRec);
            Mex = action.Receive_by_one(client, "Client");
            if (D) Debug.Log("C_Ricevuto: " + Mex);
            Actions TempActions = new Actions
            {
                User = client,
                Ricevuto = Mex,
                contesto = 0,
                lobby = lobby,
            };
            Thread newThread = new Thread(new ParameterizedThreadStart(TempActions.Run));
            newThread.Start(client);
        }
    }

    
}