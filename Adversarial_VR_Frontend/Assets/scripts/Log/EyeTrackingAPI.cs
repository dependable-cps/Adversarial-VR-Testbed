using System;
using System.IO;
using System.Text;
using Istiak;
using Tobii.XR;
using UnityEngine;
using ViveSR.anipal.Eye;
using Valve.VR.InteractionSystem;
using ViveSR;

namespace Log
{
    public class EyeTrackingAPI : ITracker
    {
        private TobiiXR_EyeTrackingData _eyeTrackingData;
        private long _frameCounter;
        private VerboseData _sRanipalEyeVerboseData;
        private EyeData _eyeData;
        private Getinferencefrommodel _Getinferencefrommodel;
        private int n = 0;

        private float[] inputArray;

        public StringBuilder InitiateTracker(StringBuilder csvLogger)
        {
            csvLogger = new StringBuilder();
            _Getinferencefrommodel = new Getinferencefrommodel();

            // Eye Verbose Data from SRanipal SDK
            _sRanipalEyeVerboseData = new VerboseData();
            _eyeData = new EyeData();

            // Create the Header file
            csvLogger = CreateCsvHeader(csvLogger);
            return csvLogger;
        }

        private StringBuilder CreateCsvHeader(StringBuilder csvLogger)
        {
            string[] header =
            {
                "Time", "#Frame",

                "ConvergenceValid",
                "Convergence_distance",

                "Left_Eye_Openness",
                "Right_Eye_Openness",

                "Left_Eye_Closed",
                "Right_Eye_Closed",

                "LeftPupilDiameter",
                "RightPupilDiameter",

                "LeftPupilPosInSensorX",
                "LeftPupilPosInSensorY",

                "RightPupilPosInSensorX",
                "RightPupilPosInSensorY",

                // Local gaze data
                "LocalGazeValid",
                "GazeOriginLclSpc_X",
                "GazeOriginLclSpc_Y",
                "GazeOriginLclSpc_Z",

                "GazeDirectionLclSpc_X",
                "GazeDirectionLclSpc_Y",
                "GazeDirectionLclSpc_Z",

                // Local gaze data
                "WorldGazeValid",
                "GazeOriginWrldSpc_X",
                "GazeOriginWrldSpc_Y",
                "GazeOriginWrldSpc_Z",

                "GazeDirectionWrldSpc_X",
                "GazeDirectionWrldSpc_Y",
                "GazeDirectionWrldSpc_Z",

                // Normalized Gaze Origin
                "NrmLeftEyeOriginX",
                "NrmLeftEyeOriginY",
                "NrmLeftEyeOriginZ",

                "NrmRightEyeOriginX",
                "NrmRightEyeOriginY",
                "NrmRightEyeOriginZ",

                // Normalized Gaze Direction 
                "NrmSRLeftEyeGazeDirX",
                "NrmSRLeftEyeGazeDirY",
                "NrmSRLeftEyeGazeDirZ",

                "NrmSRRightEyeGazeDirX",
                "NrmSRRightEyeGazeDirY",
                "NrmSRRightEyeGazeDirZ"
            };
            header.ForEach(item => csvLogger.Append(item + ","));

            // Write the Header
            csvLogger.AppendLine();
            return csvLogger;
        }


        public StringBuilder StartTracking(String time, int frameCount, StringBuilder csvLogger, PredictionInput input)
        {
            // Get Eye data from TOBI API 
            TobiiXR_EyeTrackingData localEyeData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.Local);
            TobiiXR_EyeTrackingData worldEyeData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);


            // Get Eye data from SRNipal 
            ViveSR.Error error = SRanipal_Eye_API.GetEyeData(ref _eyeData);

