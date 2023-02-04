using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class MoveController : NetworkBehaviour
    {
        [SerializeField] float speed;

        public override void OnStartClient()
        {
            if (!isOwned) return;

            FindObjectOfType<CameraController>().SetFollow(transform);
        }

        private void Update()
        {
            if (!isOwned) return;

            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            Vector3 dir = new Vector3(horizontal, 0f, vertical);
            Vector3.ClampMagnitude(dir, 1f);
            Vector3 move = speed * Time.deltaTime * dir;
            transform.Translate(move);
        }
    }
}
