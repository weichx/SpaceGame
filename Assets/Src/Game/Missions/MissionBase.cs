using System;
using SpaceGame.Engine;
using SpaceGame.Events;
using Weichx.Util;
using UnityEngine;
using Util;

namespace SpaceGame.Missions {

    public abstract class MissionBase : MonoBehaviour {
 
        protected abstract void BuildStateChart(StateChart.StateChartBuilder builder);

    }

}