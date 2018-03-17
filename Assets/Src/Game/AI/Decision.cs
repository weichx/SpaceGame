using System;
using UnityEngine;
using Weichx.ReflectionAttributes;

namespace SpaceGame.AI {

/* The idea here is that we have a list of possible actions the AI can take
 * Each of those actions requires a decision to be made: should we take this action?
 * Decisions need to be made in contexts, for example if the action is "DoBombingRun"
 * we probably want to be sure we have bombs, can do so without getting blown to bits,
 * we have support, we aren't low health, the target isn't focused on us, etc, etc, etc
 * These are called considerations. A consideration is a funciton that accepts a context
 * and returns a score between 0 and 1 signifiying how much we want to take this action.
 *
 * After we get our score, each consideration filters that score by evaluating against
 * a response curve. With this curve we can completely change the behavior / tendency
 * of an AI without actually re-writing any code!
 * 
 * Decisions can also have requirements, for example a mission might forbid the AI from attacking
 * certain ships, or we require that the target be low on health
 */
    public sealed class Decision {
        
        private static int idGenerator;

        public string name = "Unnamed Decision";
        public string description;

        [HideInInspector][NonSerialized]
        public readonly int id;
        
        public Type contextType = typeof(EntityContext);
        
        [UsePropertyDrawer(typeof(ConstructableSubclass))]
        public AIAction action;
        
        public Evaluator evaluator;
        
        [UsePropertyDrawer(typeof(ConstructableSubclass))]
        public ContextCreator contextCreator;


        public Decision() {
            this.id = idGenerator++;
        }

    }

  

}