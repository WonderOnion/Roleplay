using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class TEtetettete : MonoBehaviour {
    string message;
    string ServerIP = "79.10.254.193";
    int port = 25565;
    NetworkStream stream;
    TcpClient client;
    // Use this for initialization
    void Start ()
    {
        
        NetworkStream stream;


            client = new TcpClient(ServerIP, port);
            Debug.Log("Tentativo di connessione");
            
            Debug.Log("Connessione riuscita");
            Debug.Log("Server: " + client.GetStream());        
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKeyDown)
        {
            message = null;
            Debug.Log("Invio: ");
            message = "Premuto";//Console.ReadLine();
            int KByte = Encoding.ASCII.GetByteCount(message);          //counter dei Byte
            byte[] SendData = new byte[KByte];

            SendData = Encoding.ASCII.GetBytes(message);

            stream = client.GetStream();

            stream.Write(SendData, 0, SendData.Length);
        }
    }
}
