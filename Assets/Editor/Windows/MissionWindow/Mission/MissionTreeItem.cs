using System;
using SpaceGame.FileTypes;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public partial class MissionTreeView {

        private const string IconField = nameof(FactionDefinition.iconPointer);

        public enum ItemType {

            None,
            Faction,
            FlightGroup,
            Entity

        }

        private class MissionTreeItem : TreeViewItem {

            public readonly ItemType itemType;
            public readonly ReflectedProperty property;

            private static int idGenerator;

            public MissionTreeItem(ReflectedProperty property) {
                this.id = property[nameof(AssetDefinition.id)].intValue;
                this.displayName = property[nameof(AssetDefinition.name)].stringValue;

                this.property = property;
                if (property.DeclaredType == typeof(FactionDefinition)) {
                    this.itemType = ItemType.Faction;
                }
                else if (property.DeclaredType == typeof(FlightGroupDefinition)) {
                    this.itemType = ItemType.FlightGroup;
                }
                else if (property.DeclaredType == typeof(EntityDefinition)) {
                    this.itemType = ItemType.Entity;
                }
                else {
                    throw new ArgumentException($"Cannot make a TreeItem for {property.DeclaredType.Name}");
                }
            }

            public MissionTreeItem ParentAsMissionTreeItem => parent as MissionTreeItem;
            
            public override Texture2D icon {
                get {
                    if (itemType == ItemType.Faction) {
                        return property[IconField].GetValue<AssetPointer<Texture2D>>().GetAsset();
                    }
                    return null;
                }
            }

            public bool IsFaction => itemType == ItemType.Faction;
            public bool IsEntity => itemType == ItemType.Entity;
            public bool IsFlightGroup => itemType == ItemType.FlightGroup;

        }

    }

}