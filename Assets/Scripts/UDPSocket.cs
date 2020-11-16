﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

//ref https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.udpclient.receive?view=netframework-4.8
class UDPSocket
{
    public string readData;
    private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private IPEndPoint endPoint;// = new IPEndPoint(IPAddress.Loopback, 0);
    private UdpClient receivingUdpClient;
    private Thread udpReadingThread;
    private bool _canReadInThread;
    private int listenPort;
    private string loggingEvent;

    public bool IsAvailable()
    { return receivingUdpClient.Available > 0; }


    public UDPSocket(int readPort)
    {
        listenPort = readPort;
        receivingUdpClient = new UdpClient(listenPort);
        endPoint = new IPEndPoint(IPAddress.Loopback, 5025);
        _canReadInThread = true;
        readData = string.Empty;

        udpReadingThread = new Thread(ReadInBackground);
        udpReadingThread.Start();

        loggingEvent = string.Empty;
    }

    public void Send(string data)
    {
        byte[] sendbuf = Encoding.ASCII.GetBytes(data);

        try
        {
            socket.SendTo(sendbuf, endPoint);
        }
        catch (Exception ex)
        {
            Console.WriteLine("BUG: " + ex.ToString());
        }
        finally
        {
            socket.Close();
        }
    }

    public void ReadAsyncStart()
    {
        //https://stackoverflow.com/questions/7266101/receive-messages-continuously-using-udpclient
        System.Threading.Tasks.Task.Run(async () =>
        {
            using (var udpClient = new UdpClient(listenPort))
            {

                while (true)
                {
                    //IPEndPoint object will allow us to read datagrams sent from any source.
                    var receivedResults = await udpClient.ReceiveAsync();
                    loggingEvent += Encoding.ASCII.GetString(receivedResults.Buffer);
                    Thread.Sleep(25); // prevent CPU to go to 100%
                    udpClient.Close();
                }
            }
        });
    }

    public void AbortConnection()
    {
        _canReadInThread = false;
        udpReadingThread.Join();
        udpReadingThread.Abort();

        receivingUdpClient.Close();
    }

    ~UDPSocket() // Destructor
    {
        _canReadInThread = false;
        udpReadingThread.Join();
        udpReadingThread.Abort();

        receivingUdpClient.Close();
    }

    private void ReadInBackground()
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        while (_canReadInThread)
        {
            receivingUdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);
            if (receivingUdpClient.Available > 0)
            {
                Byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint); // Block if no timeout
                readData = Encoding.ASCII.GetString(receiveBytes);
            }
        }
    }

    private string Read()
    {
        //Creates a UdpClient for reading incoming data.
        //Creates an IPEndPoint to record the IP Address and port number of the sender. 
        // The IPEndPoint will allow you to read datagrams sent from any source.
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        string returnData = string.Empty;
        try
        {

            receivingUdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);
            if (receivingUdpClient.Available > 0)
            {
                // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

                returnData = Encoding.ASCII.GetString(receiveBytes);
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

        return returnData;
    }
}