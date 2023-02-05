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
        [SerializeField] float skillCooldown;

        [SerializeField] int maxColliderCount = 128;

        [SerializeField] MoveController move;
        Collider[] colliders;

        SyncTimer skillTimer;
        bool skillEnabled = true;

        CooldownUI cooldownUI;

        public override void OnStartServer()
        {
            colliders = new Collider[maxColliderCount];

            skillTimer = new SyncTimer(skillCooldown, 0f);
            skillTimer.Timeout += () => skillEnabled = true;
        }

        public override void OnStartClient()
        {
            if (!isOwned) return;
            cooldownUI = FindObjectOfType<CooldownUI>();
        }

        private void Update()
        {
            if (!isOwned) return;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                AttackCmd(move.LookDir);
            }
        }

        private void FixedUpdate()
        {
            if (!isServer) return;

            if (!skillEnabled)
                skillTimer.Tick(Time.fixedDeltaTime);
        }

        [Command]
        public void AttackCmd(Vector3 lookDir)
        {
            if (!skillEnabled) return;
            Debug.Log("Attack CMD");

            int count = Physics.OverlapSphereNonAlloc(transform.position, range, colliders);

            for (int i = 0; i < count; i++)
            {
                Collider collider = colliders[i];
                Vector3 dir = collider.transform.position - transform.position;
                float angle = Vector3.Angle(lookDir, dir);
                if (angle > maxAngle) continue;

                if (collider.TryGetComponent(out SheepPlantController sheepPlant))
                    sheepPlant.Reap();
                if (collider.TryGetComponent(out WolfController wolf))
                    wolf.ScareAway();
            }

            skillEnabled = false;

            ResetCooldown();
        }

        [ClientRpc]
        public void ResetCooldown()
        {
            cooldownUI?.SetCooldownTimer(skillCooldown);
        }

        private void OnDrawGizmos()
        {
            if (!enabled) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.2f, range);
        }
    }
}
