using System;
using System.Collections.Generic;
using SpaceGame.Events;
using SpaceGame.AI;
using UnityEngine;
using Util;

namespace SpaceGame.Missions {

    /*
         * Objectives:
         * 100% of Freighter Group Omega must be inspected
         * 100% of Transport Group Dayta must be inspected
         *
         * Secondary Objectives:
         * 100% of Corellian Transport group Yander must be inspected
         * 100% of Corellian Transport group Taloos must be inspected
         */

    public class Mission0 : MissionBase {

        private StateChart stateChart;

        public EntityGroup tieFighterAlpha;
        public EntityGroup freighterOence;
        public Entity vanmyrStation;

        public WaypointPath oenceWaypoints;

        public void Start() {
            stateChart = new StateChart(BuildStateChart);
        }

        public void Tick() {
            stateChart.Tick();
        }      

        protected override void BuildStateChart(StateChart.StateChartBuilder builder) {
            Action<string, Action> State = builder.State;
            Action<Action> Init = builder.Init;

            /*
             * FreighterWaypointGoal fg = new WaypoingGoal(WaypointId);
             * oence1.AddPrimaryGoal(new WaypointFollowGoal(waypointId));
             * oence1.AddPrimaryGoal(new ExitToHyperspaceGoal(hyperpointId));
             */

            Decision decision = new Decision();
            decision.action = new FlyWaypointsAction();
            decision.contextCreator = new WaypointContextCreator();
            decision.evaluator = new Evaluator<WaypointContext>();
            decision.evaluator.bonusCalculator = new BonusCalculator<WaypointContext>();
            decision.evaluator.considerations = new Consideration[] {
                new WaypointConsideration()
            };

            State("Mission Init", () => {
                Init(() => {
                    // not to have to pre-allocate
                    // de-allocate is a-ok
                    Deactivate(freighterOence);

                    SetTimeout(0f, () => Arrive(freighterOence.GetEntity("Oence 1")));
//                    SetTimeout(3f, () => Arrive(freighterOence.GetEntity("Oence 2")));
//                    SetTimeout(4f, () => Arrive(freighterOence.GetEntity("Oence 3")));
//                    SetTimeout(5f, () => Arrive(freighterOence.GetEntity("Oence 4")));
//                    SetTimeout(6f, () => Arrive(freighterOence.GetEntity("Oence 5")));

                    OnGameEvent<Evt_EntityArrived>((evt) => {
                        Debug.Log("Arrived");
                        Entity entity = evt.Entity;
                        BehaviorSet behaviors = new BehaviorSet();
                        behaviors.decisions = new List<Decision>();
                        behaviors.decisions.Add(decision);
                        entity.aiInfo.AddBehaviors(behaviors);
                        EventSystem.Instance.Trigger(new Evt_EntityBehaviorChanged(entity.id));
                    });
                });
            });

            State("Mission In Progress", () => { });

            State("Mission Failed", () => { });

            State("Mission Succeeded", () => { });

        }

    }

}