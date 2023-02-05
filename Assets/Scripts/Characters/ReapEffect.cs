using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class ReapEffect : MonoBehaviour
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
            transform.Rotate(Vector3.up, speed * Time.deltaTime);

            if (Time.time - spawnTime > duration)
                Destroy(gameObject);
        }

    }
}
