using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class ServerHost : MonoBehaviour
{
    public bool D = false;
    public int Port;
    public bool Creato;
    public Socket socket;
    public Lobby lobby;
    public Actions action = new Actions();

    public void Run()
    {
        action.lobby = lobby;
        try
        {
            // COnverto l'IP
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, Port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            socket.Bind(ip);
            socket.Listen(10);

            if (D==true)Debug.Log("Creazione server avvenuta con successo");
        }catch (Exception e) { Debug.Log("Errore durante la creazione del Server\n"  + e);return; }

        while (true)
        {
            if (D == true) Console.WriteLine("In attesa di Client");
            Socket client = socket.Accept();
            Thread newThread = new Thread(new ParameterizedThreadStart(SubThr));
            newThread.Start(client);
        }
    }

    public void SubThr(object Temp) 
    {
        
        Socket client = (Socket)Temp;
        string Name = null;
        string Mex = null;
        IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
        client.NoDelay = true;                                              //imposto che invia sempre in base alla dimensione che riceve senza dover riempire il pacchetto
        client.ReceiveBufferSize = action.BufferSize;
        action.lobby = lobby;

        try
        {
            if (D == true) Debug.Log("Connesso con: " + clientep);
            
            //ricevo l'username
            Name = action.Receive_by_one(client, "Server" );
            if (!lobby.Check_Exist_by_Name(Name))
                lobby.Add_ServerPlayer(client, Name, 0);
            else
            {
                if (lobby.Check_Online_by_ID(lobby.Retrive_ID_by_Name(Name)) == 1)
                {
                    Debug.LogError("Esiste già un client connesso con il nome: " + Name + " chiusura connessione");

                    action.Send_to_One("0", client, "Errore nella comunicazione dell'errato nome");
                    client.Close();
                    return;
                }
                else
                {
                    lobby.Set_Online_by_ID(lobby.Retrive_ID_by_Name(Name),true);
                    lobby.Set_User_by_ID(lobby.Retrive_ID_by_Name(Name),client);
                }
            }


            //avverto che l'username è stato accettato
            action.Send_to_One("1", client, "Errore nella comunicazione della genuinità del nome");

            //aggiorno tutti sul nuovo host connesso
            action.Server_Broadcast("NewU" + lobby.Retrive_ID_by_Name(Name) + "#" + 0 + "#" + Name);
            while (true)
            {

                Mex = action.Receive_by_one(client, "Server");

                if (D) Debug.Log("S_Ricevuto: " + Mex);
                Actions TempActions = new Actions
                {
                    User = client,
                    Ricevuto = Mex,
                    contesto = 0,
                    lobby = lobby,
                    AsServer = true
                };
                Thread newThread = new Thread(new ParameterizedThreadStart(TempActions.Run));
                newThread.Start(client);
            }
            if (D == true) Debug.Log("il Client si è disconnesso");
            lobby.Set_Online_by_ID(lobby.Retrive_ID_by_Name(Name),false);
            client.Close();
        }
        catch (Exception e)
        {   
            Debug.LogError("Il socket " + clientep + " si è disconnesso a causa di un errore\n" + e);
            action.Server_Broadcast("OffU" + lobby.Retrive_ID_by_Name(Name));
            lobby.Set_Online_by_ID(lobby.Retrive_ID_by_Name(Name), false);
            client.Close();
            return;
        }
    }

}


