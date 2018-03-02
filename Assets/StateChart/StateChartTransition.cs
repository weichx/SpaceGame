namespace Util {

    public partial class StateChart {

        private class StateChartTransition {

            public readonly StateChartState from;
            public readonly string targetState;

            public StateChartTransition(StateChartState from, string targetState) {
                this.from = from;
                this.targetState = targetState;
            }

        }

    }


}