//  -----------------------------------------------------------------------
//  <copyright file="PredictionResult.cs" University="UMC">
//   Copyright (c) 2024 UMC All rights reserved.
//  </copyright>
//  <author>Istiak Ahmed</author>
//  -----------------------------------------------------------------------
using UnityEngine;
namespace Istiak
{
    public class PredictionResult
    {
        private readonly ScoreData _scoreData;
        private readonly PredictionBar _predictionBar;

        public PredictionResult(PredictionBar predictionBar)
        {
            _scoreData = new ScoreData();
            _predictionBar = predictionBar;
        }
        public void OnDataReceived(float low, float medium, float high)
        {
            _scoreData.SetScoreData(low, medium, high);
            Debug.Log("Result:" + _scoreData.Probabilities);

            // Determine the highest score
            if (low >= medium && low >= high)
            {
                _predictionBar.UpdatePredictionBar(0); // Low is the highest
            }
            else if (medium >= low && medium >= high)
            {
                _predictionBar.UpdatePredictionBar(1); // Medium is the highest
            }
            else
            {
                _predictionBar.UpdatePredictionBar(2); // High is the highest
            }

        }
    }
}
