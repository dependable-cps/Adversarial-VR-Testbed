"""The app main."""
import json
import logging
from logging.config import dictConfig
import os
import traceback

from flask import Flask
from flask import render_template, request, Blueprint
import gin

from explain.logic import ExplainBot
from explain.sample_prompts_by_action import sample_prompt_for_action
import pandas as pd


# gunicorn doesn't have command line flags, using a gin file to pass command line args
@gin.configurable
class GlobalArgs:
    def __init__(self, config, baseurl):
        self.config = config
        self.baseurl = baseurl


# Parse gin global config
gin.parse_config_file("global_config.gin")

# Get args
args = GlobalArgs()

bp = Blueprint('host', __name__, template_folder='templates')

dictConfig({
    'version': 1,
    'formatters': {'default': {
        'format': '[%(asctime)s] %(levelname)s in %(module)s: %(message)s',
    }},
    'handlers': {'wsgi': {
        'class': 'logging.StreamHandler',
        'stream': 'ext://flask.logging.wsgi_errors_stream',
        'formatter': 'default'
    }},
    'root': {
        'level': 'INFO',
        'handlers': ['wsgi']
    }
})

os.environ["TOKENIZERS_PARALLELISM"] = "false"

# Parse application level configs
gin.parse_config_file(args.config)

# Setup the explainbot
BOT = ExplainBot()


@bp.route('/')
def home():
    """Load the explanation interface."""
    app.logger.info("Loaded Login")
    objective = BOT.conversation.describe.get_dataset_objective()
    return render_template("index.html", currentUserId="user", datasetObjective=objective)


@bp.route("/log_feedback", methods=['POST'])
def log_feedback():
    """Logs feedback"""
    feedback = request.data.decode("utf-8")
    app.logger.info(feedback)
    split_feedback = feedback.split(" || ")

    message = f"Feedback formatted improperly. Got: {split_feedback}"
    assert split_feedback[0].startswith("MessageID: "), message
    assert split_feedback[1].startswith("Feedback: "), message
    assert split_feedback[2].startswith("Username: "), message

    message_id = split_feedback[0][len("MessageID: "):]
    feedback_text = split_feedback[1][len("Feedback: "):]
    username = split_feedback[2][len("Username: "):]

    logging_info = {
        "id": message_id,
        "feedback_text": feedback_text,
        "username": username
    }

    BOT.log(logging_info)
    return ""


@bp.route("/sample_prompt", methods=["Post"])
def sample_prompt():
    """Samples a prompt"""
    data = json.loads(request.data)
    action = data["action"]
    username = data["thisUserName"]

    prompt = sample_prompt_for_action(action,
                                      BOT.prompts.filename_to_prompt_id,
                                      BOT.prompts.final_prompt_set,
                                      real_ids=BOT.conversation.get_training_data_ids())

    logging_info = {
        "username": username,
        "requested_action_generation": action,
        "generated_prompt": prompt
    }
    BOT.log(logging_info)

    return prompt


@bp.route("/get_response", methods=['POST'])
def get_bot_response():

    """Load the box response."""
    if request.method == "POST":
        app.logger.info("generating the bot response")
        app.logger.info("request.data")
        app.logger.info(request.form)

        try:
            #data = json.loads(request.data)
            user_text = request.form.get('userInput')
            if "Developer" in request.form.get('userRole'):
                if "why i feel" in user_text:
                    conversation = BOT.conversation
                    data = conversation.get_var('dataset').contents['X']
                    ids=data.tail(1).index.values[0]
                    user_text = "explain instance with id "+ str(ids)
                if "i change" in user_text:
                    conversation = BOT.conversation
                    data = conversation.get_var('dataset').contents['X']
                    ids=data.tail(1).index.values[0]
                    user_text = "how can I change the prediction for instance with id "+ str(ids) + " to low cybersickness"

                #user_text = data["userInput"]
                app.logger.info(f"user_text: {user_text}")
                conversation = BOT.conversation
                response = BOT.update_state(user_text, conversation)
        except Exception as ext:
            app.logger.info(f"Traceback getting bot response: {traceback.format_exc()}")
            app.logger.info(f"Exception getting bot response: {ext}")
            response = "Sorry! I couldn't understand that. Could you please try to rephrase?"
        return response

@bp.route("/get_featureimp", methods=['POST'])
def get_feature_importance():
    if request.method == "POST":
            app.logger.info("generating the faeture importance")
            app.logger.info("request.data")
            app.logger.info(request.form)
            try:
                #data = pd.read_csv(BOT.dataset_file_path,index_col=False)
                #data = data.drop('y', axis=1)
                conversation = BOT.conversation
                data = conversation.get_var('dataset').contents['X']
                ids=data.tail(1).index.values.tolist()
                exp = conversation.feature_importance(ids = ids,dataset=data).list_exp
            except Exception as ext:
                app.logger.info(f"Traceback getting bot response: {traceback.format_exc()}")
                app.logger.info(f"Exception getting bot response: {ext}")
                exp = []
            return str(exp).strip("[]").replace("'", "").replace("(", "").replace(")", "")
    
@bp.route("/update_data", methods=['POST'])
def update_data():
    if request.method == "POST":
            app.logger.info("generating the faeture importance")
            app.logger.info("request.data")
            app.logger.info(request.form)
            try:
                data = request.form.get('data')
                #data = "1,2,3,4,5,6,7,8,9,10,11,12,13"
                
                listdata = list(data.split(","))
                file = pd.read_csv(BOT.dataset_file_path)
                id=file.tail(1).index.values[0]+ 1
                app.logger.info(f"Incoming data: {listdata}")
                app.logger.info(f"Expected column count: {len(file.columns)}")
                listdata.insert(0,id)
                file.loc[len(file.index)] = listdata
                file.to_csv(BOT.dataset_file_path, index=False,mode='w')
                response = "data updated!"
            except Exception as ext:
                app.logger.info(f"Traceback getting bot response: {traceback.format_exc()}")
                app.logger.info(f"Exception getting bot response: {ext}")
                response = "Sorry! I couldn't understand that. Could you please try to rephrase?"
            return response
    

@bp.route("/update_exp", methods=['POST'])
def update_exp():
            if request.method == "POST":
                    BOT.load_dataset(BOT.dataset_file_path,
                        BOT.dataset_index_column,
                        BOT.target_variable_name,
                        BOT.categorical_features,
                        BOT.numerical_features,
                        BOT.remove_underscores,
                        store_to_conversation=True,
                        skip_prompts=BOT.skip_prompts)
                    background_dataset = BOT.load_dataset(BOT.background_dataset_file_path,
                                               BOT.dataset_index_column,
                                               BOT.target_variable_name,
                                               BOT.categorical_features,
                                               BOT.numerical_features,
                                               BOT.remove_underscores,
                                               store_to_conversation=False)
                # Load the explanations
                    BOT.load_explanations(background_dataset=background_dataset)
                    response = "data updated!"
                    return response

app = Flask(__name__)
app.register_blueprint(bp, url_prefix=args.baseurl)

if __name__ != '__main__':
    gunicorn_logger = logging.getLogger('gunicorn.error')
    app.logger.handlers = gunicorn_logger.handlers
    app.logger.setLevel(gunicorn_logger.level)

if __name__ == "__main__":
    # clean up storage file on restart
    app.logger.info(f"Launching app from config: {args.config}")
    app.run(debug=False, port=5000, host='0.0.0.0')
