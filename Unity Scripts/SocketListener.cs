using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections; // Required for IEnumerator

public class SocketListener : MonoBehaviour
{
    // IP address and port of the Raspberry Pi
    public string serverIP = "127.0.0.1"; // Replace with your Raspberry Pi's IP
    public int serverPort = 12345; // Replace with the port number your Python server is listening on

    private TcpClient client;
    private Thread receiveThread;
    private bool isConnected = false;

    // Object to move in Unity
    public GameObject objectToMove;
    public float moveSpeed = 25f;
    public float moveTime = 1f; // Time taken to move to the new position
    public float moveDistance = 2.5f; // Distance to move with each command

    private UnityMainThreadDispatcher mainThreadDispatcher;

    void Start()
    {
        // Find the UnityMainThreadDispatcher instance in the scene
        mainThreadDispatcher = FindObjectOfType<UnityMainThreadDispatcher>();
        if (mainThreadDispatcher == null)
        {
            Debug.LogError("UnityMainThreadDispatcher not found in the scene.");
            return;
        }

        // Connect to the server
        ConnectToServer();
    }

    void ConnectToServer()
    {
        try
        {
            // Create a new TcpClient instance and connect to the server
            client = new TcpClient();
            client.Connect(serverIP, serverPort);
            isConnected = true;
            Debug.Log("Connected to server!");

            // Start a new thread to receive data asynchronously
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.Start();
        }
        catch (Exception e)
        {
            // Handle connection errors
            Debug.LogError("Error connecting to server: " + e.Message);
        }
    }

    void ReceiveData()
    {
        byte[] bytes = new byte[1024];
        while (isConnected)
        {
            try
            {
                // Read data from the server
                NetworkStream stream = client.GetStream();
                int bytesRead = stream.Read(bytes, 0, bytes.Length);
                if (bytesRead > 0)
                {
                    // Convert received bytes to string
                    string data = Encoding.UTF8.GetString(bytes, 0, bytesRead);
                    Debug.Log("Received data: " + data);

                    // Process received data to move the object on the main thread
                    mainThreadDispatcher.Enqueue(() => HandleCommand(data.Trim()));
                }
            }
            catch (Exception e)
            {
                // Handle data receiving errors
                Debug.LogError("Error receiving data: " + e.Message);
                isConnected = false;
            }
        }
    }

    void HandleCommand(string command)
    {
        // Calculate the target position based on the received command
        Vector3 targetPosition = objectToMove.transform.position;

        // Move object based on received command
        if (command.Equals("forward", StringComparison.OrdinalIgnoreCase))
        {
            targetPosition += Vector3.forward * moveDistance;
        }
        else if (command.Equals("backward", StringComparison.OrdinalIgnoreCase))
        {
            targetPosition += Vector3.back * moveDistance;
        }
        /*
        else if (command.Equals("left", StringComparison.OrdinalIgnoreCase))
        {
            targetPosition += Vector3.left * moveDistance;
        }
        else if (command.Equals("right", StringComparison.OrdinalIgnoreCase))
        {
            targetPosition += Vector3.right * moveDistance;
        }
        else if (command.Equals("up", StringComparison.OrdinalIgnoreCase))
        {
            targetPosition += Vector3.up * moveDistance;
        }
        else if (command.Equals("down", StringComparison.OrdinalIgnoreCase))
        {
            targetPosition += Vector3.down * moveDistance;
        }
        */
        

        // Start a coroutine to smoothly move the object to the target position
        StartCoroutine(MoveObjectSmoothly(targetPosition));
    }

    // Coroutine to smoothly move the object to a target position.
    IEnumerator MoveObjectSmoothly(Vector3 targetPosition)
    {
        // Float to track the elapsed time.
        float elapsedTime = 0;
        Vector3 startingPosition = objectToMove.transform.position;

        // Interpolate the object's position over moveTime seconds
        while (elapsedTime < moveTime)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the object reaches the exact target position
        objectToMove.transform.position = targetPosition;
    }

    void OnDestroy()
    {
        isConnected = false;
        if (client != null)
        {
            client.Close(); // Close the TCP client connection
        }
        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Join(); // Wait for the receive thread to terminate
        }
    }
}
