using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class UDPHandler : MonoBehaviour
{
    void Awake(){
        DontDestroyOnLoad(gameObject);
    }

    Thread receiveThread;
    UdpClient udpClient;
    UdpClient udpClient2;

    int port;
    public String text;
    public bool newData;

    void start(){
        port = 5065;
    }

    public void SendString(string sData)
    {
        udpClient = new UdpClient("127.0.0.1",8000);
        Byte[] sendBytes = Encoding.ASCII.GetBytes(sData);
        try{
            udpClient.Send(sendBytes, sendBytes.Length);
        }
        catch ( Exception e ){
            Debug.Log(e);
        }
        Debug.Log("Send Data");
    }

    public void InitUDP(){
        
        print("UDP Initialized");
        Debug.Log(receiveThread);

        if(receiveThread != null){
            try{
                receiveThread.Abort();
            }
            catch(Exception e){
                Debug.Log(e);
            }
        }

        receiveThread = new Thread (new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        Debug.Log(receiveThread);
        print("setup Done");
    }

    private void ReceiveData()
    {
        udpClient2 = new UdpClient(5065);
        while (true){

            try{
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5065);
                byte[] data = udpClient2.Receive(ref anyIP);

                text = Encoding.UTF8.GetString(data);

                newData = true;
            }
            catch(Exception e){
                Debug.Log(e);
            }

        }
    }

    public void StopThread(){
        try{
            receiveThread.Abort();
        }
        catch(Exception e){
            Debug.Log(e);
        }
    }

}
