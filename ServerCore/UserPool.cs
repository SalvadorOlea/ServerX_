using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerCore
{
    class UserPool
    {
        public static List<UserObject> Users;

        public static void Init()
        {
            Users = new List<UserObject>();
        }
        public static void SetID(UserObject userobj)
        {
            userobj.ID = -1;
            int id = 0;

            while (true)
            {
                if (Users.Find(x => x.ID == id) != null)
                    id++;
                else
                {
                    userobj.ID = id;
                    break;
                }
            }

            if (Users.Count > 1)
                Users.Sort((x, y) => x.ID.CompareTo(y.ID));
        }

        public static void Broadcast(byte[] data)
        {
            Users.ForEach(x => { if (x.LoggedIn) x.SendPacket(data); });
        }

        public static void BroadcastToVroom(ushort vroom, byte[] data)
        {
            Users.ForEach(x => { if (x.LoggedIn && x.Vroom == vroom) x.SendPacket(data); });
        }

        public static ushort UserCount
        {
            get
            {
                ushort result = 0;
                Users.ForEach(x => { if (x.LoggedIn) result++; });
                return result;
            }
        }
        public static bool CanChangeName(UserObject userobj, String name)
        {
            foreach (String i in Illegal)
                if (Regex.IsMatch(name, Regex.Escape(i), RegexOptions.IgnoreCase))
                    return false;

            if (Encoding.UTF8.GetByteCount(name) > 20)
                return false;

            if (Encoding.UTF8.GetByteCount(name) < 2)
                return false;

            if (name == Settings.Get<String>("bot"))
                return false;

            return Users.Find(x => (x.LoggedIn && x.ID != userobj.ID) && (x.Name == name || x.OrgName == name)) == null;
        }
        private static String[] Illegal = new String[]
        {
            "￼", "", "­", "/", "\\", "www."
        };

        public static String PrepareUserName(UserObject userobj)
        {
            String str = userobj.OrgName;


            str = str.Replace("_", " ");

            while (Encoding.UTF8.GetByteCount(str) > 20)
                str = str.Substring(0, str.Length - 1);

            if (Encoding.UTF8.GetByteCount(str) < 2)
                return "anon " + userobj.Cookie;

            if (str == Settings.Get<String>("bot"))
                return null;

            if (Users.Find(x => x.LoggedIn && (x.Name == str || x.OrgName == str)) != null) // name in use
            {
                UserObject u = Users.Find(x => x.LoggedIn && (x.Name == str || x.OrgName == str) && (x.ExternalIP.Equals(userobj.ExternalIP) || x.Guid.Equals(userobj.Guid)));

                if (u == null)
                    return null;
                else
                {
                    u.Expired = true;
                    userobj.Ghost = true;
                    return str;
                }
            }

            return str;
        }
        public static void SendUserList(UserObject userobj)
        {
            userobj.SendPacket(AresTcpPackets.UserListBotItem());
            Users.ForEach(x => { if (x.LoggedIn && x.Vroom == userobj.Vroom) userobj.SendPacket(AresTcpPackets.UserListItem(x)); });
            userobj.SendPacket(AresTcpPackets.UserListEnd());

            Users.ForEach(x =>
            {
                if (x.LoggedIn && x.Vroom == userobj.Vroom)
                {
                    if (x.Avatar.Length > 0)
                        userobj.SendPacket(AresTcpPackets.Avatar(x));

                    if (!String.IsNullOrEmpty(x.PersonalMessage))
                        userobj.SendPacket(AresTcpPackets.PersonalMessage(x));
                }
            });
        }
        public static void SendUserFeatures(UserObject userobj)
        {
            Users.ForEach(x =>
            {
                if (x.LoggedIn && x.Vroom == userobj.Vroom && x.ID != userobj.ID)
                {
                    if (userobj.Avatar.Length > 0)
                        x.SendPacket(AresTcpPackets.Avatar(userobj));

                    if (!String.IsNullOrEmpty(userobj.PersonalMessage))
                        x.SendPacket(AresTcpPackets.PersonalMessage(userobj));

                }
            });
        }
    }
}
