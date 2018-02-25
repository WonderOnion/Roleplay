using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
/*
Essendo che il gioco è in fase prototipale nasce la necessità di dover cambiare spesso e volentieri i parametri di funzioni e di comandi.
Queste funzioni sono per la maggior parte inutili e restituiscono un return ma centralizzano tutto il lavoro di invio a client e server in un solo file andando quindi a evitare la ricerca degli utilizzi di ogni singolo comando
all'interno degli altri file.
*/
public class SendActions : MonoBehaviour
{
    public bool D = true;
    public bool AsServer = false;
    public Lobby lobby;
    public int BufferSize;

    public bool Send_to_One(string Action, Socket Receiver, string Errore)                              //comando base di invio utilizzato durante tutte le sequenze di invio, restituisce false se vi sono stati dei problemi durante l'invio, se non si vuole avere il messaggio di errore lo si lascia vuoto ""
    {
        if (AsServer)
            if (lobby.Check_Online_by_ID(lobby.Retrive_ID_by_Socket(Receiver)) != 1)
            {
                if (D) Debug.Log("La persona a cui si sta cercando di inviare il messaggio è offline");
                return false;
            }
        try
        {
            byte[] data = new byte[BufferSize];
            data = Encoding.ASCII.GetBytes(Action + '\n');
            Receiver.Send(data);
            if (D) Debug.Log("Inviato: " + Action);
        }
        catch (Exception e)
        {
            if (!Errore.Equals(""))
                Debug.LogError(Errore + e);
            return false;
        }
        return true;
    }

    public string Receive_by_one(Socket Sender, string Richiedente)
    {

        if (D) Debug.Log(Richiedente + ": Ricezione in corso...");
        string data = null;
        try
        {
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
        }
        catch (Exception e)
        {
            Debug.LogError("Errore durante la ricezione dei dati da parte di " + Richiedente + e);
        }
        if (D) Debug.Log("Ricevuto: " + data.Split('\n')[0]);
        return data.Split('\n')[0];

    }

    public void Server_Broadcast(string Action)
    {
        if (D) Debug.Log("Server: Invio in broadcast ''" + Action + "''");
        List<int> IDList = lobby.List_of_ID();
        for (int I = 0; I < IDList.Count; I++)
            try
            {
                if (lobby.Check_Online_by_ID(IDList[I]) == 1)
                {
                    if (D) Debug.Log("Server: Invio a " + lobby.Retrive_Name_by_ID(IDList[I]) + " ''" + Action + "''");
                    Send_to_One(Action, lobby.Retrive_Socket_by_ID(IDList[I]), "Errore nell'invio in broadcast a ID " + lobby.Retrive_Socket_by_ID(IDList[I]));
                }
                else
                    if (D) Debug.Log("Invio del comando in broadcast non riuscito poichè l'user è offline: " + lobby.Retrive_Socket_by_ID(IDList[I]));
            }
            catch (Exception e)
            {
                Debug.LogError("Errore nell'invio del messaggio a " + lobby.Retrive_Name_by_ID(IDList[I]) + " ID: " + IDList[I] + e);
                if (lobby.Check_Online_by_ID(IDList[I]) == 1)
                    lobby.Check_Connection(lobby.lobby[I].User);

            }
    }                                                   //comando specifico del server che invia a tutti i socket un messaggio specifico.

    public string Refresh_Lobby(List<int> ID, List<int> Online)
    {
        string Mex;
        Mex = "RefL";
        for (int I = 0; I < ID.Count; I++)
            Mex = Mex + ID[I] + "," + lobby.Check_Online_by_ID(ID[I]) + "#";
        return Mex;
    }

    public string Client_Player_Login_Inizialize(int ID, int Power, int Online, string Nome)
    {
        return "NewU" + ID + "#" + Power + "#" + Online + "#" + Nome;
    }       //invio iniziale per nuovo client in lobby al client con formato: LogU(ID)#(Power)#(Online)#(Nome)

    public string Client_Player_Logout()
    {
        return "OffU";
    }

    public string Player_Come_Online(int ID, bool state)
    {
        if (state == true)
            return "LogU" + ID;
        else
            return "OffU" + ID;
    }

    public string Lobby_Change_Role(int ID, int Power)
    {
        if (AsServer)
            return "CngR" + ID + "#" + Power;
        else
            return "CngR" + Power;
    }

    public string Lobby_Ping()
    {
        return "Ping";
    }
}
