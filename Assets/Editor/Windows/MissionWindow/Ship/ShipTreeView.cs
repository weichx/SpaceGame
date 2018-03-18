using System;
using System.Collections.Generic;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public class ShipTreeView : BaseTreeView {

        public ShipTreeView(ReflectedListProperty list, Action<IList<int>> onSelection) : base(list, onSelection) { }

    }

}