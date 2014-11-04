using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoL_Sun_Changer.Sun_Classes
{
    public class r3dSun
    {
        public r3dLight sunLight;
        public Color ambientColor;
        [Util.DynamicCastSize(24)]
        public Byte[] atmosphereMutator;

        public r3dSun()
        {
            sunLight = new r3dLight();
            ambientColor = new Color();
            atmosphereMutator = new Byte[24];
        }
    }
}
