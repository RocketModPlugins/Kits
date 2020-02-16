using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fr34kyn01535.Kits
{
    public class CommandKits : IRocketCommand
    {
        public string Help
        {
            get { return "Shows you available kits"; }
        }

        public string Name
        {
            get { return "kits"; }
        }

        public string Syntax
        {
            get { return ""; }
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
                return new List<string>() { "kits.kits" };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            List<string> availableKits = new List<string>();
            List<Kit> kits = Kits.Instance.Configuration.Instance.Kits;

            // Gets all caller's permissions
            List<Permission> callerPerms = caller.GetPermissions().Distinct(new PermissionComparer()).ToList();

            foreach (var item in kits)
            {
                // Adds the kit if it's permission is contained in the caller's permission list
                if (callerPerms.Exists(x => x.Name == $"kit.{item.Name.ToLower()}"))
                    availableKits.Add(item.Name);
            }

            UnturnedChat.Say(caller, Kits.Instance.Translations.Instance.Translate("command_kits", String.Join(", ",availableKits.ToArray())));
        }
    }

    /// <summary>
    /// Provides the comparison method for Rocket's Permission
    /// </summary>
    internal class PermissionComparer : IEqualityComparer<Permission>
    {
        public bool Equals(Permission x, Permission y)
        {
            return x.Name == y.Name && x.Cooldown == y.Cooldown;
        }

        public int GetHashCode(Permission obj)
        {
            return obj.GetHashCode();
        }
    }
}
