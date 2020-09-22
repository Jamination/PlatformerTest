using PlatformerTest.Levels;

namespace PlatformerTest.Scenes
{
    public static class SceneManager
    {
        public static Scene CurrentScene { get; private set; }
        public static Scene NextScene { get; private set; }

        public static SceneTypes SceneType;

        public static void Initialise()
        {
            CurrentScene = LevelScene.Instance;
            SceneType = SceneTypes.LevelScene;
            
            Functions.LoadLevel(Data.CurrentLevel);
            Camera.Load();
            
            CurrentScene.Load();
        }

        public static void EnterScene(SceneTypes sceneType)
        {
            SceneType = sceneType;
            switch (sceneType)
            {
                case SceneTypes.LevelScene:
                    NextScene = LevelScene.Instance;
                    break;
                case SceneTypes.EditorScene:
                    NextScene = EditorScene.Instance;
                    break;
            }
        }

        public static void RestartCurrentScene()
        {
            CurrentScene.Unload();
            CurrentScene.Load();
        }

        public static void UpdateScenes()
        {
            if (NextScene != null)
            {
                CurrentScene.Unload();
                CurrentScene = NextScene;
                NextScene = null;
                CurrentScene.Load();
            }
            CurrentScene.Update();
        }

        public static void DrawScenes()
        {
            CurrentScene.Draw();
        }
    }
}