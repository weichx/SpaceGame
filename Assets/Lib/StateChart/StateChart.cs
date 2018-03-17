using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Util {

    public partial class StateChart {

        private readonly Dictionary<string, StateChartState> stateMap;
        private readonly Stack<StateChartState> stateStack;

        private Queue<StateChartEvent> inactiveEventQueue;
        private Queue<StateChartEvent> activeEventQueue;

        private readonly StateChartState rootState;

        private readonly bool isDefined;
        private static readonly StringBuilder stringBuilder = new StringBuilder();
        private bool hasUpdates;

        public StateChart(Action<StateChartBuilder> definition) {
            stateMap = new Dictionary<string, StateChartState>();
            stateStack = new Stack<StateChartState>();
            rootState = new StateChartState("Root", null);
            activeEventQueue = new Queue<StateChartEvent>();
            inactiveEventQueue = new Queue<StateChartEvent>();
            stateMap.Add(rootState.id, rootState);
            stateStack.Push(rootState);
            definition(new StateChartBuilder(this));
            isDefined = true;
            stateStack.Pop();
            rootState.Enter(null);
            stateStack = null;
        }

        public void Tick() {
            Queue<StateChartEvent> tmp = inactiveEventQueue;
            inactiveEventQueue = activeEventQueue;
            activeEventQueue = tmp;

            while (inactiveEventQueue.Count > 0) {
                StateChartEvent evt = inactiveEventQueue.Dequeue();
                StateChartTransition transition = rootState.HandleEvent(evt.GetType(), evt);

                if (transition != null) {
                    GoToState(transition.targetState, transition.from);
                }
            }

            StateChartState ptr = rootState;
            if (hasUpdates) {
                while (ptr != null) {
                    if (ptr.updateFn != null) {
                        ptr.updateFn();
                    }

                    ptr = ptr.activeSubstate;
                }
            }
        }

        public string GetCurrentStateName() {
            StateChartState ptr = rootState;
            string id = String.Empty;
            while (ptr != null) {
                id = ptr.id;
                ptr = ptr.activeSubstate;
            }

            return id;
        }
        
        public string GetStatePath() {
            stringBuilder.Length = 0;
            StateChartState ptr = rootState;

            while (ptr != null) {
                ptr = ptr.activeSubstate;

                if (ptr != null) {
                    stringBuilder.Append(ptr.id);

                    if (ptr.activeSubstate != null) {
                        stringBuilder.Append(" -> ");
                    }
                }
            }
            return stringBuilder.ToString();
        }
        
        public bool IsInState(string stateName) {
            StateChartState state;
            return stateMap.TryGetValue(stateName, out state) && state.isActive;
        }

        private void GoToState(string stateName, StateChartState from) {
            StateChartState target;

            if (stateMap.TryGetValue(stateName, out target)) {
                StateChartState ptr = target;
                List<StateChartState> stack = new List<StateChartState>();

                if (from == target) return;

                // entering a descendent of currently active state
                if (from.isParentOf(target)) {
                    while (ptr != from) {
                        stack.Add(ptr);
                        ptr = ptr.parent;
                    }

                    ptr = stack[0];
                    stack.RemoveAt(0);
                    from.ExitChildren();
                    ptr.Enter(stack);
                }
                else {
                    // root state is always active, no need for a null check
                    while (!ptr.isActive) {
                        stack.Add(ptr);
                        ptr = ptr.parent;
                    }

                    ptr.ExitChildren();
                    stack.Reverse();
                    ptr = stack[0];
                    stack.RemoveAt(0);
                    ptr.Enter(stack);
                }

                return;
            }

            Debug.Log("Unable to go to state: " + stateName);
        }

        private void State(string stateName, Action defintion = null) {
            if (stateMap.ContainsKey(stateName)) {
                Debug.Log("Duplicate state: " + stateName);
                return;
            }

            StateChartState parentState = stateStack.Peek();
            StateChartState state = new StateChartState(stateName, parentState);
            parentState.subStates.Add(state);
            stateMap.Add(stateName, state);

            if (defintion != null) {
                stateStack.Push(state);
                defintion();
                stateStack.Pop();
                if (!hasUpdates && state.updateFn != null) {
                    hasUpdates = true;
                }
            }
        }

        public void Trigger<T>(T evt) where T : StateChartEvent {
            activeEventQueue.Enqueue(evt);
        }

        private void Enter(Action action) {
            stateStack.Peek().enterFn = action;
        }

        private void Update(Action action) {
            stateStack.Peek().updateFn = action;
        }

        private void Exit(Action action) {
            stateStack.Peek().exitFn = action;
        }

        private void Init(Action action) {
            stateStack.Peek().initFn = action;
        }

        private void Event(Type type, Action<StateChartEvent> callback) {
            stateStack.Peek().events.Add(new StateEventHandler(type, callback));
        }

        private void Event<T>(Action<T> callback) where T : StateChartEvent {
            Action<StateChartEvent> wrapped = (evt) => callback(evt as T);
            stateStack.Peek().events.Add(new StateEventHandler(typeof(T), wrapped));
        }

        private void Transition<T>(string targetState, Func<T, bool> guardFn = null) where T : StateChartEvent {
            // something wierd is happening with the way casting Func<T, bool> works
            // doesn't compile unless I wrap it like this
            Func<StateChartEvent, bool> wrappedGuard = null;

            if (guardFn != null) {
                wrappedGuard = (evt) => guardFn(evt as T);
            }

            stateStack.Peek().transitions.Add(new StateTransitionHandler(typeof(T), targetState, wrappedGuard));
        }

    }

}