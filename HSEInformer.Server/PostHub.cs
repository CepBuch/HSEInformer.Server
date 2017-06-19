using HSEInformer.Server.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSEInformer.Server
{
    public class PostHub : Hub
    {

        //[Authorize]
        //public void SendMessage(string message)
        //{
        //    //var username = Context.User.Identity.Name;
        //    Clients.All.UpdateChatMessage(Context.User.Identity.Name, message);
        //}


        //public override Task OnConnected()
        //{
        //    if (Context.User.Identity.IsAuthenticated)
        //    {
        //        Groups.Add(Context.User.Identity.Name, "authenticated");
        //    }
        //    else
        //    {
        //        Groups.Add(Context.ConnectionId, "anonymous");
        //    }
        //    return base.OnConnected();
        //}

        //public override Task OnDisconnected(bool stopCalled)
        //{
        //    RemoveFromSecurityGroups();

        //    return base.OnDisconnected(stopCalled);
        //}

        //private void RemoveFromSecurityGroups()
        //{
        //    Groups.Remove(Context.ConnectionId, "authenticated");
        //    Groups.Remove(Context.ConnectionId, "anonymous");
        //}

        //[Microsoft.AspNetCore.Authorization.Authorize]
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