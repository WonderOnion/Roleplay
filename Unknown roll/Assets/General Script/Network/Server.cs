﻿using System.Collections;
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

    

public class Server : MonoBehaviour
{
    public bool Creato;
    public int Port;


    [HideInInspector] public bool D = false;
    public Settings settings;
    public Socket socket;
    public Lobby lobby;
    public Server Host;
    public List<Socket> SocketList = new List<Socket>();
    public int Buffersize;
    

    public bool Shutdown = false;




    public void Run(object Usefull)
    {

        try
        {
            //Avverto che il server si sta inizializzando e imposto tutte le varibili essenziali alle nuove classi che ha generato
            if (D) settings.Console_Write("Server Inizializated", false);
            
            SendActions Send = new SendActions();
            Send.lobby = lobby;
            Send.AsServer = true;
            Send.BufferSize = Buffersize;

            

            Inizialize_Server();
            Accepting_Client();
        }
        catch (ThreadAbortException e)
        {
            settings.Error_Profiler("N009", 0, "Error during the server execution by SocketException." + e, 5, false);
        }



        if (D) settings.Console_Write("Server chiuso, Inizializzazione chiusura " + SocketList.Count + " Client", false);
        try
        {
            Close_All_ConnectedClient();
            lobby.Clear_lobby();    //TODO Gestire Pulizia lobby
        }
        catch (Exception e)
        {
            settings.Error_Profiler("D001", 0, "errore nella chiusura client;    Server => Run (Shutdown client): " + e, 2, false);
        }


        while (SocketList.Count > 0)
        { }
        Shutdown = false;
        Creato = false;
    }

    public void Shutdown_Server(string Caller)
    {
        //notifico dell'avvio di chiusura del server e imposto la variabile di controllo su true 
        if (D) settings.Console_Write(Caller + " call to close server (" + Caller + " => Server => Shutdown_Server", false);
        Shutdown = true;

        //ora che tutte le istanze della classe sanno che l'applicazione si deve chiudere forzo la chiusura del socket
        try
        {
            socket.Shutdown(SocketShutdown.Both);
        }
        //Tale eccezzione verrà generata sicuramente poichèho bloccato i canali di comunicazione
        catch (SocketException e)
        {
            if (D && !Shutdown) settings.Error_Profiler("D001",0,"Errore nella chiusura del server (" + Caller + " => Server => Server_Shutdown):" + e,2, false);
        }

        //ora che il socket è stato bloccato e ho chiuso tutte le comunicazioni in modo forzato provvedo a chiudere del tutto il socket
        socket.Close();
        if (D) settings.Console_Write("Socket Server chiuso", false);
    }

    private void Inizialize_Server()
    {
        try
        {
            if (D) settings.Console_Write("Inizializzazione Server", false);
            // Converto l'IP
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, Port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(ip);
            socket.Listen(10);

            if (D) settings.Console_Write("Creazione server avvenuta con successo", false);
        }
        catch (Exception e) { settings.Error_Profiler("D001",0,"Errore durante la creazione del Server\n" + e,5, false); return; }
    }

    private void Accepting_Client()
    {
        try
        {
            int I = 0;
            while (Creato || !Shutdown)
            {
                if (D) settings.Console_Write("In attesa di Client, attualmente processate " + I + " connessioni ", false);
                I++;
                Socket client = socket.Accept();

                Thread newThread = new Thread(SubThr);
                
                newThread.Start(client);
            }
        }
        catch(Exception e)
        {
            if (Shutdown)
            {
                if (D) settings.Console_Write("Socket Server non più in accettazione", false);
            }
            else
            {
                settings.Error_Profiler("N011", 0, "Chiusura in corso (Server => Accepting_Client)" + e, 2, false);
                Shutdown_Server("Server => Accepting_Client");
            }
        }
    }

    public void Close_Single_ConnectedClient(Socket ClosingClient)
    {
        if (D) settings.Console_Write("Chiusura connessione con " + (IPEndPoint)ClosingClient.RemoteEndPoint, false);
        try
        {
            ClosingClient.Shutdown(SocketShutdown.Both);
            lock (SocketList)
            {
                SocketList.Remove(ClosingClient);
            }
            ClosingClient.Close();
        }
        catch (Exception e)
        {
            settings.Error_Profiler("D001",0,"Impossibile chiudere la connessione con " + (IPEndPoint)ClosingClient.RemoteEndPoint + "  (Server => Close_Single)",2, false);
        }
    }
    
