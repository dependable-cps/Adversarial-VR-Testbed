namespace Istiak
{
    [System.Serializable]
    public class PredictionInput
    {
        public float Left_Eye_Openness;
        public float Right_Eye_Openness;
        public float LeftPupilDiameter;
        public float RightPupilDiameter;
        public float LeftPupilPosInSensorX;
        public float LeftPupilPosInSensorY;
        public float RightPupilPosInSensorX;
        public float RightPupilPosInSensorY;
        public float GazeOriginLclSpc_X;
        public float GazeOriginLclSpc_Y;
        public float GazeOriginLclSpc_Z;
        public float GazeDirectionLclSpc_X;
        public float GazeDirectionLclSpc_Y;
        public float GazeDirectionLclSpc_Z;
        public float GazeOriginWrldSpc_X;
        public float GazeOriginWrldSpc_Y;
        public float GazeOriginWrldSpc_Z;
        public float GazeDirectionWrldSpc_X;
        public float GazeDirectionWrldSpc_Y;
        public float GazeDirectionWrldSpc_Z;
        public float NrmLeftEyeOriginX;
        public float NrmLeftEyeOriginY;
        public float NrmLeftEyeOriginZ;
        public float NrmRightEyeOriginX;
        public float NrmRightEyeOriginY;
        public float NrmRightEyeOriginZ;
        public float NrmSRLeftEyeGazeDirX;
        public float NrmSRLeftEyeGazeDirY;
        public float NrmSRLeftEyeGazeDirZ;
        public float NrmSRRightEyeGazeDirX;
        public float NrmSRRightEyeGazeDirY;
        public float NrmSRRightEyeGazeDirZ;
        public float HeadQRotationX;
        public float HeadQRotationY;
        public float HeadQRotationZ;
        public float HeadQRotationW;
        public float HeadEulX;
        public float HeadEulY;
        public float HeadEulZ;
        public float Convergence_distance;
        public float Velocity;
    }
}