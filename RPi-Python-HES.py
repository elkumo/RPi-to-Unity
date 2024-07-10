import socket
import RPi.GPIO as GPIO
import time
import sys

# GPIO setup
FHS = 17  # Replace with your actual GPIO pin number
BHS = 27  # Replace with your actual GPIO pin number

GPIO.setmode(GPIO.BCM)
GPIO.setup(FHS, GPIO.IN, pull_up_down=GPIO.PUD_UP)
GPIO.setup(BHS, GPIO.IN, pull_up_down=GPIO.PUD_UP)

HOST = '0.0.0.0'  # Listen on all network interfaces
PORT = 12345      # Port number to listen on

def main():
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as server_socket:
        server_socket.bind((HOST, PORT))
        server_socket.listen(1)
        print(f"Listening on {HOST}:{PORT}")

        conn, addr = server_socket.accept()
        with conn:
            print(f"Connected by {addr}")

            try:
                while True:
                    # Read hall sensor input
                    if GPIO.input(FHS) == GPIO.LOW:
                        print('forward')
                        conn.sendall(b'forward')

                    if GPIO.input(BHS) == GPIO.LOW:
                        print('backward')
                        conn.sendall(b'backward')
                    
                    time.sleep(0.1)  # Small delay to prevent high CPU usage
            except KeyboardInterrupt:
                print("\nConnection closed by user.")
            finally:
                GPIO.cleanup()
                conn.close()

if __name__ == "__main__":
    main()