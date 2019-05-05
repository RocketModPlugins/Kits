using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;

namespace fr34kyn01535.Kits
{ 
    public class Kits : RocketPlugin<KitsConfiguration>
    {
        public static Kits Instance = null;

        public static Dictionary<string, DateTime> GlobalCooldown = new Dictionary<string,DateTime>();
        public static Dictionary<string, DateTime> InvididualCooldown = new Dictionary<string, DateTime>();

        protected override void Load()
        {
            Instance = this;
            if (IsDependencyLoaded("Uconomy"))
            {
                Logger.Log("Optional dependency Uconomy is present.");
            }
            else
            {
                Logger.Log("Optional dependency Uconomy is not present.");
            }
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList(){
                    {"command_kit_invalid_parameter","Invalid parameter, specify a kit with /kit <name>"},
                    {"command_kit_not_found","Kit not found"},
                    {"command_kit_no_permissions","You don't have permissions to use this kit"},
                    {"command_kit_cooldown_command","You have to wait {0} seconds to use this command again"},
                    {"command_kit_cooldown_kit","You have to wait {0} seconds to get this kit again"},
                    {"command_kit_failed_giving_item","Failed giving a item to {0} ({1},{2})"},
                    {"command_kit_success","You just received the kit {0}" },
                    {"command_kits","You have access to the following kits: {0}" },
                    {"command_kit_no_money","You can't afford the kit {2}. You need atleast {0} {1}." },
                    {"command_kit_money","You have received {0} {1} from the kit {2}." },
                    {"command_kit_xp","You have received {0} xp from the kit {1}." }
                };
            }
        }
    }
}
