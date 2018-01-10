using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;

public class Commands : MonoBehaviour
{
    public bool D = false;
    public string Ricevuto = null;
    public Lobby lobby;




    public void Run(object Temp)
    {
        try
        {

            if (D) Debug.Log("Inizio elaborazione comando: " + Ricevuto);
            string[] Comando = Ricevuto.Split(' ');
            if (D) Debug.Log("Ricerca comando: " + Comando);
            switch (Comando[0])
            {
                case "PrintLobby":        //stampa lobby
                    Print_Lobby();
                    break;

                default:
                    Debug.LogError("Comando non trovato: " + Comando);
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Errore nella ricerca del comando: " + Ricevuto + "\n" + e);
        }
    }

    private void Print_Lobby()
    {

    }
}
