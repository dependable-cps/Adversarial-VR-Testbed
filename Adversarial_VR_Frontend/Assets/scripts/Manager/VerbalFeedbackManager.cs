using UnityEngine;

public class VerbalFeedbackManager1 : MonoBehaviour
{
    [SerializeField]
    private float _verbalFeedbackCollectionFreq = 30f;
    private float elapsedTime = 0f;
    
    private AudioSource _audioSource;
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    
    void FixedUpdate1()
    {
        elapsedTime = elapsedTime + Time.deltaTime;
        if (elapsedTime >= _verbalFeedbackCollectionFreq)
        {
            _audioSource.Play();
            elapsedTime = -2f;
            Logger.Log(LogLevel.DEBUG, "Verbal Feedback Collected! Current Frame: " + Time.frameCount);
        }
    }
}
