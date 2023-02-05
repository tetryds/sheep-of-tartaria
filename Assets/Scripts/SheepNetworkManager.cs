using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class SheepNetworkManager : NetworkManager
    {
        [Header("Wolf")]
        [SerializeField] GameObject wolf;
        [SerializeField] WolfSpawner spawner;

        [Header("Sheep")]
        [SerializeField] GameObject sheep;

        [Header("Herder")]
        [SerializeField] Transform herderSpawn;
        [SerializeField] HerderController herderPrefab;

        [Header("Protector")]
        [SerializeField] Transform protectorSpawn;
        [SerializeField] ProtectorController protectorPrefab;

        [SerializeField] bool isHerder = true;

        int score;

        public override void OnStartClient()
        {
            NetworkClient.RegisterPrefab(sheep);
            NetworkClient.RegisterPrefab(wolf);
            NetworkClient.RegisterPrefab(herderPrefab.gameObject);
            NetworkClient.RegisterPrefab(protectorPrefab.gameObject);
        }

        public override void OnClientConnect()
        {
            if (!clientLoadedScene)
            {
                if (!NetworkClient.ready)
                    NetworkClient.Ready();


                NetworkClient.AddPlayer();
            }
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            if (isHerder)
            {
                SpawnHerder(conn);
                isHerder = false;
                //WIP: TEST ONLY
            }
            else
            {
                SpawnProtector(conn);

                //TODO: Manage game statee and win conditions
                spawner.StartSpawn();
            }
        }

        public void AddScore()
        {
            score++;
            Debug.Log($"Score incremented to {score}");
        }

        private void SpawnHerder(NetworkConnectionToClient conn)
        {
            HerderController herder = Instantiate(herderPrefab, herderSpawn.position, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, herder.gameObject);
        }

        private void SpawnProtector(NetworkConnectionToClient conn)
        {
            ProtectorController protector = Instantiate(protectorPrefab, protectorSpawn.position, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, protector.gameObject);
        }

    }
}
