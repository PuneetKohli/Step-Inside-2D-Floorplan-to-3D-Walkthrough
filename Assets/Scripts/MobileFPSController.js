#pragma strict
 
var cameraPivot : Transform;
 
var speed : float = 4;
 
private var thisTransform : Transform;
private var character : CharacterController;
private var cameraVelocity : Vector3;
private var velocity : Vector3;
 
var movementOriginX : float;
var movementOriginY : float;
 
function Start ()
{
    thisTransform = GetComponent(Transform);
    character = GetComponent(CharacterController);
    originalRotation = transform.rotation.eulerAngles.y;
    //moveOutline = new GameObject();
    movePad = new GameObject();
    moveOutline = new GameObject();
    movePad.transform.position = new Vector2(-1,-1);
    moveOutline.transform.position = new Vector2(-1,-1);
}
 
function Update ()
{
    var moveDiff : Vector2;
    for (var touch : Touch in Input.touches)
    {
        if (touch.phase == TouchPhase.Began)
        {
			 if (touch.position.x < Screen.width / 2)
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
   
    rotationX += rightFingerInput.x * 40;
    //rotationY += rightFingerInput.y * 25;
    //rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
    transform.rotation =  Quaternion.Slerp (transform.rotation, Quaternion.Euler(0, originalRotation + rotationX, 0),  0.1);
    cameraPivot.localRotation =  Quaternion.Slerp (cameraPivot.localRotation, Quaternion.Euler(cameraPivot.localRotation.x-rotationY, 0, 0),  0.1);
   
    moveDirection = thisTransform.TransformDirection(new Vector3(leftFingerInput.x, 0, leftFingerInput.y));
    moveDirection *= speed;
    moveDirection += Physics.gravity;
   
    character.Move(moveDirection * Time.smoothDeltaTime);
}
 
var rightFingerID;
var leftFingerID;
var leftFingerCenter : Vector2;
var mDiff : Vector2;
var moveStickDiff = 100;
var leftFingerInput : Vector2;
var rightFingerInput : Vector2;
 
public var moveOutline : GameObject;
public var movePad : GameObject;
 
var rotationX : float;
var rotationY : float;
var minimumY = -20;
var maximumY = 20;
 
var originalRotation : float;
var moveDirection : Vector3;