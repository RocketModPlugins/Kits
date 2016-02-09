using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fr34kyn01535.Kits
{
    public class CommandKit : IRocketCommand
    {
        public string Help
        {
            get { return "Gives you a kit"; }
        }

        public string Name
        {
            get { return "kit"; }
        }

        public string Syntax
        {
            get { return "<kit>"; }
        }

        public bool RunFromConsole
        {
            get { return false; }
        }
        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public AllowedCaller AllowedCaller
        {
            get { return Rocket.API.AllowedCaller.Player; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "kits.kit" };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length != 1)
            {
                UnturnedChat.Say(caller, Kits.Instance.Translations.Instance.Translate("command_kit_invalid_parameter"));
                return;
            }

            Kit kit = Kits.Instance.Configuration.Instance.Kits.Where(k => k.Name.ToLower() == command[0].ToLower()).FirstOrDefault();
            if (kit == null)
            {
                UnturnedChat.Say(caller, Kits.Instance.Translations.Instance.Translate("command_kit_not_found"));
                return;
            }

            bool hasPermissions = caller.HasPermission("kit." + kit.Name.ToLower());

            if (!hasPermissions)
            {
                UnturnedChat.Say(caller, Kits.Instance.Translations.Instance.Translate("command_kit_no_permissions"));
                return;
            }

            KeyValuePair<string, DateTime> globalCooldown = Kits.GlobalCooldown.Where(k => k.Key == caller.ToString()).FirstOrDefault();
            if (!globalCooldown.Equals(default(KeyValuePair<string, DateTime>)))
            {
                double globalCooldownSeconds = (DateTime.Now - globalCooldown.Value).TotalSeconds;
                if (globalCooldownSeconds < Kits.Instance.Configuration.Instance.GlobalCooldown)
                {
                    UnturnedChat.Say(caller, Kits.Instance.Translations.Instance.Translate("command_kit_cooldown_command", (int)(Kits.Instance.Configuration.Instance.GlobalCooldown - globalCooldownSeconds)));
                    return;
                }
            }

            KeyValuePair<string, DateTime> individualCooldown = Kits.InvididualCooldown.Where(k => k.Key == (caller.ToString() + kit.Name)).FirstOrDefault();
            if (!individualCooldown.Equals(default(KeyValuePair<string, DateTime>)))
            {
                double individualCooldownSeconds = (DateTime.Now - individualCooldown.Value).TotalSeconds;
                if (individualCooldownSeconds < kit.Cooldown)
                {
                    UnturnedChat.Say(caller, Kits.Instance.Translations.Instance.Translate("command_kit_cooldown_kit", (int)(kit.Cooldown - individualCooldownSeconds)));
                    return;
                }
            }

            foreach (KitItem item in kit.Items)
            {
                
                if (!player.GiveItem(item.ItemId, item.Amount))
                {
                    Logger.Log(Kits.Instance.Translations.Instance.Translate("command_kit_failed_giving_item", player.CharacterName, item.ItemId, item.Amount));
                }
            }

            player.Experience += kit.XP;

            UnturnedChat.Say(caller, Kits.Instance.Translations.Instance.Translate("command_kit_success", kit.Name));

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
