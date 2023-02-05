using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class BerranteEffect : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] float duration;

        float spawnTime;

        private void Start()
        {
            spawnTime = Time.time;
        }

        private void Update()
        {
            transform.Translate(speed * Time.deltaTime * Vector3.forward);

            if (Time.time - spawnTime > duration)
                Destroy(gameObject);
        }

    }
}
