using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class WolfController : NetworkBehaviour
    {
        [SerializeField] float searchRandomIntensity;

        [SerializeField] float searchSpeed;
        [SerializeField] float chaseSpeed;
        [SerializeField] float runSpeed;

        [SerializeField] float range;
        [SerializeField] int maxColliderCount = 32;
        [SerializeField] Rigidbody rb;

        Vector3 moveDir;

        WolfState state;

        Collider[] colliders;
        Dictionary<WolfState, Action<float>> stateMap;

        SheepController targetSheep;

        public override void OnStartServer()
        {
            colliders = new Collider[maxColliderCount];

            stateMap = new Dictionary<WolfState, Action<float>>
            {
                [WolfState.Searching] = DoSearch,
                [WolfState.Chasing] = DoChase,
                [WolfState.Running] = DoRun
            };
        }

        public void ScareAway()
        {
            state = WolfState.Running;
            DropSheep();
        }

        private void FixedUpdate()
        {
            if (!isServer) return;

            stateMap[state].Invoke(Time.fixedDeltaTime);

            ScanSurroundings();
        }


        private void DoSearch(float deltaTime)
        {
            ScanSurroundings();

            Vector2 randomFactor = UnityEngine.Random.insideUnitCircle * searchRandomIntensity;
            moveDir += new Vector3(randomFactor.x, 0f, randomFactor.y);
            moveDir = Vector3.ClampMagnitude(moveDir, 1f);
            rb.velocity = searchSpeed * moveDir;

            if (targetSheep != null)
                state = WolfState.Chasing;
        }

        private void DoChase(float deltaTime)
        {
            //Another closer sheep might show up
            ScanSurroundings();

            moveDir = (targetSheep.transform.position - transform.position).normalized;
            rb.velocity = chaseSpeed * moveDir;
        }

        private void DoRun(float deltaTime)
        {
            //WIP: Run away from the center

            moveDir = transform.position.normalized;
            rb.velocity = runSpeed * moveDir;
        }

        public void ScanSurroundings()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, range, colliders);

            SheepController closest = null;
            float closestSqrDistance = float.PositiveInfinity;

            for (int i = 0; i < count; i++)
            {
                var sheep = colliders[i].GetComponent<SheepController>();
                if (sheep == null) continue;

                float sqrDistance = (transform.position - sheep.transform.position).sqrMagnitude;
                if (sqrDistance < closestSqrDistance)
                {
                    closest = sheep;
                    closestSqrDistance = sqrDistance;
                }
            }

            if (closest != null)
            {
                targetSheep = closest;
            }
        }

        private void PickSheep(SheepController sheep)
        {
            state = WolfState.Running;
        }

        private void DropSheep()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isServer) return;

            var sheep = collision.collider.GetComponent<SheepController>();
            if (sheep == null) return;

            PickSheep(sheep);
            //Collect sheep
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.2f, range);
        }
    }
}
