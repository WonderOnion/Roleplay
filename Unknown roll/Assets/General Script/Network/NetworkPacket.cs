using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Packets : MonoBehaviour
{
    public bool D = false;              //Variabile di debug
    public bool AsServer = false;       //per sapere se si sta inviando il pacchetto dal server
    public Settings settings;           //Per poter gestire gli errori e il debug
    public Lobby lobby;                 //Per poter sapere le informazioni sugli utenti connessi
    public string Owner;

    
    public bool Risultato = false;      //Risultato dell'ultima operazione richiesta, nel caso vi siano errori sarà false, altrimenti true
    //public bool RicevutoInviato = false;//Se false significa che l'ultima azione è di Invio, True se di ricezione
    public byte Header;                 //Header del pacchetto, un intero
    public byte[] Body = null;          //Body del pacchetto, non definito di base

    public Socket Destinatario;         //persona per ricevere o inviare il pacchetto



    public void Inizialize_Packet(bool Deb,bool IsServer,Settings Setting, Lobby lobbyy,string Own)
    {
        lock (this)
        {
            D = Deb;
            AsServer = IsServer;
            settings = Setting;
            lobby = lobbyy;
            Owner = Own;
            Body = new byte[0];
        }
    }
    
    public void Clear ()
    {
        Risultato = false;
        Header = 255;
        Body = null;
    }

    /// <summary>
    /// Permette di inviare il paccketto al socket Socket Destinatario
    /// </summary>
    /// <param name="Richiedente">nome di chi richiede l'azione</param>
    /// <param name="Errore">Errore da scrivere nel caso avvengano dei problemi</param>
    public void Send_Packet (string Richiedente,string Errore)
    {
        lock (this)
        {
            try
            {
                Risultato = true;
                byte[] data = new byte[Body.Length + 1];
                data[0] = Header;
                for (int I = 0; I < Body.Length; I++)
                    data[I + 1] = Body[I];
                Destinatario.Send(data);
            }
            catch (SocketException e)
            {
                settings.Error_Profiler("N011", 0, Richiedente + "(send_Packet): Socket excpetion: " + e.ErrorCode + "  " + Errore + e, 3, false);
                Risultato = false;
            }
            catch (Exception e)
            {
                settings.Error_Profiler("N011",0, Richiedente + "(send_Packet): excpetion: " + Errore + e,3,false);
                Risultato = false;
            }
        }
        if (Risultato == false)
            return;

        if (D) settings.Console_Write(Richiedente + " (Send) " + Header,false);

    }

    /// <summary>
    /// Permette di ricevere pacchetti da Socket Destinatario (body)
    /// </summary>
    /// <param name="Richiedente">il nome di chi richiede l'invio</param>
    public void Receive_Packet (string Richiedente,int Dimension)
    {
        lock (this)
        {
            try
            {
                Risultato = true;
                if (D) settings.Console_Write(Richiedente + " > waiting Message from " + (IPEndPoint)Destinatario.RemoteEndPoint,false);

                byte[] bytes = new byte[Dimension];

                int bytesRec = Destinatario.Receive(Body);
                if (Dimension != bytesRec)
                {
                    settings.Error_Profiler("N013", 0, "Dimensione Pacchetto errata, INSIDE", 2, false);
                    Risultato = false;
                }

                else
                {
                    settings.Error_Profiler("N013", 0, "Dimensione Pacchetto errata", 2, false);
                    Risultato = false;
                }
            }
            catch (Exception e)
            {
                settings.Error_Profiler("N013",0,Owner + " (Receive)  > Retriving packets Failed " + e,2,false);
                Risultato = false;
            }
        }
        if (Risultato == false)
            return;
    }
 
    /// <summary>
    /// Imposta l'header dato il codice, se esiste ritorna true, altrimenti false
    /// </summary>
    /// <param name="Command">Codice dell'header</param>
    /// <param name="Error">Errore da scrivere in console in caso il codice non venga trovato</param>
    public bool Set_Header (byte Command, string Error)
    {

        if (Fetch_Header(Command) != -1)
        {
            Header = Command;
            return true;
        }
        settings.Error_Profiler("N014", 0, "Header Code not found: " + Error, 2, false);
        return false;
    }

    /// <summary>
    /// Utilizzato dopo la ricezione dell'header, true se viene trovato false se non viene trovato
    /// </summary>
    /// <returns></returns>
    public bool Get_Header ()
    {
        try
        {
            if (Fetch_Header(Body[0]) > 0)
            {
                Header = Body[0];
                if (D) settings.Console_Write("Header ricevuto: " + Header, false);
                return true;
            }
        }
        catch(Exception e)
        {

            settings.Error_Profiler("N015", 0, "Dimensione del body errata (NetworkPacket => GetHeader) Catch" + e, 2, false);
            return false;
        }
        settings.Error_Profiler("N015", 0, "Dimensione del body errata (NetworkPacket => GetHeader)",2,false);
        return false;
    }
    




    /// <summary>
    /// Prendendo l'attuale header controlla il codice, se esiste restituisce la dimensione in byte del body, se non esiste ritorna -1
    /// </summary>
    /// <param name="Head">Codice dell'header (è a parte perchè serve anche ad altre fuznioni per capire se esiste un determinato header</param>
    /// <returns></returns>
    public int Fetch_Header(byte Head)
    {
        switch (Head)
        {
            case 0:         //First Contact, viene usato per capire il motivo di connessione
                return 1;
            default:
                return -1;
        }
    }

    /// <summary>
    /// Utilizzata dirante il primo contatto per definire il ruolo del socket
    /// </summary>
    /// <param name="Role">il ruolo che il socket deve avere all'interno della connessione</param>
    /// <param name="Received">False Prepara per l'invio true traduce dal body</param>
    public byte First_Contact (byte Role,bool Received)
    {
        if (Received)
            if (Body.Length == 1)
                return Body[0];         //nel caso sia ricevuto controllo la dimensione e ritorno il ruolo del socet
            else
                settings.Error_Profiler("N015", 0, "Dimensione del body errata (First_Contact)", 2, false);
        else  //Preparo per l'invio del pacchetto
        {
            Set_Header(0, "Errore durante il primo contatto");
            if (Role >= 0 && Role <= 4)
            {
                //0: Connessione al server di un utente
                //1: Connessione per invio di dati
                //3: Risposta dal server per negare
                //4: Risposta dal server per Accettare
                Body = new byte[1];
                Body[0] = Role;
                return Role;    //return random differente da 255
            }
            else
            {
                settings.Error_Profiler("N015", 0, "Role selezionato non valido " + Role,2,false);
            }
        }
        return 255; //arriva qui solo in caso di errore
    }
}
