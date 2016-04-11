#pragma strict
 
var cameraPivot : Transform;
 
var speed : float = 4;
 
private var thisTransform : Transform;
private var character : CharacterController;
private var cameraVelocity : Vector3;
private var velocity : Vector3;
private var canJump = true;
 
var movementOriginX : float;
var movementOriginY : float;
 
function Start ()
{
    thisTransform = GetComponent(Transform);
    character = GetComponent(CharacterController);
    originalRotation = transform.rotation.eulerAngles.y;
    movePad.transform.position = new Vector2(-1,-1);
    moveOutline.transform.position = new Vector2(-1,-1);
    jump = false;
    doubleJump = false;
}
 
function Update ()
{
    var moveDiff : Vector2;
    for (var touch : Touch in Input.touches)
    {
        if (touch.phase == TouchPhase.Began)
        {
            if (jumpButton.HitTest(touch.position))
            {
                jump = true;
            }
            else if (touch.position.x < Screen.width / 2)
            {
                leftFingerID = touch.fingerId;
                leftFingerCenter = touch.position;
                moveOutline.transform.position.x = touch.position.x / Screen.width;
                moveOutline.transform.position.y = touch.position.y / Screen.height;
                movePad.transform.position.x = touch.position.x / Screen.width;
                movePad.transform.position.y = touch.position.y / Screen.height;
            }
            else
            {
                rightFingerID = touch.fingerId;
            }
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            if (touch.position.x < Screen.width / 2)
            {
                if (leftFingerID == touch.fingerId)
                {
                    mDiff = touch.position - leftFingerCenter;
                    var distPer = mDiff.magnitude * 100 / moveStickDiff;
                    if (distPer > 100)
                    {
                        distPer = 100;
                    }
                    leftFingerInput = mDiff.normalized * distPer / 100;
                   
                    movePad.transform.position.x = leftFingerCenter.x / Screen.width + mDiff.normalized.x * distPer / 100 * moveStickDiff / Screen.width;
                    movePad.transform.position.y = leftFingerCenter.y / Screen.height + mDiff.normalized.y * distPer / 100 * moveStickDiff / Screen.height;
                }
            }
            else
            {
                if (rightFingerID == touch.fingerId)
                {
                    rightFingerInput = touch.deltaPosition * Time.smoothDeltaTime;
                }
            }
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            if (touch.fingerId == leftFingerID)
            {
                movePad.transform.position = new Vector2(-1,-1);
                moveOutline.transform.position = new Vector2(-1,-1);
                leftFingerID = -1;
                leftFingerInput = new Vector2(0, 0);
            }
            if (touch.fingerId == rightFingerID)
            {
                rightFingerID = -1;
                rightFingerInput = new Vector2(0, 0);
            }
        }
    }
   
    rotationX += rightFingerInput.x * 25;
    rotationY += rightFingerInput.y * 25;
    rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
    transform.rotation =  Quaternion.Slerp (transform.rotation, Quaternion.Euler(0, originalRotation + rotationX, 0),  0.1);
    cameraPivot.localRotation =  Quaternion.Slerp (cameraPivot.localRotation, Quaternion.Euler(cameraPivot.localRotation.x-rotationY, 0, 0),  0.1);
   
    moveDirection = thisTransform.TransformDirection(new Vector3(leftFingerInput.x, 0, leftFingerInput.y));
    moveDirection *= speed;
    moveDirection += Physics.gravity;
   
    if (character.isGrounded)
    {
        doubleJump = false;
        if (jump || jumpingEnabled)
        {
            velocityJ = character.velocity / 3;
            velocityJ.y = jumpSpeed;
        }
        else
        {
            velocityJ = new Vector3(0, 0, 0);
        }
    }
    else
    {
        if (!doubleJump || jump || doubleJumpingEnabled)
        {
            velocityJ = character.velocity / 3;
            velocityJ.y = jumpSpeed;
            doubleJump = true;
        }
        velocityJ.y += Physics.gravity.y * Time.smoothDeltaTime;
    }
   
    moveDirection += velocityJ;
   
    character.Move(moveDirection * Time.smoothDeltaTime);
    jump = false;
}
 
var rightFingerID;
var leftFingerID;
var leftFingerCenter : Vector2;
var mDiff : Vector2;
var moveStickDiff = 100;
var leftFingerInput : Vector2;
var rightFingerInput : Vector2;
 
var moveOutline : GUITexture;
var movePad : GUITexture;
var jumpButton : GUITexture;
 
var rotationX : float;
var rotationY : float;
var minimumY = -20;
var maximumY = 20;
 
var originalRotation : float;
var moveDirection : Vector3;
var jump : boolean;
var doubleJump : boolean;
 
var jumpSpeed : float = 25;
var velocityJ : Vector3;
var doubleJumpingEnabled : boolean = true;
var jumpingEnabled : boolean = true;