using System;
using System.Collections.Generic;

namespace GameServer
{
    class ServerReceive
    {
        private const int ROOM_ID_LENGTH = 5;

        public static void WelcomeReceived(int clientID, Packet packet)
        {
            int id = packet.ReadInt();
            Console.WriteLine($"{Server.clients[clientID].ip} connected successfully");

            //check ID's match
            if (clientID != id)
            {
                Console.WriteLine($"{Server.clients[clientID].ip} has wrong ID: {id} instead of {clientID}");
            }
        }

        public static void StartHost(int clientID, Packet packet)
        {
            string roomID = packet.ReadString();

            //check if ID is of length
            if (roomID.Length != ROOM_ID_LENGTH || roomID == "")
            {
                ServerSend.ValidRoomID(clientID, false);
                return;
            }

            //approve client to start hosting
            ServerSend.ValidRoomID(clientID, true);

            //add {ID, IP} to game dict
            string hostIP = Server.clients[clientID].ip;
            Server.activeRooms.Add(roomID, hostIP.Substring(0, hostIP.IndexOf(':'))); //cut out the port out of host's IP
            Console.WriteLine($"{hostIP} started hosting with ID: {roomID}");
        }

        public static void RequestHostIP(int clientID, Packet packet)
        {
            string roomID = packet.ReadString();
            Console.WriteLine($"{Server.clients[clientID].ip} requested room with ID: {roomID}");

            //check if ID is of length
            if (roomID.Length != ROOM_ID_LENGTH || roomID == "")
            {
                ServerSend.ValidRoomID(clientID, false);
                return;
            }

            try
            {
                ServerSend.GetHostIP(clientID, Server.activeRooms[roomID]);
            }
            catch (KeyNotFoundException)
            {
                ServerSend.RoomNotFound(clientID);
            }
        }

        public static void CloseRoom(int clientID, Packet packet)
        {
            string roomID = packet.ReadString();
            string hostIP = Server.clients[clientID].ip;

            //only client who started the room can close it
            if(hostIP.Substring(0, hostIP.IndexOf(':')) == Server.activeRooms[roomID])
            {
                Server.activeRooms.Remove(roomID);
                Console.WriteLine($"{hostIP} closed room with ID: {roomID}");
            }
        }

        public static void LoginUser(int clientID, Packet packet)
        {
            string username = packet.ReadString();
            string password = packet.ReadString();
            Console.WriteLine($"{Server.clients[clientID].ip} attempted to log in with username {username} and password {password}");

            //search in database
            string real_password = Database.GetPassword(username).Trim();
            if(real_password == "")
            {
                //no results in database
                ServerSend.ValidLogin(clientID, false, "Error: Username invalid");
            }
            else if(real_password == "-")
            {
                //exception at database
                ServerSend.ValidLogin(clientID, false, "Couldn't fetch data. Please try again.");
            }
            else if(real_password != password)
            {
                //result doesn't match user input
                ServerSend.ValidLogin(clientID, false, "Error: Incorrect password");
            }
            else
            {
                ServerSend.ValidLogin(clientID, true, "");
            }
        }

        public static void RegisterUser(int clientID, Packet packet)
        {
            string username = packet.ReadString();
            string password = packet.ReadString();
            string real_password = Database.GetPassword(username);

            //username length
            if (username.Length <= 3)
            {
                ServerSend.ValidRegister(clientID, false, "Error: Username too short");
            }
            //password length
            else if(password.Length <= 5)
            {
                ServerSend.ValidRegister(clientID, false, "Error: Password too short");
            }
            //search in database
            else if(real_password != "" && real_password != "-")
            {
                ServerSend.ValidRegister(clientID, false, "Error: User already exists");
            }
            else
            {
                //create user
                if(!Database.InsertData(username, password) || real_password == "-")
                {
                    //exception at database
                    ServerSend.ValidRegister(clientID, false, "Couldn't fetch data. Please try again.");
                }
                else
                {
                    Console.WriteLine($"{Server.clients[clientID].ip} registered user with username {username} and password {password}");
                    ServerSend.ValidRegister(clientID, true, "");
                }
            }
        }
    }
}
