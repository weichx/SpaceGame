using System.Collections.Generic;

namespace SpaceGame.Editor.MissionWindow {

    public struct MissionTreeSelection {

        public readonly MissionTreeView.ItemType itemType;
        public readonly List<AssetDefinition> properties;

        public MissionTreeSelection(MissionTreeView.ItemType itemType, List<AssetDefinition> properties) {
            this.itemType = itemType;
            this.properties = properties;
        }

    }

}