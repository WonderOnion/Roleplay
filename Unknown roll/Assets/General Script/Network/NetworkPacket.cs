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


    public bool Risultato = false;      //Risultato dell'ultima operazione richiesta, nel caso vi siano errori sarà false, altrimenti true
    public bool RicevutoInviato = false;//Se false significa che l'ultima azione è di Invio, True se di ricezione
    public byte Header;                 //Header del pacchetto, un intero
    public byte[] Body = null;          //Body del pacchetto, non definito di base

    public Socket Destinatario;         //persona per ricevere o inviare il pacchetto



    public void Inizialize_Packet(bool Deb,bool IsServer,Settings Setting, Lobby lobbyy)
    {
        D = Deb;
        AsServer = IsServer;
        settings = Setting;
        lobby = lobbyy;
    }

    /// <summary>
    /// Permette di inviare il paccketto al socket Socket Destinatario
    /// </summary>
    /// <param name="Richiedente">nome di chi richiede l'azione</param>
    /// <param name="Errore">Errore da scrivere nel caso avvengano dei problemi</param>
    public void Send_Packet (string Richiedente,string Errore)
    {
        if (AsServer)
            if (lobby.Check_Online_by_ID(lobby.Retrive_ID_by_Socket(Destinatario)) != 1)
            {
                if (D) Debug.Log("La persona a cui si sta cercando di inviare il messaggio è offline");
                Risultato = false;
                return;
            }
        try
        {
            byte[] data = new byte[Body.Length+1];
            data[0] = Header;
            for (int I=0;I<Body.Length;I++)
                data[I + 1] = Body[I];
            Destinatario.Send(data);
        }
        catch (SocketException e)
        {
            Debug.LogError("Socket excpetion: " + e.ErrorCode + "  " + Errore + e);
            Risultato = false;
            return;
        }
        catch (Exception e)
        {
            Debug.LogError(Errore + e);
            Risultato = false;
            return;
        }
        if (D) Debug.Log(Richiedente + " (Send) " + Header);


        Risultato = true;
    }
    /// <summary>
    /// Permette di ricevere pacchetti da Socket Destinatario
    /// </summary>
    /// <param name="Richiedente">il nome di chi richiede l'invio</param>
    public void Receive_Packet (string Richiedente)
    {
        try
        {
            if (D) Debug.Log(Richiedente + " > waiting Message from " + (IPEndPoint)Destinatario.RemoteEndPoint);
            byte[] bytes = new byte[1];
            int bytesRec = Destinatario.Receive(bytes);
            if (bytes.Length == 1)
                Header = bytes[0];
            else
            {
                settings.Error_Profiler("N013", 0, "Dimensione Header errata", 2, false);
                Risultato = false;
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Receive  > Retriving Header Failed " + e);
            Risultato = false;
            return;
        }

        switch (Header)
        {
            case 0:
                settings.Console_Write("Debug ricevuto da" + (IPEndPoint)Destinatario.RemoteEndPoint,false);
                break;
            default:
                settings.Error_Profiler("N013", 0, "Header non trovato", 2, false);
                Risultato = false;
                //TODO aggiungere modo per scartare tutti i byte successivi sino al prossimo pacchetto
                //Possibile soluzione, richiesta al mittente con codice di debug, il mittente inviera un codice in risposta adibito solo a questo utilizzo (es 101) per 5 volte consecutive, quando ricevute tutte e 5 senza problemi significa che ci si è risincronizzati e il mittente può rispedire i pacchetti persi
                break;
        }
        Risultato = true;
    }
    
}
