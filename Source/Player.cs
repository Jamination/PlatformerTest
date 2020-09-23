using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PlatformerTest.Components;

namespace PlatformerTest
{
    public static class Player
    {
        public const int Gravity = 24;
        public const int MaxVelocityY = 1000;
        public const int MoveSpeed = 200;
        public const int JumpHeight = -480;

        public static bool IsOnFloor = false;

        public static Transform Transform;
        public static Sprite Sprite;
        public static Hitbox Hitbox;
        public static Physics Physics;

        public static CollisionInfo CollisionInfo;

        public const uint TimeCount = 60 * 10;
        public static uint FrameCount;
        
        public static Vector2[] PreviousPositions = new Vector2[TimeCount];
        public static Vector2[] PreviousVelocities = new Vector2[TimeCount];

        public static void Load()
        {
            Transform = new Transform();
            Transform.Position = Data.PlayerSpawnPos;
            Transform.Scale = Vector2.One;
            
            Sprite = new Sprite();
            Sprite.Texture = Data.Assets_Wall_1x1;
            Sprite.Colour = Color.Yellow;
            Sprite.Depth = Data.PlayerLayer;
            Hitbox.Size = new Vector2(32, 32);
            Hitbox.Offset = new Vector2(-16, -16);
            Sprite.Centered = true;
            
            Physics = new Physics();

            FrameCount = (uint)PreviousVelocities.Length;
        }

        public static void Update()
        {
            Physics.Velocity.X = MathHelper.Lerp(Physics.Velocity.X, 0f, .25f);

            if (Input.IsKeyDown(Keys.A))
                Physics.Velocity.X = -MoveSpeed;
            if (Input.IsKeyDown(Keys.D))
                Physics.Velocity.X = MoveSpeed;

            if (Input.IsKeyDown(Keys.W) && CollisionInfo.IsOnFloor)
                Physics.Velocity.Y = JumpHeight;

            if (Input.IsKeyReleased(Keys.W) && Physics.Velocity.Y < 0f)
                Physics.Velocity.Y *= .5f;
            
            Physics.Velocity.Y += Gravity;
            Physics.Velocity.Y = Math.Clamp(Physics.Velocity.Y, -MaxVelocityY, MaxVelocityY);

            Physics.LastPos = Transform.Position;
            Physics.ProjectedPos = Transform.Position + Physics.Velocity * Time.DeltaTime;
            IsOnFloor = false;
            
            Functions.CheckStaticCollisions(ref Transform, ref Physics, ref Hitbox, out CollisionInfo);

            Transform.Position = Physics.ProjectedPos;
            Camera.Position = Vector2.Lerp(Camera.Position, Transform.Position, .15f);
            
            if (Input.IsKeyDown(Keys.E))
            {
                if (FrameCount < TimeCount - 1)
                {
                    Physics.Velocity = -PreviousVelocities[FrameCount];
                    Transform.Position = PreviousPositions[FrameCount];
                    FrameCount++;
                }
            }
            else
            {
                if (FrameCount > 0)
                    FrameCount--;
                else
                    FrameCount = TimeCount - 1;

                PreviousVelocities[FrameCount] = Physics.Velocity;
                PreviousPositions[FrameCount] = Transform.Position;
            }
        }

        public static void Draw()
        {
            Functions.Draw(ref Sprite, ref Transform);
        }
    }
}