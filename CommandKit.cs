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

            KeyValuePair<string, DateTime> globalCooldown = Kits.GlobalCooldown.Where(k => k.Key == caller.CSteamID.ToString()).FirstOrDefault();
            if (!globalCooldown.Equals(default(KeyValuePair<string, DateTime>)))
            {
                double globalCooldownSeconds = (DateTime.Now - globalCooldown.Value).TotalSeconds;
                if (globalCooldownSeconds < Kits.Instance.Configuration.GlobalCooldown)
                {
                    RocketChatManager.Say(caller.CSteamID, Kits.Instance.Translate("command_kit_cooldown_command", (int)(Kits.Instance.Configuration.GlobalCooldown - globalCooldownSeconds)));
                    return;
                }
            }

            KeyValuePair<string, IndividualKitCooldown> individualCooldown = Kits.InvididualCooldown.Where(k => k.Key == caller.CSteamID.ToString() && k.Value.Kit == kit.Name).FirstOrDefault();
            if (!individualCooldown.Equals(default(KeyValuePair<string, IndividualKitCooldown>)))
            {
                double individualCooldownSeconds = (DateTime.Now - individualCooldown.Value.Cooldown).TotalSeconds;
                if (individualCooldownSeconds < kit.Cooldown)
                {
                    RocketChatManager.Say(caller.CSteamID, Kits.Instance.Translate("command_kit_cooldown_kit", (int)(kit.Cooldown - individualCooldownSeconds)));
                    return;
                }
            }

            foreach (KitItem item in kit.Items)
            {
                if (!ItemTool.tryForceGiveItem(player, item.ItemId, item.Amount))
                {
                    Logger.Log(Kits.Instance.Translate("command_kit_failed_giving_item", caller.CharacterName, item.ItemId, item.Amount));
                }
            }
            RocketChatManager.Say(caller.CSteamID, Kits.Instance.Translate("command_kit_success", kit.Name));

            Kits.GlobalCooldown.Add(caller.CSteamID.ToString(), DateTime.Now);
            Kits.InvididualCooldown.Add(caller.CSteamID.ToString(), new IndividualKitCooldown() { Cooldown = DateTime.Now, Kit = kit.Name });
        }
    }
}
