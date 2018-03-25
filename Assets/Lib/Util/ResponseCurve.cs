using System.Collections.Generic;
using Weichx.Util.Texture2DExtensions;

namespace SpaceGame.AI {

    using UnityEngine;
    using System;

    public enum ResponseCurveType {

        Constant,
        Polynomial,
        Logistic,
        Logit,
        Threshold,
        Quadratic,
        Parabolic,
        NormalDistribution,
        Bounce,
        Sin

    }

    [Serializable]
    public class ResponseCurve : ICloneable {

    #if UNITY_EDITOR
        [HideInInspector] public bool __editorOnlyFoldout__;
    #endif

        private static List<Vector2Int> s_pointList = new List<Vector2Int>(128);

        public ResponseCurveType curveType;
        public float slope; //(m)
        public float exp; //(k)
        public float vShift; //vertical shift (b)
        public float hShift; //horizonal shift (c)
        public float threshold;
        public bool invert;

        public ResponseCurve() {
            curveType = ResponseCurveType.Polynomial;
            slope = 1;
            exp = 1;
            vShift = 0;
            hShift = 0;
            threshold = 0;
            invert = false;
        }

        public ResponseCurve(ResponseCurveType type, float slope = 1, float exp = 1, bool invert = false) {
            this.curveType = type;
            this.slope = slope;
            this.exp = exp;
            vShift = 0;
            hShift = 0;
            threshold = 0;
            this.invert = invert;
        }

        public ResponseCurve(ResponseCurveType type, float slope = 1, float exp = 1, float vShift = 0, float hShift = 0, float threshold = 0, bool invert = false) {
            this.curveType = type;
            this.slope = slope;
            this.exp = exp;
            this.vShift = vShift;
            this.hShift = hShift;
            this.threshold = threshold;
            this.invert = invert;
        }

        public float Evaluate(float input) {
            input = Mathf.Clamp01(input);
            float output;
            if (input < threshold && curveType != ResponseCurveType.Constant) return 0f;
            switch (curveType) {
                case ResponseCurveType.Constant:
                    output = threshold;
                    break;
                case ResponseCurveType.Polynomial: // y = m(x - c)^k + b 
                    output = slope * (Mathf.Pow((input - hShift), exp)) + vShift;
                    break;
                case ResponseCurveType.Logistic: // y = (k * (1 / (1 + (1000m^-1*x + c))) + b
                    output = (exp * (1.0f / (1.0f + Mathf.Pow(Mathf.Abs(1000.0f * slope), (-1.0f * input) + hShift + 0.5f)))) + vShift; // Note, addition of 0.5 to keep default 0 hShift sane
                    break;
                case ResponseCurveType.Logit: // y = -log(1 / (x + c)^K - 1) * m + b
                    output = (-Mathf.Log((1.0f / Mathf.Pow(Mathf.Abs(input - hShift), exp)) - 1.0f) * 0.05f * slope) + (0.5f + vShift); // Note, addition of 0.5f to keep default 0 XIntercept sane
                    break;
                case ResponseCurveType.Quadratic: // y = mx * (x - c)^K + b
                    output = ((slope * input) * Mathf.Pow(Mathf.Abs(input + hShift), exp)) + vShift;
                    break;
                case ResponseCurveType.Sin: //sin(m * (x + c) ^ K + b
                    output = (Mathf.Sin((2 * Mathf.PI * slope) * Mathf.Pow(input + (hShift - 0.5f), exp)) * 0.5f) + vShift + 0.5f;
                    break;
                case ResponseCurveType.Parabolic:
                    output = Mathf.Pow(slope * (input + hShift), 2) + (exp * (input + hShift)) + vShift;
                    break;
                case ResponseCurveType.Bounce:
                    output = Mathf.Abs(Mathf.Sin((2f * Mathf.PI * exp) * (input + hShift + 1f) * (input + hShift + 1f)) * (1f - input) * slope) + vShift;
                    break;
                case ResponseCurveType.NormalDistribution: // y = K / sqrt(2 * PI) * 2^-(1/m * (x - c)^2) + b
                    output = (exp / (Mathf.Sqrt(2 * Mathf.PI))) * Mathf.Pow(2.0f, (-(1.0f / (Mathf.Abs(slope) * 0.01f)) * Mathf.Pow(input - (hShift + 0.5f), 2.0f))) + vShift;
                    break;
                case ResponseCurveType.Threshold:
                    output = input > hShift ? (1.0f - vShift) : (0.0f - (1.0f - slope));
                    break;
                default:
                    return 0f;// throw new Exception($"{curveType} curve has not been implemented yet");
            }

            if (invert) output = 1f - output;
            return Mathf.Clamp01(output);
        }