    public void Close_All_ConnectedClient ()
    {
        while (SocketList.Count != 0)
        {
            Close_Single_ConnectedClient(SocketList[0]);
        }
    }

    
    
    public void SubThr(object TempSocket)
    {

        Actions action = new Actions();
        SendActions Send = new SendActions();
        action.AsServer = true;
        action.Send = Send;
        action.lobby = lobby;
        Send.lobby = lobby;
        Socket client = (Socket)TempSocket;
        SocketList.Add(client);

        string Name = null;                                                 //
        string Mex = null;                                                  //
        IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;            //IP del client
        client.NoDelay = true;                                              //imposto che invia sempre in base alla dimensione che riceve senza dover riempire il pacchetto
        client.ReceiveBufferSize = action.BufferSize;                       //imposto la dimensione massima del buffersize per action
        action.lobby = lobby;                                               
        Send.BufferSize = action.BufferSize;                                //Imposto la dimesione massima del buffersize per Send
        Send.lobby = lobby;

        Packets Pacchetto = new Packets();
        Pacchetto.Inizialize_Packet(D, true, settings, lobby, "Server(" + clientep.ToString() + ")");


        if (D) settings.Console_Write("Connesso con: " + clientep, false);




        // Primo contatto
        try
        {
            Pacchetto.Destinatario = client;
            Pacchetto.AsServer = true;
            Pacchetto.Clear();



            Pacchetto.Receive_Packet("Server", 1);
            Pacchetto.Get_Header();
            if (Pacchetto.Header == 0 && Pacchetto.Risultato)
            {
                int T = Pacchetto.Fetch_Header(Pacchetto.Header);
                Pacchetto.Receive_Packet("Server", T);
                if (!Pacchetto.Risultato)
                    settings.Console_Write("Server > Errore di comunicazione (FIrstCOntact).", false);
                T = Pacchetto.First_Contact(255, true);
                Mex = T.ToString();
            }
            else
            {
                settings.Console_Write("Server > Errore di comunicazione (FIrstCOntact). pt2", false);
            }
            Pacchetto.Clear();
            //Mex =  Send.Receive_by_one(client, 1, "Server");
            if (Mex.Equals("1"))
            {
                settings.Console_Write("Server > Client connesso per invio dati. " + Mex, false);
                Pacchetto.First_Contact(4, false);
                Pacchetto.Send_Packet("Server", "Errore durante la risposta al FirstCOntact per falso");
            }
            else
            {
                settings.Console_Write("Server > Client connesso come giocatore. " + Mex, false);
                Pacchetto.First_Contact(3, false);
                Pacchetto.Send_Packet("Server", "Errore durante la risposta al FirstCOntact per vero");
            }
            Close_Single_ConnectedClient(client);
            return;
        }
        catch (Exception e)
        {
            if (Shutdown)
            {
                if (D) settings.Console_Write("connessione con " + Name + "  (" + clientep.ToString() + ") chiusa.", false);
            }
            else
            {
                settings.Error_Profiler("N012",0,"(" + Name + " || " + clientep.ToString() + ") (Server => SubThr) : " + e,1, false);
                Send.Server_Broadcast(Send.Player_Come_Online(lobby.Retrive_ID_by_Name(Name), false));
                lobby.Set_Online_by_ID(lobby.Retrive_ID_by_Name(Name), false);
                Close_Single_ConnectedClient(client);
            }
        }




        // fine primo contatto


        if (!Shutdown)
        {
            try
            {
                if (D == true) settings.Console_Write("Connesso con: " + clientep, false);

                //richiedo il motivo di connessione e nel caso l'username
                string[] ConnetionMotivation = Send.Receive_by_one(client,1, "Server").Split('#');
                switch (Int32.Parse(ConnetionMotivation[0]))          //controllo se si connette per inviare immagini
                {
                    case 0:
                        break;
                    case 1:
                        TransferChannel();
                        return;
                    default:
                        Send.Send_to_One("Server", "0", client, "Errore nella della motivazione di connessione");
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

                        Send.Send_to_One("Server", "0", client, "Errore nella comunicazione dell'errato nome");
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
                Send.Send_to_One("Server", "1", client, "Errore nella comunicazione della genuinità del nome");

                //aggiorno tutti sul nuovo host connesso
                Send.Server_Broadcast(Send.Client_Player_Login_Inizialize(lobby.Retrive_ID_by_Name(Name), 0, 1, Name));
                while (true)
                {

                    Mex = Send.Receive_by_one(client,1, "Server");

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
                Send.Server_Broadcast(Send.Player_Come_Online(lobby.Retrive_ID_by_Name(Name), false));
                lobby.Set_Online_by_ID(lobby.Retrive_ID_by_Name(Name), false);
                client.Close();
                return;
            }
        }
    }

    public void TransferChannel()
    {
        //TODO Canale di trasfermento file
    }

}


/*
public class Server : MonoBehaviour
{
    public bool D = false;
    public bool Creato;
    public int Port;
    public Socket socket;
    public Lobby lobby;
    public Server Host;
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
            // Converto l'IP
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
        int I = 0;
        while (Creato)
        {
            if (D == true) Debug.Log("In attesa di Client, attualmente processate " + I + " connessioni ");
            I++;
            Socket client = socket.Accept();
            ConnectedClient ClientClass = new ConnectedClient()
            {
                D = D,
                lobby = lobby,
                Host = Host
            };
            Thread newThread = new Thread(ClientClass.Run);

            newThread.Start(client);
        }
    }

    public void Close_Single(Socket client)
    {
        if (D) Debug.Log("Chiusura connessione con " + (IPEndPoint)client.RemoteEndPoint);
        client.Shutdown(SocketShutdown.Both);
        lock (SocketList)
        {
            SocketList.Remove(client);
        }
        client.Close();

    }

    public void Close_Server()
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
    public Server Host;
    public Lobby lobby;
    private IPEndPoint clientep;
    private string Name = null;
    private Socket client;
    public bool D = true;

    public void Run(object Temp)
    {
        client = (Socket)Temp;
        clientep = (IPEndPoint)client.RemoteEndPoint;
        client.NoDelay = true;                                              //imposto che invia sempre in base alla dimensione che riceve senza dover riempire il pacchetto
        client.ReceiveBufferSize = action.BufferSize;
        action.lobby = lobby;
        Send.BufferSize = action.BufferSize;
        Send.lobby = lobby;


        try
        {
            if (D == true) Debug.Log("Connesso con: " + clientep);

            //ricevo il motivo di connessione e nel caso l'username
            string Mex = Send.Receive_by_one(client, "Server");
            string[] ConnetionMotivation = Mex.Split('#');

            switch (Int32.Parse(ConnetionMotivation[0]))          //controllo la motivazione di connessione (0) se è un utente (1) se è un socket dedito al trasferimento dei file
            {
                case 0:
                    Name = ConnetionMotivation[1];
                    NewClient();
                    break;
                case 1:
                    TransferChannel();
                    break;
                default:
                    Send.Send_to_One("Server", "0", client, "Errore nella della motivazione di connessione");
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Errore durante ricezione della motivazione di connessione da parte di " + clientep + "\n" + e);
            Host.Close_Single(client);
            return;
        }


        Host.Close_Single(client);
    }

    public void NewClient()
    {
        string Mex = null;
        try
        {
            Client_LogIn(client, Name);

            while (Host.Creato)
            {
                Mex = Send.Receive_by_one(client, "Server");

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
            Host.Close_Single(client);
            return;
        }
        catch (Exception e)
        {
            Debug.LogError("Il socket " + clientep + " si è disconnesso a causa di un errore\n" + e);
            lobby.Set_Online_by_ID(lobby.Retrive_ID_by_Name(Name), false);
            Send.Server_Broadcast(Send.Player_Come_Online(lobby.Retrive_ID_by_Name(Name), false));
            Host.Close_Single(client);
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

                Send.Send_to_One("Server", "0", client, "Errore nella comunicazione dell'errato nome");
                Host.Close_Single(client);
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
        Send.Send_to_One("Server", "1", client, "Errore nella comunicazione della genuinità del nome");

        //aggiorno tutti sul nuovo host connesso
        Send.Server_Broadcast(Send.Client_Player_Login_Inizialize(lobby.Retrive_ID_by_Name(Name), 0, 1, Name));
    }

    public void TransferChannel()
    {

    }
}

    */
