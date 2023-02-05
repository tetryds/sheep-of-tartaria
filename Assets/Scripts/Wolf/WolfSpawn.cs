using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class WolfSpawn : MonoBehaviour
    {
        [SerializeField] Transform targetLocation;

        public Vector3 TargetPos => targetLocation.position;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.2f, 1f);
            Vector3 dir = targetLocation.position - transform.position;
            Gizmos.DrawRay(transform.position + Vector3.up * 0.2f, dir);
        }
    }
}
