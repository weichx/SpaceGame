namespace SpaceGame.AI {
    
    //measure of how much the target is changing the way it moves
    //a value of 1 means the target hasn't picked a new direction or speed for a while
    //a value of 0 means the target is totally unpredictable

    public class TargetVelocityStabilityTrend : Consideration<EntityContext> {

        protected override float Score(EntityContext context) {
            throw new System.NotImplementedException();
        }

    }

}