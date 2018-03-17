using System;
using System.Collections.Generic;

namespace SpaceGame.AI {

    public abstract class ContextCreator : IContextAware {

        public abstract Type GetContextType();

    }

    public abstract class ContextCreator<TContext> : ContextCreator where TContext : DecisionContext {

        public abstract void CreateContexts(Entity agent, List<TContext> outputList);

        public override Type GetContextType() {
            return typeof(TContext);
        }

    }

}