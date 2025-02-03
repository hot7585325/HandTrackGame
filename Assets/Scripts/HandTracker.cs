using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;



[System.Serializable]
public class HandPosition
{
    public float x;
    public float y;
    public bool isHandOpen;  // �W�[ isHandOpen �ݩ�
}

public class HandTracker : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread receiveThread;

    private Queue<string> dataQueue = new Queue<string>(); // ��Ʀ�C
    private readonly object queueLock = new object();      // �Ω���w��C��������w��

    private float screenWidth;
    private float screenHeight;


    public Vector3 worldPosition;
    public bool isHandOpen;
    void Start()
    {
        // �]�w�ù��ؤo
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        // ��l�� UDP ����
        udpClient = new UdpClient(5005);
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void Update()
    {
        // �B�z��C�����ƾ�
        if (dataQueue.Count > 0)
        {
            lock (queueLock)
            {
                while (dataQueue.Count > 0)
                {
                    string jsonString = dataQueue.Dequeue();
                    ProcessData(jsonString);
                }
            }
        }



    }

    //UDP����+�ѪR
    private void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 5005);
        while (true)
        {
            try
            {
                // �����ƾ�
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string jsonString = Encoding.UTF8.GetString(data);

                // �N�ƾڥ[�J��C
                lock (queueLock)
                {
                    dataQueue.Enqueue(jsonString);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("UDP �������~: " + ex.Message);
            }
        }
    }

    //�ѪR
    private void ProcessData(string jsonString)
    {
        // �ѪR JSON
        HandPosition MediaPipeData = JsonUtility.FromJson<HandPosition>(jsonString);

        // ½�� X �M Y �b�]�w�g�b���e�B�z�L�^
        float flippedX = 1 - MediaPipeData.x;  // ½�� X �b
        float flippedY = 1 - MediaPipeData.y;  // ½�� Y �b

        // �ഫ��ù��y��
        float screenX = flippedX * Screen.width;
        float screenY = flippedY * Screen.height;

        // �ഫ��@�ɮy�С]���] z=10�^
        worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenX, screenY, 10));
        // �B�z IsHandOpen ��
         isHandOpen = MediaPipeData.isHandOpen;  // �ǻ��� IsHandOpen ��
    }

    void OnDestroy()
    {
        // ����������
        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Abort();
        }
        udpClient.Close();
    }

    public Vector3 GetTrackPos()
    {
        return worldPosition;
    }

    public bool GetIsOpen()
    {
        return isHandOpen;
    }

 
}
