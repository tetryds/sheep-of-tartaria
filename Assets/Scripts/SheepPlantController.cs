using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class SheepPlantController : NetworkBehaviour
    {
        [SerializeField] GameObject baseSheep;

        public void Reap()
        {
            Debug.Log("reap");
            GameObject sheep = Instantiate(baseSheep, transform.position, Quaternion.identity);
            NetworkServer.Spawn(sheep);
            NetworkServer.Destroy(gameObject);
        }
    }
}
