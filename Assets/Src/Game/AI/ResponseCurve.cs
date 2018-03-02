﻿namespace SpaceGame.AI {

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
            if (input < threshold && curveType != ResponseCurveType.Constant) return 0;
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
                    throw new Exception(curveType + " curve has not been implemented yet");
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
            get { return " slope: " + slope + " exp: " + exp + " vShift: " + vShift + " hShift: " + hShift + " \n threshold: " + threshold + " inverted: " + invert; }
        }

        public override string ToString() {
            return "{type: " + curveType + ", slope: " + slope +
                   ", exp: " + exp + ", vShift: " + vShift + ", hShift: " + hShift + "}";
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