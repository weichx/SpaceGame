using System;
using SpaceGame.FileTypes;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public partial class MissionTreeView {

        private const string IconField = nameof(FactionDefinition.iconPointer);

        public enum ItemType {

            Root,
            Faction,
            FlightGroup,
            Entity

        }

        private class MissionTreeItem : TreeViewItem {

            public readonly ItemType itemType;
            public readonly MissionAsset asset;

            public MissionTreeItem() {
                this.id = -9999;
                this.depth = -1;
                this.itemType = ItemType.Root;
            }
            
            public MissionTreeItem(MissionAsset asset) {
                this.id = asset.id;
                this.displayName = asset.name;

                this.asset = asset;
                if (asset is FactionDefinition) {
                    this.itemType = ItemType.Faction;
                }
                else if (asset is FlightGroupDefinition) {
                    this.itemType = ItemType.FlightGroup;
                }
                else if (asset is EntityDefinition) {
                    this.itemType = ItemType.Entity;
                }
                else {
                    throw new ArgumentException("Asset must be non null and a known type");
                }
            }

            public MissionTreeItem ParentAsMissionTreeItem => parent as MissionTreeItem;

            public override Texture2D icon {
                get {
                    if (itemType == ItemType.Faction) {
                        return ((FactionDefinition) asset).iconPointer.GetAsset();
                    }
                    return null;
                }
            }

            public bool IsFaction => itemType == ItemType.Faction;
            public bool IsEntity => itemType == ItemType.Entity;
            public bool IsFlightGroup => itemType == ItemType.FlightGroup;
            public bool IsRoot => itemType == ItemType.Root;

            public FactionDefinition GetFaction() {
                if (itemType == ItemType.Faction) return asset as FactionDefinition;
                return ParentAsMissionTreeItem?.GetFaction();
            }

            public bool CanDropOn(MissionTreeItem droppedOn) {
                switch (itemType) {
                   case ItemType.Entity:
                       return !droppedOn.IsRoot;
                   case ItemType.Faction:
                       return droppedOn.IsRoot;
                   case ItemType.FlightGroup:
                       return droppedOn.IsFaction || droppedOn.IsFlightGroup;
                }
                return false;
            }
            
            public FlightGroupDefinition GetFlightGroup() {
                switch (itemType) {
                    case ItemType.Faction:
                        return ((FactionDefinition) asset).GetDefaultFlightGroup();
                    case ItemType.FlightGroup:
                        return asset as FlightGroupDefinition;
                }
                if (itemType != ItemType.Entity) return null;
                return ParentAsMissionTreeItem?.GetFlightGroup();
            }

            public EntityDefinition GetEntity() {
                if (itemType != ItemType.Entity) return null;
                return asset as EntityDefinition;
            }

        }

    }

}