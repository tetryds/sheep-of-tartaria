using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class FenceController : NetworkBehaviour
    {
        [SerializeField] SheepNetworkManager networkManager;

        private void OnCollisionEnter(Collision collision)
        {
            if (!isServer) return;

            var sheep = collision.collider.GetComponent<SheepController>();
            if (sheep == null) return;

            NetworkServer.Destroy(sheep.gameObject);
            networkManager.AddScore();
        }
    }
}
