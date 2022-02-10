using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class player : MonoBehaviour
{
    public bool isGrounded;
    public bool isSprinting;


    private Transform cam;
    private World world;
    

    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpForce = 10f;
    public float gravity = -10f;

    public float playerWidth = 0.4f;
    public float boundsTolerance = 0.1f;

    public float playerHeight = 1.5f;

    public float checkIncrement = 0.1f;
    public float reach = 8f;



    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;
    private float verticalMomentum = 0;
    private bool jumpRequest;

    public Transform HighlightBlock;
    public Transform PlaceHighlightBlock;

    public Text selectedBlockText;
    public byte selectedBlockIndex;

    private void Start() {


        cam = GameObject.Find("Main Camera").transform;
        world = GameObject.Find("World").GetComponent<World>();
        Cursor.lockState = CursorLockMode.Locked;
        selectedBlockText.text = world.blocktypes[selectedBlockIndex].blockName + " block selected";

    }
    private void FixedUpdate() {
         mouseUpdate();

       

        calculateVelocity();
        if (jumpRequest)
            Jump();

        transform.Rotate(Vector3.up * mouseHorizontal);
        cam.Rotate(Vector3.right * -mouseVertical);
        transform.Translate(velocity, Space.World);
        

    }

    private void Update() {

        //mouseUpdate();
        getPlayerInputs();
        placeCursorBlock();
    }

    void Jump() {

        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;

    }



    



    private void calculateVelocity() {

        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;

        if (isSprinting)
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
        else
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;

        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        if ((velocity.z > 0 && front) || (velocity.z < 0 && back))
            velocity.z = 0;
        if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
            velocity.x = 0;

        if (velocity.y < 0)
            velocity.y = checkDownSpeed(velocity.y);
        else if (velocity.y > 0)
            velocity.y = checkUpSpeed(velocity.y);
    }
    
    
    private void mouseUpdate() {



        if (cam.transform.localEulerAngles.x > 90 && cam.transform.localEulerAngles.x < 270)
            mouseVertical = - Input.GetAxis("Mouse Y");
        else mouseVertical = Input.GetAxis("Mouse Y");
       // if (cam.transform.localEulerAngles.x < 90)

            // else if (cam.transform.localEulerAngles.x > -90)






            mouseHorizontal = Input.GetAxis("Mouse X");




    }

    private void getPlayerInputs() {

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Sprint"))
            isSprinting = true;
        if (Input.GetButtonUp("Sprint"))
            isSprinting = false;

        if (isGrounded && Input.GetButtonDown("Jump"))
            jumpRequest = true;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0) {

            if (scroll > 0)
                selectedBlockIndex++;
            else
                selectedBlockIndex--;

            if (selectedBlockIndex > (byte)(world.blocktypes.Length - 1))
                selectedBlockIndex = 1;
            if (selectedBlockIndex < 1)
                selectedBlockIndex = (byte)(world.blocktypes.Length - 1);

            selectedBlockText.text = world.blocktypes[selectedBlockIndex].blockName + " block selected";
        }

        if (HighlightBlock.gameObject.activeSelf) {

            if (Input.GetMouseButtonDown(0))
            {
                
                world.getChunkFromVector3(new Vector3(HighlightBlock.position.x + 0.02f, HighlightBlock.position.y + 0.02f, HighlightBlock.position.z + 0.02f)).editVoxel(new Vector3(HighlightBlock.position.x+0.02f, HighlightBlock.position.y + 0.02f, HighlightBlock.position.z + 0.02f), 0);
                
            
            }
                if (Input.GetMouseButtonDown(1))
                    if (
                    PlaceHighlightBlock.position != new Vector3(Mathf.FloorToInt(cam.position.x + playerWidth), Mathf.FloorToInt(cam.position.y), Mathf.FloorToInt(cam.position.z + playerWidth)) &&
                    PlaceHighlightBlock.position != new Vector3(Mathf.FloorToInt(cam.position.x + playerWidth), Mathf.FloorToInt(cam.position.y), Mathf.FloorToInt(cam.position.z - playerWidth)) &&
                    PlaceHighlightBlock.position != new Vector3(Mathf.FloorToInt(cam.position.x - playerWidth), Mathf.FloorToInt(cam.position.y), Mathf.FloorToInt(cam.position.z - playerWidth)) &&
                    PlaceHighlightBlock.position != new Vector3(Mathf.FloorToInt(cam.position.x - playerWidth), Mathf.FloorToInt(cam.position.y), Mathf.FloorToInt(cam.position.z + playerWidth)) &&
                    PlaceHighlightBlock.position != new Vector3(Mathf.FloorToInt(cam.position.x), Mathf.FloorToInt(cam.position.y), Mathf.FloorToInt(cam.position.z))&&

                    PlaceHighlightBlock.position != new Vector3(Mathf.FloorToInt(cam.position.x + playerWidth), Mathf.FloorToInt(cam.position.y -1f), Mathf.FloorToInt(cam.position.z + playerWidth)) &&
                    PlaceHighlightBlock.position != new Vector3(Mathf.FloorToInt(cam.position.x + playerWidth), Mathf.FloorToInt(cam.position.y -1f), Mathf.FloorToInt(cam.position.z - playerWidth)) &&
                    PlaceHighlightBlock.position != new Vector3(Mathf.FloorToInt(cam.position.x - playerWidth), Mathf.FloorToInt(cam.position.y - 1f), Mathf.FloorToInt(cam.position.z - playerWidth)) &&
                    PlaceHighlightBlock.position != new Vector3(Mathf.FloorToInt(cam.position.x - playerWidth), Mathf.FloorToInt(cam.position.y - 1f), Mathf.FloorToInt(cam.position.z + playerWidth)) &&
                    PlaceHighlightBlock.position != new Vector3(Mathf.FloorToInt(cam.position.x), Mathf.FloorToInt(cam.position.y - 1f), Mathf.FloorToInt(cam.position.z))




                    )
                    world.getChunkFromVector3(PlaceHighlightBlock.position).editVoxel(PlaceHighlightBlock.position, selectedBlockIndex);
        }

    }
    

    private void placeCursorBlock() {


        float step = checkIncrement;
        Vector3 lastPos = new Vector3();

        while (step < reach) {

            Vector3 pos = cam.position + (cam.forward * step);

            if (world.CheckForVoxel(pos)) {
                HighlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x)-0.01f, Mathf.FloorToInt(pos.y) - 0.01f, Mathf.FloorToInt(pos.z) - 0.01f);                PlaceHighlightBlock.position = lastPos;

                HighlightBlock.gameObject.SetActive(true);
               //PlaceHighlightBlock.gameObject.SetActive(true);

                return;

                    }

            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
            step += checkIncrement;
        }


        HighlightBlock.gameObject.SetActive(false);
       // PlaceHighlightBlock.gameObject.SetActive(false);

    }


    private float checkDownSpeed(float downSpeed) {


        if (

            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth))
            )
        {
            isGrounded = true;
            return 0;


        }
        else {
            isGrounded = false;
            return downSpeed;
                
                }
    
    }



   
    
    
    
    private float checkUpSpeed(float upSpeed)
    {


        if (

            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y+ 2f + upSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 2f+ upSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y +2f+ upSpeed, transform.position.z + playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y +2f+ upSpeed, transform.position.z + playerWidth))
            )
        {

            return 0;


        }
        else
        {

            return upSpeed;

        }

    }

    public bool front {
        get {
        
            if(
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y +1f, transform.position.z + playerWidth)) 
                )

                return true;
            else
                return false;
        }
    }

    public bool back
    {
        get
        {

            if (
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth))
                )

                return true;
            else
                return false;
        }
    }

    public bool left
    {
        get
        {

            if (
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z )) ||
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z ))
                )

                return true;
            else
                return false;
        }
    }

    public bool right
    {
        get
        {

            if (
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z)) ||
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z))
                )

                return true;
            else
                return false;
        }
    }




}
