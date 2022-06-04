using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YauctioneerAnimation : MonoBehaviour
{
    Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();

    }

    
    void Update()
    {

        bool isOffer = animator.GetBool("isOffer");
        bool keyPressed = Input.GetKey("e");

        if (!isOffer && keyPressed)
        {
            animator.SetBool("isOffer", true);
        }
        if (isOffer && !keyPressed)
        {
            animator.SetBool("isOffer", false);
        }
    }
}
