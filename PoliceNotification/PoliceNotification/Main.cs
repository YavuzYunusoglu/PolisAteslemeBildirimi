using Rocket.API.Collections;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PoliceNotification
{
    public class Main : RocketPlugin<Configuration>
    {
        public static Main Instance { get; private set; }
        protected override void Load()
        {
            UseableGun.onBulletSpawned += onFireGun;
            Instance = this;
        }
        protected override void Unload()
        {
            Instance = null;
            UseableGun.onBulletSpawned -= onFireGun;
        }

        private void onFireGun(UseableGun gun, BulletInfo bullet)
        {
            UnturnedPlayer player = UnturnedPlayer.FromPlayer(gun.player);
            if (player == null) return;
            if(!Cooldown.ContainsKey(player.CSteamID))
            {
                Cooldown.Add(player.CSteamID, DateTime.Now);
            }
            double totalSecond = (DateTime.Now - Cooldown[player.CSteamID]).TotalSeconds;
            if (totalSecond < Configuration.Instance.cooldownSuresi) return;
            Cooldown[player.CSteamID] = DateTime.Now;
            LocationNode node = LevelNodes.nodes.OfType<LocationNode>().OrderBy(k => Vector3.Distance(k.point, player.Position)).FirstOrDefault();
            foreach(SteamPlayer p in Provider.clients)
            {
                UnturnedPlayer police = UnturnedPlayer.FromSteamPlayer(p);
                foreach (RocketPermissionsGroup group in R.Permissions.GetGroups(police, true))
                {
                    if (group.Id == Configuration.Instance.polisID)
                    {
                        police.Player.quests.askSetMarker(police.CSteamID, true, player.Position);
                        ChatManager.serverSendMessage($"<size=15><color=#00FFFF>[Polis]</color></size> {Translate("AteslemeBildirimi", node.name)}", Color.white, p, p, EChatMode.GLOBAL, Configuration.Instance.polisIcon, true);
                    }
                }
            }
        }
        public Dictionary<CSteamID, DateTime> Cooldown = new Dictionary<CSteamID, DateTime>();
        public override TranslationList DefaultTranslations => new TranslationList
        {
            {"AteslemeBildirimi","Birisi {0} bölgesinde silah ateşledi, haritanda işaretledik. Gidip bir kontrol et!" }
        };
    }
}
