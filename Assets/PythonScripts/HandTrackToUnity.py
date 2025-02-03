import mediapipe as mp
import cv2
import socket
import json
import math

# Mediapipe 手部追蹤初始化
mp_hands = mp.solutions.hands
hands = mp_hands.Hands(max_num_hands=1, min_detection_confidence=0.7, min_tracking_confidence=0.7)
mp_drawing = mp.solutions.drawing_utils

# UDP 通訊初始化
UDP_IP = "127.0.0.1"  # Unity 的 IP
UDP_PORT = 5005       # Unity 的接收埠
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# 開啟相機
cap = cv2.VideoCapture(0)

# 計算兩個點之間的距離
def calculate_distance(landmark1, landmark2):
    return math.sqrt((landmark1.x - landmark2.x) ** 2 + (landmark1.y - landmark2.y) ** 2 + (landmark1.z - landmark2.z) ** 2)

# 判斷手指是否張開
def is_hand_open(hand_landmarks):
    # 設定每根手指的開合閾值（這個數值需要根據實際情況調整）
    open_threshold = 0.05
    
    # 拇指（指尖 4 和 掌心 1）
    thumb_open = calculate_distance(hand_landmarks.landmark[4], hand_landmarks.landmark[1]) > open_threshold
    
    # 食指（指尖 8 和 掌心 5）
    index_open = calculate_distance(hand_landmarks.landmark[8], hand_landmarks.landmark[5]) > open_threshold
    
    # 中指（指尖 12 和 掌心 9）
    middle_open = calculate_distance(hand_landmarks.landmark[12], hand_landmarks.landmark[9]) > open_threshold
    
    # 無名指（指尖 16 和 掌心 13）
    ring_open = calculate_distance(hand_landmarks.landmark[16], hand_landmarks.landmark[13]) > open_threshold
    
    # 小指（指尖 20 和 掌心 17）
    pinky_open = calculate_distance(hand_landmarks.landmark[20], hand_landmarks.landmark[17]) > open_threshold
    
    # 檢查是否所有的手指都張開
    is_open = thumb_open and index_open and middle_open and ring_open and pinky_open
    
    return is_open

print("開始傳輸右手座標...")

while cap.isOpened():
    ret, frame = cap.read()
    if not ret:
        break

    # 影像轉換為 RGB
    frame_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)

    # 偵測手部
    results = hands.process(frame_rgb)

    # 取得影像尺寸
    height, width, _ = frame.shape

    if results.multi_hand_landmarks:
        for hand_landmarks, handedness in zip(results.multi_hand_landmarks, results.multi_handedness):
            # 僅傳輸右手數據
            if handedness.classification[0].label == "Left":  # 修改為右手
                # 判斷手是否打開
                is_hand_open_bool = is_hand_open(hand_landmarks)  # 修正這裡，確保是調用函數

                # 提取手腕位置 (Landmark 0)
                wrist_x = hand_landmarks.landmark[0].x * width
                wrist_y = hand_landmarks.landmark[0].y * height

                # 打包數據並傳輸
                data = {
                    "x": hand_landmarks.landmark[0].x,  # 手腕的 x 座標（0~1比例）
                    "y": hand_landmarks.landmark[0].y,  # 手腕的 y 座標（0~1比例）
                    "isHandOpen": is_hand_open_bool  # 傳遞是否開放手的布林值
                }
                sock.sendto(json.dumps(data).encode(), (UDP_IP, UDP_PORT))

                # 繪製手部關節與邊框
                mp_drawing.draw_landmarks(frame, hand_landmarks, mp_hands.HAND_CONNECTIONS)

    # 顯示影像
    cv2.imshow('Hand Tracking', frame)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
sock.close()

