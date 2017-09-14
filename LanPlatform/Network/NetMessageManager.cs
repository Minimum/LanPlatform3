using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using LanPlatform.Accounts;
using LanPlatform.DAL;
using LanPlatform.Models;

namespace LanPlatform.Network
{
    public class NetMessageManager
    {
        private PlatformContext Context;

        public NetMessageManager(AppInstance instance)
        {
            Context = instance.Context;
        }

        public static void AddMessageSingleQuick(AppInstance instance, long userId, NetMessage message)
        {
            NetMessageManager manager = new NetMessageManager(instance);

            message.Targets.Add(userId);

            manager.AddMessage(message);

            return;
        }

        public static void AddMessageQuick(AppInstance instance, NetMessage message)
        {
            NetMessageManager manager = new NetMessageManager(instance);

            manager.AddMessage(message);

            return;
        }

        public static void AddMessageBroadcastQuick(AppInstance instance, NetMessage message)
        {
            NetMessageManager manager = new NetMessageManager(instance);

            manager.AddMessageBroadcast(message);

            return;
        }

        public void AddMessage(NetMessage message)
        {
            Context.NetMessage.Add(new NetMessageOutput(message));

            return;
        }

        public void AddMessageBroadcast(NetMessage message)
        {
            message.Targets.Add(0);

            AddMessage(message);

            return;
        }

        public List<NetMessageOutput> GetMessageOutputs(long accountId, long startId, long count)
        {
            return Context.NetMessage.Where(
                s =>
                    Context.NetMessageTarget.Where(target => target.User == accountId)
                        .Select(access => access.Id)
                        .Contains(s.Id))
                .ToList();
        }

        public long GetLastMessageId(long accountId)
        {
            NetMessageOutput message = Context.NetMessage.Where(
                s =>
                    Context.NetMessageTarget.Where(target => target.User == accountId)
                        .Select(access => access.Id)
                        .Contains(s.Id))
                .OrderByDescending(s => s.Id)
                .SingleOrDefault();

            return (message == null) ? 0 : message.Id;
        }
    }
}