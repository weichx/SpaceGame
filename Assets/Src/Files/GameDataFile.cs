using UnityEngine;

namespace SpaceGame.FileTypes {

    public class GameDataFile : ScriptableObject {

        public bool debugReset;
        // todo -- for safety in case of catastrophic failure: split this into sections
        [TextArea(20, 40)] public string database;


    }

}