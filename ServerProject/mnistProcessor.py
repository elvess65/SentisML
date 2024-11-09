import data as d
import numpy as np
import tensorflow as tf

model = None

def load_model(path):
    model = tf.keras.models.load_model(path)
    return model

def preprocess_img(img):
    img = tf.keras.utils.normalize(img)
    return np.expand_dims(img, axis=-1)

def predict(model, imgs):
    predictions = model.predict(imgs)
    result = np.argmax(predictions, axis=1)[0]
    
    print("predicted: ", result, predictions[0])
    return (result, predictions[0])

def initialize():
    global model
    model = load_model(d.MNIST_MODEL_PATH)

def process(img):
    images = np.array([preprocess_img(img)])
    return predict(model, images)