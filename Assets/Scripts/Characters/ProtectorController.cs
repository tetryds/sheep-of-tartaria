using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class ProtectorController : NetworkBehaviour
    {
        [SerializeField] float range;

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
                if (colliders[i].TryGetComponent(out SheepPlantController sheepPlant))
                    sheepPlant.Reap();
                if (colliders[i].TryGetComponent(out WolfController wolf))
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
