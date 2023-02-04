using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Animations;

namespace Sheep
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] PositionConstraint positionConstraint;
        
        public void SetFollow(Transform transform)
        {
            positionConstraint.SetSources(new List<ConstraintSource> { new ConstraintSource() { sourceTransform = transform, weight = 1f } });
            positionConstraint.constraintActive = true;
        }
    }
}
