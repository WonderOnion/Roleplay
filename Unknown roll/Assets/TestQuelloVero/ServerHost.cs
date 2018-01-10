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
    public bool D = true;
    public bool Creato;
    public int Port;
    public Socket socket;
    public Lobby lobby;
    public Actions action = new Actions();


    public void Run()
    {
        SendActions Send = new SendActions();
        action.lobby = lobby;
        action.AsServer = true;
        Send.BufferSize = action.BufferSize;
        Send.lobby = lobby;

        try
        {
            Debug.Log("Inizializzazione Server");
            // COnverto l'IP
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, Port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(ip);
            socket.Listen(10);

            if (D == true) Debug.Log("Creazione server avvenuta con successo");
        }
        catch (Exception e) { Debug.Log("Errore durante la creazione del Server\n" + e); return; }

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
        SendActions Send = new SendActions();
        Socket client = (Socket)Temp;
        string Name = null;
        string Mex = null;
        IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
        client.NoDelay = true;                                              //imposto che invia sempre in base alla dimensione che riceve senza dover riempire il pacchetto
        client.ReceiveBufferSize = action.BufferSize;
        action.lobby = lobby;
        Send.BufferSize = action.BufferSize;
        Send.lobby = lobby;

        try
        {
            if (D == true) Debug.Log("Connesso con: " + clientep);
            
            //richiedo il motivo di connessione e nel caso l'username
            string[] ConnetionMotivation = Send.Receive_by_one(client, "Server" ).Split('#');
            switch (Int32.Parse(ConnetionMotivation[0]))          //controllo se si connette per inviare immagini
            {
                case 0:
                    break;
                case 1:
                TransferChannel();
                    return;
                default:
                    Send.Send_to_One("0", client, "Errore nella della motivazione di connessione");
                    return;
            }
            Name = ConnetionMotivation[1];
            if (!lobby.Check_Exist_by_Name(Name))
                lobby.Add_ServerPlayer(client, Name, 0);
            else
            {
                if (lobby.Check_Online_by_ID(lobby.Retrive_ID_by_Name(Name)) != 0)
                {
                    Debug.LogError("Esiste già un client connesso con il nome: " + Name + " chiusura connessione");

                    Send.Send_to_One("0", client, "Errore nella comunicazione dell'errato nome");
                    client.Close();
                    return;
                }
                else
                {
                    lobby.Set_Online_by_ID(lobby.Retrive_ID_by_Name(Name),true);
                    lobby.Set_User_by_ID(lobby.Retrive_ID_by_Name(Name),client);
                    Send.Server_Broadcast(Send.Player_Come_Online(lobby.Retrive_ID_by_Name(Name), true));
                }
            }


            //avverto che l'username è stato accettato
            Send.Send_to_One("1", client, "Errore nella comunicazione della genuinità del nome");

            //aggiorno tutti sul nuovo host connesso
            Send.Server_Broadcast(Send.Client_Player_Login_Inizialize(lobby.Retrive_ID_by_Name(Name),0,1,Name));
            while (true)
            {

                Mex = Send.Receive_by_one(client, "Server");

                if (D) Debug.Log("S_Ricevuto: " + Mex);
                Actions TempActions = new Actions
                {
                    User = client,
                    Ricevuto = Mex,
                    contesto = 0,
                    lobby = lobby,
                    AsServer = true,
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
            Send.Server_Broadcast(Send.Player_Come_Online(lobby.Retrive_ID_by_Name(Name),false));
            lobby.Set_Online_by_ID(lobby.Retrive_ID_by_Name(Name), false);
            client.Close();
            return;
        }
    }
    public void TransferChannel()
    {

    }
}


