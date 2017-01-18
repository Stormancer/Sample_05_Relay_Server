using Stormancer.Core;
using Stormancer.Plugins;
using Stormancer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Plugins.API;
using Stormancer.Server;

namespace Server.Plugins.Relay
{
    public class Player
    {
        public byte[] UserData;
        public long ConnectionId;
    }
    class RelayPlugin : IHostPlugin
    {
        internal const string METADATA_KEY = "stormancer.relay";

        public void Build(HostPluginBuildContext ctx)
        {
            ctx.HostDependenciesRegistration += (IDependencyBuilder builder) =>
              {
                  builder.Register<RelayController>().InstancePerRequest();

              };

            ctx.SceneCreated += (ISceneHost scene) =>
             {
                 if (scene.Metadata.ContainsKey(METADATA_KEY))
                 {
                     scene.AddController<RelayController>();

                     scene.Connected.Add(peer =>
                     {
                         foreach (var p in scene.RemotePeers)
                         {
                             if (p.Id != peer.Id)
                             {
                                 peer.Send("players.add", new Player { ConnectionId = p.Id, UserData = p.UserData });
                             }
                         }

                         scene.Broadcast("players.add", new Player { ConnectionId = peer.Id, UserData = peer.UserData });
                         return Task.FromResult(true);
                     });

                     scene.Disconnected.Add(disconnectionEvent =>
                     {
                         scene.Broadcast("players.remove", disconnectionEvent.Peer.Id);
                         return Task.FromResult(true);
                     });
                 }
             };
        }
    }
}
