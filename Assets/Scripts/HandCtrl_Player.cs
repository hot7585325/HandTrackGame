using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCtrl_Player : MonoBehaviour
{
    public HandTracker tracker;
    public Rigidbody2D body;
    public Animator animator;
    public int HP=3;

    private void Update()
    {
        Jump();
        TrackPos();
    }

 

    public void Hurt()
    {
        Debug.Log("Hurt");
        animator.SetTrigger("Hurt");
        HP -=1;
        UIManager.instance.UpdateHPUI();

        if( HP <= 0 )
        {
            GameManager.Instance.GameOver();
        }

    }

    
    public void Jump()
    {

            if (tracker.GetIsOpen())
            {
                //Debug.Log("¥´¶}");
                body.AddForce(Vector3.up*3);
        }
            else
            {
                //Debug.Log("´¤®±");
            }       
    }


    public void TrackPos()
    {
        transform.position = new Vector2(tracker.GetTrackPos().x,transform.position.y);
    }



}