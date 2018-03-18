using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SpaceGame.Editor.MissionWindow {

    public partial class MissionTreeView {

        private class EntityTreeItem : MissionTreeItem {

            public EntityDefinition entity;

            public EntityTreeItem(EntityDefinition entity) {
                this.entity = entity;
                this.id = Random.Range(int.MinValue, int.MaxValue);
                this.displayName = "Entity";
            }

        }

    }

}