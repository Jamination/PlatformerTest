using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PlatformerTest.Levels;

namespace PlatformerTest.Scenes
{
    public sealed class LevelScene : Scene
    {
        public static LevelScene Instance = new LevelScene();
        
        public override void Load()
        {
            Data.GameInstance.IsMouseVisible = false;
            Camera.Zoom = 1f;
            Camera.Position = Player.Transform.Position;

            Player.Load();
            
            Pool.StaticObjectTree.Clear();
            for (uint i = 0; i < Pool.StaticObjects.Length; i++)
            {
                if (!Pool.StaticObjects[i].Active || !Pool.StaticObjects[i].Collidable)
                    continue;
                
                Pool.StaticObjectTree.Add(new SpatialItem()
                {
                    Position = Pool.StaticObjects[i].Transform.Position,
                    Size = Pool.StaticObjects[i].Hitbox.Size,
                    Origin = -Pool.StaticObjects[i].Hitbox.Offset,
                    ObjectID = i,
                });
            }
        }

        public override void Update()
        {
            Player.Update();
            
            if (Input.IsKeyPressed(Keys.R))
                SceneManager.EnterScene(SceneTypes.EditorScene);
        }

        public override void Draw()
        {
            Pool.DrawStaticObjects();
            Player.Draw();
        }
    }
}