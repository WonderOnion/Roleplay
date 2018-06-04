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

    public bool D = false;
    public string Name;
    public Actions action;
    public SendActions Send;
    public Lobby lobby;
    public Socket Servente;
    public Settings settings;
    private bool Shutdown = false;
    public int BufferSize;
    public Packets Pacchetto;


    private void Start()
    {
        action = new Actions();
        Send = new SendActions();
        Send.lobby = lobby;
        Pacchetto = new Packets();
        Pacchetto.Inizialize_Packet(D, false, settings, lobby,Name);
    }


    public void Run(object Usefull)
    {
        try
        { 
            if (D) settings.Console_Write("client Creato", false);



            if (!Shutdown)
                Inizialize_Client();
            if (!Shutdown)
                FirstContact();
            if (!Shutdown)
                Comunicazione();
        }
        catch (Exception e)
        {
            settings.Error_Profiler("N008", 0, "Error during the Client execution by SocketException." + e, 2, false);
        }


        if (D) settings.Console_Write("Client chiuso", false);
        Shutdown = false;
        Creato = false;
    }

    public void Inizialize_Client()
    {
        try
        {
            IPHostEntry ipHostInfo = Dns.Resolve(ServerIP); //risolvo l'indirizzo del server
            IPAddress ipAddress = ipHostInfo.AddressList[0]; //prendo l'indirizzo del server dalle varie informazioni
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, Port);


            if (D) settings.Console_Write("Client inizializzato", false);
            //creo il socket
            Servente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            //mi collego all'endpoint
            if (D) settings.Console_Write("tentativo di connessione al server: " + ServerIP + ":" + Port, false);
            var result = Servente.BeginConnect(remoteEP, null, null);

            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(ConnectionTimeOut));

            Servente.NoDelay = true;

            if (!success)
            {
                settings.Console_Write("<color=\"red\">TimeOut Connessione verso il Server", false);
                Creato = false;
                return;
            }
        }
        catch (Exception e)
        {
            settings.Console_Write("<color=\"red\">Errore durante la connessione al server\n" + e, false);
            Creato = false;
            return;
        }
        if (D) settings.Console_Write("Connessione con il server eseguita: " + ServerIP, false);
       
    }

    void FirstContact()
    {
        string Mex;



        //Primo Contatto
        Pacchetto.Destinatario = Servente;
        Pacchetto.Clear();


        try
        {
            Mex = Pacchetto.First_Contact(1, false).ToString();
            Pacchetto.Send_Packet("Client", "Errore durante invio first Contact");

            Pacchetto.Clear();
            Pacchetto.Receive_Packet("Client", 1);
            if (Pacchetto.Get_Header())
            {
                if (Pacchetto.Header == 0)
                {
                    Mex = Pacchetto.First_Contact(0, true).ToString();
                    switch(Mex)
                    {
                        case "3":
                            settings.Console_Write("Il server ha rifiutato la tua connessione causa codice", false);
                            Shutdown_Client("Client => FirstContact (ByServerRefuse)");
                            return;
                        case "4":
                            settings.Console_Write("Benvenuto", false);
                            break;
                        default:
                            settings.Error_Profiler("N015", 0, "(Client)Codice First contact errato", 3, false);
                            Shutdown_Client("Client => FirstContact (ByWrongBodyCode)");
                            return;
                    }
                }

            }
            else
            {
                settings.Error_Profiler("N014", 0, "(Client)Errore nel Header:" + Name + " > Test di invio", 3, false);
                Shutdown_Client("Client => FirstContact (ByError)");
            }
            if (!Pacchetto.Risultato)
            {
                settings.Error_Profiler("N011", 0, "(Client)Errore nell'invio del pacchetto:" + Name + " > Test di invio", 3, false);
                Shutdown_Client("Client => FirstContact (ByError)");
            }
        }
        catch (Exception e)
        {
            if (Shutdown)
            {
                if (D) settings.Console_Write("Client non più in comunicazione (Inizialize_Client)", false);
            }
            else
            {
                settings.Error_Profiler("N010", 0, "Errore nell'inizializzazione\n" + e, 2, false);
                Shutdown_Client("Client => FirstContact");
            }
        }



        return;


        //OLD

        try
        {
            Pacchetto.Set_Header(1, "Errore durante il primo contatto");
            Mex = "";

            Mex = Send.Receive_by_one(Servente,1, Name);

            settings.Console_Write("Messaggio ricevuto dal client: " + Mex,false);

            Shutdown_Client("Fine comunicazione");

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
                if (D) settings.Console_Write("Client non più in comunicazione (Username)", false);
            }
            else
            {
                settings.Error_Profiler("N010",0,"Errore nel passaggio dell'username\n" + e,2, false);
                Shutdown_Client("Client => FirstContact");
            }
        }

        if (!Shutdown)
        {
            //richiesta di aggiornamento lobby
            try
            {
                List<int> IDlist = gameObject.GetComponent<Lobby>().List_of_ID();
                List<int> OnlineList = new List<int>();
                for (int I = 0; I < IDlist.Count; I++)
                    OnlineList.Add(gameObject.GetComponent<Lobby>().Check_Online_by_ID(IDlist[I]));
                Send.Send_to_One(Name, Send.Refresh_Lobby(IDlist, OnlineList), Servente, "Errore durante la richiesta di refresh della lobby");
            }
            catch (Exception e)
            {
                if (Shutdown)
                {
                    if (D) settings.Console_Write("Client non più in comunicazione (Aggiornamento Lobby)", false);
                }
                else
                {
                    settings.Error_Profiler("D001",0,"Errore nella richiesta di aggiornamento della lobby \n" + e,2, false);
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
                    Mex = Send.Receive_by_one(Servente,1, "Client");

                    Actions TempActions = new Actions
                    {
                        User = Servente,
                        Ricevuto = Mex,
                        contesto = 0,
                        lobby = gameObject.GetComponent<Lobby>(),
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
                if (D) settings.Console_Write("Socket Client non più in accettazione", false);
            }
            else
            {
                settings.Error_Profiler("N011", 0, "Chiusura in corso (Client => Comunicazione)", 5, false);
                Shutdown_Client("Client => Comunicazione");
            }
        }
    }

    public void Shutdown_Client(string Caller)
    {
        //notifico dell'avvio di chiusura del client e imposto la variabile di controllo su true 
        if (D) settings.Console_Write(Caller + " call to close Client (" + Caller + " => Client => Shutdown_Client", false);
        Shutdown = true;

        //ora che tutte le istanze della classe sanno che l'applicazione si deve chiudere forzo la chiusura del socket
        try
        {
            //TODO Client: avvertire il server della disconnessione
            Servente.Shutdown(SocketShutdown.Both);
        }
        //Tale eccezzione verrà generata sicuramente poichèho bloccato i canali di comunicazione
        catch (SocketException e)
        {
            if (D && !Shutdown) settings.Error_Profiler("N005", 0, "Errore nella chiusura del Client (" + Caller + " => Client => Shudown_Client):" + e, 2, false);
        }

        //ora che il socket è stato bloccato e ho chiuso tutte le comunicazioni in modo forzato provvedo a chiudere del tutto il socket
        Servente.Close();
        if (D) settings.Console_Write("Socket Client chiuso", false);
    }
    
}