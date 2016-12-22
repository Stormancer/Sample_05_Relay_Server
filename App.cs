using Stormancer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Plugins.API;
using Stormancer.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Server
{
    public class App
    {
        public const string GAMESESSION_SCENE_TYPE = "gameSession";
        public void Run(IAppBuilder builder)
        {
            
            
            
           

            builder.SceneTemplate("relay", scene =>
            {
                scene.AddRelay();

            });

           

          
        }
    }
}
