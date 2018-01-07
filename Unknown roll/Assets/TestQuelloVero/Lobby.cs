﻿using System.Collections;
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;


public class Lobby : MonoBehaviour
{
    public bool D = true;
    int ID = 1;
    public List<Player> lobby = new List<Player>();


    //          aggiunte
    public void Add_ServerPlayer(Socket User, string Nome,int power)
    {
        Player player = new Player
        {
            User = User,
            Name = Nome,
            InGameName = Nome,
            Power = power
        };
        lock (lobby)
        {
            player.ID = ID;
            ID++;
            lobby.Add(player);
        }
    }

    public void Add_ClientPlayer(int ID, string Nome, int power)
    {
        if (Check_Exist_by_ID(ID))
        {
            if (D)Debug.Log("ID già presente");
            return;
        }
            
        Player player = new Player
        {
            ID = ID,
            Name = Nome,
            InGameName = Nome,
            Power = power
        };
        lock (lobby)
        {
            lobby.Add(player);
        }
    }

    public void Set_Power_by_Socket(Socket User, int power)
    {
        lock (lobby)
        {
            for (int I = 0; I < lobby.Count; I++)
                if (lobby[I].User == User)
                {
                    lobby[I].Power = power;
                    return;
                }
            Debug.LogError("Non è stato possibile assegnare il potere " + power + " a " + (IPEndPoint)User.RemoteEndPoint);
        }
    }

    public void Set_Power_by_ID(int ID, int power)
    {
        lock (lobby)
        {
            for (int I = 0; I < lobby.Count; I++)
                if (lobby[I].ID == ID)
                {
                    lobby[I].Power = power;
                    return;
                }
            Debug.LogError("Non è stato possibile assegnare il potere " + power + " a " + ID);
        }
    }

    public void Set_Pedina_by_Socket(Socket User, string pedina)
    {
        lock (lobby)
        {
            for (int I = 0; I < lobby.Count; I++)
                if (lobby[I].User == User)
                {
                    lobby[I].Pedina = pedina;
                    return;
                }
        }
        Debug.LogError("Non è stato possibile la pedina " + pedina + " a " + (IPEndPoint)User.RemoteEndPoint);
    }

    public void Set_Pedina_by_ID(int ID, string pedina)
    {
        lock (lobby)
        {
            for (int I = 0; I < lobby.Count; I++)
                if (lobby[I].ID == ID)
                {
                    lobby[I].Pedina = pedina;
                    return;
                }
        }
        Debug.LogError("Non è stato possibile la pedina " + pedina + " a " + ID);
    }

    public void Set_InGameName_by_ID (int ID,string InGameName)
    {
        lock (lobby)
        {
            for (int I = 0; I < lobby.Count; I++)
                if (lobby[I].ID == ID)
                {
                    lobby[I].InGameName = InGameName;
                    return;
                }
            Debug.LogError("Non è stato possibile assegnare il nome in gioco " + InGameName + " a " + ID);
        }
    }

    public void Set_InGameName_by_Name(string Name, string InGameName)
    {
        lock (lobby)
        {
            for (int I = 0; I < lobby.Count; I++)
                if (lobby[I].Name == Name)
                {
                    lobby[I].InGameName = InGameName;
                    return;
                }
            Debug.LogError("Non è stato possibile assegnare il nome in gioco " + InGameName + " a " + Name);
        }
    }

    public void Set_InGameName_by_Socket(Socket User, string InGameName)
    {
        lock (lobby)
        {
            for (int I = 0; I < lobby.Count; I++)
                if (lobby[I].User == User)
                {
                    lobby[I].InGameName = InGameName;
                    return;
                }
            Debug.LogError("Non è stato possibile assegnare il nome in gioco " + InGameName + " a " + (IPEndPoint)User.RemoteEndPoint);
        }
    }


    //          rimozioni
    public void Remove_by_Name(string Name)
    {
        lock (lobby)
        {
            for (int I = 0; I < lobby.Count; I++)
                if (lobby[I].Name == Name)
                {
                    lobby.RemoveAt(I);
                    return;
                }
        }
        Debug.LogError("Non è stato trovatoil giocatore: " + Name + ", impossibile rimuoverlo");
    }

    public void Remove_by_Socket(Socket User)
    {
        lock (lobby)
        {
            for (int I = 0; I < lobby.Count; I++)
                if (lobby[I].User == User)
                {
                    lobby.RemoveAt(I);
                    break;
                }
        }

        Debug.LogError("Non è stato trovatoil giocatore: " + (IPEndPoint)User.RemoteEndPoint + ", impossibile rimuoverlo");
    }

    public void Remove_by_ID(int ID)
    {
        lock (lobby)
        {
            for (int I = 0; I < lobby.Count; I++)
                if (lobby[I].ID == ID)
                {
                    lobby.RemoveAt(I);
                    return;
                }
        }
        Debug.LogError("Non è stato trovatoil giocatore: " + ID + ", impossibile rimuoverlo");
    }


    //          generali

    public Socket Retrive_Socket_by_Name(string Name)
    {
        for (int I = 0; I < lobby.Count; I++)
            if (lobby[I].Name == Name)
            {
                return lobby[I].User;
            }
        return null;
    }       //restituisce null se non esiste

