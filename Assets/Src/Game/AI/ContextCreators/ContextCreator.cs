using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceGame.AI {

    public class ContextCreator {}

    public abstract class ContextCreator<TContext> : ContextCreator where TContext : DecisionContext {

        public virtual void CreateContexts(Entity agent, List<TContext> outputList) { }


    }

}