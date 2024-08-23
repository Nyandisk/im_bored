using System.Text;

namespace im_bored.net{
    public enum PacketHeader{
        USERNAME,
        DISCONNECT
    }
    public class Packet{
        public PacketHeader Header{get;set;}
        public byte[] Data{get;set;}
        public string StringData{get{return Encoding.UTF8.GetString(Data);}}
        public int Size{get{return Data.Length+1;}}
        public Packet(PacketHeader header){
            Header = header;
            Data = [];
        }
        public Packet(PacketHeader header, byte[] data){
            Header = header;
            Data = data;
        }
        public Packet(PacketHeader header, string data){
            Header = header;
            Data = Encoding.UTF8.GetBytes(data);
        }
        public byte[] GetBytes(){
            byte[] bytes = new byte[Data.Length + 1];
            bytes[0] = (byte)Header;
            Array.Copy(Data,0,bytes,1,Data.Length);
            return bytes;
        }
        public override string ToString(){
            return $"{Header}|{StringData}({Data})|{Size}";
        }
    }
}
