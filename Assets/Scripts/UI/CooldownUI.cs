using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Sheep
{
    public class CooldownUI : MonoBehaviour
    {
        [SerializeField] Image image;

        float resetTime;
        float timeout;

        private void Start()
        {
            timeout = 1f;
            resetTime = -1f;
        }

        public void SetCooldownTimer(float timeout)
        {
            resetTime = Time.time;
            this.timeout = timeout;
        }

        private void FixedUpdate()
        {
            float timeLeft = Time.time - resetTime;
            float percentLeft = timeLeft / timeout;
            percentLeft = Mathf.Clamp(percentLeft, 0f, 1f);
            image.fillAmount = percentLeft;
        }
    }
}
