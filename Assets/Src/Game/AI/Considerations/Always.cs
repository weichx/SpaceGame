using System;

namespace SpaceGame.AI {

    public class Always :  Consideration<EntityContext> {

        public Always() {
            this.curve = new ResponseCurve();
        }
        
        protected override float Score(EntityContext context) {
            return 1;
        }

        public override Type GetContextType() {
            return typeof(DecisionContext);
        }

    }

}