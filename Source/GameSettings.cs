﻿using Microsoft.Xna.Framework;

namespace PlatformerTest
{
    public static class GameSettings
    {
        public const int StartWindowWidth = 1920;
        public const int StartWindowHeight = 1080;
        
        public const int VirtualWindowWidth = 640;
        public const int VirtualWindowHeight = 360;
        
        public const bool StartFullScreen = true;

        public static readonly Color ClearColour = new Color(0f, .05f, .1f, 1f);
    }
}