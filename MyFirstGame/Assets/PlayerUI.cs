using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class PlayerUI : MonoBehaviour
    {
        #region Private Fields
        [Tooltip("UI Text to display Player's Name")]
        [SerializeField]
        private Text playerNameText;

        [Tooltip("UI Slider to display Player's health")]
        [SerializeField]
        private Slider playerHealthSlider;

        [Tooltip("Pixel offset from the player target")]
        [SerializeField]
        private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

        float characterControllerHeight = 0f;
        Transform targetTransform;
        Renderer targetRenderer;
        CanvasGroup _canvasGroup;
        Vector3 targetPosition;

        private PlayerManager target;
        #endregion

        private void Awake()
        {
            this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false); // find is apparently very slow

            _canvasGroup = this.GetComponent<CanvasGroup>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // player health
            if(playerHealthSlider != null)
            {
                playerHealthSlider.value = target.Health;
            }

            if(target == null)
            {
                // destroy, like if photon destroys bc a dc
                Destroy(this.gameObject);
                return;
            }
        }

        void LateUpdate()
        {
            // do not show ui if not visible to camera
            if (targetRenderer != null)
            {
                this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
            }

            // follow Target gameobject
            if (targetTransform != null)
            {
                targetPosition = targetTransform.position;
                targetPosition.y += characterControllerHeight;
                this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
                // worldtoscreenpoint matches a 2d and 3d pose
            }
        }

        #region Public Methods
        public void SetTarget(PlayerManager _target)
        {
            if(_target == null)
            {
                Debug.LogError("missing playmakermanager target for playerui.settarget", this);
                return;
            }

            // caches references for efficiency
            target = _target;
            targetTransform = this.target.GetComponent<Transform>();
            targetRenderer = this.target.GetComponent<Renderer>();
            CharacterController characterController = _target.GetComponent<CharacterController>();
            // get data from player that wont change during lifetime of component
            if (characterController != null)
            {
                characterControllerHeight = characterController.height; // player is basedd off of a CharacterController, which has height
            }

            if(playerNameText != null)
            {
                playerNameText.text = target.photonView.Owner.NickName;
            }
        }
        #endregion
    }
}