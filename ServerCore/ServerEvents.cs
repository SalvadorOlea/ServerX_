using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class ServerEvents
    {
        public static bool OnJoinCheck(UserObject userobj)
        {
            return true;
        }
        public static void OnJoin(UserObject userobj)
        {

        }
        public static void CycleTick()
        {

        }
        public static bool OnPersonalMessage(UserObject userobj, String text)
        {
            return true;
        }

        public static bool OnAvatarReceived(UserObject userobj)
        {
            return true;
        }
        public static void OnPart(UserObject user)
        {

        }
        public static bool OnVroomJoinCheck(UserObject userobj, ushort vroom)
        {
            return true;
        }
        public static void OnVroomPart(UserObject userobj)
        {

        }
        public static void OnVroomJoin(UserObject userobj)
        {

        }
    }
}
