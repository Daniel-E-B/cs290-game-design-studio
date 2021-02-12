using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {
        #region Private Fields
        [SerializeField]
        private float directionDampTime = 0f;//0.25f;
        #endregion

        #region monobehavior callbacks
        private Animator animator;
        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            if (!animator)
            {
                Debug.LogError("playeranimatormanager missing animator component", this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(photonView.IsMine == false && PhotonNetwork.IsConnected == true) // if its not connected we still want to be able to test
            {
                return;
            }

            if (!animator)
            {
                return;
            }
            // jumpinng stuff
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            // can only jump if running
            if(stateInfo.IsName("Base Layer.Run"))
            {
                // trigger param
                if (Input.GetButtonDown("Fire2"))
                {
                    animator.SetTrigger("Jump");
                }
            }


            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            //if (v < 0)
            //{
            //    v = 0;
            //}
            animator.SetFloat("Speed", v); // the guy doesnt move backwards anyway (used to be h*h + v*v) for moving in turns and smoothing
            //animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime); // could increase damp time but i dont like it
            animator.SetFloat("Direction", h);
        }
        #endregion
    }
}