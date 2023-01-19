using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using UnityEngine.SceneManagement;
public class PlayerManager : MonoBehaviourPunCallbacks,IPunObservable
{
    [SerializeField]
    private GameObject beams;
    bool isFiring;
    public float Health = 1f;
    public GameObject PlayerUiPrefab; 
    public static GameObject LocalPlayerInstance;

    private void Awake()
    {
        if (beams == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
        }
        else
        {
            beams.SetActive(false);
        }
        if (photonView.IsMine)
        {
            PlayerManager.LocalPlayerInstance = this.gameObject;
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();
        if(_cameraWork!=null){
            if (photonView.IsMine)
            {
                _cameraWork.OnStartFollowing();
            }
        }
        else{
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        if(PlayerUiPrefab!=null)
        {
            GameObject _uiGo = Instantiate(PlayerUiPrefab);
            _uiGo.SendMessage("SetTarget",this,SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine){
            return;
        }
        // We are only interested in Beamers
        // we should be using tags but for the sake of distribution, let's simply check by name.
        if (!other.name.Contains("BeamLeft")){
            return;
        }
        Health -= 0.1f;

    }
    void OnTriggerStay(Collider other)
    {
        // we dont' do anything if we are not the local player.
        if (!photonView.IsMine)
        {
            return;
        }
        // We are only interested in Beamers
        // we should be using tags but for the sake of distribution, let's simply check by name.
        if (!other.name.Contains("Beam"))
        {
            // we slowly affect health when beam is constantly hitting us, so player has to move to prevent death.
            return;
        }
        Health -= 0.3f * Time.deltaTime;

    }

    void OnLevelWasLoaded(int level)
    {
        this.CalledOnLevelWasLoaded(level);
    }
    

    void CalledOnLevelWasLoaded(int level)
    {
        // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }
        GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
        _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }


    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInput();
        }

        if(beams!=null && isFiring!=beams.activeInHierarchy)
        {
            beams.SetActive(isFiring);
        }
        if(Health<=0)
        {
            
            GameManager.Instance.LeaveRoom();
            Destroy(this);
        }
    }

    void ProcessInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isFiring)
            {
                isFiring = true;
            }
        }
        if (Input.GetButtonUp("Fire1"))
        {
            if (isFiring)
            {
                isFiring = false;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(isFiring);
            stream.SendNext(Health);
        }
        else{
            this.isFiring = (bool)stream.ReceiveNext();
            this.Health = (float)stream.ReceiveNext();
        }

    }
    public override void OnDisable()
    {
        // Always call the base to remove callbacks
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene,LoadSceneMode loadingMode)
    {
        this.CalledOnLevelWasLoaded(scene.buildIndex);
    }

}
