using System.Globalization;
using System.Text.RegularExpressions;

namespace Weichx.Util {

    public static class StringUtil {

        public static string SplitAndTitlize(string input) {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Regex.Replace(input, "(\\B[A-Z])", " $1"));
        }

        public static string NicifyName(string input, string replace = null) {
            if (replace != null) input = input.Replace(replace, string.Empty);
            return SplitAndTitlize(input);
        }
    }

}