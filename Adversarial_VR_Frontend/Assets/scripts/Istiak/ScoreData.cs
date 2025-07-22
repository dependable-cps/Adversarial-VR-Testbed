//  -----------------------------------------------------------------------
//  <copyright file="ScoreData.cs" University="UMC">
//   Copyright (c) 2024 UMC All rights reserved.
//  </copyright>
//  <author>Istiak Ahmed</author>
//  -----------------------------------------------------------------------

namespace Istiak
{
    public class ScoreData
    {
        public float Low;
        public float Medium;
        public float High;
        public string Probabilities;
        public void SetScoreData(float low, float medium, float high)
        {
            Low = low;
            Medium = medium;
            High = high;
            Probabilities = $"Low: {Low}\nMedium: {Medium}\nHigh: {High}";
        }
    }
}
