using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class SheepController : NetworkBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] float windDown;
        [SerializeField] float randomWeight;
        [SerializeField] Rigidbody rb;

        Vector3 moveDir;

        bool free = true;

        RigidbodyConstraints defaultConstraints;

        public void AddMoveEffect(Vector3 dir)
        {
            Vector2 randomFactor = Random.insideUnitCircle * randomWeight;
            moveDir += dir + new Vector3(randomFactor.x, 0f, randomFactor.y);
        }

        public void ForceMove(Vector3 pos)
        {
            rb.MovePosition(pos);
        }

        private void Start()
        {
            defaultConstraints = rb.constraints;
        }

        public void PickUp()
        {
            free = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.velocity = Vector3.zero;
            rb.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.down);
            rb.constraints = defaultConstraints;
            rb.isKinematic = true;
        }

        public void Release()
        {
            free = true;
            rb.velocity = Vector3.zero;
            rb.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            rb.constraints = defaultConstraints;
            rb.isKinematic = false;
        }

        private void FixedUpdate()
        {
            if (!isServer) return;

            if (!free) return;

            Move(Time.fixedDeltaTime);
            WindDown(Time.fixedDeltaTime);
            AddRandom(Time.fixedDeltaTime);
        }

        private void Move(float deltaTime)
        {
            //Vector3 move = speed * deltaTime * moveDir;
            Vector3 move = Vector3.ClampMagnitude(moveDir, 1f);
            rb.velocity = speed * move;
        }

        private void WindDown(float deltaTime)
        {
            moveDir *= 1 - windDown * deltaTime;
        }

        private void AddRandom(float deltaTime)
        {
            //Add random from perlin noise
        }
    }
}
