using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace NonstopBGM
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Range(0f, 1f)]
        [Increment(0.1f)]
        [DefaultValue(0.5f)]
        public float VolumeRatio { get; set; }

        public override void OnChanged()
        {
            NonstopBGM.VolumeRatio = VolumeRatio;
        }
    }
}
