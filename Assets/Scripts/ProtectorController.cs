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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                AttackCmd();
            }
        }

        [Command]
        public void AttackCmd()
        {
            Debug.Log("attack");
            int count = Physics.OverlapSphereNonAlloc(transform.position, range, colliders);

            for (int i = 0; i < count; i++)
            {
                var sheepPlant = colliders[i].GetComponent<SheepPlantController>();
                if (sheepPlant != null)
                    sheepPlant.Reap();

            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.2f, range);
        }
    }
}
