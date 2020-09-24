﻿using System;
 using Coroutine;
 using PlatformerTest;
 using Microsoft.Xna.Framework;
 using Microsoft.Xna.Framework.Graphics;
 using Microsoft.Xna.Framework.Input;
 using PlatformerTest.Scenes;

 namespace PlatformerTest
{
    public sealed class Game1 : Game
    {
        public Game1()
        {
            Data.GameInstance = this;
            Data.Window = Window;
            Data.Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Data.Content = Content;
        }
        
        protected override void Initialize()
        {
            Window.Title = "Platformer Test";
            
            IsMouseVisible = false;
            IsFixedTimeStep = true;

            Window.AllowUserResizing = true;
            Window.AllowAltF4 = true;

            Data.Graphics.IsFullScreen = GameSettings.StartFullScreen;

            Data.Graphics.PreferMultiSampling = true;
            Data.Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Data.Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            Data.Graphics.SynchronizeWithVerticalRetrace = true;
            
            Data.Graphics.PreferredBackBufferWidth = GameSettings.StartWindowWidth;
            Data.Graphics.PreferredBackBufferHeight = GameSettings.StartWindowHeight;
            
            Data.Graphics.ApplyChanges();
            
            base.Initialize();

            Window.ClientSizeChanged += OnScreenSizeChange;
            OnScreenSizeChange(null, null);
        }

        private void OnScreenSizeChange(object sender, EventArgs e)
        {
            Data.ScreenSize = new Vector2(Data.Graphics.PreferredBackBufferWidth, Data.Graphics.PreferredBackBufferHeight);
            
            float outputAspectRatio = Window.ClientBounds.Width / (float)Window.ClientBounds.Height;
            float preferredAspectRatio = GameSettings.StartWindowWidth / (float)GameSettings.StartWindowHeight;

            if (preferredAspectRatio > 0f)
            {
                if (outputAspectRatio <= preferredAspectRatio)
                {
                    // output is taller than it is wider, bars on top/bottom
                    int presentHeight = (int)((Window.ClientBounds.Width / preferredAspectRatio) + 0.5f);
                    int barHeight = (Window.ClientBounds.Height - presentHeight) / 2;
                    Data.RenderRect = new Rectangle(0, barHeight, Window.ClientBounds.Width, presentHeight);
                }
                else
                {
                    // output is wider than it is tall, bars left/right
                    int presentWidth = (int)((Window.ClientBounds.Height * preferredAspectRatio) + 0.5f);
                    int barWidth = (Window.ClientBounds.Width - presentWidth) / 2;
                    Data.RenderRect = new Rectangle(barWidth, 0, presentWidth, Window.ClientBounds.Height);
                }
            }
        }

        protected override void LoadContent()
        {
            Data.SpriteBatch = new SpriteBatch(GraphicsDevice);
            Data.ScreenSize = new Vector2(Data.Graphics.PreferredBackBufferWidth, Data.Graphics.PreferredBackBufferHeight);
            Data.MainRenderTarget = new RenderTarget2D(Data.Graphics.GraphicsDevice, GameSettings.VirtualWindowWidth, GameSettings.VirtualWindowHeight, false, SurfaceFormat.Color, DepthFormat.Depth24, 1, RenderTargetUsage.DiscardContents);
            Data.LoadAssets();
            SceneManager.Initialise();
        }

        protected override void Update(GameTime gameTime)
        {
            Time.GameTime = gameTime;
            Time.DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            base.Update(gameTime);
            CoroutineHandler.Tick(gameTime.ElapsedGameTime.TotalSeconds);
            
            Input.UpdateState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Input.IsKeyPressed(Input.KeyMap["quit"]))
                Exit();
            
            SceneManager.UpdateScenes();
            
            Camera.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(Data.MainRenderTarget);
            GraphicsDevice.Clear(GameSettings.ClearColour);
            
            Data.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp,
                DepthStencilState.Default, transformMatrix: Camera.Transform);
            SceneManager.DrawScenes();
            Data.SpriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1f, 0);
            Data.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default);
            Data.SpriteBatch.Draw(Data.MainRenderTarget, Data.RenderRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            Data.SpriteBatch.End();

            GraphicsDevice.Textures[0] = null;

            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            SceneManager.CurrentScene.Unload();
            base.Dispose(disposing);
        }
    }
}