            if (error == Error.WORK)
            {
                _sRanipalEyeVerboseData = _eyeData.verbose_data;

                // Left Eye Data
                SingleEyeData sRleftEyeData = _sRanipalEyeVerboseData.left;
                // Right Eye
                SingleEyeData sRrightEyeData = _sRanipalEyeVerboseData.right;

                // Write in the CSV file
                csvLogger.AppendFormat(
                    "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}," +
                    "{19}, {20}, {21}, {22}, {23}, {24}, {25},{26}, {27}, {28}, {29}, {30}, {31}, {32}, {33}, {34}, {35}," +
                    "{36}, {37}, {38}, {39}",
                    // Time and Frame count
                    time, frameCount,

                    // Convergence Distance
                    worldEyeData.ConvergenceDistanceIsValid,
                    worldEyeData.ConvergenceDistance,

                    // Eye Openness 
                    sRleftEyeData.eye_openness,
                    sRrightEyeData.eye_openness,

                    // Eye blinking 
                    localEyeData.IsLeftEyeBlinking,
                    localEyeData.IsRightEyeBlinking,

                    // Pupil Diameter
                    sRleftEyeData.pupil_diameter_mm,
                    sRrightEyeData.pupil_diameter_mm,

                    // Pupil Position in Sensor area (x, y)
                    sRleftEyeData.pupil_position_in_sensor_area.x,
                    sRleftEyeData.pupil_position_in_sensor_area.y,
                    sRrightEyeData.pupil_position_in_sensor_area.x,
                    sRrightEyeData.pupil_position_in_sensor_area.y,

                    // IS local Gaze Valid
                    localEyeData.GazeRay.IsValid,

                    // Local Space Gaze Origin Combined
                    localEyeData.GazeRay.Origin.x,
                    localEyeData.GazeRay.Origin.y,
                    localEyeData.GazeRay.Origin.z,

                    // Local Space Gaze Direction Combined
                    localEyeData.GazeRay.Direction.x,
                    localEyeData.GazeRay.Direction.y,
                    localEyeData.GazeRay.Direction.z,

                    // IS World Gaze Valid
                    worldEyeData.GazeRay.IsValid,

                    //world space Gaze Origin Combined
                    worldEyeData.GazeRay.Origin.x,
                    worldEyeData.GazeRay.Origin.y,
                    worldEyeData.GazeRay.Origin.z,

                    // world space Gaze Direction Combined
                    worldEyeData.GazeRay.Direction.x,
                    worldEyeData.GazeRay.Direction.y,
                    worldEyeData.GazeRay.Direction.z,

                    // Gaze Origin in mm 
                    sRleftEyeData.gaze_origin_mm.x,
                    sRleftEyeData.gaze_origin_mm.y,
                    sRleftEyeData.gaze_origin_mm.z,
                    sRrightEyeData.gaze_origin_mm.x,
                    sRrightEyeData.gaze_origin_mm.y,
                    sRrightEyeData.gaze_origin_mm.z,

                    // Normalized Gaze direction
                    sRleftEyeData.gaze_direction_normalized.x,
                    sRleftEyeData.gaze_direction_normalized.y,
                    sRleftEyeData.gaze_direction_normalized.z,
                    sRrightEyeData.gaze_direction_normalized.x,
                    sRrightEyeData.gaze_direction_normalized.y,
                    sRrightEyeData.gaze_direction_normalized.z
                );
                csvLogger.AppendLine();
                GetDataToPredict(sRrightEyeData, sRleftEyeData, worldEyeData, localEyeData, input);
            }

