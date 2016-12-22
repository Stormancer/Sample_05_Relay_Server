using Server.Plugins.Relay;
using Stormancer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormancer
{
    public static class RelayExtensions
    {
        public static void AddRelay(this ISceneHost scene)
        {
            scene.Metadata[RelayPlugin.METADATA_KEY] = "enabled";
        }
    }
}
