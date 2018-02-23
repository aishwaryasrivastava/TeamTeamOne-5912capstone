﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeAnimationHandler : MonoBehaviour {

    Animator anim;
    public bool attackLock;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        attackLock = false;
	}

    // Update is called once per frame
    void Update() {
    }
        public void ToWalking()
        {
            anim.SetBool("isAttacking", false);
            anim.SetBool("isIdle", false);
            anim.SetBool("isWalking", true);
        }

        public void ToIdle()
        {
            anim.SetBool("isAttacking", false);
            anim.SetBool("isIdle", true);
            anim.SetBool("isWalking", false);
        }

         

        public void ToAttacking()
        {
            anim.SetBool("isAttacking", true);
            anim.SetBool("isIdle", false);
            anim.SetBool("isWalking", false);
        }

    public void Attack()
    {
        if (!attackLock)
        {
            //attackLock = true;
            ToAttacking();
        }
    }

    public void ReleaseAttackLock()
    {
        attackLock = false;
    }
    
}
