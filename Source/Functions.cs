using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlatformerTest.Components;
using PlatformerTest.Levels;

namespace PlatformerTest
{
    public static class Functions
    {
        public static void Draw(ref Sprite sprite, ref Transform transform)
        {
            var centerOrigin = Vector2.Zero;
            
            if (sprite.Centered)
                centerOrigin = sprite.Texture.Bounds.Size.ToVector2() * .5f;
            
            Data.SpriteBatch.Draw(
                sprite.Texture,
                new Vector2((int)transform.Position.X, (int)transform.Position.Y),
                sprite.SourceRect,
                sprite.Colour,
                transform.Rotation,
                centerOrigin + sprite.Origin,
                transform.Scale,
                sprite.Effects,
                sprite.Depth
            );
        }

        public static void LoadLevel(uint index)
        {
            Pool.Reset();
            switch (index)
            {
                case 0:
                    Level_0.Load();
                    break;
                case 1:
                    Level_1.Load();
                    break;
                case 2:
                    Level_2.Load();
                    break;
            }
            Data.CurrentLevel = index;
            PutCameraOnPlayer();
        }

        public static T Choose<T>(params T[] list) => list[Data.Random.Next(0, list.ToArray().Length)];
        
        public static float Map(float value, float fromLow, float fromHigh, float toLow, float toHigh) => 
            (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
        
        public static Vector2 ScreenToWorld(Vector2 onScreen) => 
            Vector2.Transform(onScreen, Matrix.Invert(Camera.Transform));

        public static bool Intersects(Hitbox HB1, Vector2 pos1, Hitbox HB2, Vector2 pos2) =>
            new Rectangle((pos1 + HB1.Offset).ToPoint(), HB1.Size.ToPoint()).Intersects(
                new Rectangle((pos2 + HB2.Offset).ToPoint(), HB2.Size.ToPoint()));

        public static Rectangle Intersect(Hitbox HB1, Vector2 pos1, Hitbox HB2, Vector2 pos2) => Rectangle.Intersect(
            new Rectangle((pos1 + HB1.Offset).ToPoint(), HB1.Size.ToPoint()),
            new Rectangle((pos2 + HB2.Offset).ToPoint(), HB2.Size.ToPoint()));

        public static Rectangle GetBounds(ref Hitbox hitbox, ref Transform transform) => new Rectangle(
            transform.Position.ToPoint() + hitbox.Offset.ToPoint(), hitbox.Size.ToPoint() * transform.Scale.ToPoint());

        public static int GetObjectAtPos(Vector2 position)
        {
            var objectsNearby = Pool.StaticObjectTree.Query(position).ToArray();

            for (uint i = 0; i < objectsNearby.Length; i++)
            {
                ref var gameObject = ref Pool.StaticObjects[objectsNearby[i].ObjectID];
                if (GetBounds(ref gameObject.Hitbox, ref gameObject.Transform).Contains(position))
                    return (int)gameObject.ID;
            }

            return -1;
        }

        public static bool IsObjectAt(Vector2 position) => GetObjectAtPos(position) != -1;

        public static void CheckStaticCollisions(ref Transform transform, ref Physics physics, ref Hitbox hitbox, out CollisionInfo info)
        {
            info = new CollisionInfo();

            var nearbyObjects = Pool.StaticObjectTree.Query(new Rectangle(transform.Position.ToPoint() + (hitbox.Offset * 2).ToPoint(),
                (hitbox.Size * 2).ToPoint() * transform.Scale.ToPoint())).ToArray();
            
            for (int i = 0; i < nearbyObjects.Length; i++)
            {
                ref var levelObject = ref Pool.StaticObjects[nearbyObjects[i].ObjectID];
                
                if (!levelObject.Collidable || !levelObject.Active)
                    continue;

                if (Functions.Intersects(hitbox, new Vector2(physics.ProjectedPos.X, transform.Position.Y),
                    levelObject.Hitbox, levelObject.Transform.Position))
                {
                    if (levelObject.Transform.Position.X > transform.Position.X)
                    {
                        physics.ProjectedPos.X -= Functions.Intersect(hitbox, new Vector2(physics.ProjectedPos.X, transform.Position.Y),
                            levelObject.Hitbox, levelObject.Transform.Position).Size.X;
                        
                        info.IsOnRightWall = true;
                        info.IsOnWall = true;
                    }
                    else
                    {
                        physics.ProjectedPos.X += Functions.Intersect(hitbox, new Vector2(physics.ProjectedPos.X, transform.Position.Y),
                            levelObject.Hitbox, levelObject.Transform.Position).Size.X;
                        
                        info.IsOnLeftWall = true;
                        info.IsOnWall = true;
                    }

                    physics.Velocity.X = 0f;
                }

                if (Functions.Intersects(hitbox, new Vector2(transform.Position.X, physics.ProjectedPos.Y),
                    levelObject.Hitbox, levelObject.Transform.Position))
                {
                    if (levelObject.Transform.Position.Y > transform.Position.Y)
                    {
                        physics.ProjectedPos.Y -= Functions.Intersect(hitbox,
                            new Vector2(transform.Position.X, physics.ProjectedPos.Y),
                            levelObject.Hitbox, levelObject.Transform.Position).Size.Y;
                        info.IsOnFloor = true;
                    }
                    else
                    {
                        physics.ProjectedPos.Y += Functions.Intersect(hitbox,
                            new Vector2(transform.Position.X, physics.ProjectedPos.Y),
                            levelObject.Hitbox, levelObject.Transform.Position).Size.Y;
                        info.IsOnCeiling = true;
                    }

                    physics.Velocity.Y = 0f;
                }
            }
        }

        public static bool RayIntersectsRect(Ray ray, Rectangle rect, out Vector2 contactPoint, out Vector2 contactNormal, out float hitTime)
        {
            contactPoint = contactNormal = Vector2.Zero;
            hitTime = 0f;
            
            var near = (rect.Location.ToVector2() - ray.Position) / ray.Direction;
            var far = (rect.Location.ToVector2() + rect.Size.ToVector2() - ray.Position) / ray.Direction;
            
            if (near.X > far.X)
            {
                float nearX = near.X;
                far.X = nearX;
                near.X = far.X;
            }
            
            if (near.Y > far.Y)
            {
                float nearY = near.Y;
                far.Y = nearY;
                near.Y = far.Y;
            }

            if (near.X > far.Y || near.Y > far.X) return false;
            
            float hitNear = Math.Max(near.X, near.Y);
            float hitFar = Math.Max(far.X, far.Y);

            if (hitFar < 0) return false;

            contactPoint = ray.Position + hitNear * ray.Direction;
            
            if (near.X > near.Y)
                if (ray.Direction.X < 0)
                    contactNormal = new Vector2(1, 0);
                else
                    contactNormal = new Vector2(-1, 0);
            else if (near.X < near.Y)
                if (ray.Direction.Y < 0)
                    contactNormal = new Vector2(0, 1);
                else
                    contactNormal = new Vector2(0, -1);

            return true;
        }

        public static bool IsRectTouchingLeft(Rectangle rect1, Rectangle rect2, float margin)
        {
            return rect1.Right + margin * Time.DeltaTime > rect2.Left &&
                   rect1.Left < rect2.Left &&
                   rect1.Bottom > rect2.Top &&
                   rect1.Top < rect2.Bottom;
        }

        public static bool IsRectTouchingRight(Rectangle rect1, Rectangle rect2, float margin)
        {
            return rect1.Left + margin * Time.DeltaTime < rect2.Right &&
                   rect1.Right > rect2.Right &&
                   rect1.Bottom > rect2.Top &&
                   rect1.Top < rect2.Bottom;
        }

        public static bool IsRectTouchingTop(Rectangle rect1, Rectangle rect2, float margin)
        {
            return rect1.Bottom + margin * Time.DeltaTime > rect2.Top &&
                   rect1.Top < rect2.Top &&
                   rect1.Right > rect2.Left &&
                   rect1.Left < rect2.Right;
        }

        public static bool IsRectTouchingBottom(Rectangle rect1, Rectangle rect2, float margin)
        {
            return rect1.Top + margin * Time.DeltaTime < rect2.Bottom &&
                   rect1.Bottom > rect2.Bottom &&
                   rect1.Right > rect2.Left &&
                   rect1.Left < rect2.Right;
        }

        public static void PutCameraOnPlayer() => Camera.Position = Player.Transform.Position;
        
        public static void SaveLevel(uint index)
        {
            string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string levelPath = baseDirectory.Remove(baseDirectory.Length - 23) + "Source/Levels";
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("namespace PlatformerTest.Levels\n");
            stringBuilder.Append("{\n");
            stringBuilder.Append($"\tpublic static class Level_{index}\n");
            stringBuilder.Append("\t{\n");
            stringBuilder.Append("\t\tpublic static void Load()\n");
            stringBuilder.Append("\t\t{\n");
            stringBuilder.Append($"\t\t\tFunctions.SetPlayerSpawnPoint({Data.PlayerSpawnPos.X}, {Data.PlayerSpawnPos.Y});\n");
            stringBuilder.Append(CollectLevelObjects());
            stringBuilder.Append("\t\t}\n");
            stringBuilder.Append("\t}\n");
            stringBuilder.Append("}");
            File.WriteAllText($"{levelPath}\\Level{index}.cs", stringBuilder.ToString());
        }

        public static string CollectLevelObjects()
        {
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < Pool.StaticObjects.Length; ++i)
            {
                if (Pool.StaticObjects[i].Active && Pool.StaticObjects[i].Type != ObjType.Undefined)
                    stringBuilder.Append("\t\t\tFunctions.SpawnLevelObject(" + (int)Pool.StaticObjects[i].Transform.Position.X + ", " + (int)Pool.StaticObjects[i].Transform.Position.Y + ", ObjType." + Pool.StaticObjects[i].Type + ");\n");
            }
            return stringBuilder.ToString();
        }

