using System;
using System.Collections.Generic;

namespace Util {

    public partial class StateChart {

        private class StateChartState {

            public readonly string id;

            public bool isActive;
            private bool isInitialized;

            public readonly List<StateChartState> subStates;
            public readonly List<StateEventHandler> events;
            public readonly List<StateTransitionHandler> transitions;
            public readonly StateChartState parent;
            public StateChartState activeSubstate;

            public Action enterFn;
            public Action updateFn;
            public Action initFn;
            public Action exitFn;

            public StateChartState(string stateName, StateChartState parent) {
                id = stateName;
                isActive = false;
                isInitialized = false;
                subStates = new List<StateChartState>();
                events = new List<StateEventHandler>();
                transitions = new List<StateTransitionHandler>();
                this.parent = parent;
            }

            public StateChartTransition HandleEvent(Type evtType, StateChartEvent evt) {
                for (int i = 0; i < events.Count; i++) {
                    if (events[i].type == evtType) {
                        events[i].callback(evt);
                    }
                }

                for (int i = 0; i < transitions.Count; i++) {
                    if (transitions[i].type == evtType && transitions[i].guardFn(evt)) {
                        return new StateChartTransition(this, transitions[i].targetState);
                    }
                }

                return activeSubstate != null ? activeSubstate.HandleEvent(evtType, evt) : null;
            }

            public bool isParentOf(StateChartState other) {
                StateChartState ptr = other.parent;

                while (ptr != null) {
                    if (ptr == this) return true;
                    ptr = ptr.parent;
                }

                return false;
            }

            public void Enter(List<StateChartState> enterPath) {
                if (isActive) return;
                isActive = true;

                if (parent != null) parent.activeSubstate = this;

                if (!isInitialized) {
                    isInitialized = true;

                    if (initFn != null) {
                        initFn();
                        initFn = null;
                    }
                }

                if (enterFn != null) enterFn();

                if (enterPath != null && enterPath.Count > 0) {
                    activeSubstate = enterPath[enterPath.Count - 1];
                    enterPath.RemoveAt(enterPath.Count - 1);
                    activeSubstate.Enter(enterPath);
                }
                else if (subStates.Count > 0) {
                    activeSubstate = subStates[0];
                    activeSubstate.Enter(enterPath);
                }
                else {
                    activeSubstate = null;
                }
            }

            public void Exit() {
                if (!isActive) return;
                isActive = false;

                if (exitFn != null) exitFn();

                if (activeSubstate != null) {
                    activeSubstate.Exit();
                    activeSubstate = null;
                }
            }

            public void ExitChildren() {
                if (activeSubstate != null) {
                    activeSubstate.Exit();
                    activeSubstate = null;
                }
            }

            public override string ToString() {
                return "State: " + id + (isActive ? " --> active" : "");
            }

        }

    }

}