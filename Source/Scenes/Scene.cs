﻿namespace PlatformerTest.Scenes
{
    public abstract class Scene
    {
        public abstract void Load();
        public abstract void Update();
        public abstract void Draw();
        public virtual void Unload() { }
    }
}