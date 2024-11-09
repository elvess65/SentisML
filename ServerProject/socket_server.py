import cv2
import numpy as np
import socket
import struct
import simpleProcessor 
import mnistProcessor

HOST = '127.0.0.1'  # localhost IP address
PORT = 9999         # Port to listen on

def process_texture(byte_data):

    # Convert received bytes to numpy array
    img_np = np.frombuffer(byte_data, dtype=np.uint8)
    
    # Decode image
    img = cv2.imdecode(img_np, cv2.IMREAD_GRAYSCALE)

    print(f"process_texture. bytes: {byte_data} shape: {img.shape}")

    return mnistProcessor.process(img)

def process_simple(byte_data):

    float_value = struct.unpack('f', byte_data)[0]

    print(f"process_simple. bytes: {byte_data} float: {float_value}")
    
    return simpleProcessor.process(float_value)

def process_server(server_socket):
     while True:
        # Establish connection with client
        client_socket, addr = server_socket.accept()
        
        print("Connection from:", addr)
        
        # Receive data from client
        data = client_socket.recv(4096)
        
        if not data:
            break
        
        print(f"Received bytes: {len(data)}")

        processed_data = None
        if len(data) == 4:
            processed_data = process_simple(data)
        else:
            # Process received texture
            processed_data = process_texture(data)
        
        # Send back processed data to client
        client_socket.sendall(bytes(str(processed_data), "utf-8"))
        
        # Keep the connection open until client finishes reading the response
        client_socket.shutdown(socket.SHUT_RDWR)
        client_socket.close()

def start_server():
    # Create a socket object
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    
    # Bind to the IP address and port
    server_socket.bind((HOST, PORT))
    
    # Listen for incoming connections
    server_socket.listen(5)
    
    print(f"Server listening {HOST} at port {PORT}...")
    
    process_server(server_socket)

    # Close server socket
    server_socket.close()

if __name__ == "__main__":
    simpleProcessor.initialize()
    mnistProcessor.initialize()
    
    start_server()