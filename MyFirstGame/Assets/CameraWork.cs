using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MyCompany.MyGame { 
public class CameraWork : MonoBehaviour
{

    #region Private Fields


    [Tooltip("The distance in the local x-z plane to the target")] // (y is up)
    [SerializeField]
    private float distance = 7.0f;


    [Tooltip("The height we want the camera to be above the target")]
    [SerializeField]
    private float height = 3.0f;


    [Tooltip("Allow the camera to be offseted vertically from the target, for example giving more view of the scenerey and less ground.")] // uh how is this different than the last one
    [SerializeField]
    private Vector3 centerOffset = Vector3.zero;


    [Tooltip("Set this as false if a component of a prefab being instantiated by Photon Network, and manually call OnStartFollowing() when and if needed.")]
    [SerializeField]
    private bool followOnStart = false; // true for offline testing. 
    // dont want the camera to jump between players in a multiplayer environment
    // control CameraWork by turning it off and on depending on wether the player it has to follow is the local player or not
    // networked: call OnStartFollowing() when the player is a local player. Do this in PlayerManager


    [Tooltip("The Smoothing for the camera to follow the target")]
    [SerializeField]
    private float smoothSpeed = 0.125f;


    // cached transform of the target
    Transform cameraTransform;


    // maintain a flag internally to reconnect if target is lost or camera is switched
    bool isFollowing;


    // Cache for camera offset
    Vector3 cameraOffset = Vector3.zero;


    #endregion


    #region MonoBehaviour Callbacks

    // Start is called before the first frame update
    void Start()
    {
        if (followOnStart)
        {
            OnStartFollowing();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate() // apparently called after all Update()s have been called
    {
        if(cameraTransform == null && isFollowing)
        {
            OnStartFollowing();
        }

        if (isFollowing)
        {
            Follow();
        }
    }

    #endregion

    #region Public Methods
    public void OnStartFollowing()
    {
        cameraTransform = Camera.main.transform;
        isFollowing = true;
        // no smoothing
        Follow();
        //Cut();
    }
    #endregion

    #region Private Methods
    // follow smoothly
    void Follow()
    {
        cameraOffset.z = -distance;
        cameraOffset.y = height;

        //cameraTransform.position = Vector3.Lerp(cameraTransform.position, this.transform.position + this.transform.TransformVector(cameraOffset), smoothSpeed * Time.deltaTime);
        // lerp = linear interpolation
        // I want this to look 3ps-y so the above is bad and I ended up just using the same code as cut so I just removed cut


        cameraTransform.position = this.transform.position + this.transform.TransformVector(cameraOffset);


        cameraTransform.LookAt(this.transform.position + centerOffset);
    }

    //void Cut()
    //{
    //    cameraOffset.z = -distance;
    //    cameraOffset.y = height;

    //    cameraTransform.position = this.transform.position + this.transform.TransformVector(cameraOffset);

    //    cameraTransform.LookAt(this.transform.position + centerOffset);
    //}
    #endregion
}
}