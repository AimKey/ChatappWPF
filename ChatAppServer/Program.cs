using ChatAppServer.Model;
using ChatAppServer.Net.IO;
using System.Net;
using System.Net.Sockets;

namespace ChatAppServer
{
    internal class Program
    {
        static List<Client> _users;
        static TcpListener _tcpListener;
        static void Main(string[] args)
        {
            // List of clients that are connected to the server
            _users = new List<Client>();

            // make sure it is the same as the one in the NET/Server.cs file in ChatApp
            // For safety use ipconfig in cmd and get your ipv4 address
            //_tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 3000);
            _tcpListener = new TcpListener(IPAddress.Parse("192.168.1.19"), 3000);
            _tcpListener.Start(); // Start listen for incoming connections

            while (true)
            {
                // Start accepting the incoming connections if there are any
                var clientSocket = _tcpListener.AcceptTcpClient();
                var client = new Client(clientSocket);
                _users.Add(client);

                // Broadcast the connection to all user here
                BroadcastConnection();
            }

            Console.ReadKey();

        }
        static void BroadcastConnection()
        {
            // For every user, send other user and current user their details
            foreach (var user in _users)
            {
                foreach (var usr in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1); // 1 is broadcast introduction message
                    broadcastPacket.WriteMessage(usr.Username);
                    broadcastPacket.WriteMessage(usr.Id.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }
        public static void BroadcastMessage(String msg)
        {
            foreach (var user in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(5); // 5 is broadcast message sent by user
                broadcastPacket.WriteMessage(msg);
                broadcastPacket.WriteMessage(user.Username);
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }
        }
        public static void BroadcastDisconnect(String userId)
        {
            var disconnectedUser = _users.Find(x => x.Id.ToString() == userId);
            _users.Remove(disconnectedUser); // Remove it from out list
            foreach (var user in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10); // 10 is broadcast information about user that disconnected
                broadcastPacket.WriteMessage(userId);
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }

            // Broadcast to all users
            BroadcastMessage($"{disconnectedUser.Username} has left the chat.");
        }

        // TODO: Comeback to this later
        public static void BroadCastFile(String path)
        {
            foreach (var user in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(2); // 2 is broadcast file sent by user
                broadcastPacket.WriteFile(path);
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }
        }
    }
}
