using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class ProtectorController : NetworkBehaviour
    {
        [SerializeField] float range;
        [SerializeField] float maxAngle;

        [SerializeField] int maxColliderCount = 128;

        [SerializeField] MoveController move;
        Collider[] colliders;

        public override void OnStartServer()
        {
            colliders = new Collider[maxColliderCount];
        }

        private void Update()
        {
            if (!isOwned) return;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                AttackCmd();
            }
        }

        [Command]
        public void AttackCmd()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, range, colliders);

            for (int i = 0; i < count; i++)
            {
                Collider collider = colliders[i];
                Vector3 dir = collider.transform.position - transform.position;
                float angle = Vector3.Angle(move.LookDir, dir);
                if (angle > maxAngle) continue;

                if (collider.TryGetComponent(out SheepPlantController sheepPlant))
                    sheepPlant.Reap();
                if (collider.TryGetComponent(out WolfController wolf))
                    wolf.ScareAway();
            }
        }

        private void OnDrawGizmos()
        {
            if (!enabled) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.2f, range);
        }
    }
}
