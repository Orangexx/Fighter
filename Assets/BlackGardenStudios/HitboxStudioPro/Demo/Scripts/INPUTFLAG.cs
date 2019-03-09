using UnityEngine;

namespace BlackGardenStudios.HitboxStudioPro.Demo
{
    public enum INPUTFLAG
    {
        UP = 0x1,
        DOWN = 0x2,
        RIGHT = 0x4,
        LEFT = 0x8,
        JUMP = 0x10,
        LIGHT = 0x20,
        HEAVY = 0x40,
        DODGE = 0x80,
        SPECIAL = 0x100,
        GUARD = 0x200,
        NEGATIVE = 0x8000
    }
}