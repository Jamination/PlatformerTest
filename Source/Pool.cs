using System;
using Dcrew.Spatial;

namespace PlatformerTest
{
    public static class Pool
    {
        public static GameObject[] StaticObjects = new GameObject[1000];

        public static Quadtree<SpatialItem> StaticObjectTree = new Quadtree<SpatialItem>();
        
        public static void DrawStaticObjects()
        {
            for (int i = 0; i < StaticObjects.Length; i++)
            {
                if (StaticObjects[i].Active && StaticObjects[i].Type != ObjType.Undefined)
                    Functions.Draw(ref StaticObjects[i].Sprite, ref StaticObjects[i].Transform);
            }
        }

        public static void Reset()
        {
            for (int i = 0; i < StaticObjects.Length; i++)
                StaticObjects[i].Active = false;
            StaticObjectTree.Clear();
        }
    }
}