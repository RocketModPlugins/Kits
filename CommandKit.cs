using Rocket;
using Rocket.Logging;
using Rocket.RocketAPI;
using SDG;
using System;
using System.Collections.Generic;
using System.Linq;

namespace unturned.ROCKS.Kits
{
    public class CommandKit : IRocketCommand
    {
        public string Help
        {
            get { return "Gives you a kit"; }
        }

        public string Name
        {
            get { return "kit";}
        }

        public bool RunFromConsole
        {
            get { return false; }
        }

        public void Execute(RocketPlayer caller, string[] command)
        {
            if (command.Length != 1)
            {
                RocketChatManager.Say(caller, Kits.Instance.Translate("command_kit_invalid_parameter"));
                return;
            }

            Kit kit = Kits.Instance.Configuration.Kits.Where(k => k.Name.ToLower() == command[0].ToLower()).FirstOrDefault();
            if (kit == null)
            {
                RocketChatManager.Say(caller, Kits.Instance.Translate("command_kit_not_found"));
                return;
            }

            bool hasPermissions = caller.Permissions.Contains("kit.*") || RocketPermissionManager.GetPermissions(caller.CSteamID).Where(p => p.ToLower() == ("kit." + kit.Name.ToLower())).FirstOrDefault() != null;

            if (!hasPermissions)
            {
                RocketChatManager.Say(caller, Kits.Instance.Translate("command_kit_no_permissions"));
                return;
            }

            KeyValuePair<string, DateTime> globalCooldown = Kits.GlobalCooldown.Where(k => k.Key == caller.ToString()).FirstOrDefault();
            if (!globalCooldown.Equals(default(KeyValuePair<string, DateTime>)))
            {
                double globalCooldownSeconds = (DateTime.Now - globalCooldown.Value).TotalSeconds;
                if (globalCooldownSeconds < Kits.Instance.Configuration.GlobalCooldown)
                {
                    RocketChatManager.Say(caller, Kits.Instance.Translate("command_kit_cooldown_command", (int)(Kits.Instance.Configuration.GlobalCooldown - globalCooldownSeconds)));
                    return;
                }
            }

            KeyValuePair<string, DateTime> individualCooldown = Kits.InvididualCooldown.Where(k => k.Key == (caller.ToString() + kit.Name)).FirstOrDefault();
            if (!individualCooldown.Equals(default(KeyValuePair<string, DateTime>)))
            {
                double individualCooldownSeconds = (DateTime.Now - individualCooldown.Value).TotalSeconds;
                if (individualCooldownSeconds < kit.Cooldown)
                {
                    RocketChatManager.Say(caller, Kits.Instance.Translate("command_kit_cooldown_kit", (int)(kit.Cooldown - individualCooldownSeconds)));
                    return;
                }
            }

            foreach (KitItem item in kit.Items)
            {
                if (!ItemTool.tryForceGiveItem(caller.Player, item.ItemId, item.Amount))
                {
                    Logger.Log(Kits.Instance.Translate("command_kit_failed_giving_item", caller.CharacterName, item.ItemId, item.Amount));
                }
            }
            RocketChatManager.Say(caller, Kits.Instance.Translate("command_kit_success", kit.Name));

            if (Kits.GlobalCooldown.ContainsKey(caller.ToString()))
            {
                Kits.GlobalCooldown[caller.ToString()] = DateTime.Now;
            }
            else
            {
                Kits.GlobalCooldown.Add(caller.ToString(), DateTime.Now);
            }

            if (Kits.GlobalCooldown.ContainsKey(caller.ToString()))
            {
                Kits.InvididualCooldown[caller.ToString() + kit.Name] = DateTime.Now;
            }
            else
            {
                Kits.InvididualCooldown.Add(caller.ToString() + kit.Name, DateTime.Now);
            }
        }
    }
}
