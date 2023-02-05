using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sheep
{
    public class GameManager : NetworkBehaviour
    {
        [Header("Gameplay")]
        [SerializeField] int requiredSheepCount;
        [SerializeField] float gameTimeout;
        [SerializeField] float restartTimeout;

        [SerializeField] WolfSpawner spawner;
        [SerializeField] SheepCounterUI sheepCounter;
        [SerializeField] TimerUI timerUI;

        [SerializeField] SheepNetworkManager networkManager;

        [Header("UI")]
        [SerializeField] GameObject startUI;
        [SerializeField] GameObject wonUI;
        [SerializeField] GameObject lostUI;

        int playerCount;
        int score = 0;

        StateMachine<GameState, GameEvent, Action<float>> stateMachine;

        float lastTransitionTime = 0f;

        public override void OnStartServer()
        {
            stateMachine = new StateMachine<GameState, GameEvent, Action<float>>(GameState.Awaiting, DoAwait)
                .AddState(GameState.Running, DoRun)
                .AddState(GameState.Won, DoWin)
                .AddState(GameState.Lost, DoLose)
                .AddTransition(GameEvent.Start, GameState.Awaiting, GameState.Running, OnStart)
                .AddTransition(GameEvent.Win, GameState.Running, GameState.Won, OnWin)
                .AddTransition(GameEvent.Lose, GameState.Running, GameState.Lost, OnLose);
        }

        ////Might move this to the UI itself
        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        //    {
        //        //Connect and play
        //    }
        //}

        private void FixedUpdate()
        {
            if (!isServer) return;

            stateMachine.Behavior?.Invoke(Time.fixedDeltaTime);
        }

        private void DoAwait(float deltaTime)
        {

        }

        private void DoRun(float deltaTime)
        {
            if (Time.time - lastTransitionTime > gameTimeout)
            {
                stateMachine.RaiseEvent(GameEvent.Lose);
            }
        }

        private void DoWin(float deltaTime)
        {
            if (Time.time - lastTransitionTime > restartTimeout)
            {
                RestartGame();
            }
        }

        private void DoLose(float deltaTime)
        {
            if (Time.time - lastTransitionTime > restartTimeout)
            {
                RestartGame();
            }
        }

        private void OnStart()
        {
            DisableStartUI();

            sheepCounter.EnableUI();
            sheepCounter.SetRequired(requiredSheepCount);

            spawner.StartSpawn();

            lastTransitionTime = Time.time;

            timerUI.StartTimer(gameTimeout);
        }

        private void OnWin()
        {
            EnableWonUI();

            lastTransitionTime = Time.time;
        }

        private void OnLose()
        {
            EnableLostUI();

            lastTransitionTime = Time.time;
        }

        [ClientRpc]
        private void DisableStartUI()
        {
            startUI?.SetActive(false);
            wonUI?.SetActive(false);
            lostUI?.SetActive(false);
        }

        [ClientRpc]
        private void EnableWonUI()
        {
            startUI?.SetActive(false);
            wonUI?.SetActive(true);
            lostUI?.SetActive(false);
        }

        [ClientRpc]
        public void RestartGame()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                networkManager.StopHost();
            }
            else if (NetworkServer.active)
            {
                networkManager.StopServer();
            }

            ReloadScene();
        }

        private void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        [ClientRpc]
        private void EnableLostUI()
        {
            startUI?.SetActive(false);
            wonUI?.SetActive(false);
            lostUI?.SetActive(true);
        }

        public void AddScore()
        {
            score++;
            if (score >= requiredSheepCount)
                stateMachine.RaiseEvent(GameEvent.Win);
        }

        public void PlayerSpawned()
        {
            playerCount++;
            if (playerCount >= 2)
                stateMachine.RaiseEvent(GameEvent.Start);
        }

    }
}
