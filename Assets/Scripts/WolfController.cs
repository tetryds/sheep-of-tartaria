using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class WolfController : NetworkBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] float windDown;
        [SerializeField] float randomWeight;
        [SerializeField] Rigidbody rb;

        Vector3 moveDir;

        public void AddMoveEffect(Vector3 dir)
        {
            Vector2 randomFactor = Random.insideUnitCircle * randomWeight;
            moveDir += dir + new Vector3(randomFactor.x, 0f, randomFactor.y);
        }

        private void FixedUpdate()
        {
            if (!isServer) return;

            Move(Time.fixedDeltaTime);
            WindDown(Time.fixedDeltaTime);
            AddRandom(Time.fixedDeltaTime);
        }

        private void Move(float deltaTime)
        {
            //Vector3 move = speed * deltaTime * moveDir;
            rb.velocity = speed * moveDir;
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
