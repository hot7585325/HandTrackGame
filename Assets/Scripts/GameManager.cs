using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance=this;
        }
    }

    private void Update()
    {
        CameraMove();
    }

    public void CameraMove()
    {
        Camera.main.transform.position += new Vector3(2 * Time.deltaTime, 0, 0);
    }

    public void GameOver()
    {
        SceneManager.LoadScene(0);
    }

}
