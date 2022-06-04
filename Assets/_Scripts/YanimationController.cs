using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YanimationController : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        bool isAsking = animator.GetBool("isAsking");
        bool keyPressed = Input.GetKey("a");

        if (!isAsking && keyPressed)
        {
            animator.SetBool("isAsking", true);
        }
        if (isAsking && !keyPressed)
        {
            animator.SetBool("isAsking", false);
        }
    }
}
