using System.Collections.Generic;

namespace SpaceGame.AI {

    public interface IContextCreator<TContext> where TContext : DecisionContext {

        void CreateContexts(List<TContext> outputList);

    }

}