            return csvLogger;
        }

        private void GetDataToPredict(SingleEyeData sRrightEyeData, SingleEyeData sRleftEyeData,
            TobiiXR_EyeTrackingData worldEyeData, TobiiXR_EyeTrackingData localEyeData, PredictionInput input)
        {
            // float[] inputArray = {sRrightEyeData.gaze_origin_mm.z,
            // sRrightEyeData.gaze_origin_mm.x,
            // sRleftEyeData.gaze_origin_mm.z,
            // worldEyeData.GazeRay.Origin.y,
            // sRrightEyeData.gaze_origin_mm.y,
            // sRleftEyeData.gaze_origin_mm.x,
            // worldEyeData.GazeRay.Origin.x,
            // sRleftEyeData.gaze_origin_mm.y,
            // worldEyeData.GazeRay.Origin.z,
            // localEyeData.GazeRay.Origin.x,
            // sRrightEyeData.pupil_diameter_mm};
            //
            // if ( n%100 == 0){
            //  //_Getinferencefrommodel.Start();
            // var prediction = _Getinferencefrommodel.predict(inputArray);
            // Debug.Log( "predicted cybersickness : " + Convert.ToString(prediction));
            // Debug.Log(score.proba);}
            // n++;
            // float[] inputArray =
            // {
            //     sRrightEyeData.gaze_origin_mm.z, sRrightEyeData.gaze_origin_mm.x,
            //     sRleftEyeData.gaze_origin_mm.z, worldEyeData.GazeRay.Origin.y,
            //     sRrightEyeData.gaze_origin_mm.y, sRleftEyeData.gaze_origin_mm.x,
            //     worldEyeData.GazeRay.Origin.x, sRleftEyeData.gaze_origin_mm.y,
            //     worldEyeData.GazeRay.Origin.z, localEyeData.GazeRay.Origin.x,
            //     localEyeData.GazeRay.Origin.y, localEyeData.GazeRay.Origin.z,
            //     sRrightEyeData.pupil_diameter_mm, sRleftEyeData.pupil_diameter_mm,
            //     sRleftEyeData.pupil_position_in_sensor_area.x, sRleftEyeData.pupil_position_in_sensor_area.y,
            //     sRrightEyeData.pupil_position_in_sensor_area.x, sRrightEyeData.pupil_position_in_sensor_area.y,
            //     worldEyeData.GazeRay.Direction.x, worldEyeData.GazeRay.Direction.y,
            // };

            // PredictDlModelResult.Instance.StartPredict(inputArray);

            input.Left_Eye_Openness = sRleftEyeData.eye_openness;
            input.Right_Eye_Openness = sRrightEyeData.eye_openness;
            input.LeftPupilDiameter = sRleftEyeData.pupil_diameter_mm;
            input.RightPupilDiameter = sRrightEyeData.pupil_diameter_mm;
            input.LeftPupilPosInSensorX = sRleftEyeData.pupil_position_in_sensor_area.x;
            input.LeftPupilPosInSensorY = sRleftEyeData.pupil_position_in_sensor_area.y;
            input.RightPupilPosInSensorX = sRrightEyeData.pupil_position_in_sensor_area.x;
            input.RightPupilPosInSensorY = sRrightEyeData.pupil_position_in_sensor_area.y;
            input.GazeOriginLclSpc_X = localEyeData.GazeRay.Origin.x;
            input.GazeOriginLclSpc_Y = localEyeData.GazeRay.Origin.y;
            input.GazeOriginLclSpc_Z = localEyeData.GazeRay.Origin.z;
            input.GazeDirectionLclSpc_X = localEyeData.GazeRay.Direction.x;
            input.GazeDirectionLclSpc_Y = localEyeData.GazeRay.Direction.y;
            input.GazeDirectionLclSpc_Z = localEyeData.GazeRay.Direction.z;
            input.GazeOriginWrldSpc_X = worldEyeData.GazeRay.Origin.x;
            input.GazeOriginWrldSpc_Y = worldEyeData.GazeRay.Origin.y;
            input.GazeOriginWrldSpc_Z = worldEyeData.GazeRay.Origin.z;
            input.GazeDirectionWrldSpc_X = worldEyeData.GazeRay.Direction.x;
            input.GazeDirectionWrldSpc_Y = worldEyeData.GazeRay.Direction.y;
            input.GazeDirectionWrldSpc_Z = worldEyeData.GazeRay.Direction.z;
            input.NrmLeftEyeOriginX = sRleftEyeData.gaze_origin_mm.x;
            input.NrmLeftEyeOriginY = sRleftEyeData.gaze_origin_mm.y;
            input.NrmLeftEyeOriginZ = sRleftEyeData.gaze_origin_mm.z;
            input.NrmRightEyeOriginX = sRrightEyeData.gaze_origin_mm.x;
            input.NrmRightEyeOriginY = sRrightEyeData.gaze_origin_mm.y;
            input.NrmRightEyeOriginZ = sRrightEyeData.gaze_origin_mm.z;
            input.NrmSRLeftEyeGazeDirX = sRleftEyeData.gaze_direction_normalized.x;
            input.NrmSRLeftEyeGazeDirY = sRleftEyeData.gaze_direction_normalized.y;
            input.NrmSRLeftEyeGazeDirZ = sRleftEyeData.gaze_direction_normalized.z;
            input.NrmSRRightEyeGazeDirX = sRrightEyeData.gaze_direction_normalized.x;
            input.NrmSRRightEyeGazeDirY = sRrightEyeData.gaze_direction_normalized.y;
            input.NrmSRRightEyeGazeDirZ = sRrightEyeData.gaze_direction_normalized.z;
            input.Convergence_distance = worldEyeData.ConvergenceDistance;
        }


        public void SaveToFile(String path, StringBuilder csvLogger)
        {
            System.IO.File.WriteAllText(
                path + "eye_tracking_data-" + DateTime.Now.ToString("dd-MMM-yyyy-(hh-mm-ss)") + ".csv",
                csvLogger.ToString());
            Debug.Log("Eye Tracking data saved at: " + path);
            Logger.Log(LogLevel.INFO, "Eye Tracking data saved at: " + path);
        }
    }
}