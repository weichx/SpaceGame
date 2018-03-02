using UnityEngine;

namespace Freespace.POFModel {
// https://www.hard-light.net/forums/index.php?topic=94318 -- from Axem
//Paths for turrets (and all subsystems) are attack paths for fighters to follow when they get orders to attack subsystems.

//Path points have a radius to help guide ships to the path without needing to be exactly on the point. Basically once a ship gets within a path point's radius, its told to start moving to the next one in the path. So the path farthest out gets the largest radius, and they get smaller as they get closer.
    public class PathInfo {

        public string name;
        public string parentName;
        public PathVertex[] points;

    }

    public struct PathVertex {

        public float radius;
        public Vector3 position;

        public int[] subObjectIndices;
        //todo turrets go here somehow

    }

}