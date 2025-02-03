using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
   public static UIManager instance;
   public Text MeterText;
   public GameObject HP;
   public Transform HP_Panel;
   public HandCtrl_Player Player;
    public List<GameObject> HPObjs;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        for (int i = 0; i < Player.HP; i++)
        {
            GameObject hp= Instantiate(HP, HP_Panel);
            Debug.Log("生命值生成");
            HPObjs.Add(hp);
        }
    }


    private void Update()
    {
        UpdateMeterUI();
    }

    public void UpdateHPUI()
    {
        HPObjs[0].SetActive(false);
        HPObjs.RemoveAt(0);
    }


    public void UpdateMeterUI()
    {
        int Meter= (int)Camera.main.transform.position.x;
        MeterText.text= Meter.ToString();
    }
}
