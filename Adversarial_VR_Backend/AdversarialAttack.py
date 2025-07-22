from flask import Flask, request, jsonify
import numpy as np
import tensorflow as tf
from cleverhans.tf2.attacks.fast_gradient_method import fast_gradient_method
from cleverhans.tf2.attacks.momentum_iterative_method import momentum_iterative_method
from cleverhans.tf2.attacks.projected_gradient_descent import projected_gradient_descent
from cleverhans.tf2.attacks.carlini_wagner_l2 import carlini_wagner_l2
import h5py

app = Flask(__name__)

# Confirm model format
try:
    with h5py.File('model/LSTM_Maze.keras', 'r') as f:
        print("HDF5 format confirmed.")
except:
    print("Not HDF5 format.")

# Load Keras model
model = tf.keras.models.load_model('model/LSTM_Maze.keras')
print("[+] Model loaded successfully")

EPSILON = 0.1  # Attack strength


@app.route('/predict', methods=['POST'])
def predict():
    try:
        input_data = request.get_json()

        # Full feature list (41 total) — match training order
        features = [
            input_data["Left_Eye_Openness"],
            input_data["Right_Eye_Openness"],
            input_data["LeftPupilDiameter"],
            input_data["RightPupilDiameter"],
            input_data["LeftPupilPosInSensorX"],
            input_data["LeftPupilPosInSensorY"],
            input_data["RightPupilPosInSensorX"],
            input_data["RightPupilPosInSensorY"],
            input_data["GazeOriginLclSpc_X"],
            input_data["GazeOriginLclSpc_Y"],
            input_data["GazeOriginLclSpc_Z"],
            input_data["GazeDirectionLclSpc_X"],
            input_data["GazeDirectionLclSpc_Y"],
            input_data["GazeDirectionLclSpc_Z"],
            input_data["GazeOriginWrldSpc_X"],
            input_data["GazeOriginWrldSpc_Y"],
            input_data["GazeOriginWrldSpc_Z"],
            input_data["GazeDirectionWrldSpc_X"],
            input_data["GazeDirectionWrldSpc_Y"],
            input_data["GazeDirectionWrldSpc_Z"],
            input_data["NrmLeftEyeOriginX"],
            input_data["NrmLeftEyeOriginY"],
            input_data["NrmLeftEyeOriginZ"],
            input_data["NrmRightEyeOriginX"],
            input_data["NrmRightEyeOriginY"],
            input_data["NrmRightEyeOriginZ"],
            input_data["NrmSRLeftEyeGazeDirX"],
            input_data["NrmSRLeftEyeGazeDirY"],
            input_data["NrmSRLeftEyeGazeDirZ"],
            input_data["NrmSRRightEyeGazeDirX"],
            input_data["NrmSRRightEyeGazeDirY"],
            input_data["NrmSRRightEyeGazeDirZ"],
            input_data["HeadQRotationX"],
            input_data["HeadQRotationY"],
            input_data["HeadQRotationZ"],
            input_data["HeadQRotationW"],
            input_data["HeadEulX"],
            input_data["HeadEulY"],
            input_data["HeadEulZ"],
            input_data["Convergence_distance"],
            input_data["Velocity"]
        ]

        # Reshape to match model input: (1, 41, 1)
        x = np.array(features).reshape(1, 41, 1).astype(np.float32)
        print("Original input (x):", x.reshape(-1))

        # ✅ Clean prediction
        clean_pred = model.predict(x)
        clean_class = int(np.argmax(clean_pred))

        # === ADVERSARIAL ATTACKS: Choose ONE by uncommenting ===

        # -- 1. MI-FGSM (Momentum Iterative Fast Gradient Sign Method) -- [Your current default]
        x_adv = momentum_iterative_method(model, x, eps=EPSILON, eps_iter=0.01, nb_iter=20, norm=np.inf, clip_min=0, clip_max=1)

        # -- 2. C&W L2 Attack --
        # NOTE: Carlini Wagner attack is slow, and you may need to increase 'max_iterations' for real success
        # x_adv = carlini_wagner_l2(model, x, np.eye(clean_pred.shape[-1])[clean_class], max_iterations=100, learning_rate=0.01, batch_size=1)

        # -- 3. PGD (Projected Gradient Descent) --
        # x_adv = projected_gradient_descent(model, x, eps=EPSILON, eps_iter=0.01, nb_iter=40, norm=np.inf, clip_min=0, clip_max=1)


        print("Adversarial input (x_adv):", x_adv.numpy().reshape(-1))
        adv_pred = model.predict(x_adv)
        adv_class = int(np.argmax(adv_pred))

        print("clean_prediction:", "Low")
        print("compromised_prediction:", "High")

        # ✅ Return both results
        return jsonify({
            "clean_prediction": clean_class,
            "clean_confidence": clean_pred.tolist(),
            "compromised_prediction": adv_class,
            "compromised_confidence": adv_pred.tolist(),
            "compromised_input": x_adv.numpy().reshape(-1).tolist()
        })

    except Exception as e:
        return jsonify({"error": str(e)}), 400


if __name__ == '__main__':
    app.run(host='0.0.0.0', port=8000, debug=True)
