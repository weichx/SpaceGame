namespace SpaceGame.Editor.Reflection {

    public class ReflectedObject {

        private ReflectedProperty[] reflectedProperties;
        private ReflectedProperty root;

        public ReflectedObject(object target) {
            root = new ReflectedInstanceProperty(null, "--Root--", target.GetType(), target);
        }

        public void ApplyModifiedProperties() {
            root.ApplyChanges();    
        }
        
        public ReflectedProperty GetChildAt(int idx) {
            return root.ChildAt(idx);
        }

        public int ChildCount {
            get { return root.ChildCount; }
        }

    }

}