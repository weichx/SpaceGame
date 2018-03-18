using System.Collections.Generic;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public struct MissionTreeSelection {

        public readonly MissionTreeView.ItemType itemType;
        public readonly List<ReflectedProperty> properties;

        public MissionTreeSelection(MissionTreeView.ItemType itemType, List<ReflectedProperty> properties) {
            this.itemType = itemType;
            this.properties = properties;
        }

    }

}