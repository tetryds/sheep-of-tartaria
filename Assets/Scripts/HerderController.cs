using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class HerderController : NetworkBehaviour
    {
        [SerializeField] float range;
        [SerializeField] AnimationCurve lateralHerd;
        [SerializeField] AnimationCurve forwardHerd;
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
                HerdCmd(move.Dir);
            }
        }

        [Command]
        public void HerdCmd(Vector3 moveDir)
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, range, colliders);

            for (int i = 0; i < count; i++)
            {
                var sheep = colliders[i].GetComponent<SheepController>();
                if (sheep != null)
                    Herd(sheep, moveDir);
            }
        }

        public void Herd(SheepController sheep, Vector3 moveDir)
        {
            Vector3 dir = sheep.transform.position - transform.position;
            dir = Vector3.ProjectOnPlane(dir, Vector3.up);

            Vector3 forwardDir = Vector3.Project(dir, moveDir);
            bool isInFront = Vector3.Dot(forwardDir, moveDir) >= 0f;
            float forwardIntensity = isInFront ? forwardHerd.Evaluate(forwardDir.magnitude) : 0f;
            Vector3 forwardMove = forwardIntensity * forwardDir.normalized;

            Vector3 lateralDir = Vector3.ProjectOnPlane(dir, moveDir);
            float lateralIntensity = lateralHerd.Evaluate(lateralDir.magnitude);
            Vector3 lateralMove = lateralDir.normalized * lateralIntensity;

            Vector3 move = forwardMove + lateralMove;

            sheep.AddMoveEffect(move);
        }
    }
}
