using System.Collections;
using System.Collections.Generic;
using System.Media;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class FenceController : NetworkBehaviour
    {
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip clip;
        [SerializeField] GameManager gameManager;

        private void OnCollisionEnter(Collision collision)
        {
            if (!isServer) return;

            var sheep = collision.collider.GetComponent<SheepController>();
            if (sheep == null) return;

            NetworkServer.Destroy(sheep.gameObject);
            gameManager.AddScore();
        }

        [ClientRpc]
        public void PlaySound()
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