        public static int SpawnLevelObject(float x, float y, ObjType type)
        {
            for (int i = 0; i < Pool.StaticObjects.Length; i++)
            {
                if (!Pool.StaticObjects[i].Active)
                {
                    Pool.StaticObjects[i] = new GameObject();
                    Pool.StaticObjects[i].Active = true;
                    Pool.StaticObjects[i].Type = type;
                    Pool.StaticObjects[i].StartPos = new Vector2(x, y);
                    Pool.StaticObjects[i].Transform.Position = new Vector2(x, y);
                    Pool.StaticObjects[i].Transform.Scale = Vector2.One;
                    Pool.StaticObjects[i].Sprite.Colour = Color.White;
                    Pool.StaticObjects[i].Sprite.Centered = true;
                    Pool.StaticObjects[i].Collidable = true;
                    Functions.SetLevelObjectType(ref Pool.StaticObjects[i], type);
                    return i;
                }
            }
            return -1;
        }

        public static GameObject CreateLevelObject(ObjType type)
        {
            var gameObject = new GameObject();
            gameObject.Active = true;
            gameObject.Type = type;
            gameObject.Transform.Scale = Vector2.One;
            gameObject.Sprite.Colour = Color.White;
            gameObject.Sprite.Centered = true;
            gameObject.Collidable = true;
            Functions.SetLevelObjectType(ref gameObject, type);
            return gameObject;
        }
        
