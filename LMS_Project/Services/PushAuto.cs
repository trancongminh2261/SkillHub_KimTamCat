using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LMS_Project.Services
{
    public class PushAuto
    {
        /// <summary>
        /// Tự động chạy 1 phút 1 lần
        /// </summary>
        public static void PushOneMinute()
        {
            Task.Run(() => {
                ///Tự động tắt phòng zoom
                ZoomRoomService.AutoCloseRoom();
                NotificationInVideoService.SeenNotification();
            });
        }
        public static void PushOneDay()
        {
            Task.Run(() =>
            {
                ///Tự động khoá tài khoản
                //UserInformation.AutoInActive();
                //Account.PushNotiRemindStudy();
            });
        }
    }
}