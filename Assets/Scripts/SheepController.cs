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
        RigidbodyConstraints defaultConstraints;

        public bool Free { get; private set; } = true;

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
            if (!Free) return;
            Free = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.velocity = Vector3.zero;
            rb.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.down);
            rb.constraints = defaultConstraints;
            rb.isKinematic = true;
        }

        public void Release()
        {
            Free = true;
            rb.velocity = Vector3.zero;
            rb.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            rb.constraints = defaultConstraints;
            rb.isKinematic = false;
        }

        private void FixedUpdate()
        {
            if (!isServer) return;

            if (!Free) return;

            Move(Time.fixedDeltaTime);
            WindDown(Time.fixedDeltaTime);
            AddRandom(Time.fixedDeltaTime);
        }

        private void Move(float deltaTime)
        {
            //Vector3 move = speed * deltaTime * moveDir;
            Vector3 move = Vector3.ClampMagnitude(moveDir, 1f);
            Vector3 gravity = Vector3.down * 5f;
            rb.velocity = speed * move + gravity;
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
