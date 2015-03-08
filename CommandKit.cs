using Rocket;
using Rocket.Logging;
using Rocket.RocketAPI;
using SDG;
using System;
using System.Collections.Generic;
using System.Linq;

namespace unturned.ROCKS.Kits
{
    class CommandKit : Command
    {
        public CommandKit()
        {
            base.commandName = "kit";
            base.commandHelp = "Gives you a kit";
            base.commandInfo = base.commandName + " - " + base.commandHelp;
        }

        protected override void execute(SteamPlayerID caller, string command)
        {
            if (!RocketCommand.IsPlayer(caller)) return;

            bool hasPermissions = RocketPermissionManager.GetPermissions(caller.CSteamID).Where(p => p.ToLower() == ("kit." + command.Trim().ToLower()) || p.ToLower() == "kit.*").FirstOrDefault() != null;

            if (!hasPermissions)
            {
                RocketChatManager.Say(caller.CSteamID, "You don't have permissions to use that command");
                return;
            }

            if (String.IsNullOrEmpty(command.Trim())) {
                RocketChatManager.Say(caller.CSteamID,"Invalid parameter");
                return;
            }

            Kit kit = Kits.Configuration.Kits.Where(k => k.Name.ToLower() == command.Trim().ToLower()).FirstOrDefault();
            if (kit == null)
            {
                RocketChatManager.Say(caller.CSteamID, "Kit not found");
                return;
            }
            else { 
                Player player = PlayerTool.getPlayer(caller.CSteamID);
                KitsPlayerComponent kp = player.transform.GetComponent<KitsPlayerComponent>();

                double gt = (DateTime.Now - kp.GlobalKitCooldown).TotalSeconds;
                if (gt < Kits.Configuration.GlobalCooldown)
                {
                    RocketChatManager.Say(caller.CSteamID, "You have to wait " + (Kits.Configuration.GlobalCooldown - gt) + " seconds to use this command again");
                    return;
                }

                DateTime kitCooldown = kp.SpecificKitCooldown.ContainsKey(kit.Name.ToLower()) ? kp.SpecificKitCooldown[kit.Name] : DateTime.MinValue;

                double kt = (DateTime.Now - kitCooldown).TotalSeconds;
                if (gt < kit.Cooldown)
                {
                    RocketChatManager.Say(caller.CSteamID, "You have to wait " + (kit.Cooldown - gt) + " seconds to get this kit again");
                    return;
                }

                foreach (KitItem item in kit.Items)
                {
                    if (!ItemTool.tryForceGiveItem(player, item.ItemId, item.Amount))
                    {
                        Logger.Log("Failed giving a item to " + caller.CharacterName + " (" + item.ItemId + "," + item.Amount + ")");
                    }
                }
                RocketChatManager.Say(caller.CSteamID, "You just received the kit " + kit.Name);
                kp.SpecificKitCooldown[kit.Name] = DateTime.Now;
                kp.GlobalKitCooldown = DateTime.Now;

            }


        }
    }
}
