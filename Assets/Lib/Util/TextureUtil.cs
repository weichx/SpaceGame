using UnityEngine;
using Weichx.Util.Texture2DExtensions;

namespace Weichx.Util {

    public static class TextureUtil {

        public static Texture2D CreateBackground(Color color) {
            Texture2D texture = new Texture2D(2, 2);
            texture.SetColor(color);
            return texture;
        }

    }

}