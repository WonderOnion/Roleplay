using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class Serverino : MonoBehaviour
{
    public IPAddress ip = Dns.GetHostEntry("localhost").AddressList[0];
    public int port = 3939;
    public TcpListener server;
    public TcpClient client;
    // Use this for initialization
    void Start ()
    {
        server = new TcpListener(ip, port);
        client = default(TcpClient);
        try
        {
            server.Start();
            Debug.Log("Server avviato.");
        }
        catch (Exception e)
        {
            Debug.Log("Errore nella creazione del server\n" + e.ToString());
        }

    }

	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log("In attesa di una connessione...");
        client = server.AcceptTcpClient();

        byte[] ReceivedBuffer = new byte[100];
        NetworkStream stream = client.GetStream();

        stream.Read(ReceivedBuffer, 0, ReceivedBuffer.Length);

        string Messaggio = Encoding.ASCII.GetString(ReceivedBuffer, 0, ReceivedBuffer.Length);

        Debug.Log(Messaggio + ReceivedBuffer.Length);
    }
}
