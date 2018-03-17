using UnityEngine;

namespace Weichx.Util {

    public static class RectExtensions {

        public static Rect SliceVertical(this Rect rect, float height = 16f) {
            Rect retn = new Rect(rect);
            retn.height = height;
            rect.y += height;
            rect.height += height;
            return retn;
        }
        
        public static Rect SliceHorizontal(this Rect rect, float width) {
            Rect retn = new Rect(rect);
            retn.width = width;
            rect.x += width;
            rect.width += width;
            return retn;
        }

    }

}