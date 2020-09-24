using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PlatformerTest.Levels;

namespace PlatformerTest.Scenes
{
    public sealed class EditorScene : Scene
    {
        public static EditorScene Instance = new EditorScene();

        public byte ObjToCreateType = 1;

        public int GrabbedObjectID = -1;
        public Vector2 GrabbedObjectOffset;

        public Vector2 CameraVelocity;

        public GameObject ObjectBeingPlaced;

        public bool TileMode = false;

        public float LastPlacedObject;

        public override void Load()
        {
            Data.GameInstance.IsMouseVisible = true;
            Camera.Position = Data.PlayerSpawnPos;
            Player.Load();
            ObjectBeingPlaced = Functions.CreateLevelObject((ObjType)ObjToCreateType);
            ObjectBeingPlaced.Active = false;
            ObjectBeingPlaced.Sprite.Colour.A = 100;
        }

        public override void Update()
        {
            if (ObjectBeingPlaced.Active)
            {
                if (Input.IsKeyPressed(Keys.Q) && ObjToCreateType > 1)
                {
                    ObjToCreateType--;
                    ObjectBeingPlaced = Functions.CreateLevelObject((ObjType)ObjToCreateType);
                }
                else if (Input.IsKeyPressed(Keys.E) && ObjToCreateType < Enum.GetNames(typeof(ObjType)).Length - 1)
                {
                    ObjToCreateType++;
                    ObjectBeingPlaced = Functions.CreateLevelObject((ObjType)ObjToCreateType);
                }
            }
            
            ObjectBeingPlaced.Sprite.Colour.A = 100;

            if (Input.IsKeyPressed(Keys.LeftAlt))
                TileMode = !TileMode;
            
            CameraVelocity = Vector2.Lerp(CameraVelocity, Vector2.Zero, .25f);

            if (Input.IsKeyDown(Keys.A))
                CameraVelocity.X = MathHelper.Lerp(CameraVelocity.X, -8f / Camera.Zoom, .25f);
            if (Input.IsKeyDown(Keys.D))
                CameraVelocity.X = MathHelper.Lerp(CameraVelocity.X, 8f / Camera.Zoom, .25f);
            if (Input.IsKeyDown(Keys.W))
                CameraVelocity.Y = MathHelper.Lerp(CameraVelocity.Y, -8f / Camera.Zoom, .25f);
            if (Input.IsKeyDown(Keys.S) && !Input.IsKeyDown(Keys.LeftControl))
                CameraVelocity.Y = MathHelper.Lerp(CameraVelocity.Y, 8f / Camera.Zoom, .25f);

            Camera.Position += CameraVelocity;
            
            if (Input.IsKeyPressed(Keys.F))
                ObjectBeingPlaced.Active = !ObjectBeingPlaced.Active;

            if (Input.IsLeftMouseReleased() && ObjectBeingPlaced.Active && !TileMode)
                Functions.SpawnLevelObject(ObjectBeingPlaced.Transform.Position.X,
                    ObjectBeingPlaced.Transform.Position.Y, ObjectBeingPlaced.Type);
            else if (Input.IsLeftMouseDown() && ObjectBeingPlaced.Active && TileMode && LastPlacedObject != ObjectBeingPlaced.Transform.Position.X + ObjectBeingPlaced.Transform.Position.Y && !Functions.IsObjectAt(Data.MousePosition))
            {
                Functions.SpawnLevelObject(
                    ObjectBeingPlaced.Transform.Position.X / ObjectBeingPlaced.Sprite.Texture.Width *
                    ObjectBeingPlaced.Sprite.Texture.Width,
                    ObjectBeingPlaced.Transform.Position.Y / ObjectBeingPlaced.Sprite.Texture.Height *
                    ObjectBeingPlaced.Sprite.Texture.Height, ObjectBeingPlaced.Type);
                
                LastPlacedObject = ObjectBeingPlaced.Transform.Position.X / ObjectBeingPlaced.Sprite.Texture.Width *
                    ObjectBeingPlaced.Sprite.Texture.Width + ObjectBeingPlaced.Transform.Position.Y /
                    ObjectBeingPlaced.Sprite.Texture.Height *
                    ObjectBeingPlaced.Sprite.Texture.Height;
            }

            if (!TileMode)
            {
                if (Input.IsKeyDown(Keys.LeftShift))
                {
                    ObjectBeingPlaced.Transform.Position = new Vector2(
                        (int) Data.MousePosition.X / 2 * 2 + (Math.Sign(Data.MousePosition.X) * 2),
                        (int) Data.MousePosition.Y / 2 * 2 + (Math.Sign(Data.MousePosition.Y) * 2));
                }
                else if (!Input.IsKeyDown(Keys.LeftShift) && !Input.IsKeyDown(Keys.LeftAlt))
                {
                    ObjectBeingPlaced.Transform.Position = new Vector2(
                        (int) Data.MousePosition.X / 4 * 4 + (Math.Sign(Data.MousePosition.X) * 4),
                        (int) Data.MousePosition.Y / 4 * 4 + (Math.Sign(Data.MousePosition.Y) * 4));
                }
            }
            else
            {
                ObjectBeingPlaced.Transform.Position = new Vector2(
                    (int) Data.MousePosition.X / ObjectBeingPlaced.Sprite.Texture.Width *
                    ObjectBeingPlaced.Sprite.Texture.Width + Math.Sign(Data.MousePosition.X) * ObjectBeingPlaced.Sprite.Texture.Width * .5f,
                    (int) Data.MousePosition.Y /
                    ObjectBeingPlaced.Sprite.Texture.Height *
                    ObjectBeingPlaced.Sprite.Texture.Height + Math.Sign(Data.MousePosition.Y) * ObjectBeingPlaced.Sprite.Texture.Height * .5f);
            }
            
            if (Input.IsKeyPressed(Keys.R))
            {
                Functions.SaveLevel(Data.CurrentLevel);
                SceneManager.EnterScene(SceneTypes.LevelScene);
            }

            if (Input.IsKeyPressed(Keys.OemMinus) && Camera.Zoom > 0)
                Camera.Zoom -= .25f;
            else if (Input.IsKeyPressed(Keys.OemPlus))
                Camera.Zoom += .25f;
            
            if (Input.IsMiddleMousePressed())
                Functions.SetPlayerSpawnPoint((int) Data.MousePosition.X / 4 * 4 + (Math.Sign(Data.MousePosition.X) * 4),
                    (int) Data.MousePosition.Y / 4 * 4 + (Math.Sign(Data.MousePosition.Y) * 4));
            
            if (Input.IsKeyPressed(Keys.NumPad0))
                Functions.LoadLevel(0);
            else if (Input.IsKeyPressed(Keys.NumPad1))
                Functions.LoadLevel(1);
            else if (Input.IsKeyPressed(Keys.NumPad2))
                Functions.LoadLevel(2);
            
            if (Input.IsLeftMousePressed() && !ObjectBeingPlaced.Active)
            {
                for (int i = 0; i < Pool.StaticObjects.Length; i++)
                {
                    if (Pool.StaticObjects[i].Active &&
                        new Rectangle(
                            Pool.StaticObjects[i].Transform.Position.ToPoint() -
                            (Pool.StaticObjects[i].Sprite.Texture.Bounds.Size.ToVector2() * Pool.StaticObjects[i].Transform.Scale * .5f).ToPoint(),
                            Pool.StaticObjects[i].Hitbox.Size.ToPoint() * Pool.StaticObjects[i].Transform.Scale.ToPoint()).Contains(Data.MousePosition))
                    {
                        GrabbedObjectID = i;
                        GrabbedObjectOffset = Pool.StaticObjects[i].Transform.Position - new Vector2((int) Data.MousePosition.X / 4 * 4, (int) Data.MousePosition.Y / 4 * 4);
                    }
                }
            }

            if (GrabbedObjectID != -1)
            {
                if (!Input.IsKeyDown(Keys.LeftShift))
                    Pool.StaticObjects[GrabbedObjectID].Transform.Position =
                        new Vector2((int) Data.MousePosition.X / 4 * 4, (int) Data.MousePosition.Y / 4 * 4) + GrabbedObjectOffset;
                else
                    Pool.StaticObjects[GrabbedObjectID].Transform.Position =
                        new Vector2((int) Data.MousePosition.X / 2 * 2, (int) Data.MousePosition.Y / 2 * 2) + GrabbedObjectOffset;
                
                if (Input.IsLeftMouseReleased())
                    GrabbedObjectID = -1;
            }

            if (!TileMode)
            {
                if (Input.IsRightMousePressed())
                {
                    for (int i = 0; i < Pool.StaticObjects.Length; i++)
                    {
                        if (Pool.StaticObjects[i].Active &&
                            new Rectangle(
                                Pool.StaticObjects[i].Transform.Position.ToPoint() -
                                (Pool.StaticObjects[i].Sprite.Texture.Bounds.Size.ToVector2() * Pool.StaticObjects[i].Transform.Scale * .5f).ToPoint(),
                                Pool.StaticObjects[i].Hitbox.Size.ToPoint() * Pool.StaticObjects[i].Transform.Scale.ToPoint()).Contains(Data.MousePosition))
                        {
                            Pool.StaticObjects[i].Active = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (Input.IsRightMouseDown())
                {
                    ObjectBeingPlaced.Sprite.Colour.A = 0;
                    for (int i = 0; i < Pool.StaticObjects.Length; i++)
                    {
                        if (Pool.StaticObjects[i].Active &&
                            new Rectangle(
                                Pool.StaticObjects[i].Transform.Position.ToPoint() -
                                (Pool.StaticObjects[i].Sprite.Texture.Bounds.Size.ToVector2() * Pool.StaticObjects[i].Transform.Scale * .5f).ToPoint(),
                                Pool.StaticObjects[i].Hitbox.Size.ToPoint() * Pool.StaticObjects[i].Transform.Scale.ToPoint()).Contains(Data.MousePosition))
                        {
                            Pool.StaticObjects[i].Active = false;
                            break;
                        }
                    }
                }
            }

            if (Input.IsKeyDown(Keys.LeftControl) && Input.IsKeyPressed(Keys.S))
                Functions.SaveLevel(Data.CurrentLevel);
        }

        public override void Draw()
        {
            Pool.DrawStaticObjects();
            Player.Draw();
            if (ObjectBeingPlaced.Active)
                Functions.Draw(ref ObjectBeingPlaced.Sprite, ref ObjectBeingPlaced.Transform);
        }

        public override void Unload()
        {
            Functions.SaveLevel(Data.CurrentLevel);
        }
    }
}