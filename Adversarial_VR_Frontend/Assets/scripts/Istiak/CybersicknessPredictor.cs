using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace Istiak
{
    public class CybersicknessPredictor : MonoBehaviour
    {
        public string apiUrl = "http://128.206.20.62:8000/predict";
        public PredictionBar predictionBar;
        public CustomTunnelingVignetteController customTunnelingVignetteController;
        private bool isBusy;
        private int currentMitigationPrediction = 0;
        private int currentCSPrediction = 0;
        private Coroutine mitigationRoutine;
        private bool isRecord;
        private bool isSS;

        private void Start()
        {
            if(!isSS) StartMitigationLoop();
            customTunnelingVignetteController.severityLevel = 0;
            predictionBar.UpdatePredictionBar(0);
        }

        private void StartMitigationLoop()
        {
            mitigationRoutine ??= StartCoroutine(MitigationLoop());
        }

        public void StopMitigationLoop()
        {
            if (mitigationRoutine == null) return;
            StopCoroutine(mitigationRoutine);
            mitigationRoutine = null;
            Debug.Log("Mitigation loop stopped.");
        }

        private IEnumerator MitigationLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(3, 4));
                
                customTunnelingVignetteController.severityLevel = 0;
                
                predictionBar.UpdatePredictionBar(Random.Range(0, 2));
                
                yield return new WaitForSeconds(Random.Range(1, 3));
                
                predictionBar.UpdatePredictionBar(currentMitigationPrediction);
                
                yield return new WaitForSeconds(1.5f);
                
                customTunnelingVignetteController.severityLevel = currentMitigationPrediction;
                predictionBar.UpdatePredictionBar(Random.Range(0, 2));
            }
        }
        
        public void PredictCybersickness(PredictionInput input)
        {
            if(isSS) return;
            if (isRecord) return;
            if (isBusy) return;
            StartCoroutine(SendPredictionRequest(input, () => { isBusy = false; }));
        }

        private void SS(int lvl)
        {
            customTunnelingVignetteController.severityLevel = lvl;
            predictionBar.UpdatePredictionBar(Random.Range(0, 2));
        }
        private void Update()
        {
            if (isSS)
            {
                if (Input.GetKeyDown(KeyCode.N))
                {
                    SS(0);
                }
                else if (Input.GetKeyDown(KeyCode.L))
                {
                    SS(1);
                }
                else if (Input.GetKeyDown(KeyCode.M))
                {
                    SS(2);
                }
                else if (Input.GetKeyDown(KeyCode.H))
                {
                    SS(3);
                }
                
                return;
            }
            if (!isRecord) return;
            if (Input.GetKeyDown(KeyCode.N))
            {
                currentMitigationPrediction = 0;
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                currentMitigationPrediction = 1;
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                currentMitigationPrediction = 2;
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                currentMitigationPrediction = 3;
            }
        }

        IEnumerator SendPredictionRequest(PredictionInput input, Action callback)
        {
            isBusy = true;
            string jsonData = JsonUtility.ToJson(input);
            var request = new UnityWebRequest(apiUrl, "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                currentMitigationPrediction = 0;
                currentCSPrediction = 0;
            }
            else
            {
                PredictionResponse response = JsonUtility.FromJson<PredictionResponse>(request.downloadHandler.text);

                if (response == null)
                {
                    Debug.LogError("Failed to parse prediction response.");
                    currentMitigationPrediction = 0;
                    currentCSPrediction = 0;
                    callback();
                    yield break;
                }

                // Build output with null-safety
                string cleanConf = (response.clean_confidence != null && response.clean_confidence.Length > 0)
                    ? string.Join(", ", response.clean_confidence[0])
                    : "N/A";

                string advConf = (response.compromised_confidence != null && response.compromised_confidence.Length > 0)
                    ? string.Join(", ", response.compromised_confidence[0])
                    : "N/A";

                string advInput = (response.compromised_input != null)
                    ? string.Join(", ", response.compromised_input)
                    : "N/A";

                string output = $"Clean Prediction: {response.clean_prediction}\n" +
                                $"Compromised Prediction: {response.compromised_prediction}\n" +
                                $"Confidence Clean: {cleanConf}\n" +
                                $"Confidence Adv: {advConf}\n" +
                                $"Adv input: {advInput}";

                Debug.Log(output);
                currentCSPrediction = response.clean_prediction;
                currentMitigationPrediction = response.compromised_prediction;
            }

            callback();
        }
    }
}