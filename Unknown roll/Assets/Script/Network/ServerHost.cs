using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

/*
Una volta richiamata si occuperà di creare un server sulla macchina del richiedente e successivamente si occuperà di capire la motivazione di collegamento
e di servire tale richiesta.
inoltre i singoli socket, sganciati su differenti thread, controllano il corretto funzionamento della lobby avvertendo se una persona si collega o scollega.
*/

public class ServerHost : MonoBehaviour
{
    public bool D = false;
    public bool Creato;
    public int Port;
    public Socket socket;
    public Lobby lobby;
    public ServerHost Host;
    public SendActions Send;
    public Actions action = new Actions();
    public List<Socket> SocketList = new List<Socket>();


    public void Run()
    {
        action.lobby = lobby;
        action.AsServer = true;
        SendActions Send = new SendActions();
        Send.lobby = lobby;
        Send.AsServer = true;
        Send.BufferSize = action.BufferSize;

        Inizialize_Server();
        Accepting_Client();

    }

    private void Inizialize_Server()
    {
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
    }

    private void Accepting_Client()
    {
        while (Creato)
        {
            
            if (D == true) Console.WriteLine("In attesa di Client");
            Socket client = socket.Accept();
            ConnectedClient NewClient = new ConnectedClient()
            {
                D = D,
                lobby = lobby,
                Host = Host
            };
            Thread newThread = new Thread(new ParameterizedThreadStart(NewClient.Run));

            newThread.Start(client);
            newThread.Start(client);
        }
    }

    public void ChiudiContatti()
    {
        Creato = false;
        try
        {
            socket.Shutdown(SocketShutdown.Both);
            lock (SocketList)
            {
                foreach (Socket T in SocketList)
                {
                    T.Shutdown(SocketShutdown.Both);
                    T.Close();
                }
                SocketList.Clear();
            }
            lobby.Clear_lobby();
            socket.Close();
        }
        catch (SocketException Se)
        {
            Debug.LogError("Socket exception: " + Se.ErrorCode);
        }
        catch (Exception e)
        {
            Debug.LogError("errore durante chiusura connessioni server: " + e);
        }
    }

}

public class ConnectedClient : MonoBehaviour
{
    private SendActions Send = new SendActions();
    private Actions action = new Actions();
    public ServerHost Host;
    public Lobby lobby;
    private IPEndPoint clientep;
    private string Name = null;
    private Socket client;
    public bool D = false;

    public void Run(object Temp)
    {
        client = (Socket)Temp;
        clientep = (IPEndPoint)client.RemoteEndPoint;
        client.NoDelay = true;                                              //imposto che invia sempre in base alla dimensione che riceve senza dover riempire il pacchetto
        client.ReceiveBufferSize = action.BufferSize;
        action.lobby = lobby;
        Send.BufferSize = action.BufferSize;
        Send.lobby = lobby;

        NewClient();
    }

    public void NewClient()
    {
        string Mex = null;
        try
        {
            if (D == true) Debug.Log("Connesso con: " + clientep);

            //ricevo il motivo di connessione e nel caso l'username
            string[] ConnetionMotivation = Send.Receive_by_one(client, "Server").Split('#');
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

            Client_LogIn(client, Name);

            while (Host.Creato)
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
            lobby.Set_Online_by_ID(lobby.Retrive_ID_by_Name(Name), false);
            client.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("Il socket " + clientep + " si è disconnesso a causa di un errore\n" + e);
            lobby.Set_Online_by_ID(lobby.Retrive_ID_by_Name(Name), false);
            Send.Server_Broadcast(Send.Player_Come_Online(lobby.Retrive_ID_by_Name(Name), false));
            client.Close();
            return;
        }
    }

    private void Client_LogIn(Socket client, string Name)
    {
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
                lobby.Set_Online_by_ID(lobby.Retrive_ID_by_Name(Name), true);
                lobby.Set_User_by_ID(lobby.Retrive_ID_by_Name(Name), client);
                Send.Server_Broadcast(Send.Player_Come_Online(lobby.Retrive_ID_by_Name(Name), true));
            }
        }


        //avverto che l'username è stato accettato
        Send.Send_to_One("1", client, "Errore nella comunicazione della genuinità del nome");

        //aggiorno tutti sul nuovo host connesso
        Send.Server_Broadcast(Send.Client_Player_Login_Inizialize(lobby.Retrive_ID_by_Name(Name), 0, 1, Name));
    }

    public void TransferChannel()
    {

    }
}


