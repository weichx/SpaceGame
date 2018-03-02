using System;

namespace SpaceGame {

    [Flags]
    public enum Disposition {

        Friendly,
        Self,
        Neutral,
        Hostile,
        Unknown

    }

}