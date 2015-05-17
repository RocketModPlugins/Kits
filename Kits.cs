using Rocket.Unturned.Plugins;
using SDG;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace unturned.ROCKS.Kits
{
    public class Kits : RocketPlugin<KitsConfiguration>
    {
        public static Kits Instance = null;

        public static Dictionary<string, DateTime> GlobalCooldown = new Dictionary<string,DateTime>();
        public static Dictionary<string, DateTime> InvididualCooldown = new Dictionary<string, DateTime>();

        protected override void Load()
        {
            Instance = this;
        }

        public override Dictionary<string, string> DefaultTranslations
        {
            get
            {
                return new Dictionary<string, string>(){
                    {"command_kit_invalid_parameter","Invalid parameter, specify a kit with /kit <name>"},
                    {"command_kit_not_found","Kit not found"},
                    {"command_kit_no_permissions","You don't have permissions to use this kit"},
                    {"command_kit_cooldown_command","You have to wait {0} seconds to use this command again"},
                    {"command_kit_cooldown_kit","You have to wait {0} seconds to get this kit again"},
                    {"command_kit_failed_giving_item","Failed giving a item to {0} ({1},{2})"},
                    {"command_kit_success","You just received the kit {0}" }
                };
            }
        }
    }
}
