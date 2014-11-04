using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoL_Sun_Changer.Sun_Classes
{
    public class r3dLight
    {
        public Vector3f unk;
        public UInt32 flags;
        public UInt32 pNext; //Pointer To Next Light
        public Box bBox;
        public float intensity;
        public UInt32 Type;
        public float radius1;
        public float radius2;
        public float tmp1;
        public UInt32 pLightSystem; //Pointer to the Lighting System which contains more lights etc
        public float R;
        public float G;
        public float B;
        public float R1;
        public float G1;
        public float B1;
        public Vector3f direction;
        public float spotAngle;
        public float fallOfAngle;
        public float att0;
        public float att1;
        public float att2;
        public UInt32 bLocalLight;
        public UInt32 shadowIndex;
        public UInt32 updateKey;
        public UInt32 pProjectMap; //Pointer to Texture

        public r3dLight()
        {
            unk = new Vector3f();
            bBox = new Box();
            direction = new Vector3f();
        }

         enum r3dLightTypes
         {
             R3D_OMNI_LIGHT = 0x0,
             R3D_DIRECT_LIGHT = 0x1,
             R3D_SPOT_LIGHT = 0x2,
             R3D_PROJECTOR_LIGHT = 0x3,
             R3D_CUBE_LIGHT = 0x4
         }

         enum r3dLightFlags //(bitfield)
         {
             R3D_LIGHT_ON     = 0x2,
             R3D_LIGHT_STATIC = 0x4,
             R3D_LIGHT_DYNAMIC = 0x8,
             LIGHT_HEAP = 0x10,
             R3D_LIGHT_AUTOREMOVE = 0x20,
             R3D_LIGHT_ALWAYSVISIBLE = 0x40,
             R3D_LIGHT_SHADOW_CAST_STATIC = 0x80,
             R3D_LIGHT_SHADOW_DIRTY = 0x100,
         }
    }
}
