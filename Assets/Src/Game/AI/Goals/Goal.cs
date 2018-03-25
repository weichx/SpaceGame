using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Weichx.ReflectionAttributes;
using Weichx.ReflectionAttributes.Markers;

namespace SpaceGame.AI {

    public class Goal {

        [HideInInspector] public int id;
        [HideInInspector] public string name;
        [HideInInspector] public GoalLevel goalLevel;

        public float priority = 1;

        [CreateOnReflect, DefaultExpanded]  public Consideration[] considerations;

        [UsedImplicitly]
        public Goal() { }

    }



    public class DestroyShips : Goal {

        public List<int> shipIds = new List<int>();

    }

    public class AttackGoal : Goal {

        [UsePropertyDrawer(typeof(EntitySelector))]
        public int entityId;
        
        [UsePropertyDrawer(typeof(EntitySelector))]
        public int flightGroupId;

        [UsedImplicitly]
        public AttackGoal() { }

    }

    public enum GoalType {

        Attack,
        Defend,
        Inspect,
        Escort,
        Patrol,
        Dock

    }

    public enum GoalLevel {

        Faction,
        FlightGroup,
        Entity

    }

}