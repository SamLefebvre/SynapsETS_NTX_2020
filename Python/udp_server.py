import random
import time
import socket

UDP_IP = "127.0.0.1"
UDP_PORT = 5020

while True:
    rand = random.uniform(0, 5)
    rand = round(rand,2)
    data = str(rand)

    print(data)
    sock = socket.socket(socket.AF_INET, # Internet
                        socket.SOCK_DGRAM) # UDP
    sock.connect((UDP_IP, UDP_PORT))
    sock.send(data.encode())
    time.sleep(0.5)