using System;

namespace Base
{
    public static class TileProperties
    {
        public const float Width = 1.0f;

        public static readonly float OuterRadius = Width / (float)Math.Sqrt(3.0d);
        public static readonly float InnerRadius = Width * 0.5f;
        public static readonly float Height = OuterRadius + OuterRadius;
    }
}