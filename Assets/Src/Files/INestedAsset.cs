namespace SpaceGame {

    public interface INestedAsset<in T> where T : IParentAsset {

        void SetParentAsset(T asset, int siblingIndex = -1);
        void SetSiblingIndex(int index);
        
    }

}