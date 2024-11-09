import data as d
import tensorflow as tf

model = None

def load_model(path):
    model = tf.keras.models.load_model(path)
    return model

def predict(model, input_val):
    prediction = model.predict([input_val])
    result = prediction[0][0]
    print("predicted: ", result)
    return result

def initialize():
    global model
    model = load_model(d.SIMPLE_MODEL_PATH)

def process(input_val):
    return predict(model, input_val)