﻿using System.Collections.Generic;
using Weichx.Util;
using SpaceGame.Engine;
using UnityEngine;

namespace SpaceGame.AI {

    public class WaypointContextCreator : ContextCreator<WaypointContext> {

        public float floatVal;
        
        public override void CreateContexts(Entity agent, List<WaypointContext> outputList) {
            ListX<WaypointPath> waypointPaths = GameData.Instance.waypointPaths;

            for (int i = 0; i < waypointPaths.Count; i++) {
                WaypointContext context = new WaypointContext(agent, waypointPaths[i]);
                outputList.Add(context);
            }
        }

    }
    
    public class WaypointContextCreator2 : WaypointContextCreator {

        public string strVal;
        
        public override void CreateContexts(Entity agent, List<WaypointContext> outputList) {
            ListX<WaypointPath> waypointPaths = GameData.Instance.waypointPaths;

            for (int i = 0; i < waypointPaths.Count; i++) {
                WaypointContext context = new WaypointContext(agent, waypointPaths[i]);
                outputList.Add(context);
            }
        }

    }
    
    public class WaypointContextCreator3 : ContextCreator<WaypointContext> {

        public override void CreateContexts(Entity agent, List<WaypointContext> outputList) {
            Debug.Log("GENERIC CREATE CONTEXTS");
            ListX<WaypointPath> waypointPaths = GameData.Instance.waypointPaths;

            for (int i = 0; i < waypointPaths.Count; i++) {
                WaypointContext context = new WaypointContext(agent, waypointPaths[i]);
                outputList.Add(context);
            }
        }

    }

}