using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoL_Sun_Changer.Sun_Classes
{
    public class Box
    {
        public Vector3f m_Min;
        public Vector3f m_Max;

        public Box()
        {
            m_Min = new Vector3f();
            m_Max = new Vector3f();
        }
    }
}
