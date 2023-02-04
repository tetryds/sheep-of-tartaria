using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class SheepNetworkManager : NetworkManager
    {
        [Header("Herder")]
        [SerializeField] Transform herderSpawn;
        [SerializeField] HerderController herderPrefab;

        [Header("Protector")]
        [SerializeField] Transform protectorSpawn;
        [SerializeField] ProtectorController protectorPrefab;

        [SerializeField] bool isHerder = true;

        public override void OnStartClient()
        {
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
            }
            else
            {
                SpawnProtector(conn);
            }

            //Transform startPos = GetStartPosition();
            //GameObject player = startPos != null
            //    ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            //    : Instantiate(playerPrefab);

            //// instantiating a "Player" prefab gives it the name "Player(clone)"
            //// => appending the connectionId is WAY more useful for debugging!
            //player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
            //NetworkServer.AddPlayerForConnection(conn, player);
        }

        private void SpawnHerder(NetworkConnectionToClient conn)
        {
            HerderController herder = Instantiate(herderPrefab, herderSpawn.position, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, herder.gameObject);
            Debug.Log("Ts");
        }

        private void SpawnProtector(NetworkConnectionToClient conn)
        {
            ProtectorController protector = Instantiate(protectorPrefab, protectorSpawn.position, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, protector.gameObject);
        }
    }
}
