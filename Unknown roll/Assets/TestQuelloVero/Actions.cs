using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;

public class Actions : MonoBehaviour
{
    public bool D = true;
    public bool AsServer = false;
    public Socket User = null;
    public string Ricevuto = null;
    public int contesto; // 0 = lobby 1 = inGame
    public Lobby lobby;
    public int BufferSize;




    public void Run(object Temp)
    {
        try
        {

            if (D) Debug.Log("Inizio elaborazione Azione: " + Ricevuto);
            string Comando = string.Format(Ricevuto.Substring(0, 4));
            if (D) Debug.Log("Ricerca Azione: " + Comando);
            switch (Comando)
            {
                case "List":        //formato: List****----°°°°... *=comando -=numero di pacchetti(dimensione variabile) °= pacchetto attuale(dimensione variabile) ...=informazioni
                    break;
                case "RefL":        //refresh della lobby eseguibile solo dal server, vengono ricevuti tutti gli id di tutti i player connessi, il server risponderà se e solo se vi sono dei dati da aggiornare
                    Refresh_Lobby(Ricevuto.Substring(4, Ricevuto.Length-4));
                    break;
                case "NewU":        //nuovo player connesso
                    Client_Player_Login_Inizialize(Ricevuto.Substring(4,Ricevuto.Length-4));
                    break;
                case "LogU":
                    Player_Come_Online(Ricevuto.Substring(4, Ricevuto.Length - 4));
                    break;
                case "OffU":        //avverte della disconnessione di un client
                    Client_Player_LogOut(Ricevuto.Substring(4, Ricevuto.Length-4));
                    break;
                case "CngR":        //Change Role formato Client richiedente CngR(power) invio da server CngR(ID)#(Power)
                    Lobby_Change_Role(Ricevuto.Substring(4, Ricevuto.Length-4));
                    break;
                case "MexA":        //invio Messaggio a tutti
                    break;
                case "MexT":        //invio messaggio a un utente specifico
                    break;

                default:
                    Debug.LogError("Azione non trovata: " + Comando);
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Errore nella ricerca del Azione: " + Ricevuto +"\n" + e);
        }
    }



    //Generali
    public void Send_to_One (string Action, Socket Receiver,string Errore)                              //comando base di invio utilizzato durante tutte le sequenze di invio
    {
        if (D) Debug.Log("Invio: " + Action);
        try
        {
            byte[] data = new byte[BufferSize];
            data = Encoding.ASCII.GetBytes(Action + '\n');
            Receiver.Send(data);
        }
        catch (Exception e)
        {
            Debug.LogError(Errore);
        }
    }

    public string Receive_by_one (Socket Sender,string Richiedente)
    {
        if (D) Debug.Log(Richiedente + ": Ricezione in corso...");
        string data = null;
        while (true)
        {
            byte[] bytes = new byte[BufferSize];
            int bytesRec = Sender.Receive(bytes);
            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
            if (data.IndexOf('\n') > -1)
            {
                break;
            }
        }
        return data.Split('\n')[0];
    }

    public void Server_Broadcast(string Action)
    {
        if (D) Debug.Log("Server: Invio in broadcast ''" + Action + "''");
        for (int I = 0; I < lobby.lobby.Count; I++)
            try
            {
                if (D) Debug.Log("Server: Invio a " + lobby.lobby[I].Name + " ''" + Action + "''");
                Send_to_One(Action,lobby.lobby[I].User,"Errore nell'invio in broadcast a " + lobby.lobby[I].Name);
            }
            catch (Exception e)
            {
                Debug.LogError("Errore nell'invio del messaggio a " + lobby.lobby[I].Name + " -> " + (IPEndPoint)User.RemoteEndPoint);
            }
    }                                                   //comando specifico del server che invia a tutti i socket un messaggio specifico.

    private void Client_Player_Login_Inizialize(string Action)
    {
        bool Deb = true;
        if (Deb) Debug.Log("Inizio aggiunta client player: " + Action);
        string[] temp = Action.Split('#');
        lobby.Add_ClientPlayer(Int32.Parse(temp[0]), temp[2], Int32.Parse(temp[1]));
        if (Deb) Debug.Log("Aggiunto player: " + Action);

    }                                   //invio iniziale per nuovo client in lobby al client con formato: LogU(ID)#(Power)#(Nome)

    private void Player_Come_Online (string Action)
    {
        if (lobby.Check_Exist_by_ID(Int32.Parse(Action)))
            lobby.Set_Online_by_ID(Int32.Parse(Action), true);
        else
            Debug.LogError("Il client che si sta tentando di far tornare online non è presente nella lista: " + Action);
    }

    private void Client_Player_LogOut (string Action)
    {
        if (lobby.Check_Exist_by_ID(Int32.Parse(Action)))
            lobby.Set_Online_by_ID(Int32.Parse(Action),false);
        else
            Debug.LogError("Il client che si sta tentando di disconnettere non è presente nella lista: " + Action);
    }                                           //eliminazione di un profilo dalla lista formato: OffU(ID) es: OffU4

    private void Server_Send_Message_to_All(string Action)
    {
        byte[] data = new byte[100];
        data = Encoding.ASCII.GetBytes("MexR" + lobby.Retrive_Name_by_Socket(User) + ": " + Action);
        for (int I = 0; I < lobby.lobby.Count; I++)
            try
            {
                lobby.lobby[I].User.Send(data, data.Length, SocketFlags.None);
            }
            catch(Exception e)
            {
                Debug.LogError("Errore nell'invio del messaggio a " + lobby.lobby[I].Name + " -> " + (IPEndPoint)User.RemoteEndPoint);
            }
        
    }

    private void Server_Send_Message_to_One(string Action)
    {
        String[] substrings = Action.Split('#');
        Socket TUser = lobby.Retrive_Socket_by_Name(substrings[0]);

        byte[] data = new byte[100];
        data = Encoding.ASCII.GetBytes("MexR" + lobby.Retrive_Name_by_Socket(User)+": " + Action);
        for (int I = 0; I < lobby.lobby.Count; I++)
            if (TUser == lobby.lobby[I].User)
                try
                {
                    lobby.lobby[I].User.Send(data, data.Length, SocketFlags.None);
                }
                catch (Exception e)
                {
                    Debug.LogError("Errore nell'invio del messaggio privato a " + lobby.lobby[I].Name + " -> " + (IPEndPoint)User.RemoteEndPoint);
                }
    }
    
    public void Refresh_Lobby(string Action)
    {

        bool Deb = false;
        int numero;
        if (Deb) Debug.Log("Entrato in Refresh Lobby");
        bool result;
        string[] temp = Action.Split('#');
        int I = 0;
        if (Deb) Debug.Log("Numero di elementi ricevuti: " + temp.Length);
        if (string.Equals(temp[0], ""))
            I++;
        for (I = 0; I < temp.Length; I++)
        {
            if (Deb) Debug.Log("Controllo se l'elemento è online : " + temp[I]);
            result = Int32.TryParse(temp[I], out numero);
            if (result)
                if (!lobby.Check_Exist_by_ID(numero))
                    Send_to_One("OffU" + temp[I], User, "Errore nell'invio logOutUser");
        }
        if (Deb) Debug.Log("Inizio controllo mancanze...");
        List<int> IDList = lobby.List_of_ID();
        if (Deb) Debug.Log("Elementi atttualmente connessi: " + IDList.Count);
        bool[] IDExist = new bool[IDList.Count];
        for (I = 0; I < IDExist.Length; I++)
            IDExist[I] = false;
        for (I = 0; I < IDList.Count; I++)
        {
            if (Deb) Debug.Log("Controllo esistenza ID: " + IDList[I]);
            for (int J = 0; J < temp.Length; J++)
            {
                result = Int32.TryParse(temp[J], out numero);
                if (result)
                    if (numero == IDList[I])
                    {
                        if (Deb) Debug.Log("Trovato");
                        IDExist[I] = true;
                    }
            }
            if(!IDExist[I])
            {
                if (Deb) Debug.Log("Non trovato, invio dati...");
                Send_to_One("NewU" + IDList[I] + "#" + lobby.Retrive_Power_by_ID(IDList[I]) + "#" + lobby.Retrive_Name_by_ID(IDList[I]), User, "Errore nell'aggiornamento della lobby del client");
            }
        }
    }                                                   //viene inviata la lista degli ID attualmente connessi dal client, il server riceve e esegue il codice che controlla tutti gli id che comunica tutte le differenze tramite i comandi appositi formato di ricezione: RefL1#4#5#12

    // Lobby
    private void Lobby_Change_Role (string Action)
    {
        if (AsServer)
        {
            string utente = lobby.Retrive_Name_by_Socket(User);
            Debug.Log("cambio di potere di " + utente + " da " + lobby.Retrive_Power_by_Name(utente) + " a " + Action);
            lobby.Set_Power_by_ID(lobby.Retrive_ID_by_Socket(User), Int32.Parse(Action));
            Server_Broadcast("CngR" +lobby.Retrive_ID_by_Name(utente) + "#" + lobby.Retrive_Power_by_Name(utente));
        }
        else
        {
            string[] azioni = Action.Split('#');
            Debug.Log("cambio di potere di " + lobby.Retrive_Name_by_ID(Int32.Parse(azioni[0])) + " da " + lobby.Retrive_Power_by_Name(lobby.Retrive_Name_by_ID(Int32.Parse(azioni[0]))) + " a " + azioni[1]);
            lobby.Set_Power_by_ID(Int32.Parse(azioni[0]), Int32.Parse(azioni[1]));
        }
    }


    //InGame
}

