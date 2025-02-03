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
    public bool isHandOpen;  // 增加 isHandOpen 屬性
}

public class HandTracker : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread receiveThread;

    private Queue<string> dataQueue = new Queue<string>(); // 資料佇列
    private readonly object queueLock = new object();      // 用於鎖定佇列的執行緒安全

    private float screenWidth;
    private float screenHeight;


    public Vector3 worldPosition;
    public bool isHandOpen;
    void Start()
    {
        // 設定螢幕尺寸
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        // 初始化 UDP 接收
        udpClient = new UdpClient(5005);
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void Update()
    {
        // 處理佇列中的數據
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

    //UDP接收+解析
    private void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 5005);
        while (true)
        {
            try
            {
                // 接收數據
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string jsonString = Encoding.UTF8.GetString(data);

                // 將數據加入佇列
                lock (queueLock)
                {
                    dataQueue.Enqueue(jsonString);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("UDP 接收錯誤: " + ex.Message);
            }
        }
    }

    //解析
    private void ProcessData(string jsonString)
    {
        // 解析 JSON
        HandPosition MediaPipeData = JsonUtility.FromJson<HandPosition>(jsonString);

        // 翻轉 X 和 Y 軸（已經在之前處理過）
        float flippedX = 1 - MediaPipeData.x;  // 翻轉 X 軸
        float flippedY = 1 - MediaPipeData.y;  // 翻轉 Y 軸

        // 轉換到螢幕座標
        float screenX = flippedX * Screen.width;
        float screenY = flippedY * Screen.height;

        // 轉換到世界座標（假設 z=10）
        worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenX, screenY, 10));
        // 處理 IsHandOpen 值
         isHandOpen = MediaPipeData.isHandOpen;  // 傳遞的 IsHandOpen 值
    }

    void OnDestroy()
    {
        // 停止接收執行緒
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
