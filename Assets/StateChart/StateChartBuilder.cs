using System;
using UnityEngine;

namespace Util {

    public partial class StateChart {

        public class StateChartBuilder {

            private readonly StateChart chart;

            public StateChartBuilder(StateChart chart) {
                this.chart = chart;
            }

            public void State(string stateName, Action definition = null) {
                if (chart.isDefined) {
                    Debug.Log("Unable to create new States once a StateChart has been created");
                    return;
                }

                chart.State(stateName, definition);
            }

            public void Trigger<T>(T evt) where T : StateChartEvent {
                chart.Trigger(evt);
            }

            public void Enter(Action action) {
                if (chart.isDefined) {
                    Debug.Log("Unable to add Enter handlers once a StateChart has been created");
                    return;
                }

                chart.Enter(action);
            }

            public void Update(Action action) {
                if (chart.isDefined) {
                    Debug.Log("Unable to add Update handlers once a StateChart has been created");
                    return;
                }

                chart.Update(action);
            }

            public void Exit(Action action) {
                if (chart.isDefined) {
                    Debug.Log("Unable to add Exit handlers once a StateChart has been created");
                    return;
                }

                chart.Exit(action);
            }

            public void Init(Action action) {
                if (chart.isDefined) {
                    Debug.Log("Unable to add Init handlers once a StateChart has been created");
                    return;
                }

                chart.Init(action);
            }

            public void Event(Type type, Action<StateChartEvent> callback) {
                if (chart.isDefined) {
                    Debug.Log("Unable to add Event handlers once a StateChart has been created");
                    return;
                }

                chart.Event(type, callback);
            }

            public void Event<T>(Action<T> callback) where T : StateChartEvent {
                if (chart.isDefined) {
                    Debug.Log("Unable to add Event handlers once a StateChart has been created");
                    return;
                }

                chart.Event(callback);
            }

            public void Transition<T>(string targetState, Func<T, bool> guardFn = null) where T : StateChartEvent {
                if (chart.isDefined) {
                    Debug.Log("Unable to add Transition handlers once a StateChart has been created");
                    return;
                }

                chart.Transition(targetState, guardFn);
            }

        }

    }

}