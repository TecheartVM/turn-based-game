using System;
using UnityEngine;

namespace Behaviour.Structures
{
    [Serializable]
    public struct PoissonHexSamplingSettings
    {
        [Range(1, 16)]
        public int maxSamplesPerNode;

        [Range(3, 16)]
        public int minRadius;

        [Range(4, 16)]
        public int maxRadius;

        public int fieldPadding;

        public int fieldExtent;

        public int iterations;
    }
}