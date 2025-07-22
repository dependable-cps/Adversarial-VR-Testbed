using UnityEngine;

public class RotatingCoin : MonoBehaviour
{
    public float rotationSpeed = 30f;  // Degrees per second

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Increment score (you can link to your score manager)
            Debug.Log("Coin collected!");
            ScoreManager.Instance.AddScore(1); // if using a singleton ScoreManager

            // Disable or destroy coin
            gameObject.SetActive(false); // or Destroy(gameObject);
        }
    }
}