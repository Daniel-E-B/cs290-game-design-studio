using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.MyGame
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields
        [Tooltip("max # of players per room")]
        [SerializeField] // makes it show up in the unity inspector
        private byte maxPlayersPerRoom = 4;
        #endregion

        #region Private Fields

        string gameVersion = "1";
        bool isConnecting;

        #endregion

        #region Public Fields
        [Tooltip("the ui panel to enter username, connect, and play")]
        [SerializeField]
        private GameObject controlPanel;

        [Tooltip("the ui label to say connection in progress")]
        [SerializeField]
        private GameObject progressLabel;
        #endregion

        #region MonoBehavior Callbacks

        /// <summary>
        /// MonoBehavior method is called on GameObject by Unity during initialization
        /// </summary>
        void Awake()
        {
            // makes sure PhotonNetwork.LoadLevel() can be used on the master client and all clients in the same room sync their level
            // MasterClient can call PhotonNetwork.LoadLevel() and all connected players will automatically load that same level.
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            //Connect();
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// start the connection process
        /// if already connected, join a random room
        /// else, connect to photon
        /// </summary>
        /// 
        public void Connect()
        {
            Debug.Log("connect button pressed");
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        #endregion

        #region MonoBehaviorPunCallbacks Callbacks
        public override void OnConnectedToMaster()
        {
            Debug.Log("onconnectedtomaster called by pun");
            // we don't want to do anything if we are not attempting to join a room.
            // this case where isConnecting is false is typically when you lost or quit the game
            if (isConnecting)
            {
                 //first we try to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }

        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("on join random failed called. creating a room instead");
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom }); // null is the name
        }
        public override void OnJoinedRoom()
        {
            // load arena from lobby if we are the first player, otherwise rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log(" loading room for 1");
                PhotonNetwork.LoadLevel("Room for 1");
            }


        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarningFormat("ondisconnect called with cause {0}", cause);
        }
        #endregion
        // Update is called once per frame
        //void Update()
        //{

        //}
    }
}