using System;
using System.Collections.Generic;

namespace Sheep
{
    public class StateMachine<TState, TEvent, TBehav>
    {
        Dictionary<TState, TBehav> stateMap = new Dictionary<TState, TBehav>();

        Dictionary<TEvent, TransitionMap<TState>> eventMap = new Dictionary<TEvent, TransitionMap<TState>>();

        TState current;

        public TState Current
        {
            get => current;
            private set
            {
                current = value;
                StateChanged?.Invoke(current);
            }
        }

        public TBehav Behavior => stateMap[current];

        public event Action<TState> StateChanged;

        public StateMachine(TState initial)
        {
            AddState(initial);
            Current = initial;
        }

        public StateMachine(TState initial, TBehav behavior)
        {
            AddState(initial, behavior);
            Current = initial;
        }

        public StateMachine<TState, TEvent, TBehav> AddState(TState key)
        {
            AddState(key, default(TBehav));
            return this;
        }

        public StateMachine<TState, TEvent, TBehav> AddState(TState key, TBehav behavior)
        {
            stateMap.Add(key, behavior);
            return this;
        }

        public StateMachine<TState, TEvent, TBehav> AddTransition(TEvent key, TState from, TState to)
        {
            AddTransition(key, from, to, null);
            return this;
        }

        public StateMachine<TState, TEvent, TBehav> AddTransition(TEvent key, TState from, TState to, Action trigger)
        {
            if (!stateMap.ContainsKey(from))
                throw new Exception("Attempting to add transition FROM unknown state. Add state before creating transitions.");

            if (!stateMap.ContainsKey(to))
                throw new Exception("Attempting to add transition TO unknown state. Add state before creating transitions.");

            if (!eventMap.ContainsKey(key))
                eventMap.Add(key, new TransitionMap<TState>());

            TransitionMap<TState> tMap = eventMap[key];
            tMap.AddTransition(from, to, trigger);
            return this;
        }

        public StateMachine<TState, TEvent, TBehav> AddGlobalTransition(TEvent key, TState to)
        {
            AddGlobalTransition(key, to, null);
            return this;
        }

        public StateMachine<TState, TEvent, TBehav> AddGlobalTransition(TEvent key, TState to, Action trigger)
        {
            if (!stateMap.ContainsKey(to))
                throw new Exception($"Attempting to add transition TO unknown state '{to}'. Add state before creating transitions.");

            if (!eventMap.ContainsKey(key))
                eventMap.Add(key, new TransitionMap<TState>());

            eventMap[key].SetGlobalTransition(to, trigger);
            return this;
        }

        public void SetState(TState key)
        {
            if (!stateMap.ContainsKey(key))
                throw new Exception($"Cannot set unknown state '{key}'. Make sure it has been added.");

            Current = key;
        }

        public void RaiseEvent(TEvent eventKey)
        {
            if (eventMap.TryGetValue(eventKey, out TransitionMap<TState> map)
                && map.TryGetTransition(Current, out Transition<TState> transition))
            {
                Current = transition.Target;
                transition.Trigger();
            }
        }
    }

    //Behaviourless
    public class StateMachine<TState, TEvent>
    {
        HashSet<TState> states = new HashSet<TState>();

        Dictionary<TEvent, TransitionMap<TState>> eventMap = new Dictionary<TEvent, TransitionMap<TState>>();

        TState current;

        public TState Current
        {
            get => current;
            private set
            {
                current = value;
                StateChanged?.Invoke(current);
            }
        }

        public event Action<TState> StateChanged;

        public StateMachine(TState initial)
        {
            AddState(initial);
            Current = initial;
        }

        public StateMachine<TState, TEvent> AddState(TState key)
        {
            states.Add(key);
            return this;
        }

        public StateMachine<TState, TEvent> AddTransition(TEvent key, TState from, TState to)
        {
            AddTransition(key, from, to, null);
            return this;
        }

        public StateMachine<TState, TEvent> AddTransition(TEvent key, TState from, TState to, Action trigger)
        {
            if (!states.Contains(from))
                throw new Exception("Attempting to add transition FROM unknown state. Add state before creating transitions.");

            if (!states.Contains(to))
                throw new Exception("Attempting to add transition TO unknown state. Add state before creating transitions.");

            if (!eventMap.ContainsKey(key))
                eventMap.Add(key, new TransitionMap<TState>());

            TransitionMap<TState> tMap = eventMap[key];
            tMap.AddTransition(from, to, trigger);
            return this;
        }

        public StateMachine<TState, TEvent> AddGlobalTransition(TEvent key, TState to)
        {
            AddGlobalTransition(key, to, null);
            return this;
        }

        public StateMachine<TState, TEvent> AddGlobalTransition(TEvent key, TState to, Action trigger)
        {
            if (!states.Contains(to))
                throw new Exception($"Attempting to add transition TO unknown state '{to}'. Add state before creating transitions.");

            if (!eventMap.ContainsKey(key))
                eventMap.Add(key, new TransitionMap<TState>());

            eventMap[key].SetGlobalTransition(to, trigger);
            return this;
        }

        public void SetState(TState key)
        {
            if (!states.Contains(key))
                throw new Exception($"Cannot set unknown state '{key}'. Make sure it has been added.");

            Current = key;
        }

        public void RaiseEvent(TEvent eventKey)
        {
            if (eventMap.TryGetValue(eventKey, out TransitionMap<TState> map)
                && map.TryGetTransition(Current, out Transition<TState> transition))
            {
                Current = transition.Target;
                transition.Trigger();
            }
        }
    }
}