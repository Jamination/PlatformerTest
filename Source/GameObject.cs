using Microsoft.Xna.Framework;
using PlatformerTest.Components;

namespace PlatformerTest
{
    public struct GameObject
    {
        public uint ID;
        public bool Active, Collidable;
        public ObjType Type;
        public Transform Transform;
        public Sprite Sprite;
        public Hitbox Hitbox;
        public Physics Physics;
        public Vector2 StartPos;
    }
}