using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class TcpPacketProcessor
    {
        public static void Evaluate(UserObject userobj, ProtoMessage msg, AresTCPPacketReader packet, uint time)
        {
            UserPool.Broadcast(AresTcpPackets.NoSuch(msg.ToString()));
            if (!userobj.LoggedIn)
                if (msg > ProtoMessage.MSG_CHAT_CLIENT_LOGIN)
                    throw new Exception();
            switch (msg)
            {
                case ProtoMessage.MSG_CHAT_CLIENT_LOGIN:
                    Login(userobj, packet, time);
                    break;
                case ProtoMessage.MSG_CHAT_CLIENT_PUBLIC:
                    PublicText(userobj, packet);
                    break;
                case ProtoMessage.MSG_CHAT_CLIENT_AVATAR:
                    Avatar(userobj, packet);
                    break;
                case ProtoMessage.MSG_CHAT_CLIENT_PERSONAL_MESSAGE:
                    String text = packet.ReadString();
                    if (ServerEvents.OnPersonalMessage(userobj, text))
                        userobj.PersonalMessage = text;
                    break;
                case ProtoMessage.MSG_CHAT_CLIENT_UPDATE_STATUS:
                    
                    break;
                case ProtoMessage.MSG_CHAT_SERVER_UPDATE_USER_STATUS:
                    userobj.LastFastPing = time;
                    AresTcpPackets.UpdateUserStatus(userobj, userobj);
                    break;
                case ProtoMessage.MSG_CHAT_CLIENT_COMMAND:
                    //Command(userobj, packet.ReadString());
                    break;

            }
        }
        private static void Login(UserObject userobj, AresTCPPacketReader packet, uint time)
        {
            userobj.PopulateCredentials(packet);
            if (!ServerEvents.OnJoinCheck(userobj))
            {
                userobj.Expired = true;
                userobj.LoggedIn = userobj.Ghost;
            }
            if (!userobj.Ghost)
                UserPool.BroadcastToVroom(userobj.Vroom, AresTcpPackets.Join(userobj));

            userobj.LoggedIn = true;
            userobj.SendPacket(AresTcpPackets.LoginAck(userobj));
            userobj.SendPacket(AresTcpPackets.MyFeatures(userobj));
            userobj.SendPacket(AresTcpPackets.TopicFirst());
            UserPool.SendUserList(userobj);
            userobj.SendPacket(AresTcpPackets.OpChange(userobj));
            userobj.SendPacket(Avatars.Server(userobj));
            ServerEvents.OnJoin(userobj);
        }
        private static void Avatar(UserObject userobj, AresTCPPacketReader packet)
        {
            byte[] buff = packet.ReadBytes();
            if(!userobj.Avatar.SequenceEqual(buff))
            {
                if (ServerEvents.OnAvatarReceived(userobj))
                {
                    if (!userobj.Expired)
                    {
                        userobj.Avatar = buff;
                        userobj.avatar = buff;
                        if (userobj.Avatar.Length < 20)
                            userobj.Avatar = Avatars.default_avatar;
                    }
                }
            }
            Avatars.CheckAvatars();
        }
        private static void PublicText(UserObject userobj, AresTCPPacketReader packet)
        {
            String text = packet.ReadString();

            //UserPool.Users.ForEach(x => { x.SendPacket(AresTcpPackets.Public(userobj.Name, text)); });
            UserPool.Users.ForEach(x => {
                if (x.LoggedIn && x.Vroom == userobj.Vroom)
                {
                    if (!x.Ignores.Contains(userobj.Name))
                    {
                        x.SendPacket(AresTcpPackets.NoSuch(userobj.Avatar.Length.ToString()));
                        if (String.IsNullOrEmpty(userobj.CustomName))
                            x.SendPacket(AresTcpPackets.Public(userobj.Name, text));
                        else
                            x.SendPacket(AresTcpPackets.NoSuch(userobj.CustomName + text));
                    }
                }
            });
        }
    }
}
