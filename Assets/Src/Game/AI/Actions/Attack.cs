using SpaceGame.Events;
using SpaceGame.Weapons;
using Weichx.Util;
using UnityEngine;

namespace SpaceGame.AI.Actions {


    /*
     *
     * How do we go from data in the editor to instances of entities?
     * Option 1: Everything through the editor: Seems to make the most sense.
     *           Entity Component has actual data but only at runtime.
     *           the mission definition screen defines what that is
     * 
     * Entity
     *     - Ship Model
     *     - Ship Stats
     *     - AI Behaviors
     *     - AI Preferences
     *     - AI Goals
     *     - Weapon System [Hardpoints]
     *     - Weapon Loadout
     *     - Weapon Overrides
     *     - Faction
     *     - Flight Group
     *     - Name / Call Sign
     *     - Unique Id
     *     - Flight System
     *     - Cargo System
     *     - Navigation System
     *     - Communication System
     *     - Sensor System
     *     - Damage System
     *     - Turret System
     */
    
    public class BasicAttackAction : AIAction<EntityContext> {

        public override void Setup() { }

        public override bool Tick() {
            TransformInfo trasformInfo = context.agent.transformInfo;
            TransformInfo targetTransformInfo = context.other.transformInfo;

            Vector3 toTarget = trasformInfo.DirectionTo(targetTransformInfo);

//            float cooldown = context.agent.WeaponSystem.CurrentWeaponCooldownRemaining;
//            if (cooldown <= 0) {
//                context.agent.WeaponSystem.Fire();
//            }
            
            EventSystem.Instance.Trigger(
                Evt_WeaponFired.Spawn(new FiringParameters() {
                    ownerId = context.agent.id,
                    position = trasformInfo.position,
                    direction = toTarget,
                    weaponType = WeaponType.Vulcan
                })
            );

            return false;
        }

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

    public class WeaponRangeConsideration { }

}