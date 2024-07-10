using UnityEngine;
using TMPro;
using System;
using System.Net;
using System.Net.Sockets;

public class DisplayIPAddress : MonoBehaviour
{
    public TextMeshProUGUI displayText; // Reference to the TextMeshProUGUI element

    void Start()
    {
        if (displayText != null)
        {
            UpdateIPAddressText();
        }
        else
        {
            Debug.LogWarning("displayText is not assigned in the inspector.");
        }
    }

    void UpdateIPAddressText()
    {
        string hostName = GetHostName();
        string publicIPAddress = GetPublicIPAddress();
        string localIPAddress = GetLocalIPAddress();

        // Update the TextMeshProUGUI component with the information
        displayText.text = "Hostname: " + hostName + "\n" +
                           "Public IP Address: " + publicIPAddress + "\n" +
                           "Local IPv4 Address: " + localIPAddress;
    }

    string GetHostName()
    {
        try
        {
            return Dns.GetHostName();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to retrieve hostname: " + e.Message);
            return "N/A";
        }
    }

    string GetPublicIPAddress()
    {
        try
        {
            string publicIpAddress = new WebClient().DownloadString("http://icanhazip.com").Trim();
            return publicIpAddress;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to retrieve public IP address: " + e.Message);
            return "N/A";
        }
    }

    string GetLocalIPAddress()
    {
        string localIpAddress = "127.0.0.1"; // Default to loopback address if no valid IP found

        try
        {
            // Get host name
            string hostName = Dns.GetHostName();

            // Get list of IP addresses for the current host
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);

            // Find the first IPv4 address
            foreach (IPAddress addr in addresses)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork) // Check for IPv4
                {
                    localIpAddress = addr.ToString();
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to retrieve local IP address: " + e.Message);
        }

        return localIpAddress;
    }
}
