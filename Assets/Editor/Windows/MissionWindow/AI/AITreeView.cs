﻿using System;
 using System.Collections.Generic;
 using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public class AITreeView : BaseTreeView {

        
        public AITreeView(ReflectedListProperty list, Action<IList<int>> onSelection) 
            : base(list, onSelection) {
        }

    }

}