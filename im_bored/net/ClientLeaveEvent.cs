using System.Net.Sockets;

namespace im_bored.net{
    public class ClientLeaveEvent(IClientData client) : EventArgs{
        public IClientData Client{get;} = client;
    }
}