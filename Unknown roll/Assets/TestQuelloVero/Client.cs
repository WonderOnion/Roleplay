using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Client : MonoBehaviour
{
    string message;
    public string ServerIP;
    public int Port = 25565;
    public bool Creato = false;
    NetworkStream stream;
    public int ConnectionTimeOut = 5;
    public void Run()
    {

        DateTime Tentativo = DateTime.Now;
        while (true)
        {
            Debug.Log("Tentativo di connessione al server: " + ServerIP + ":" + Port);
            TcpClient client = new TcpClient();
            var result = client.BeginConnect(ServerIP, Port, null, null);

            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(ConnectionTimeOut));

            if (!success)
            {
                Debug.LogError("TimeOut Connessione verso il Server");
                Creato = false;
                break;
            }

            Debug.Log("Connessione riuscita al server: " + ServerIP + ":" + Port);
            Debug.Log("Server: " + client.GetStream());
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    message = null;
                    Debug.Log("Invio: ");
                    message = "Premuto"; //Console.ReadLine();
                    int KByte = Encoding.ASCII.GetByteCount(message);          //counter dei Byte
                    byte[] SendData = new byte[KByte];

                    SendData = Encoding.ASCII.GetBytes(message);

                    stream = client.GetStream();

                    stream.Write(SendData, 0, SendData.Length);
                }
                if (Input.GetKeyDown(KeyCode.E))
                    break;
            }
            stream.Close();
            client.Close();
            break;

        }
        Debug.Log("Chiusura thread Client");
        Creato = false;

    }
}
