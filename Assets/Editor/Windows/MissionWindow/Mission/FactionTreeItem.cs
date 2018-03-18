using UnityEngine;

namespace SpaceGame.Editor.MissionWindow {

    public partial class MissionTreeView {

        private class FactionTreeItem : MissionTreeItem {

            public FactionDescription faction;
            
            public FactionTreeItem(FactionDescription faction) {
                this.faction = faction;
                this.id = Random.Range(int.MinValue, int.MaxValue);
                this.displayName = faction.name;
                this.icon = faction.icon;
            }

        }        

    }

}