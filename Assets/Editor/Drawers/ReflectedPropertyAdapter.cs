using SpaceGame.Editor.GUIComponents;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace Rotorz.ReorderableList {

    public class ReflectedPropertyAdapter : IReorderableListAdapter {

        private ReflectedListProperty _arrayProperty;

        public float FixedItemHeight;

        public ReflectedProperty this[int index] {
            get { return _arrayProperty[index]; }
        }

        public ReflectedProperty arrayProperty {
            get { return _arrayProperty; }
        }

        public ReflectedPropertyAdapter(ReflectedListProperty arrayProperty, float fixedItemHeight = 0f) {
            this._arrayProperty = arrayProperty;
            this.FixedItemHeight = fixedItemHeight;
        }

        #region IReorderableListAdaptor - Implementation

        /// <inheritdoc/>
        public int Count {
            get { return _arrayProperty.ElementCount; }
        }

        /// <inheritdoc/>
        public virtual bool CanDrag(int index) {
            return true;
        }

        /// <inheritdoc/>
        public virtual bool CanRemove(int index) {
            return true;
        }

        /// <inheritdoc/>
        public void Add() {
            _arrayProperty.ElementCount++;
        }

        /// <inheritdoc/>
        public void Insert(int index) {
            _arrayProperty.InsertElement(index);
        }

        /// <inheritdoc/>
        public void Duplicate(int index) {
            _arrayProperty.Duplicate(index);
        }

        /// <inheritdoc/>
        public void Remove(int index) {
            _arrayProperty.RemoveElementAt(index);
        }

        /// <inheritdoc/>
        public void Move(int sourceIndex, int destIndex) {
            _arrayProperty.MoveElement(sourceIndex, destIndex);
        }

        /// <inheritdoc/>
        public void Clear() {
            _arrayProperty.Clear();
        }

        /// <inheritdoc/>
        public virtual void BeginGUI() { }

        /// <inheritdoc/>
        public virtual void EndGUI() { }

        /// <inheritdoc/>
        public virtual void DrawItemBackground(Rect position, int index) { }

        /// <inheritdoc/>
        public virtual void DrawItem(Rect position, int index) {
            EditorGUI.indentLevel -= 2;
            EditorGUIX.PropertyField(position, this[index]);
            EditorGUI.indentLevel += 2;
        }

        /// <inheritdoc/>
        public virtual float GetItemHeight(int index) {
            return FixedItemHeight != 0f
                ? FixedItemHeight
                : this[index].Drawer.GetPropertyHeight(this[index]);
        }

        #endregion

    }

}