using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class WolfSpawner : NetworkBehaviour
    {
        [SerializeField] AnimationCurve softWolfCap;
        [Range(0f, 1f)]
        [SerializeField] float spawnChance;
        [Range(0f, 1f)]
        [SerializeField] float spawnChanceSoftCapped;
        [SerializeField] float spawnPeriod;

        [SerializeField] WolfController baseWolf;

        WolfSpawn[] spawnPoints;

        bool spawning = false;
        SyncTimer timer;

        float startTime = 0f;

        public override void OnStartServer()
        {
            spawnPoints = FindObjectsOfType<WolfSpawn>();
            timer = new SyncTimer(spawnPeriod, 0f);
            timer.Timeout += CheckSpawn;
        }

        public void StartSpawn()
        {
            spawning = true;
            startTime = Time.time;
        }

        private void FixedUpdate()
        {
            if (!isServer) return;
            if (!spawning) return;

            timer.Tick(Time.fixedDeltaTime);
        }

        private void CheckSpawn()
        {
            float time = Time.time - startTime;
            int softCap = Mathf.RoundToInt(softWolfCap.Evaluate(time));

            int wolfCount = FindObjectsOfType(typeof(WolfController)).Length;
            float chance = wolfCount >= softCap ? spawnChanceSoftCapped : spawnChance;

            if (UnityEngine.Random.value < chance)
                SpawnWolf();
        }

        private void SpawnWolf()
        {
            WolfSpawn spawn = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

            WolfController newWolf = Instantiate(baseWolf, spawn.transform.position, Quaternion.identity);
            newWolf.BiasTargetPos = spawn.TargetPos;
            NetworkServer.Spawn(newWolf.gameObject);
        }

        private void OnDrawGizmos()
        {
            if (spawnPoints == null) return;

            Gizmos.color = Color.magenta;
            foreach (WolfSpawn spawnPoint in spawnPoints)
            {
                if (spawnPoint == transform) continue;
                Gizmos.DrawWireSphere(spawnPoint.transform.position + Vector3.up * 0.2f, 1f);
            }
        }
    }
}
