namespace SpaceGame.AI {

//*  Aggression   -> tendency to prefer killing targets
//*  Guardian     -> tendency to prefer survival of allies
//*  Preservation -> tendency to prefer survival of self
//*  Obedience    -> tendency to prioritize goal / order following
//*  Coordination -> tendency to prefer group actions and team defense
    public class AIProfile {

        public float aggression;
        public float guardian;
        public float obedient;
        public float teamplayer;
        public float preservation;

    }

}