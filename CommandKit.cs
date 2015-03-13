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
            if (String.IsNullOrEmpty(command.Trim()))
            {
                RocketChatManager.Say(caller.CSteamID, Kits.Instance.Translate("command_kit_invalid_parameter"));
                return;
            }

            if (!RocketCommand.IsPlayer(caller)) return;

            Kit kit = Kits.Instance.Configuration.Kits.Where(k => k.Name.ToLower() == command.Trim().ToLower()).FirstOrDefault();
            if (kit == null)
            {
                RocketChatManager.Say(caller.CSteamID, Kits.Instance.Translate("command_kit_not_found"));
                return;
            }

            Player player = PlayerTool.getPlayer(caller.CSteamID);
            bool hasPermissions = RocketPermissionManager.CheckPermissions(player.SteamChannel.SteamPlayer,"kit.*") || RocketPermissionManager.GetPermissions(caller.CSteamID).Where(p => p.ToLower() == ("kit." + command.Trim().ToLower())).FirstOrDefault() != null;

            if (!hasPermissions)
            {
                RocketChatManager.Say(caller.CSteamID, Kits.Instance.Translate("command_kit_no_permissions"));
                return;
            }

            KitsPlayerComponent kp = player.transform.GetComponent<KitsPlayerComponent>();

            double gt = (DateTime.Now - kp.GlobalKitCooldown).TotalSeconds;
            if (gt < Kits.Instance.Configuration.GlobalCooldown)
            {
                RocketChatManager.Say(caller.CSteamID, Kits.Instance.Translate("command_kit_cooldown_command", (int)(Kits.Instance.Configuration.GlobalCooldown - gt)));
                return;
            }

            DateTime kitCooldown = kp.SpecificKitCooldown.ContainsKey(kit.Name.ToLower()) ? kp.SpecificKitCooldown[kit.Name] : DateTime.MinValue;

            double kt = (DateTime.Now - kitCooldown).TotalSeconds;
            if (gt < kit.Cooldown)
            {
                RocketChatManager.Say(caller.CSteamID, Kits.Instance.Translate("command_kit_cooldown_kit", (int)(kit.Cooldown - gt)));
                return;
            }

            foreach (KitItem item in kit.Items)
            {
                if (!ItemTool.tryForceGiveItem(player, item.ItemId, item.Amount))
                {
                    Logger.Log(Kits.Instance.Translate("command_kit_failed_giving_item", caller.CharacterName, item.ItemId, item.Amount));
                }
            }
            RocketChatManager.Say(caller.CSteamID, Kits.Instance.Translate("command_kit_success", kit.Name));
            kp.SpecificKitCooldown[kit.Name] = DateTime.Now;
            kp.GlobalKitCooldown = DateTime.Now;

        }
    }
}
