using System;
using UnityEngine;
using System.Collections.Generic;
using Weichx.ReflectionAttributes;

namespace SpaceGame {

    //class MissionTemplate : AssetTemplate<Mission>
    //class MissionDefinition : AssetDefinition<MissionTemplate>

//    class AssetTemplate<T> {
//
//        public AssetDefinition<T> CreateDefinition() {
//            return null;
//        }
//
//    }
//
//    class Thing { }
//
//    abstract class AssetDefinition<T> {
//
//        protected AssetTemplate<T> template;
//
//        public abstract T CreateInstance();
//
//    }
//
//    class SomeAssetTemplate : AssetTemplate<Thing> { }
//
//    class SomeAssetDefinition : AssetDefinition<Thing> {
//
//        public override Thing CreateInstance() {
//            SomeAssetDefinition x = (SomeAssetDefinition)template.CreateDefinition();
//        }
//
//    }

    public class MissionDefinition : AssetDefinition {

        [ReadOnly] public string createdAt;

        public List<FlightGroupDefinition> flightGroupDefinitions;
        public List<FactionDefinition> factionsDefinitions;
        public List<EntityDefinition> entityDefinitions;

        public MissionDefinition() {
            this.name = "Unnamed Mission";
            this.flightGroupDefinitions = new List<FlightGroupDefinition>(8);
            this.factionsDefinitions = new List<FactionDefinition>(4);
            this.entityDefinitions = new List<EntityDefinition>(16);
            this.createdAt = $"{DateTime.Now.ToShortTimeString()} on {DateTime.Now.ToShortDateString()}";
        }

    }

}