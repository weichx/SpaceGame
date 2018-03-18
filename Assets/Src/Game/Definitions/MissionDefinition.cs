using System;
using UnityEngine;
using System.Collections.Generic;
using Weichx.ReflectionAttributes;

namespace SpaceGame {

    public class MissionDefinition {

        [ReadOnly]
        public string createdAt;
        public string name;
        
        [SerializeField] 
        private string serializedEntityDefinitions;        
        [HideInInspector]
        public string guid;

        public List<Faction> factions;
        public List<FlightGroup> flightGroups;
        public List<EntityDefinition> entityDefinitions;
        
        public MissionDefinition() {
            this.name = "Unnamed Mission";
            this.guid = Guid.NewGuid().ToString();
            this.entityDefinitions = new List<EntityDefinition>(8);
            this.createdAt = DateTime.Now.ToShortTimeString() + " on " + DateTime.Now.ToShortDateString();
        }
        
        

    }

}