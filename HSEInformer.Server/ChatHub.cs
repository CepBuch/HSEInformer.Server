using HSEInformer.Server.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSEInformer.Server
{
    public class ChatHub : Hub
    {

        static List<HubUser> Users = new List<HubUser>();

        // Отправка сообщений
        public void SendMessage(string name, string message)
        {
            Clients.All.UpdateChatMessage(name, message);
        }

        //// Подключение нового пользователя
        //public void Connect(string userName)
        //{
        //    var id = Context.ConnectionId;


        //    if (!Users.Any(x => x.ConnectionId == id))
        //    {
        //        Users.Add(new HubUser { ConnectionId = id, Name = userName });
        //        // Посылаем сообщение текущему пользователю
        //        Clients.Caller.onConnected(id, userName, Users);

        //        // Посылаем сообщение всем пользователям, кроме текущего
        //        Clients.AllExcept(id).onNewUserConnected(id, userName);
        //    }
        //}

        // Отключение пользователя
        //public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        //{
        //    var item = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        //    if (item != null)
        //    {
        //        Users.Remove(item);
        //        var id = Context.ConnectionId;
        //        Clients.All.onUserDisconnected(id, item.Name);
        //    }

        //    return base.OnDisconnected(stopCalled);
        //}
    }
}