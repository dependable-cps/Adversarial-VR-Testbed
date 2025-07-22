using UnityEditor;
using UnityEngine;

namespace Manager
{
    public class SimulationTimeManager : MonoBehaviour
    {
        public float simulationEndTime = 300;
        private float _timeToStop;

        private void Start()
        {
            _timeToStop = 0f;
        }

        private void FixedUpdate()
        {
            _timeToStop = _timeToStop + Time.deltaTime;
            bool forceStop = Input.GetKeyDown(KeyCode.Escape);
        
            if (_timeToStop >= simulationEndTime || forceStop)
            {
                if (forceStop)
                {
                    Logger.Log(LogLevel.INFO,
                        "Simulation Stopped Forcefully!");
                }
                else
                {
                    Logger.Log(LogLevel.INFO, "Simulation completed. Total runtime: " + simulationEndTime + "(s)");
                }

#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            }
        }
    }
}
