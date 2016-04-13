using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_NetworkSetup : NetworkBehaviour
{
    [SerializeField]
    Camera fpsCharacterCam;
    [SerializeField]
    AudioListener audioListener;

    private GameObject SceneCam;

    void Start()
    {
        if (isLocalPlayer)
        {
            SceneCam = GameObject.Find("Scene Camera");
            SceneCam.SetActive(false);
            fpsCharacterCam.enabled = true;
            //   GetComponent<CharacterController>().enabled = true;
            GetComponent<FirstPersonConroller>().enabled = true;
            audioListener.enabled = true;
        }
        transform.SetParent(GameObject.Find("World").transform);
    }
}
