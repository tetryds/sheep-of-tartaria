using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class HerderController : NetworkBehaviour
    {
        [SerializeField] float range;
        [SerializeField] int maxColliderCount = 128;

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
                HerdCmd();
            }
        }

        [Command]
        public void HerdCmd()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, range, colliders);

            for (int i = 0; i < count; i++)
            {
                var sheep = colliders[i].GetComponent<SheepController>();
                if (sheep != null)
                    Herd(sheep);
            }
        }

        public void Herd(SheepController sheep)
        {
            Vector3 dir = sheep.transform.position - transform.position;
            dir = Vector3.ProjectOnPlane(dir, Vector3.up);
            sheep.AddMoveEffect(dir.normalized);
        }
    }
}
