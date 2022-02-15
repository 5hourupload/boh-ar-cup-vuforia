using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Vuforia;

public class tcpClient : MonoBehaviour
{
    public string m_ipAddress = "127.0.0.1";
    public int m_port = 2001;
    private TcpClient m_tcpClient;
    private NetworkStream m_networkStream;
    private bool m_isConnection;
    private string m_message;

    public DefaultTrackableEventHandlerCustom DTEHC;
    public GameObject cup;

    private void Awake()
    {
        try
        {
            m_tcpClient = new TcpClient(m_ipAddress, m_port);
            m_networkStream = m_tcpClient.GetStream();
            m_isConnection = true;
            Debug.LogFormat("connection success");
        }
        catch (SocketException)
        {
            Debug.LogError("connection fail");
        }
    }

    private void Update()
    {
        if (!m_isConnection)
        {
            GUILayout.Label("not connected");
            return;
        }
        Vector3 p = cup.transform.position;
        Vector3 r = cup.transform.rotation.eulerAngles;

        m_message = p.x + " " + p.y + " " + p.z + " " + r.x + " " + r.y + " " + r.z;

        if (DTEHC.getStatus() == TrackableBehaviour.Status.DETECTED ||
            DTEHC.getStatus() == TrackableBehaviour.Status.TRACKED ||
            DTEHC.getStatus() == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            try
            {
                var buffer = Encoding.UTF8.GetBytes(m_message);
                m_networkStream.Write(buffer, 0, buffer.Length);
                Debug.Log("send success " + m_message);
            }
            catch (Exception)
            {
                Debug.LogError("send fail");
            }
        }

        //Debug.LogError(cup.transform);
    }

    private void OnDestroy()
    {
        m_tcpClient?.Dispose();
        m_networkStream?.Dispose();
        Debug.Log("disconnected");
    }
}