        public void Reset() {
            slope = 1;
            exp = 1;
            vShift = 0;
            hShift = 0;
            threshold = 0;
            invert = false;
        }

        public string DisplayString {
            get { return $" slope: {slope} exp: {exp} vShift: {vShift} hShift: {hShift} \n threshold: {threshold} inverted: {invert}"; }
        }

        public string ShortDisplayString {
            get {
                string retn = $" {Enum.GetName(typeof(ResponseCurveType), curveType)} ({slope}, {exp}, {vShift}, {hShift})";
                if (threshold != 0) retn += $" [{threshold}]";
                if (invert) retn += " inverted";
                return retn;
            }
        }

        public override string ToString() {
            return $"{{type: {curveType}, slope: {slope}, exp: {exp}, vShift: {vShift}, hShift: {hShift}}}";
        }

        public static ResponseCurve CreateLinearCurve() {
            return new ResponseCurve(ResponseCurveType.Polynomial, 1, 1, 0, 0);
        }

        public static ResponseCurve Create2PolyCurve() {
            return new ResponseCurve(ResponseCurveType.Polynomial, 1, 2, 0, 0);
        }

        public static ResponseCurve Create4PolyCurve() {
            return new ResponseCurve(ResponseCurveType.Polynomial, 1, 4, 0, 0);
        }

        public static ResponseCurve Create6PolyCurve() {
            return new ResponseCurve(ResponseCurveType.Polynomial, 1, 6, 0, 0);
        }

        public static ResponseCurve Create8PolyCurve() {
            return new ResponseCurve(ResponseCurveType.Polynomial, 1, 8, 0, 0);
        }

        public static ResponseCurve CreateInvertedLinearCurve() {
            return new ResponseCurve(ResponseCurveType.Polynomial, 1, 1, 0, 0, 0, true);
        }

        public static ResponseCurve CreateInverted2PolyCurve() {
            return new ResponseCurve(ResponseCurveType.Polynomial, 1, 2, 0, 0, 0, true);
        }

        public static ResponseCurve CreateInverted4PolyCurve() {
            return new ResponseCurve(ResponseCurveType.Polynomial, 1, 4, 0, 0, 0, true);
        }

        public static ResponseCurve CreateInverted6PolyCurve() {
            return new ResponseCurve(ResponseCurveType.Polynomial, 1, 6, 0, 0, 0, true);
        }

        public static ResponseCurve CreateInverted8PolyCurve() {
            return new ResponseCurve(ResponseCurveType.Polynomial, 1, 8, 0, 0, 0, true);
        }

        public void DrawGraph(Texture2D graphTexture, float width, float height) {
            graphTexture.Resize((int) width, (int) height);
            Rect graphRect = new Rect(0, 0, width, height);
            DrawGraphLines(graphRect, graphTexture);
            graphTexture.FlipVertically();
            graphTexture.Apply(true);
        }

        public void DrawGraphLines(Rect rect, Texture2D graphTexture, Color background, Color lineColor) {
            int graphX = (int) rect.x;
            int graphY = (int) rect.y;
            int graphWidth = (int) rect.width;
            int graphHeight = (int) rect.height;

            Color[] pixels = graphTexture.GetPixels(graphX, graphY, graphWidth, graphHeight);

            for (int i = 0; i < pixels.Length; i++) {
                pixels[i] = background;
            }

            graphTexture.SetPixels(graphX, graphY, graphWidth, graphHeight, pixels);

            s_pointList.Clear();

            for (int i = 0; i < graphWidth; i++) {
                float x = i / (float) graphWidth;
                float y = Evaluate(x);

                if (float.IsNaN(y)) continue; //not sure why this can happen but it does with non integer exponents

                //inverted y because text rendindering is upside down and i cant figure out 
                //how to flip the text correctly, so im flipping the graph instead

                s_pointList.Add(new Vector2Int(i, (int) ((1 - y) * graphHeight)));
            }

            Color fadedGrey = new Color(0.24f, 0.24f, 0.24f, 0.5f);
            int quarterWidth = (int) (graphWidth * 0.25f);
            int quarterHeight = (int) (graphHeight * 0.25f);

//            graphTexture.DrawLine(graphX, graphY + graphHeight, graphX + graphWidth, graphY, fadedGrey);

            graphTexture.DrawLine(graphX, quarterHeight * 1, graphX + graphWidth, quarterHeight * 1, fadedGrey);
            graphTexture.DrawLine(graphX, quarterHeight * 2, graphX + graphWidth, quarterHeight * 2, fadedGrey);
            graphTexture.DrawLine(graphX, quarterHeight * 3, graphX + graphWidth, quarterHeight * 3, fadedGrey);

            graphTexture.DrawLine(quarterWidth * 1, graphY, quarterWidth * 1, graphY + graphHeight, fadedGrey);
            graphTexture.DrawLine(quarterWidth * 2, graphY, quarterWidth * 2, graphY + graphHeight, fadedGrey);
            graphTexture.DrawLine(quarterWidth * 3, graphY, quarterWidth * 3, graphY + graphHeight, fadedGrey);

            if (s_pointList.Count >= 2) {
                int lastX = s_pointList[0].x;
                int lastY = s_pointList[0].y;
                for (int i = 1; i < s_pointList.Count; i++) {
                    int y = s_pointList[i].y;
                    graphTexture.DrawLine(lastX, lastY, i, y, lineColor);

                    lastX = i;
                    lastY = y;
                }
            }
        }

