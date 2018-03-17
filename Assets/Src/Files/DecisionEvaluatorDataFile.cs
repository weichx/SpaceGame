using System;
using System.Collections.Generic;
using SpaceGame.AI;
using UnityEngine;
using Weichx.Persistence;

namespace SpaceGame.FileTypes {

    public class DecisionEvaluatorDataFile : ScriptableObject {

        public string serializedEvaluators;

        [NonSerialized] public List<Decision> decisions;

        public void Save() {
            serializedEvaluators = Snapshot<List<Decision>>.Serialize(decisions);
        }
        
        public void Save(List<Decision> evaluators) {
            this.decisions = evaluators;
            Save();
        }
        
        private void OnEnable() {
            if (decisions == null) {
                decisions = Snapshot<List<Decision>>.Deserialize(serializedEvaluators);
            }
        }
        
        public IList<Decision> GetDecisions() {
            return decisions ?? (decisions = Snapshot<List<Decision>>.Deserialize(serializedEvaluators));
        }

    }

}