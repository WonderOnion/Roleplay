using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

/*
Questo è un file temporaneo per l'interfaccia di testing con le funzioni essenziali in merito.
*/

public class Init : MonoBehaviour
{
    public static bool D = true;
    private static string IP = "79.10.254.193";
    private static string Port = "25565";
    public int BufferSize = 256;
    private static string Name = "";
    private string Slobby = "";
    private Lobby lobby;
    private SendActions Send;
    private List<int> Tlist;
    private static Client Cliente;
    private Create_Socket Crea = new Create_Socket {
        D = D,
    };

    private void Start()
    {
        Cliente = GameObject.Find("Network").GetComponent<Client>();
        lobby = GameObject.Find("Network").GetComponent<Lobby>();
        Send = GameObject.Find("Network").GetComponent<SendActions>();

    }

    void OnGUI()
    {
        //----------------------Server-------------------------
        if (GUI.Button(new Rect(10, 10, 100, 100),"diventa l'host"))
        {
            Crea.IP = IP;
            Crea.Port = Port;
            Crea.Name = Name;
            Crea.BufferSize = BufferSize;
            Crea.Create_Server();
        }


        //----------------------Client-------------------------

        if (GUI.Button(new Rect(120, 10, 100, 100), "Partecipa"))
        {
            Crea.IP = IP;
            Crea.Port = Port;
            Crea.Name = Name;
            Crea.Create_Client();
        }


        //----------------------IP/Port-------------------------
        IP = GUI.TextField(new Rect(10, 120, 140, 30),IP,30);
        Port = GUI.TextField(new Rect(150, 120, 70, 30), Port, 30);
        GUI.Label(new Rect(10, 160, 40, 30), "Nome:");
        Name = GUI.TextField(new Rect(50, 160, 150, 30), Name, 30);


        //----------------------lobby-------------------------
        GUI.Label(new Rect(300, 10, 400, 300), Slobby);
        if (GUI.Button(new Rect(660, 10, 100, 30), "Master"))
            Send.Send_to_One("Client",Send.Lobby_Change_Role(-1, 2), Cliente.Servente, "Errore nella comunicazione del nuovo potere");
        if(GUI.Button(new Rect(660, 50, 100, 30), "giocatore"))
            Send.Send_to_One("Client", Send.Lobby_Change_Role(-1, 1), Cliente.Servente, "Errore nella comunicazione del nuovo potere");
        if (GUI.Button(new Rect(660, 90, 100, 30), "spettatore"))
            Send.Send_to_One("Client", Send.Lobby_Change_Role(-1, 3), Cliente.Servente, "Errore nella comunicazione del nuovo potere");
        if (GUI.Button(new Rect(660, 130, 100, 30), "Vai Offline"))
            Crea.Close_Connection();
		
    }
    void Update()
    {
        if (!Cliente.Creato)
            return;
        Slobby = "";
        Tlist = lobby.List_of_ID();
        Slobby = Tlist.Count + "\n";
        for(int I = 0; I < Tlist.Count;I++)
        {
            Slobby = Slobby + Tlist[I] + " | " + lobby.Retrive_Name_by_ID(Tlist[I]) + "  ";
            switch (lobby.Retrive_Power_by_ID(Tlist[I]))
            {
                case 0:
                    Slobby = Slobby + "none";
                    break;
                case 1:
                    Slobby = Slobby + "Player";
                    break;
                case 2:
                    Slobby = Slobby + "Master";
                    break;
                case 3:
                    Slobby = Slobby + "Spettatore";
                    break;
                default:
                    Slobby = Slobby + "ERR";
                    break;
            }


            if (Cliente.lobby.Check_Online_by_ID(Tlist[I]) == 1)
                Slobby = Slobby + "   Online\n";
            else
                Slobby = Slobby + "   Offline\n";

        }
    }
}

public class Create_Socket : MonoBehaviour

{
	public List<Thread> ThreadList = new List<Thread>();
    public bool D = false;
    public int BufferSize = 256;
    public Client Cliente;
    public Server Host;
    public SendActions Send;
    public string IP = "79.10.254.193";
    public string Port = "25565";
    public string Name = "";

    public void Create_Server()
    {
        Host = GameObject.Find("Network").GetComponent<Server>();
        if (Host.Creato == false && Name != "")
        {
            Host.Creato = true;
            try
            {
                Host.Port = Int32.Parse(Port);
                Host.lobby = GameObject.Find("Network").GetComponent<Lobby>();
                Host.action = GameObject.Find("Network").GetComponent<Actions>();
                Host.action.BufferSize = BufferSize;
                Host.Host = Host;
            }
            catch (Exception e) { Debug.LogError("Errore nella conversione della porta in INT\n" + e); Host.Creato = false; return; }
            if (D) Debug.Log("Controllo Porta eseguito");
            Thread Hos = new Thread(() => Host.Run("Nothing"));
			ThreadList.Add(Hos);
            Hos.Start();
            Debug.Log("Thread Host creato");
            Create_Client();
        }
        else
        {
            Debug.LogError("Esiste già un thread del Server o il nome non è impostato\nCreazione Server");
        }
    }

    public void Create_Client()
    {
        Cliente = GameObject.Find("Network").GetComponent<Client>();
        Host = GameObject.Find("Network").GetComponent<Server>();
        if (Cliente.Creato == false && Name != "")
        {
            Cliente.Creato = true;
            
            try
            {
                Cliente.Port = Int32.Parse(Port);
                Cliente.ServerIP = IP;
                Cliente.Name = Name;
                Cliente.lobby = GameObject.Find("Network").GetComponent<Lobby>();
                Cliente.action = GameObject.Find("Network").GetComponent<Actions>();
                Cliente.action.BufferSize = BufferSize;
            }
            catch (Exception e) { Debug.LogError("Errore nella conversione della porta in INT\n" + e); Cliente.Creato = false; return; }
            if (D) Debug.Log("Controllo porta e assegnazione IP eseguita");
            Thread Cli = new Thread(() => Cliente.Run());
			ThreadList.Add(Cli);
            Cli.Start();
            Debug.Log("Thread Client creato");
        }
        else
        {
            Debug.LogError("Esiste Già un thread Cliente o il nome non è impostato\nCreazione Client");
        }
    }

    public void Close_Connection()
    {
        Cliente.ChiudiContatti();
        //Host.Close_Server();
        GameObject.Find("Network").GetComponent<Lobby>().Clear_lobby();
    }

}

