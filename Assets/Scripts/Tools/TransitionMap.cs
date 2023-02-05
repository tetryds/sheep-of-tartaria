using System;
using System.Collections.Generic;

namespace Sheep
{
    internal class TransitionMap<T>
    {
        Dictionary<T, Transition<T>> transitionMap = new Dictionary<T, Transition<T>>();
        Transition<T> global;

        public void SetGlobalTransition(T to, Action trigger)
        {
            global = new Transition<T>(to, trigger);
        }

        public void AddTransition(T from, T to, Action trigger)
        {
            Transition<T> transition = new Transition<T>(to, trigger);
            if (transitionMap.ContainsKey(from))
                throw new Exception($"Transition from '{from}' to '{to}' already exists!");
            transitionMap.Add(from, transition);
        }

        public bool TryGetTransition(T from, out Transition<T> transition)
        {
            if (transitionMap.TryGetValue(from, out transition))
            {
                return true;
            }
            if (global != null)
            {
                transition = global;
                return true;
            }
            return false;
        }
    }
}