﻿using Weichx.Util;
using UnityEngine;

namespace SpaceGame.AI {

    public class WaypointConsideration : Consideration<WaypointContext> {

        public float minDist = 0;
        public float maxDist = 1000f;

        protected override float Score(WaypointContext context) {
            WaypointPath path = context.path;
            if (path == null) return 0;
            WaypointTracker tracker = path.GetTrackerForEntity(context.agent.index);
            Vector3 waypoint = tracker.CurrentWaypoint;
            float distSquared = context.agent.transformInfo.DistanceToSquared(waypoint);
            return MathUtil.PercentageOfRangeClamped(distSquared, minDist * minDist, maxDist * maxDist);
        }

    }
    
}