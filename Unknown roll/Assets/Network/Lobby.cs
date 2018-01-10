using System.Collections;
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;


public class Lobby : MonoBehaviour
{
    public bool D = true;
    private int ID = 1;
    public List<Player> lobby = new List<Player>();
    public Actions action;

    public void Start()
    {
        action = GameObject.Find("Network").GetComponent<Actions>();
    }




    //          aggiunte
    public void Add_ServerPlayer( Socket User, string Nome,int power)
    {
        if (Check_Exist_by_Name(Nome))
            if (Check_Online_by_ID(Retrive_ID_by_Name(Nome)) == 0)
            {
                Set_Online_by_ID(Retrive_ID_by_Name(Nome), true);
                return;
            }
            else
            {
                Debug.LogError("L'utente che si sta cercando di far connettere è già online ->" + Check_Online_by_ID(Retrive_ID_by_Name(Nome)));
                return;
            }
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

    public void Add_ClientPlayer(int ID, int power, int online, string Nome)
    {
        if (Check_Exist_by_ID(ID))
        {
            if (Check_Online_by_ID(ID) == 0)
                Set_Online_by_ID(Retrive_ID_by_Name(Nome),true);
            return;
        }
        bool On = false;
        if (online == 1)
            On = true;
        Player player = new Player
        {
            ID = ID,
            Name = Nome,
            InGameName = Nome,
            Power = power,
            Online = On
        };
        lock (lobby)
        {
            lobby.Add(player);
        }
    }

    public void Set_Online_by_ID(int ID,bool stato)
    {
        int temp = Check_Online_by_ID(ID);
        if (temp == 2)
        {
            Debug.LogError("ID richiesto non esiste + ID\n");
            return;
        }
        if(temp == 1 && stato == true)
        {
            Debug.LogError("il player associato all'ID richiesto è già online + ID\n");
            return;
        }
        if (temp == 0 && stato == false)
        {
            Debug.LogError("il player associato all'ID richiesto è già offline + ID\n");
            return;
        }
        int I;
        lock (lobby)
        {
            for (I = 0; I < lobby.Count; I++)
                if (lobby[I].ID == ID)
                {
                    lobby[I].Online = stato;
                    break;
                }
            
        }
        if (I < lobby.Count)
            return;
        Debug.LogError("Non è stato possibile segnare online " + ID);
    }

    public void Set_Power_by_ID(int ID, int power)
    {
        lock (lobby)
        {
            int I;
            lock (lobby)
            {
                for (I = 0; I < lobby.Count; I++)
                    if (lobby[I].ID == ID)
                    {
                        lobby[I].Power = power;
                        break;
                    }
            }
            if (I < lobby.Count)
                return;
            Debug.LogError("Non è stato possibile assegnare il potere " + power + " a " + ID);
        }
    }

    public void Set_User_by_ID(int ID, Socket User)
    {
        lock (lobby)
        {
            int I;
            lock (lobby)
            {
                for (I = 0; I < lobby.Count; I++)
                    if (lobby[I].ID == ID)
                    {
                        lobby[I].User = User;
                        break;
                    }
            }
            if (I < lobby.Count)
                return;
            Debug.LogError("Non è stato possibile assegnare l'user a "+ ID);
        }
    }

    public void Set_Pedina_by_ID(int ID, string pedina)
    {
        int I;
        lock (lobby)
        {
            for (I = 0; I < lobby.Count; I++)
                if (lobby[I].ID == ID)
                {
                    lobby[I].Pedina = pedina;
                    break;
                }
        }
        if (I < lobby.Count)
            return;
        Debug.LogError("Non è stato possibile la pedina " + pedina + " a " + ID);
    }

    public void Set_InGameName_by_ID (int ID,string InGameName)
    {
        int I;
        lock (lobby)
        {
            for (I = 0; I < lobby.Count; I++)
                if (lobby[I].ID == ID)
                {
                    lobby[I].InGameName = InGameName;
                    return;
                }
            
        }
        if (I < lobby.Count)
            return;
        Debug.LogError("Non è stato possibile assegnare il nome in gioco " + InGameName + " a " + ID);
    }

    public void Set_InGameName_by_Socket(Socket User, string InGameName)
    {
        int I;
        lock (lobby)
        {
            for (I = 0; I < lobby.Count; I++)
                if (lobby[I].User == User)
                {
                    lobby[I].InGameName = InGameName;
                    break;
                }
            
        }
        if (I < lobby.Count)
            return;
        Debug.LogError("Non è stato possibile assegnare il nome in gioco " + InGameName + " a " + (IPEndPoint)User.RemoteEndPoint);
    }


    //          rimozioni

    public void Remove_by_Socket(Socket User)
    {
        int I;
        lock (lobby)
        {
            for (I = 0; I < lobby.Count; I++)
                if (lobby[I].User == User)
                {
                    lobby.RemoveAt(I);
                    break;
                }
        }
        if (I < lobby.Count)
            return;

        Debug.LogError("Non è stato trovatoil giocatore: " + (IPEndPoint)User.RemoteEndPoint + ", impossibile rimuoverlo");
    }

    public void Remove_by_ID(int ID)
    {
        int I;
        lock (lobby)
        {
            for (I = 0; I < lobby.Count; I++)
                if (lobby[I].ID == ID)
                {
                    lobby.RemoveAt(I);
                    break;
                }
        }
        if (I < lobby.Count)
            return;
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
        int I;
        lock (lobby)
        {
            for (I = 0; I < lobby.Count; I++)
                if (lobby[I].User == User)
                    break;
        }
        if (I < lobby.Count)
            return lobby[I].Name;
        Debug.LogError("Non è stato trovatoil giocatore: " + (IPEndPoint)User.RemoteEndPoint + ", impossibile restituire il nome");
        return "";
    }       //restituisce "" se non esiste

    public int Retrive_ID_by_Socket(Socket User)
    {
        int I;
        lock (lobby)
        {
            for (I = 0; I < lobby.Count; I++)
                if (lobby[I].User == User)
                    break;
        }
        if (I < lobby.Count)
            return lobby[I].ID;
        Debug.LogError("Non è stato trovatoil giocatore: " + (IPEndPoint)User.RemoteEndPoint + "impossibile restituire l'ID");
        return -1;
    }           //restituisce -1 se non esiste

    public int Retrive_ID_by_Name(string Name)
    {
        for (int I = 0; I < lobby.Count; I++)
            if (string.Equals(lobby[I].Name,Name))
            {
                if (D) Debug.LogFormat("Retrive_ID_by_Name, nome ricevuto: {0} risultato: {1}", Name, lobby[I].ID);
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

    public List<int> List_of_ID()
    {
        List<int> Temp = new List<int>();
        lock (lobby)
        {
            for (int I = 0; I < lobby.Count; I++)
            {
                Temp.Add(lobby[I].ID);
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
            byte[] data = new byte[action.BufferSize];
            data = Encoding.ASCII.GetBytes("Ping");
            lobby[I].User.Send(data, data.Length, SocketFlags.None);
        }
        catch(Exception e)
        {
            Debug.LogError("Tentativo di check con il client fallito " + (IPEndPoint)User.RemoteEndPoint + " imposto offline e chiudo il socket.");
            Set_Online_by_ID(lobby[I].ID, false);

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

    public int Check_Online_by_ID(int ID)
    {
        int I = 0;
        for (I = 0; I < lobby.Count; I++)
            if (lobby[I].ID == ID)
                if (lobby[I].Online == true)
                    return 1;
                else
                    return 0;
        return 2;
    }                   // ritorna 0 se offline 1 se online e 2 se non esiste nessuno con quell'ID

}


public class Player : MonoBehaviour
{
    public bool Online = true;
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