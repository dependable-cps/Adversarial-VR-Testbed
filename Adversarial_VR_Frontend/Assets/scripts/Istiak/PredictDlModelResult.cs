//  -----------------------------------------------------------------------
//  <copyright file="PredictDlModelResult.cs" University="UMC">
//   Copyright (c) 2024 UMC All rights reserved.
//  </copyright>
//  <author>Istiak Ahmed</author>
//  ----

using System.Collections.Generic;
using UnityEngine;

namespace Istiak
{
    public class PredictDlModelResult : MonoBehaviour
    {
        private List<GetInferenceFromDeepLearningModel> _inferences;
        [SerializeField] private List<string> modelPaths;
        [SerializeField] private List<PredictionBar> predictionBars;
        public static PredictDlModelResult Instance;
        public bool isFake = true;

        private void Start()
        {
            _inferences = new List<GetInferenceFromDeepLearningModel>();
            for (int i = 0; i < modelPaths.Count; i++)
            {
                PredictionResult predictionResult = new(predictionBars[i]);
                GetInferenceFromDeepLearningModel inference = new(predictionResult, modelPaths[i]);
                _inferences.Add(inference);
            }

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            var randomNumber = Random.Range(4, 6) * 0.5f;
            Invoke(nameof(FakeData), randomNumber);
        }

        [ContextMenu("Check")]
        private void FakeData()
        {
            float[] inputArray =
            {
                1.234f, 0.5687f,
                0.1232994f, 0.645694f,
                0.03994f, 0.0022994f,
                0.7535994f, 0.016994f,
                0.252994f, 0.03308753f,
                4.052994f, 0.252994f,
                0.8753f, 1.052994f,
                0.994f, 0.08753f,
                .052994f, 0.72994f,
                0.53f, .04994f
            };
            foreach (GetInferenceFromDeepLearningModel inference in _inferences)
            {
                inference.Predict(inputArray, isFake);
            }
            var randomNumber = Random.Range(4, 6) * 0.5f;
            Invoke(nameof(FakeData), randomNumber);
        }

        public void StartPredict(float[] inputArray)
        {
            if (isFake) return;
            foreach (GetInferenceFromDeepLearningModel inference in _inferences)
            {
                inference.Predict(inputArray, isFake);
            }
        }
    }
}