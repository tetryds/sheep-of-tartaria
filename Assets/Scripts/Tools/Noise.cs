using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Animations;

namespace Sheep
{
    public class Noise
    {
        public static Vector3 GetPerlinNoiseXZ(float scale, float time, float frequency, float xShift, float zShift)
        {
            float x = (Mathf.PerlinNoise(xShift, time * frequency) - 0.5f) * scale * 2f;
            float y = 0f;
            float z = (Mathf.PerlinNoise(zShift, time * frequency) - 0.5f) * scale * 2f;

            return new Vector3(x, y, z);
        }
    }
}
