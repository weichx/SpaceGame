using System;

namespace Util {

    public partial class StateChart {

        private class StateEventHandler {

            public readonly Type type;
            public readonly Action<StateChartEvent> callback;

            public StateEventHandler(Type type, Action<StateChartEvent> callback) {
                this.type = type;
                this.callback = callback;
            }

        }

    }

}