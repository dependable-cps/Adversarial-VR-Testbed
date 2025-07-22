using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Manager
{
    public class SimulationManager : MonoBehaviour
    {
        public InputField firstName;
        public InputField lastName;
        
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public void StartSimulation()
        {
            Debug.Log("Loading Simulation...");
            String playerName = firstName.text + lastName.text; 
            PlayerPrefs.SetString("playerName", playerName);
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
    }
}
