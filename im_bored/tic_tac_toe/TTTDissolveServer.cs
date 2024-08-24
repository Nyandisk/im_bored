using System.Net;
using System.Net.Sockets;
using im_bored.net;

namespace im_bored.tic_tac_toe{
    public class TTTDissolveServer {
        public enum TTTDissolveReason{
            Unknown = 0,
            Malformed,
            ServerFull,
            UserExists,
            Timeout
        }
        public readonly struct TTTDissolveClientData(TcpClient client, NetworkStream stream) : IClientData{
            public NetworkStream Stream{get;} = stream;
            public TcpClient Client{get;} = client;
        }
        public event EventHandler<ClientJoinEvent>? ClientJoined;
        public event EventHandler<ClientLeaveEvent>? ClientLeft;
        public event EventHandler<PacketReceivedEvent>? PacketReceived;
        private readonly Dictionary<string, TTTDissolveClientData> _playerList = [];
        private readonly TcpListener _listener;
        private readonly bool _quiet;
        private readonly object _logLock = new();
        private readonly uint _maxPlayers;
        private static readonly int _bufferSize = 1024;
        public TTTDissolveServer(ushort port, uint maxPlayers, bool quiet = false){
            _listener = new(IPAddress.Any,port);
            _quiet = quiet;
            _maxPlayers = maxPlayers;
        }
        private void ServerLog(object loggable){
            if (_quiet) return;
            lock(_logLock){
                Console.WriteLine(loggable);
            }
        }
        private static string GetIP(TcpClient client){
            return ((IPEndPoint)client.Client.RemoteEndPoint!).Address.ToString();
        }
        private TcpClient? GetClient(string username){
            try{
                return _playerList[username].Client;
            }catch(KeyNotFoundException){
                return null;
            }
        }
        private void RemovePlayer(string username){
            ClientLeft?.Invoke(this,new(_playerList[username]));
            _playerList.Remove(username);
        }
        private void DisconnectPlayer(string username, TTTDissolveReason reason){
            TTTDissolveClientData clientData = _playerList[username];
            clientData.Stream.Write(new Packet(PacketHeader.DISCONNECT,[(byte)reason]));
            clientData.Client.Close();
            ClientLeft?.Invoke(this,new(clientData));
            _playerList.Remove(username);
        }
        private void ProcessClient(TcpClient client){
            ServerLog($"Accepting client {GetIP(client)}");
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[_bufferSize];
            int dataRead = stream.Read(buffer,0,buffer.Length);
            if (dataRead == 0){
                ServerLog($"Client connection closed while waiting for username");
                client.Close();
                return;   
            }
            Packet packet = new(buffer[..dataRead]);
            if (packet.Header != PacketHeader.USERNAME){
                ServerLog($"Client sent malformed packet");
                client.GetStream().Write(new Packet(PacketHeader.DISCONNECT,[(byte)TTTDissolveReason.Malformed]));
                client.Close();
                return;
            }
            string username = packet.StringData;
            if (GetClient(username) != null){
                ServerLog($"Player by the username '{username}' already exists");
                client.GetStream().Write(new Packet(PacketHeader.DISCONNECT,[(byte)TTTDissolveReason.UserExists]));
                client.Close();
                return;
            }
            ServerLog($"New client joined: {username}");
            TTTDissolveClientData clientData = new(client, stream);
            _playerList[username] = clientData;
            ClientJoined?.Invoke(this,new(clientData));
            while(client.Connected){
                buffer = new byte[_bufferSize];
                dataRead = stream.Read(buffer,0,buffer.Length);
                if (dataRead == 0){
                    ServerLog($"Client {username} connection closed");
                    RemovePlayer(username);
                    return;   
                }
                packet = new(buffer[..dataRead]);
                PacketReceived?.Invoke(this,new(packet,clientData));
            }
        }
        public void Run(){
            _listener.Start();
            while(true){
                ServerLog("Waiting for new connections...");
                TcpClient client = _listener.AcceptTcpClient();
                if (_playerList.Count >= _maxPlayers){
                    ServerLog("Disconnecting new client: Server full");
                    client.GetStream().Write(new Packet(PacketHeader.DISCONNECT,[(byte)TTTDissolveReason.ServerFull]));
                    client.Close();
                    continue;
                }
                Thread clientThread = new(() => {ProcessClient(client);});
                clientThread.Start();
            }
        }
    }
}