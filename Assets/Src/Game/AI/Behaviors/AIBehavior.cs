using Weichx.ReflectionAttributes;

namespace SpaceGame.AI.Behaviors {

    public sealed class AIBehavior {

        public string name;
        
        [CreateOnReflect]
        public AIAction[] actions;

        [CreateOnReflect]
        public Consideration[] considerations;

        public AIBehavior() {
            this.name = "Behavior";
            this.actions = new AIAction[0];
            this.considerations = new Consideration[0];
        }
        
        public bool CanSatisfyGoalType(GoalType goalType) {
            return true;
        }

    }

}