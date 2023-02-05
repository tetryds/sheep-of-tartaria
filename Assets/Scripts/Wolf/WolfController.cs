using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class WolfController : NetworkBehaviour
    {
        [Header("Search")]
        [SerializeField] float searchRandomFrequency;
        [SerializeField] float searchRandomIntensity;
        [SerializeField] AnimationCurve searchBiasCurve;

        [Header("Speed")]
        [SerializeField] float searchSpeed;
        [SerializeField] float chaseSpeed;
        [SerializeField] float runSpeed;
        [SerializeField] float runRandomIntensity;

        [Header("Behavior")]
        [SerializeField] float range;
        [SerializeField] int maxColliderCount = 32;
        [SerializeField] Rigidbody rb;
        [SerializeField] Transform sheepPivot;

        [Header("Scare")]
        [SerializeField] float scareRange;
        [SerializeField] float scareIntensity;
        [SerializeField] float scareRandomIntensity;

        Vector3 moveDir;

        WolfState state;

        Collider[] colliders;
        Dictionary<WolfState, Action<float>> stateMap;

        SheepController targetSheep;

        SheepController pickedSheep;

        public Vector3 BiasTargetPos;

        float timeShift;

        WolfSpawn closestSpawn;

        public override void OnStartServer()
        {
            colliders = new Collider[maxColliderCount];

            stateMap = new Dictionary<WolfState, Action<float>>
            {
                [WolfState.Searching] = DoSearch,
                [WolfState.Chasing] = DoChase,
                [WolfState.Running] = DoRun
            };

            timeShift = UnityEngine.Random.value * 200f;
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

            if (pickedSheep != null)
                pickedSheep.ForceMove(sheepPivot.position);
        }


        private void DoSearch(float deltaTime)
        {
            ScanSurroundings();

            moveDir += Noise.GetPerlinNoiseXZ(searchRandomIntensity, Time.time * timeShift, searchRandomFrequency, 0f, 0.25f);
            Vector3 biasDir = BiasTargetPos - transform.position;
            float searchBiasWeight = searchBiasCurve.Evaluate(biasDir.magnitude);
            moveDir += biasDir.normalized * searchBiasWeight;
            moveDir = Vector3.ClampMagnitude(moveDir, 1f);
            rb.velocity = searchSpeed * moveDir;

            if (targetSheep != null)
                state = WolfState.Chasing;
        }

        private void DoChase(float deltaTime)
        {
            if (targetSheep != null && !targetSheep.Free)
            {
                state = WolfState.Searching;
                return;
            }
            //Another closer sheep might show up
            ScanSurroundings();

            if (targetSheep != null)
                moveDir = (targetSheep.transform.position - transform.position).normalized;
            rb.velocity = chaseSpeed * moveDir;
        }

        private void DoRun(float deltaTime)
        {
            //WIP: Run away from the center towards something that makes sense
            if (closestSpawn == null)
                closestSpawn = GetClosestSpawn();
            moveDir = (closestSpawn.transform.position - transform.position).normalized;
            moveDir += Noise.GetPerlinNoiseXZ(runRandomIntensity, Time.time * timeShift, searchRandomFrequency, 0f, 0.25f);
            moveDir = Vector3.ClampMagnitude(moveDir, 1f);
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
                if (sheep == null || !sheep.Free) continue;

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

        private WolfSpawn GetClosestSpawn()
        {
            WolfSpawn closest = null;
            float closestSqrDistance = float.PositiveInfinity;

            WolfSpawn[] spawns = FindObjectsOfType<WolfSpawn>();

            for (int i = 0; i < spawns.Length; i++)
            {
                float sqrDistance = (transform.position - spawns[i].transform.position).sqrMagnitude;
                if (sqrDistance < closestSqrDistance)
                {
                    closest = spawns[i];
                    closestSqrDistance = sqrDistance;
                }
            }
            return closest;
        }

        private void PickSheep(SheepController sheep)
        {
            state = WolfState.Running;
            sheep.PickUp();
            pickedSheep = sheep;

            ScareSheep();
        }

        private void DropSheep()
        {
            pickedSheep?.Release();
            pickedSheep = null;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isServer) return;

            if (collision.collider.TryGetComponent(out SheepController sheep))
                PickSheep(sheep);

            if (state == WolfState.Running)
            {
                if (collision.collider.TryGetComponent(out WolfSpawn _))
                    Despawn();
            }

        }

        private void ScareSheep()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, scareRange, colliders);

            for (int i = 0; i < count; i++)
            {
                if (colliders[i].TryGetComponent(out SheepController sheep))
                {
                    Vector2 randomFactor = UnityEngine.Random.insideUnitCircle * scareRandomIntensity;
                    Vector3 randomDir = new Vector3(randomFactor.x, 0f, randomFactor.y);
                    Vector3 dir = (sheep.transform.position - transform.position).normalized * scareIntensity;
                    dir = Vector3.ProjectOnPlane(dir, Vector3.up);
                    sheep.AddMoveEffect(dir + randomDir);
                }
            }
        }

        private void Despawn()
        {
            if (pickedSheep != null)
            {
                NetworkServer.Destroy(pickedSheep.gameObject);
                pickedSheep = null;
            }
            NetworkServer.Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.2f, range);
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.2f, scareRange);
        }
    }
}
