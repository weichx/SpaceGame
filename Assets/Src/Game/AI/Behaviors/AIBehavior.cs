namespace SpaceGame.AI.Behaviors {

    public abstract class AIBehavior {

        public AIAction[] actions;

        public Consideration[] considerations;

        public abstract bool CanSatisfyGoalType(GoalType goalType);

    }

}