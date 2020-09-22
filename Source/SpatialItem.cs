using Dcrew.Spatial;
using Microsoft.Xna.Framework;

namespace PlatformerTest
{
    public class SpatialItem : IBounds
    {
        public uint ObjectID;
        
        public Vector2 Position {
            get => _bounds.XY;
            set => _bounds.XY = value;
        }

        public Vector2 Size {
            get => _bounds.Size;
            set => _bounds.Size = value;
        }

        public Vector2 Origin {
            get => _bounds.Origin;
            set => _bounds.Origin = value;
        }
        
        public Rectangle Bounds => new Rectangle(Position.ToPoint(), Size.ToPoint());

        RotRect IBounds.Bounds => _bounds;
 
        RotRect _bounds = new RotRect { Size = new Vector2(32, 32) };
    }
}