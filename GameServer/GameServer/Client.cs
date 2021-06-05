//class that handles a connected client
//has methods for connection, sending and receiving data
//reads packets and executes commands using the thread opened on the main program


using System;
using System.Net.Sockets;

namespace GameServer
{
    class Client
    {
        public static int dataBufferSize = 4096;

        public int id;
        public TCP tcp;
        public string ip;
        public int accountID;

        public Client(int _id)
        {
            id = _id;
            tcp = new TCP(id);
        }

        private void Disconnect()
        {
            Console.WriteLine($"{ip} disconnected");
            tcp.Disconnect();
        }

        public class TCP
        {
            public TcpClient socket;

            private int id;
            private NetworkStream stream;
            private byte[] receiveBuffer;
            private Packet packetData;

            public TCP(int _id)
            {
                id = _id;
            }

            //creates object to handle client
            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();
                receiveBuffer = new byte[dataBufferSize];
                packetData = new Packet();

                //receive data
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                //send welcome
                ServerSend.Welcome(id, "Connected to server");
            }

            //sends a packet to the client
            public void SendData(Packet packet)
            {
                try
                {
                    if (socket == null)
                    {
                        return;
                    }

                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error at send data to player {id}: {ex}");
                }
            }

            //async callback to receive packet
            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    //get stream length
                    int length = stream.EndRead(result);
                    if (length <= 0)
                    {//no data
                        Server.clients[id].Disconnect();
                        return;
                    }

                    //copy data in buffer
                    byte[] data = new byte[length];
                    Array.Copy(receiveBuffer, data, length);

                    //handle data
                    packetData.Reset(HandleData(data));

                    //continue receiving data
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error at receive data to player {id}: {ex}");
                    Server.clients[id].Disconnect();
                }
            }

            //reads packet
            //returns if packet should be reset
            private bool HandleData(byte[] data)
            {
                int length = 0; //packet length
                packetData.SetBytes(data); //byte[] data -> packet class

                if (packetData.UnreadLength() >= 4)
                {
                    //first int is packet length
                    length = packetData.ReadInt();
                    if (length <= 0)
                    {//packet is empty
                        return true;
                    }
                }

                //iterate over packet
                while (length > 0 && packetData.UnreadLength() <= length)
                {
                    byte[] packetBytes = packetData.ReadBytes(length);

                    //create a thread
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        Packet packet = new Packet(packetBytes);
                        int packetID = packet.ReadInt(); //get packet id
                        Server.onPacketReceive[packetID](id, packet); //invoke command for packet

                        packet.Dispose();
                    });

                    length = 0;
                    if (packetData.UnreadLength() >= 4)
                    {
                        length = packetData.ReadInt();
                        if (length <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (length <= 1)
                {
                    return true;
                }

                return false;
            }

            public void Disconnect()
            {
                socket.Close();
                stream = null;
                packetData = null;
                receiveBuffer = null;
                socket = null;
            }
        }
    }
}
