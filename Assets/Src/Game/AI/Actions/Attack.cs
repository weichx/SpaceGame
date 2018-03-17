using Weichx.Util;
using UnityEngine;

namespace SpaceGame.AI.Actions {


    public class AIData {

        public int id;
        public Entity entity;
        public Entity target; // ITargetable might be better
        
    }
    
    // WeaponSystem
    // IntegritySystem (Hull & Shields)
    // CommunicationSystem
    // NavigationSystem
    // SensorSystem
    // EngineSystem
    // CargoSystem
    
   
    interface IConsideration<T> where T : DecisionContext {}
  
    public class OrientationToTargetConsideration : IConsideration<DecisionContext> {

        

    }

    public class AreaFriendliness { }

    public class AreaSaftely { }

    public class RemainingHealthConsideration { }

    public class TargetHeatlhConsideration { }

    public class TargetMissionImportance { }

    public class TargetSupport { }

    public class CoordinatedAttack { }

    public class EscortDuty { }

    public class StayNearOrigin { }

    public class HostilesTargetingMe { }

    public class WeaponRangeConsideration : IConsideration<DecisionContext> {


    }


}