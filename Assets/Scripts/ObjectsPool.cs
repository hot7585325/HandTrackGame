using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectsPool : MonoBehaviour
{
    public Queue<GameObject> ObjPool = new Queue<GameObject>();
    public int InitialObjsCount;
    public GameObject Obj;
    public I_Observer_List ObeserberList;

    private void Awake()
    {
        if (gameObject.GetComponent<I_Observer_List>() != null)
        {
            ObeserberList=gameObject.GetComponent<I_Observer_List>();
            Debug.Log("附上觀察者清單");
        }
    }


    private void Start()
    {
        InitialObjsPool();
    }

    //初始化

    public void InitialObjsPool()
    {
        for (int i = 0; i < InitialObjsCount; i++)
        {
          GameObject obj=  Instantiate(Obj);
          
          ObjPool.Enqueue(obj);
          obj.SetActive(false);
            if (ObeserberList != null)
            {
                ObeserberList.AddList(obj);
            }
        }
    }


    //取出

    public void Use_ObjPool_Obj(Vector3 Pos, Quaternion Rote, Transform Parent)
    {
        if (ObjPool.Count > 0)
        {
             GameObject o = ObjPool.Dequeue();
            o.transform.parent = Parent;
            o.transform.position = Pos;
            o.transform.rotation = Rote;
            if (ObeserberList != null)
            {
                ObeserberList.AddList(o);
            }
            o.SetActive(true);

        }
        else
        {
            GameObject obj = Instantiate(Obj);
            if (ObeserberList != null)
            {
                ObeserberList.AddList(obj);
            }
            ObjPool.Enqueue(obj);
        }
    }




    //回收

    public void Recovery_ObjPool_Obj(GameObject o)
    {
        ObjPool.Enqueue(o);
        o.SetActive(false);
    }

}
