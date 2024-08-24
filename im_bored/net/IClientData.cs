using System.Net.Sockets;

namespace im_bored.net{
    public interface IClientData{
        public NetworkStream Stream{get;}
        public TcpClient Client{get;}
    }   
}