﻿using System.Globalization;
using System.Text.RegularExpressions;

namespace SpaceGame.Util {

    public static class StringUtil {

        public static string SplitAndTitlize(string input) {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Regex.Replace(input, "(\\B[A-Z])", " $1"));
        }

    }

}