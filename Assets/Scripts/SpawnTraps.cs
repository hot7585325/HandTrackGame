using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTraps : MonoBehaviour,I_Observer_List
{
    public float Space;
    public Vector2 SpawnPosRange;
    public List<GameObject> SpikeList;
    public ObjectsPool SpikePool;

    private void Start()
    {
        Initial();
    }


    private void Update()
    {
        DistanceSpawn();
    }


    public void Initial()
    {
        for (int i = 0; i < 10; i++)
        {
            SpikePool.Use_ObjPool_Obj(transform.position * i*2, Quaternion.identity, null);
        }
    }

    public void DistanceSpawn()
    {
       if( Camera.main.transform.position.x> SpikeList[SpikeList.Count- 2].transform.position.x)
        {
            SpikePool.Use_ObjPool_Obj(RandomPos(),Quaternion.identity,null);
        }
    }

    public Vector3 RandomPos()
    {
        float y = Random.RandomRange(SpawnPosRange.x, SpawnPosRange.y);
        return new Vector3(Camera.main.transform.position.x + Space, y, 0);
    }

    public void AddList(GameObject Obj)
    {
        SpikeList.Add(Obj);
    }

    public void RemoveList(GameObject Obj)
    {
        SpikeList.Remove(Obj);
    }
}