    public Socket Retrive_Socket_by_ID(int ID)
    {
        for (int I = 0; I < lobby.Count; I++)
            if (lobby[I].ID == ID)
            {
                return lobby[I].User;
            }
        return null;
    }              //restituisce null se non esiste

    public int Retrive_Power_by_Socket(Socket User)
    {
        for (int I = 0; I < lobby.Count; I++)
            if (lobby[I].User == User)
            {
                return lobby[I].Power;
            }
        return -1;
    }           //restituisce -1 se non esiste

    public int Retrive_Power_by_ID(int ID)
    {
        for (int I = 0; I < lobby.Count; I++)
            if (lobby[I].ID == ID)
            {
                return lobby[I].Power;
            }
        return -1;
    }                   //restituisce -1 se non esiste

    public int Retrive_Power_by_Name(string Name)
    {
        for (int I = 0; I < lobby.Count; I++)
            if (lobby[I].Name == Name)
            {
                return lobby[I].Power;
            }
        return -1;
    }           //restituisce -1 se non esiste

    public string Retrive_Name_by_Socket(Socket User)
    {
        lock (lobby)
        {
            for (int I = 0; I < lobby.Count; I++)
                if (lobby[I].User == User)
                    return lobby[I].Name;
        }
        Debug.LogError("Non è stato trovatoil giocatore: " + (IPEndPoint)User.RemoteEndPoint + ", impossibile restituire il nome");
        return "";
    }       //restituisce "" se non esiste

    public int Retrive_ID_by_Socket(Socket User)
    {
        lock (lobby)
        {
            for (int I = 0; I < lobby.Count; I++)
                if (lobby[I].User == User)
                    return lobby[I].ID;
        }
        Debug.LogError("Non è stato trovatoil giocatore: " + (IPEndPoint)User.RemoteEndPoint + "impossibile restituire l'ID");
        return -1;
    }           //restituisce -1 se non esiste

    public int Retrive_ID_by_Name(string Name)
    {
        for (int I = 0; I < lobby.Count; I++)
            if (string.Equals(lobby[I].Name,Name))
            {
                if (D) Debug.LogFormat("Retrive_ID_by_Name, nome ricevuto - {0} risultato - {1}", Name, lobby[I].ID);
                return lobby[I].ID;
            }
        Debug.LogError("Non è stato trovato il giocatore: " + Name + "impossibile restituire l'ID");
        return -1;
    }            //restituisce -1 se non esiste

    public string Retrive_Name_by_ID(int ID)
    {
        for (int I = 0; I < lobby.Count; I++)
            if (lobby[I].ID == ID)
            {
                return lobby[I].Name;
            }
        return null;
    }              //restituisce null se non esiste

    public List<string> List_of_Player_with_Power()
    {
        List<string> Temp = new List<string>();
        lock (lobby)
        {
            for(int I =0;I<lobby.Count;I++)
            {
                Temp.Add(lobby[I].Power + lobby[I].Name);
            }
        }
        return Temp;
    }       //restituisce una lista composta da potere + nome es "0Drevar" oppure "2Drevar" etc, il potere è sempre e solo il primo carattere

    public List<int> List_of_ID()
    {
        List<int> Temp = new List<int>();
        lock (lobby)
        {
            for (int I = 0; I < lobby.Count; I++)
            {
                Temp.Add(lobby[I].Power + lobby[I].ID);
            }
        }
        return Temp;
    }                           //restituisce una lista di interi di tutti gli ID all'interno della propria lobby

    public void Check_Connection(Socket User)
    {
        int I = 0;
        for (I = 0;I < lobby.Count; I++)
        {
            if (lobby[I].User == User)
                break;
        }
        if (I == lobby.Count)
        {
            Debug.LogError("Impossibile trovare l'User " + (IPEndPoint)lobby[I].User.RemoteEndPoint);
            return;
        }
        try
        {
            byte[] data = new byte[100];
            data = Encoding.ASCII.GetBytes("Ping");
            lobby[I].User.Send(data, data.Length, SocketFlags.None);
        }
        catch(Exception e)
        {
            Debug.LogError("Tentativo di check con il client fallito " + (IPEndPoint)User.RemoteEndPoint + " eliminazione dalla lobby e chiusura socket.");
            Remove_by_Socket(User);

        }
    }              //effettua un controllo di connessione con un socket, nel caso lo elimina dalla lista

    public bool Check_Exist_by_ID(int ID)
    {
        int I = 0;
        for (I = 0; I < lobby.Count; I++)
            if (lobby[I].ID == ID)
                return true;
        return false;
    }

    public bool Check_Exist_by_Name(string Name)
    {
        int I = 0;
        for (I = 0; I < lobby.Count; I++)
            if (lobby[I].Name == Name)
                return true;
        return false;
    }
}


public class Player : MonoBehaviour
{
    public Socket User;         //utilizzato solo dal server
    public int ID;              //utilizzato dai client per far riferimento ad un player specifico
    public string Name;
    public string InGameName;
    public int Power;
    // 0 = null
    // 1 = player
    // 2 = Master
    // 3 = Spettatore
    public string Pedina;   //ID locale della pedina
}