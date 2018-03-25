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

    /*
     *
     * Goals have a type [Destroy, Disable, Defend, Dock, Patrol / Go To]
     * Goals have a subtype e.g. [ Destroy Group, Destroy Single Target, Destroy Type ]
     * Goals have a priority rating of [0 - 1]
     * Goals have a nessessity/desire rating of [0 - 1] which helps the goal to be selected
     * Goals know what game events are relevant to them
     * Goals know when they are completed / failed
     * Goals have success / failure handlers (optionally) 
     * 
     * Goals[Explicit]
     *     Investigate Transports On Arrival
     *     Attack If Cargo is tainted
     *     Engage escort fighters
     * 
     * Goals[Implicit]
     *     Survive => Run, attack attacker, avoid attacker, avoid warhead
     *     Destroy Hostiles => [Hostile Nearby, Has Support] 
     *     Defend Allies => [Hostile Nearby, Hostile Attacking Ally, Ally in Trouble]
     *     Patrol
     *     Wait / Idle
     *
     * OnRelevantEvent => Re-evaluate goals
     *
     * Decide() => {
     *     foreach goal => goal.scoreTargetForDestruction(target);
     *     foreach goal => goal.scoreTargetForDefend(target);
     *     foreach preference => preference.scoreTargetForDestruction(target);
     *     foreach preference => preference.scoreTargetForApproach(target);
     *     foreach goal => scoreTargetForEvasion(target)
     *     mindset = DecideMindset();        => Attack, Preserve, Neutral
     *     action = mindset.DecideAction();  => Attack, Defend, Go To, Follow Goal
     *         what are my threats?
     *         what are my allies doing?
     *         what ae my goals?
     *
     *     decideHighLevelAction() =>
     *         Engage With Guns
     *         Engage Mixed
     *         Formup And Engage
     *         
     *     decideLowLevelAction()
     *     decideLocamotion();
     *     action.DecideTarget();    => Use WeaponX on Target Y
     *
     *
     *    what am i doing? - driven by goals and personality.
     *    refresh this decision periodically or on important
     *    scenario events such as entity arrive / leave / destroy
     *         - Attack Something
     *         - Defend Something
     *         - Evade Something
     *         - Inspect Something
     *         - Dock with Something
     *         - Formate with something
     *         - Enter Scenario
     *         - Exit Scenario
     *         - Patrol some path or wander
     *
     *     what am i doing it to? - driven by tendencies, goal priorities, and environment. Refreshed when succeeded or arbitrary time expired
     *         - pick target
     *         - pick waypoint
     *         - pick escort
     *         - pick wingman
     *         - pick an evasion strategy
     * 
     *     how am I doing it? - this layer figures out where it wants to go (and how [Arrive, Get Near, Exact Path Follow, etc]) refreshed every frame
     *         - do evasive move (aim a progressive series of directions)
     *         - aim at target
     *         - aim at predicted target position
     *         - get seperation from target
     *         - set weapon, set weapon linkage
     * 
     *     how do I get there? - this layer might not exist
     *         - set speed & adjust for avoidance
     *         - set orientation & adjust for avoidance
     *         - fire weapon when ready
     *         - call for help
     *         - execute given maneuver
     * }
     *
     *
     * Goal Levels
     *     Faction
     *     FlightGroup
     *     Entity
     *
     * All entites will try to fulfill faciton goals
     * All flight group members will try to fulfill flight group goals before faction goals
     * Entities will try to fulfill their own goals before flight group + faction
     * 
     * Goals
     *     + Destroy All Red Ships
     *     - Defend All Blue Ships
     *          Name : Defend Blue Ships
     *          GoalType: GoalType.Defend
     *          EndCondition: Time
     *          GoalLevel: GoalLevel.Faction
     *          GoalPriorityBonus: 0, 1 | function pointer
     *          ScoreAttackTarget: Fn
     *          ScoreDefendTarget: Fn
     *          ScoreWaypoint: Fn
     *          ScoreDockManeuver: Fn
     *          OnSuccess => Fn
     *          OnFailure => Fn     
     *          ShipGroup: Function() =>
     * 
     *      - Patrol Waypoints
     *         EndCondition: PathTraversed | Time | Event Occurred
     *         Interruptable: true
     *         RetryAfterInterrupt: true
     *
     *      - Dock
     *
     *      - Destroy Fighters Attacking X
     *         
     */

    public class Mission0 : MissionBase {

        private StateChart stateChart;

        public void Start() {
            stateChart = new StateChart(BuildStateChart);
        }

        public void Tick() {
            stateChart.Tick();
        }

        protected override void BuildStateChart(StateChart.StateChartBuilder builder) {
            Action<string, Action> State = builder.State;
            Action<Action> Init = builder.Init;

//            Goal goal = new Goal();
//            goal.goalLevel = GoalLevel.Faction;
//            goal.goalType = GoalType.Attack;
//
//            goal.ScoreAttackEntity = (entity) => {
//                if (entity.faction.Name == "Empire") return 1f;
//                return 0.5f;
//            };
//
//            goal.onSuccess = () => {
//                Debug.Log("Mission Accompilished");
//            };

//            db.GetFaction("Empire").GetFlightGroup("Alpha").AddGoal(goal);
//            
//            State("Mission Init", () => {
//                Init(() => {
//                    db.GetFlightGroup("Alpha").Spawn();
//                    
//                });
//            });

            State("Mission In Progress", () => { });

            State("Mission Failed", () => { });

            State("Mission Succeeded", () => { });

        }

    }

}