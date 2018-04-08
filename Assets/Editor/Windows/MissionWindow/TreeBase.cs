using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SpaceGame.Assets;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SpaceGame.Editor.MissionWindow {

    public abstract class TreeBase : TreeView {

        private List<int> utilList;

        protected TreeBase(TreeViewState state) : base(state) {
            utilList = new List<int>();
        }

        [PublicAPI]
        public void UpdateDisplayName(MissionAsset asset) {
            TreeViewItem item = FindItem(asset.id, rootItem);
            if (item != null) item.displayName = asset.DisplayName;
        }

        [PublicAPI]
        public void UpdateDisplayName(GameAsset asset) {
            TreeViewItem item = FindItem(asset.id, rootItem);
            if (item != null) item.displayName = asset.DisplayName;
        }

        [PublicAPI]
        public void UpdateDisplayName(int id, string name) {
            TreeViewItem item = FindItem(id, rootItem);
            if (item != null) item.displayName = name ?? string.Empty;
        }

        public void SetSingleSelection(int id) {
            Reload();
            SelectFireAndFrame(id);
        }

        public void OnGUILayout() {
            OnGUI(GUILayoutUtility.GetRect(0, 10000, 0, 10000));
        }

        protected bool IsInSelectedHierarchy(int id) {
            TreeViewItem item = FindItem(id, rootItem);
            IList<int> selection = GetSelection();
            while (item != rootItem) {
                if (selection.Contains(item.id)) {
                    return true;
                }
                item = item.parent;
            }
            return false;
        }

        public void ClearSelection() {
            utilList.Clear();
            SetSelection(utilList);
        }

        protected void SelectFireAndFrame(int id) {
            Reload();
            utilList.Clear();
            if (id != -1) {
                utilList.Add(id);
            }
            SetSelection(utilList, TreeViewSelectionOptions.RevealAndFrame | TreeViewSelectionOptions.FireSelectionChanged);
        }

        protected void SelectFireAndFrame(TreeViewItem item) {
            utilList.Clear();
            utilList.Add(item.id);
            SetSelection(utilList, TreeViewSelectionOptions.RevealAndFrame | TreeViewSelectionOptions.FireSelectionChanged);
        }

        protected T FindItem<T>(int id) where T : TreeViewItem {
            return (T) FindItem(id, rootItem);
        }

    }

}