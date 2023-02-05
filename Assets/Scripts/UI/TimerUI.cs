using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sheep
{
    public class TimerUI : NetworkBehaviour
    {
        [SerializeField] TextMeshProUGUI timerText;

        float timeout;

        [ClientRpc]
        public void StartTimer(float timeout)
        {
            this.timeout = timeout;
        }

        private void FixedUpdate()
        {
            if (timeout <= 0) return;

            timeout -= Time.fixedDeltaTime;
            TimeSpan time = TimeSpan.FromSeconds(timeout);
            timerText.text = time.ToString("mm':'ss");
        }
    }
}
