using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace LoLCameraSharp.MemoryFunctions
{
    class Pattern
    {
        public uint FindPattern(string pattern, string mask, ref MemoryEditor m)
        {
            byte[] data = m.ReadBytes(m.baseModule, m.moduleSize);
            byte[] patternBytes = GetBytesFromPattern(pattern);
            return (uint)m.baseModule + Find(data, mask, patternBytes);
        }

        private static byte[] GetBytesFromPattern(string pattern)
        {
            string[] split = pattern.Split(new[] { '\\', 'x' }, StringSplitOptions.RemoveEmptyEntries);
            var ret = new byte[split.Length];
            for (int i = 0; i < split.Length; i++)
            {
                ret[i] = byte.Parse(split[i], NumberStyles.HexNumber);
            }
            return ret;
        }

        private static uint Find(byte[] data, string mask, byte[] byteMask)
        {
            for (uint i = 0; i < data.Length; i++)
            {
                if (DataCompare(data, (int)i, byteMask, mask))
                    return i;
            }
            return 0;
        }

        private static bool DataCompare(byte[] data, int offset, byte[] byteMask, string mask)
        {
            for (int i = 0; i < mask.Length; i++)
            {
                if (mask[i] == 'x' && byteMask[i] != data[i + offset])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
