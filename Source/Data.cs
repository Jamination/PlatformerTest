﻿using System;
 using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

 namespace PlatformerTest
{
    public static class Data
    {
        public static GraphicsDeviceManager Graphics;
        public static SpriteBatch SpriteBatch;
        public static GameWindow Window;
        public static ContentManager Content;
        public static Game1 GameInstance;

        public static RenderTarget2D MainRenderTarget;
        
        public static Rectangle RenderRect;

        public static Vector2 MousePosition => Functions.ScreenToWorld(Input.CurrentMouseState.Position.ToVector2() / VirtualToRealScreenRatio);
        
        public static Vector2 ScreenSize;
        public static readonly Vector2 ScreenCentre = new Vector2(GameSettings.VirtualWindowWidth * .5f, GameSettings.VirtualWindowHeight * .5f);
        
        public static Vector2 VirtualToRealScreenRatio => new Vector2(ScreenSize.X / GameSettings.VirtualWindowWidth, ScreenSize.Y / GameSettings.VirtualWindowHeight);
        
        public static Random Random = new Random();

        public static Vector2 PlayerSpawnPos;

        public static Texture2D Assets_Wall_1x1, Assets_Wall_2x2, Assets_Tree1;

        public static uint CurrentLevel = 0;

        public static float TreeLayer = .75f;
        public static float PlayerLayer = 0f;
        public static float WallLayer = .5f;

        public static void LoadAssets()
        {
            Assets_Wall_1x1 = Content.Load<Texture2D>("Sprites/Wall_1x1");
            Assets_Wall_2x2 = Content.Load<Texture2D>("Sprites/Wall_2x2");
            Assets_Tree1 = Content.Load<Texture2D>("Sprites/Tree1");
        }
    }
}