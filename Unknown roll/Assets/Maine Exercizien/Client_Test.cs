using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class ClientTest : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Debug.Log("Avvio...");        
        string message;
        message = "asd";
        string ServerIP = "localhost";
        int port = 3939;

        TcpClient client = new TcpClient(ServerIP, port);
        Debug.Log("Connessione riuscita");
        int KByte = Encoding.ASCII.GetByteCount(message);          //counter dei Byte

        byte[] SendData = new byte[KByte];

        SendData = Encoding.ASCII.GetBytes(message);

        NetworkStream stream = client.GetStream();

        stream.Write(SendData, 0, SendData.Length);
        stream.Close();
        client.Close();

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
