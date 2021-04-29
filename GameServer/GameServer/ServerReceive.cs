using System;
using System.Collections.Generic;
using System.Net;

namespace GameServer
{
    class ServerReceive
    {
        private const int ROOM_ID_LENGTH = 5;
        private const string CHARACTERS = "ABCDEFGHJKLMNPQRSTUVWXYZ123456789";
        private const string DEFAULT_GATEWAY = "192.168.14.1";

        //confirm client has connected
        public static void WelcomeReceived(int clientID, Packet packet)
        {
            int id = packet.ReadInt();

            //set ip
            string ip = Server.clients[clientID].tcp.socket.Client.RemoteEndPoint.ToString().Split(':')[0];
            if(ip == DEFAULT_GATEWAY)
            {
                //remote client
                ip = packet.ReadString();
            }

            Server.clients[clientID].ip = ip;
            Console.WriteLine($"{ip} connected");

            //check ID's match
            if (clientID != id)
            {
                Console.WriteLine($"{ip} has wrong ID: {id} instead of {clientID}");
            }

            //check if ip is valid
            IPAddress addr = new IPAddress(0);
            if(!IPAddress.TryParse(ip, out addr))
            {
                Console.WriteLine($"{ip} has invalid IP address");
            }
        }

        //let player open a lobby
        public static void StartHost(int clientID, Packet packet)
        {
            string roomID = GenerateRoomID();
            string hostIP = Server.clients[clientID].ip;

            //add {room ID, player IP} to room dict
            Server.activeRooms.Add(roomID, hostIP);
            
            //send room ID
            ServerSend.GetRoomID(clientID, roomID);

            Console.WriteLine($"{hostIP} started hosting with ID: {roomID}");
        }

        //return random string
        static string GenerateRoomID()
        {
            Random rand = new Random();
            string roomName = "";

            for (int i = 0; i < ROOM_ID_LENGTH; i++)
            {
                roomName += CHARACTERS[rand.Next(CHARACTERS.Length)];
            }

            return roomName;
        }

        //send ip of client with given room code
        public static void RequestHostIP(int clientID, Packet packet)
        {
            string roomID = packet.ReadString();
            Console.WriteLine($"{Server.clients[clientID].ip} requested room with ID: {roomID}");

            //check if ID is of length
            if (roomID.Length != ROOM_ID_LENGTH)
            {
                ServerSend.GetHostIP(clientID, "");
                return;
            }

            try
            {
                ServerSend.GetHostIP(clientID, Server.activeRooms[roomID]);
            }
            catch (KeyNotFoundException)
            {
                ServerSend.GetHostIP(clientID, "");
            }
        }

        //delete entry from room dict
        public static void CloseRoom(int clientID, Packet packet)
        {
            string roomID = packet.ReadString();
            string hostIP = Server.clients[clientID].ip;

            Server.activeRooms.Remove(roomID);
            Console.WriteLine($"{hostIP} closed room with ID: {roomID}");
        }

        public static void LoginUser(int clientID, Packet packet)
        {
            string username = packet.ReadString();
            string password = packet.ReadString();
            Console.WriteLine($"{Server.clients[clientID].ip} attempted to log in with username {username} and password {password}");

            //fetch data
            string real_password;
            int accountID, colour1, colour2;
            bool success = Database.GetData(out accountID, username, out real_password, out colour1, out colour2);

            if(success)
            {
                if(accountID > -1)
                {
                    if(real_password == password)
                    {
                        //allow access to account
                        Server.clients[clientID].accountID = accountID;
                        ServerSend.LoginResponse(clientID, true, "", colour1, colour2);
                    }
                    else
                    {
                        //result doesn't match user input
                        ServerSend.LoginResponse(clientID, false, "Error: Incorrect password", -1, -1);
                    }
                }
                else
                {
                    //no results in database
                    ServerSend.LoginResponse(clientID, false, "Error: Username invalid", colour1, colour2);
                }
            }
            else
            {
                //exception at database
                ServerSend.LoginResponse(clientID, false, "Couldn't fetch data. Please try again.", colour1, colour2);
            }
        }

        //create entry in DB
        public static void RegisterUser(int clientID, Packet packet)
        {
            string username = packet.ReadString();
            string password = packet.ReadString();

            //fetch data
            string real_password;
            int accountID, colour1, colour2;
            bool success = Database.GetData(out accountID, username, out real_password, out colour1, out colour2);

            if(success)
            {
                if(accountID == -1)
                {
                    //create user
                    success = Database.RegisterUser(username, password);

                    if (success)
                    {
                        Server.clients[clientID].accountID = accountID;
                        Console.WriteLine($"{Server.clients[clientID].ip} registered user with username {username} and password {password}");
                        ServerSend.RegisterResponse(clientID, true, "");
                    }
                    else
                    {
                        ServerSend.RegisterResponse(clientID, false, "Couldn't complete operation. Please try again.");
                    }
                }
                else
                {
                    //user exists in database
                    ServerSend.RegisterResponse(clientID, false, "Error: User already exists");
                }
            }
            else
            {
                //exception at database
                ServerSend.RegisterResponse(clientID, false, "Couldn't complete operation. Please try again.");
            }
        }

        //set colours in database
        public static void SetColours(int clientID, Packet packet)
        {
            int colour1 = packet.ReadInt();
            int colour2 = packet.ReadInt();

            //set colours
            bool success = Database.SetColours(Server.clients[clientID].accountID, colour1, colour2);
            Console.WriteLine($"{Server.clients[clientID].ip} set their colours as {colour1} and {colour2}");
            ServerSend.ColourResponse(clientID, success);
        }
    }
}
