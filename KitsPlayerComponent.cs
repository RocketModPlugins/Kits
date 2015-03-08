using Rocket.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace unturned.ROCKS.Kits
{
    public class KitsPlayerComponent : RocketPlayerComponent
    {
        public DateTime GlobalKitCooldown = DateTime.MinValue;
        public Dictionary<string, DateTime> SpecificKitCooldown = new Dictionary<string, DateTime>();
        private void Start()
        { 
            
        }
    }
}
