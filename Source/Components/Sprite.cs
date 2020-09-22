﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformerTest.Components
{
    public struct Sprite
    {
        public Texture2D Texture;
        public Rectangle? SourceRect;
        public Color Colour;
        public Vector2 Origin;
        public SpriteEffects Effects;
        public float Depth;
        public bool Centered;
    }
}