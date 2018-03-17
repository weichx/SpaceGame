using System;
using System.Collections.Generic;
using SpaceGame.AI;
using UnityEngine;
using Weichx.Persistence;

namespace SpaceGame.FileTypes {

    public class DecisionEvaluatorDataFile : ScriptableObject {

        public string serializedEvaluators;

        [NonSerialized] public List<Evaluator> evaluators;

        public void Save() {
            serializedEvaluators = Snapshot<List<Evaluator>>.Serialize(evaluators);
        }
        
        private void OnEnable() {
            if (evaluators == null) {
                evaluators = Snapshot<List<Evaluator>>.Deserialize(serializedEvaluators);
            }
        }

        private void OnDisable() {
            
        }

    }

}