        public static void SetLevelObjectType(ref GameObject gameObject, ObjType type)
        {
            switch (type)
            {
                case ObjType.Undefined:
                    break;
                case ObjType.Wall_1x1:
                    gameObject.Sprite.Texture = Data.Assets_Wall_1x1;
                    gameObject.Hitbox.Size = new Vector2(32, 32);
                    gameObject.Hitbox.Offset = new Vector2(-16, -16);
                    gameObject.Sprite.Depth = Data.WallLayer;
                    break;
                case ObjType.Wall_2x2:
                    gameObject.Sprite.Texture = Data.Assets_Wall_2x2;
                    gameObject.Hitbox.Size = new Vector2(64, 64);
                    gameObject.Hitbox.Offset = new Vector2(-32, -32);
                    gameObject.Sprite.Depth = Data.WallLayer;
                    break;
                case ObjType.Tree1:
                    gameObject.Sprite.Texture = Data.Assets_Tree1;
                    gameObject.Hitbox.Size = new Vector2(45, 98);
                    gameObject.Collidable = false;
                    gameObject.Transform.Scale = Vector2.One * 2;
                    gameObject.Sprite.Depth = Data.TreeLayer;
                    break;
            }
        }

        public static void SetPlayerSpawnPoint(float x, float y)
        {
            Data.PlayerSpawnPos = new Vector2(x, y);
            Player.Transform.Position = Data.PlayerSpawnPos;
        }
    }
}
