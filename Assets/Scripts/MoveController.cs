using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class MoveController : NetworkBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] MoveDirIcon dirIcon;

        public Vector3 Dir { get; private set; }
        public Vector3 LookDir { get; private set; }

        public override void OnStartClient()
        {
            Dir = Vector3.right;
            LookDir = Dir.normalized;

            if (!isOwned) return;
            FindObjectOfType<CameraController>().SetFollow(transform);
            dirIcon.ShowIcon(true);
        }

        private void Update()
        {
            if (!isOwned) return;

            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            Dir = new Vector3(horizontal, 0f, vertical);
            Dir = Vector3.ClampMagnitude(Dir, 1f);
            Vector3 move = speed * Time.deltaTime * Dir;
            if (Dir != Vector3.zero)
                LookDir = Dir.normalized;
            transform.Translate(move);
        }
    }
}
