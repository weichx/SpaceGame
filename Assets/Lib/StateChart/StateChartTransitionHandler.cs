using System;

namespace Util {

    public partial class StateChart {

        private class StateTransitionHandler {

            public readonly Type type;
            public readonly string targetState;
            public readonly Func<StateChartEvent, bool> guardFn;

            public StateTransitionHandler(Type type, string targetState, Func<StateChartEvent, bool> guardFn) {
                this.type = type;
                this.targetState = targetState;
                this.guardFn = guardFn ?? DefaultGuardFn;
            }

            private static bool DefaultGuardFn(StateChartEvent evt) {
                return true;
            }

        }

    }

}