//  -----------------------------------------------------------------------
//  <copyright file="GetInferenceFromDeepLearningModel.cs" University="UMC">
//   Copyright (c) 2024 UMC All rights reserved.
//  </copyright>
//  <author>Istiak Ahmed</author>
//  -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using UnityEngine;

namespace Istiak
{
    public class GetInferenceFromDeepLearningModel

    {
        private static readonly string ModelPathPrefix = Application.streamingAssetsPath + "/Models/";
        private const string ModelPathSuffix = ".ONNX";
        private readonly InferenceSession _session;
        private readonly PredictionResult _predictionResult;

        public GetInferenceFromDeepLearningModel(PredictionResult predictionResult, string path)
        {
            _predictionResult = predictionResult;
            string modelPath = ModelPathPrefix + path + ModelPathSuffix;
            _session = new InferenceSession(modelPath);
        }

        public void Predict(float[] inputArray, bool isFake)
        {
            DenseTensor<float> inputTensor = new(inputArray,
                new[]
                {
                    1, 20,
                    1
                });
            List<NamedOnnxValue> inputs = new()
            {
                NamedOnnxValue.CreateFromTensor("input", inputTensor)
            };
            IDisposableReadOnlyCollection<DisposableNamedOnnxValue> sessionOutput = _session.Run(inputs);
            DenseTensor<float> rawResult = (DenseTensor<float>)sessionOutput.ToArray()[0].Value;
            if (isFake)
            {
                _predictionResult.OnDataReceived(Random.Range(0,100), Random.Range(0,100), Random.Range(0,100));
            }
            else
            {
                _predictionResult.OnDataReceived(rawResult.GetValue(0), rawResult.GetValue(1), rawResult.GetValue(2));
            }
        }
    }
}