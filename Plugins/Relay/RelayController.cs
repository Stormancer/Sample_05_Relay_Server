using Server.Plugins.API;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stormancer;
using Stormancer.Plugins;
using Stormancer.Diagnostics;
using Stormancer.Core;

namespace Server.Plugins.Relay
{


    public class RelayController : ControllerBase
    {

        private readonly ILogger _logger;
        private readonly ISceneHost _scene;

        public RelayController(ISceneHost scene, ILogger logger)
        {
            _logger = logger;
            _scene = scene;

        }

        public Task rpc(RequestContext<IScenePeerClient> ctx)
        {
            return Task.WhenAll(_scene.RemotePeers.ToArray()
                .Where(p => p.Id != ctx.RemotePeer.Id)
                .Select(p => p.RpcVoid("relay.rpc", s =>
                {
                    ctx.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
                    ctx.InputStream.CopyTo(s);
                }))
                );
        }

        public Task route(Packet<IScenePeerClient> packet)
        {
            var reliability = (PacketReliability)packet.Stream.ReadByte();
            var peers = _scene.RemotePeers.Where(p => p.Id != packet.Connection.Id);
            _scene.Send(new MatchArrayFilter(peers), "relay.route", s =>
            {
                packet.Stream.CopyTo(s);
            }, PacketPriority.MEDIUM_PRIORITY, reliability);

            return Task.FromResult(true);
        }

        public Task rpc2Player(RequestContext<IScenePeerClient> ctx)
        {
            var destinationId = ctx.ReadObject<long>();
            return Task.WhenAll(_scene.RemotePeers.ToArray()
                .Where(p => p.Id == destinationId)
                .Select(p => p.RpcVoid("relay.rpc", s =>
                {
                    ctx.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
                    ctx.InputStream.CopyTo(s);
                }))
                );
        }

        public Task route2Player(Packet<IScenePeerClient> packet)
        {

            var reliability = (PacketReliability)packet.Stream.ReadByte();
            var destinationId = packet.ReadObject<long>();

            _scene.Send(new MatchPeerFilter(destinationId), "relay.route", s =>
            {
                packet.Stream.CopyTo(s);
            }, PacketPriority.MEDIUM_PRIORITY, reliability);

            return Task.FromResult(true);
        }


    }


}
