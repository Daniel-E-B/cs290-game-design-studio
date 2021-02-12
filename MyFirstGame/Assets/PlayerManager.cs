using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Com.MyCompany.MyGame
{
    public class PlayerManager : MonoBehaviourPun, IPunObservable
    {
        #region Private Fields
        [Tooltip("The Beams GameObject to control")]
        [SerializeField]
        private GameObject beams;

        [Tooltip("The current Health of our player")]
        public float Health = 1f;

        [Tooltip("The local player instance. Use to know if the local player is in the scene")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("the player's ui gameobject prefab")]
        [SerializeField]
        public GameObject PlayerUIPrefab;

        // true when the user is firing
        bool IsFiring;
        #endregion

        #region MonoBehaviour CallBacks

        #if !UNITY_5_4_OR_NEWER
        // see CalledOnLevelWasLoaded. Outdated in Unity 5.4
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
        #endif


        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f)) // if there is nothing below
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }

            GameObject _uiGo = Instantiate(this.PlayerUIPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

        #if UNITY_5_4_OR_NEWER
        public void OnDisable() // supposed to say override but doesnt exist
        {
            // Always call the base to remove callbacks
            //base.OnDisable(); this just wasnt there maybe the tutorial is old
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        #endif

        //init
        private void Awake()
        {
            if (beams == null)
            {
                Debug.LogError("<Color=Red><a>missing a beams reference</a></Color>", this);
            }
            else
            {
                beams.SetActive(false);
            }

            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }

            // survive level synchronization
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

            if(_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
                else
                {
                    Debug.LogError("missing camerawork component on player prefab\nits dumb to log this here because it will error if there is more than one player when everything is working\nthat being said it has helped fix bugs");
                }
            }
            
            #if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            #endif

            if(PlayerUIPrefab != null)
            {
                GameObject _uiGo = Instantiate(PlayerUIPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("missing playeruiprefab reference on player prefab", this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine) // sus that this is in here twice TODO see if removing one breaks
            {
                ProcessInputs();
            }

            // trigger beams active state
            if(beams != null && IsFiring != beams.activeInHierarchy)
            {
                beams.SetActive(IsFiring);
            }

            if (photonView.IsMine || !PhotonNetwork.IsConnected)
            {
                this.ProcessInputs();
                if (Health <= 0f)
                {
                    GameManager.Instance.LeaveRoom();
                }
            }
        }
        #endregion

        #region Custom
        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                //Debug.Log("firing");
                if (!IsFiring)
                {
                    IsFiring = true;
                }
            }
            if (Input.GetButtonUp("Fire1"))
            {
                if (IsFiring)
                {
                    IsFiring = false;
                }
            }
        }

        // TODO: these are not working but health does work
        // called when collider 'other' enters the trigger
        private void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }
            if (!other.name.Contains("Beam"))
            {
                return;
            }
            Health -= 0.1f;

        }

        private void OnTriggerStay(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }
            if (!other.name.Contains("Beam"))
            {
                return;
            }
            // slowly remove health while beam hitting
            Health -= 0.1f * Time.deltaTime;

        }
        #endregion
        

        #region IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // send the other players our data (we own this player)
                stream.SendNext(IsFiring);
                stream.SendNext(Health);
            }
            else
            {
                // network player, receive data (this class is the same whether its us or not us
                this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
            }
        }
        #endregion

        #if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
        #endif
    }
}