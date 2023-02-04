using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Animations;

namespace Sheep
{
    public class SyncTimer
    {
        //How often should the timer expire
        readonly float period;

        //Accumulated elapsed time since last Timeout expire
        float elapsed;

        //Timeout event that will be invoked every time the period is ellapsed
        public event Action Timeout;

        /// <summary>
        /// Creates a SyncTimer instance.
        /// </summary>
        /// <param name="period">Period of time between timeouts. If this period is 0 or lower, expires every tick.</param>
        /// <param name="initialElapsed">Sets initial elapsed time for the timer.</param>
        public SyncTimer(float period, float initialElapsed)
        {
            this.period = period < 0f ? 0f : period;
            elapsed = initialElapsed;
        }

        /// <summary>
        /// Advances timer by deltaTime. If the internal timer has expired, invokes Timeout callback.
        /// If the deltaTime is larger than the period, Timeout can be invoked more than once per Tick.
        /// </summary>
        /// <param name="deltaTime">Elapsed time in seconds. Has no effect if zero or lower</param>
        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0) return;

            if (period == 0)
            {
                Timeout?.Invoke();
                return;
            }
            else
            {
                elapsed += deltaTime;

                while (elapsed >= period)
                {
                    elapsed -= period;
                    Timeout?.Invoke();
                }
            }
        }
    }
}
