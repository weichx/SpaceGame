﻿using System.Collections.Generic;
using Src.Engine;
using UnityEngine;

namespace SpaceGame.AI {

    public class WaypointContextCreator : ContextCreator<WaypointContext> {

        public override void CreateContexts(Entity agent, List<WaypointContext> outputList) {
            Debug.Log("GENERIC CREATE CONTEXTS");
            List<WaypointPath> waypointPaths = GameData.Instance.waypointPaths;

            for (int i = 0; i < waypointPaths.Count; i++) {
                WaypointContext context = new WaypointContext(agent, waypointPaths[i]);
                outputList.Add(context);
            }
        }

    }

}