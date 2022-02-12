using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceNotification
{
    public class Configuration : IRocketPluginConfiguration
    {
        public double cooldownSuresi;
        public string polisID;
        public string polisIcon;
        public void LoadDefaults()
        {
            polisIcon = "https://image.flaticon.com/icons/png/512/190/190624.png";
            cooldownSuresi = 7;
            polisID = "Polis";
        }
    }
}
