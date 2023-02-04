using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class MoveController : NetworkBehaviour
    {
        [SerializeField] float speed;

        public Vector3 Dir { get; private set; }

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

            Dir = new Vector3(horizontal, 0f, vertical);
            Dir = Vector3.ClampMagnitude(Dir, 1f);
            Vector3 move = speed * Time.deltaTime * Dir;
            transform.Translate(move);
        }
    }
}
