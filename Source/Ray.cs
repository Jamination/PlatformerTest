using Microsoft.Xna.Framework;

namespace PlatformerTest
{
    public struct Ray
    {
        public Vector2 Position, Direction;

        public Ray(Vector2 position, Vector2 direction)
        {
            Position = position;
            Direction = direction;
        }
    }
}