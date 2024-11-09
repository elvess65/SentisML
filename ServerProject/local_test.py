import cv2
import data as d
import numpy as np
import tensorflow as tf

def load_model(path):
    return tf.keras.models.load_model(path)

def predict_mnist(model, img):
    return model.predict(img)

def predict_simple(model, set):
    for i in range(len(set)):
        prediction = model.predict([set[i]])
        print("predicted: ", prediction[0][0])

def preprocess_img(img):
    img = tf.keras.utils.normalize(img)
    img = img.reshape(28, 28, 1)
    return img

def run_simple():
    model = load_model(d.SIMPLE_MODEL_PATH)
    test_set = [5, 2, 10]
    predict_simple(model, test_set)

def run_mnist():
    model = load_model(d.MNIST_MODEL_PATH)

    imgs = []
    for i in range(len(d.TEST_IMGS_PATH)):
        path = d.TEST_IMGS_PATH[i]

        test_img = cv2.imread(path, cv2.IMREAD_GRAYSCALE)
        test_img = preprocess_img(test_img)

        imgs.append(test_img)

    imgs = np.array(imgs)
    
    predictions = predict_mnist(model, imgs)
    result = np.argmax(predictions, axis=1)
    print(result)

def run():
    #run_simple()
    run_mnist()

run()
