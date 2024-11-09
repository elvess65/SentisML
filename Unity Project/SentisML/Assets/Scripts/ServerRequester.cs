using System;
using System.Text;
using UnityEngine;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.Generic;

public class ServerRequester : MonoBehaviour
{
    public string HOST = "127.0.0.1";
    public int PORT;
    public Texture2D textureToSend;
    public float valToSend;

    private byte[] bytesToSendTexture;
    private byte[] bytesToSendFloat;

    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            var r = await SendTextureRequest(textureToSend);
            var probabilityStr = string.Join(", ", r.probability);
            Debug.Log($"{r.index}. [{probabilityStr}]");
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            var r = await SendFloatRequest(valToSend);
            Debug.Log(r);
        }
    }

    public async Task<(int index, float[] probability)> SendTextureRequest(Texture2D inputTexture)
    {
        bytesToSendTexture = inputTexture.EncodeToPNG();

        var task = SendTexture();
        await task;

        return task.Result;
    }

    public async Task<double> SendFloatRequest(float inputValue)
    {
        bytesToSendFloat = BitConverter.GetBytes(inputValue);

        var task = SendFloat();
        await task;

        return task.Result;
    }


    async Task<(int, float[])> SendTexture()
    {
        try
        {
            // Create a TCP client and connect to the server
            using var client = new TcpClient();
            
            await client.ConnectAsync(HOST, PORT);

            // Get the network stream for sending data
            using (NetworkStream stream = client.GetStream())
            {
                // Send the texture data to the server
                await stream.WriteAsync(bytesToSendTexture, 0, bytesToSendTexture.Length);

                // Read the response asynchronously
                byte[] buffer = new byte[4096];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                // Convert the response to a string
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log("Received response: " + response);

                int index = ParseInt(response);
                float[] probabilityDistribution = ParseFloatArray(response);

                return (index, probabilityDistribution);
            }
            
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending/receiving data: " + e.Message);
            return (-1, new float[0]);
        }
    }

    void Parse(string response)
    {
        // Parse the integer
        int intValue = ParseInt(response);
        Debug.Log("Integer value: " + intValue);

        // Parse the float array
        float[] floatArray = ParseFloatArray(response);
        Debug.Log("Float array:");
        foreach (float f in floatArray)
        {
            Debug.Log(f);
        }
    }

    int ParseInt(string response)
    {
        int start = response.IndexOf('(') + 1;
        int end = response.IndexOf(',');
        string intString = response.Substring(start, end - start).Trim();
        return int.Parse(intString);
    }

    float[] ParseFloatArray(string response)
    {
        int start = response.IndexOf('[') + 1;
        int end = response.IndexOf(']');
        string floatArrayString = response.Substring(start, end - start);
        string[] floatStrings = floatArrayString.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

        List<float> floats = new();
        for (int i = 0; i < floatStrings.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(floatStrings[i]))
                continue;

            float.TryParse(floatStrings[i], NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
            floats.Add(result);
        }
        return floats.ToArray();
    }

    async Task<double> SendFloat()
    {
        try
        {
            // Create a TCP client and connect to the server
            using var client = new TcpClient();
            
            await client.ConnectAsync(HOST, PORT);

            // Get the network stream for sending data
            using (NetworkStream stream = client.GetStream())
            {
                // Send the float data to the server
                await stream.WriteAsync(bytesToSendFloat, 0, bytesToSendFloat.Length);

                // Read the response asynchronously
                byte[] buffer = new byte[4096];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                // Convert the response to a string
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log("Received float response: " + response);

                if (double.TryParse(response, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                {
                    return result;
                }
                else
                {
                    return -1;
                }
            }
            
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending/receiving float data: " + e.Message);
            return -1;
        }
    }
}