        public void DrawGraphLines(Rect rect, Texture2D graphTexture) {
            DrawGraphLines(rect, graphTexture, new Color(0.2f, 0.2f, 0.2f), Color.green);
        }

        public object Clone() {
            ResponseCurve curve = new ResponseCurve();
            curve.slope = slope;
            curve.exp = exp;
            curve.vShift = vShift;
            curve.hShift = hShift;
            curve.threshold = threshold;
            curve.curveType = curveType;
            return curve;
        }

    }
/*
switch (CurveShape)
            {
            case CurveType.Constant:
                value = YIntercept;
                break;
            case CurveType.Linear:
                // y = m(x - c) + b ... x expanded from standard mx+b
                value = (SlopeIntercept* (x - XIntercept)) + YIntercept;
                break;
            case CurveType.Quadratic:
                // y = mx * (x - c)^K + b
                value = ((SlopeIntercept* x) * Mathf.Pow(Mathf.Abs(x + XIntercept), Exponent)) + YIntercept;
                break;
            case CurveType.Logistic:
                // y = (k * (1 / (1 + (1000m^-1*x + c))) + b
                value = (Exponent* (1.0f / (1.0f + Mathf.Pow(Mathf.Abs(1000.0f * SlopeIntercept), (-1.0f * x) + XIntercept + 0.5f)))) + YIntercept; // Note, addition of 0.5 to keep default 0 XIntercept sane
                break;
            case CurveType.Logit:
                // y = -log(1 / (x + c)^K - 1) * m + b
                value = (-Mathf.Log((1.0f / Mathf.Pow(Mathf.Abs(x - XIntercept), Exponent)) - 1.0f) * 0.05f * SlopeIntercept) + (0.5f + YIntercept); // Note, addition of 0.5f to keep default 0 XIntercept sane
                break;
            case CurveType.Threshold:
                value = x > XIntercept? (1.0f - YIntercept) : (0.0f - (1.0f - SlopeIntercept));
                break;
            case CurveType.Sine:
                // y = sin(m * (x + c)^K + b
                value = (Mathf.Sin(SlopeIntercept* Mathf.Pow(x + XIntercept, Exponent)) * 0.5f) + 0.5f + YIntercept;
                break;
            case CurveType.Parabolic:
                // y = mx^2 + K * (x + c) + b
                value = Mathf.Pow(SlopeIntercept* (x + XIntercept), 2) + (Exponent* (x + XIntercept)) + YIntercept;
                break;
            case CurveType.NormalDistribution:
                // y = K / sqrt(2 * PI) * 2^-(1/m * (x - c)^2) + b
                value = (Exponent / (Mathf.Sqrt(2 * 3.141596f))) * Mathf.Pow(2.0f, (-(1.0f / (Mathf.Abs(SlopeIntercept) * 0.01f)) * Mathf.Pow(x - (XIntercept + 0.5f), 2.0f))) + YIntercept;
                break;
            case CurveType.Bounce:
                value = Mathf.Abs(Mathf.Sin((6.28f * Exponent) * (x + XIntercept + 1f) * (x + XIntercept + 1f)) * (1f - x) * SlopeIntercept) + YIntercept;
                break;
            }
            if (FlipY)
                value = 1.0f - value;

            // Constrain the return to a normal 0-1 range.
            return Mathf.Clamp01(value);
*/

}