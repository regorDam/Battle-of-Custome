using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FirstPersonConroller : NetworkBehaviour
{
    //Player Settings
    public float mouseSensitivityX = 250f;
    public float mouseSensitivityY = 250f;
    public float walkSpeed = 8f;
    public float runSpeed = 14f;
    public float jumpForce = 220;
    public bool godMode;
    public float godForceMax = 5;
    public LayerMask groundedMask;
    public Transform bulletSpawn;
    //Connections
    Transform cameraT;
    Rigidbody rgb;
    public KnightHashIDs knightHash;
    public Animator anim;

    //Helpers
    float verticalLookRotation;
    float godForceMin = 1;
    float godForce;
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    bool grounded;
    bool dead;
    bool m_focus = true;
    string nameknight = "knight_FBX_Walk";
    string namedracon = "Dragon_FBX_WALK";
    string nameObj;
    int type;
    public GameObject bodyK, bodyD;
    GameObject body;

    void Awake()
    {


        if (GameObject.FindGameObjectsWithTag("Player").Length % 2 == 0)
        {
            type = 1;
            nameObj = nameknight;
            transform.Find(namedracon).gameObject.SetActive(false);
            //body = Instantiate(bodyK, bodyK.transform.position, bodyK.transform.rotation) as GameObject;

            //body.transform.localPosition = bodyK.transform.localPosition;
            //body.transform.localRotation = bodyK.transform.localRotation;
        }
        else
        {
            type = 2;
            nameObj = namedracon;
            //body = Instantiate(bodyD, bodyD.transform.position, bodyD.transform.rotation) as GameObject;
            transform.Find(nameknight).gameObject.SetActive(false);
        }

      
        //body = Instantiate(Resources.Load("Prefabs/Player/" + nameObj, typeof(GameObject))) as GameObject;
        //body.transform.SetParent(transform);

        anim = transform.Find(nameObj).GetComponent<Animator>();
        GetComponent<NetworkAnimator>().animator = anim;
        knightHash = new KnightHashIDs(anim);
    }
    void Start ()
    {
        cameraT = transform.Find("Main Camera").transform;
        rgb = GetComponent<Rigidbody>();
        //body.transform.localPosition = new Vector3(0.0f, -1.0f, -0.03f);
       // body.transform.rotation = transform.rotation;
       /*
        if(type == 1)
        {
            body.transform.localRotation = bodyK.transform.localRotation;
        }
        else
        {
            body.transform.localRotation = bodyD.transform.localRotation;
        }
        */
    }
	
	void Update ()
    {
        if (NetworkClient.active)
        {
            if (!m_focus)
                return;
            ReadInputs();
        }
    }

    void OnApplicationFocus(bool value)
    {
        m_focus = value;

    }

    void ReadInputs()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        
        //Vector3 rotateDir = Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivityX;
        transform.Rotate( Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivityX);
       
        verticalLookRotation += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivityY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
        cameraT.localEulerAngles = Vector3.left * verticalLookRotation;

        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        if(moveDir != Vector3.zero)
        {
            //RUN CALLANIMATOR
            anim.SetFloat(knightHash.walk, 1f);
        }
        else
            anim.SetFloat(knightHash.walk, 0.0f);

        Vector3 targetMoveAmount;

        if (Input.GetKeyDown(KeyCode.P))
            godMode = !godMode;

        if (godMode)
            godForce = godForceMax;
        else
            godForce = godForceMin;


        if ((Input.GetKey("left shift") || Input.GetKey("right shift")))
        {
            targetMoveAmount = moveDir * (runSpeed * godForce);
            //RUN CALLANIMATOR
            anim.SetBool(knightHash.run, true);
        }
        else
        {
            anim.SetBool(knightHash.run, false);
            targetMoveAmount = moveDir * (walkSpeed + godForce);
        }
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);

        if (Input.GetButtonDown("Jump"))
        {
            if (grounded)
            {
                rgb.AddForce(transform.up * (jumpForce * godForce));
            }

        }

        if (Input.GetButtonDown("Fire1"))
        {
            //FIRE CALLANIMATOR
            anim.SetTrigger(knightHash.attack);
            CmdFire();
        }


        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1 + .1f, groundedMask))
        {
            grounded = true;
        }
        else
        {
            //grounded = false;
        }
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if(collisionInfo.gameObject.layer == 8)
        {
            grounded = true;
        }
    }
    void OnCollisionExit()
    {
        grounded = false;
    }

    void FixedUpdate()
    {
        rgb.MovePosition(rgb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    [Command]
    void CmdFire()
    {
        GameObject bullet;
        bullet = (GameObject)Instantiate(Resources.Load("Prefabs/Bullet", typeof(GameObject)), bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<Bullet>().Config(gameObject, 2);
        //bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bullet.GetComponent<Bullet>().speed;
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * bullet.GetComponent<Bullet>().speed);
        if (!GetComponentInChildren<AudioSource>().isPlaying)
        {
            GetComponentInChildren<AudioSource>().Play();
        }

        NetworkServer.Spawn(bullet);

    }



    public class KnightHashIDs
    {
        public int die;
        public int attack;
        public int run;
        public int walk;

        public KnightHashIDs(Animator refAnim)
        {
            die = Animator.StringToHash("Die");
            attack = Animator.StringToHash("Attack");
            run = Animator.StringToHash("Run");
            walk = Animator.StringToHash("WalkSpeed");
        }
    }
}
