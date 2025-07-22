namespace Istiak
{
    [System.Serializable]
    public class PredictionResponse
    {
        public int clean_prediction;
        public float[][] clean_confidence;
        public int compromised_prediction;
        public float[][] compromised_confidence;
        public float[] compromised_input;
    }
}