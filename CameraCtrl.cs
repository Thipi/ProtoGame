using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//written by: Tipi
public class CameraCtrl : MonoBehaviour {

//This cameraScript is superdooper fancy. I totally overdid this...

        //NOTE: CREATE AN EMPTY GAMEOBJECT: ProtoLookAt, Attach it to Protos
        //head and assing it to be the ProtoLookAt for CameraCtrl
        //
        //we desperately need a project tool for this...

    public static CameraCtrl Instance; //Completely unnecessary reference to this script.

    public Transform ProtoLookAt; //Hmm... where is Proto? Ah, there he is.

    //Floats needed in Start() to verify distance and set start dist.
    //
    public float Distance = 8f; //Initial Distance from Proto
    public float DistMin = 3f; //Near Limit < Distance to clamp Camera
    public float DistMax = 10f; //Far Limit > Distance to clamp Camera

    private float startDistance = 0f; 
    private float desiredDist = 0f;

    //Handling
    //
    private float Horizontal = 0f; // mouseX/InputMan.RightHorizontal
    private float Vertical = 0f; //mouseY/InputMan.RightVertical
    InputMan InputManager;

    //Handling: Sensitivity values to be multiplied by inputs
    //publics:x_Sensitivity
    //        y_Sensitivity
    //        mouseWheel_Sensitivity (NOT ADDED TO INPUTMAN YET!!)
    public float xSensitivity = 5f;
    public float ySensitivity = 5f;
    public float MWSensitivity = 5f;

    // Defining limits to Y-rotation
    //         Y_MinLimit
    //         Y_MaxLimit
    public float YMin = -25f;
    public float YMax = 90f;

    //Smoothing camera movement and calculating distance. 
    //
    public float DistSmooth = 0.05f; //Transition time in seconds. Duration of smooth. between start Dist. and desired Dist.
    float velocityDist = 0f;  //Velocity distance stores point along the smoothing curve. Updates constantly.
    Vector3 desiredPosition = Vector3.zero;

    
    //For positioning the camera.
    // 
    public float xSmooth = 0.05f;//smoothing for x orbit (x+z)
    public float ySmooth = 0.1f; //smoothing for y axis
    float velX = 0f;        //velocity on X axis
    float velY = 0f;       //velocity on Y
    float velZ = 0f;       //vel. on Z
    Vector3 position = Vector3.zero;

    //Collision/Occlusion/Shearing Handling
    public float OcclusionDistance = 0.5f;//distance of ifOcc-step that camera is brought forward if occluded
    public int MaxOccChecks = 10; //max number times iterated before forcing cam
    //Collision/occlusion/shearing reset distance
    public float DistResumeSmooth = 1f;
    float distSmoothing = 0f;
    float preOccludedDist = 0f; //This is the distance before occlusion.


    void Awake()
    {
        Instance = this; //another pointless reference.
    }

    void Start() 
    //verifying distance, setting start distance.
    {
        //Calling InputManager
        InputManager = GetComponent<InputMan>();

        //Distance needs to be between min and max dist-values.
        Distance = Mathf.Clamp(Distance, DistMin, DistMax);

        //StartDistance(Validate D in awake and store it.)
        startDistance = Distance;

        //Calling Reset() method.
        Reset();
    }

    void LateUpdate()
    //Happens last. Check ProtoLookAt isn't null. 
    {
        //Look for ProtoLookAt and make sure it is not null.
        //If null, DO NOTHING!
        if(ProtoLookAt == null)
        {
            return;
        }

        //Call HandleInput(), process mouse/joystick input
        HandleInput();


        var countingLoop = 0;
        do
        {
            //Call for the method that calculates the desired position
            //Where is camera going?
            DesiredPosition();

            countingLoop++; //adding 1 to loopCounting

        } while (CheckIfOccluded(countingLoop)); //after loop camera should be free.



        //Call for Update Position, smoothly move from position to
        //desired position and look at ProtoLookAt.
        UpdatePosition();
    }

    void HandleInput()
    //Looking for mouse/joystick input and process it.
    {
        //Creating variable for DeadZone, so bumping doesn't move camera.
        // There is a tiny zone on mousewheel - + axis that doesn't register
        //movement

        var DeadZone = 0.01f;

        //Getting the x-axis data from mouse/RightHorizontal
        // multiply with x_sensitivity += horizontal input.
        Horizontal += InputMan.RightHorizontal() * xSensitivity;

        //Get y-axis input data * y_sensitivity -= vertical input.
        Vertical -= InputMan.RightVertical() * ySensitivity;

        //X is unlimited (orbiting around Proto)

        //LIMITING VERTICAL
        //Y limits: Clamping distance between DistMin and DistMax.
        //HelperClass(Helper.cs): ClampAngle(angle, DMin, DMax)
        //Helper converts whatever rotation sended to 0-360 either + or -
        //Makes sure it is clamped and returns it to camera.

        Vertical = Helper.ClampAngle(Vertical, YMin, YMax);

        if(Input.GetAxis("Mouse ScrollWheel")< -DeadZone || Input.GetAxis("Mouse ScrollWheel") > DeadZone)
        {
            desiredDist = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * MWSensitivity, DistMin, DistMax);
            preOccludedDist = desiredDist; //matching these two together
            distSmoothing = DistSmooth;
        }


        //mouseWheel. Checking camera is outside DeadZone
        //Applying mouseWheel-sensitivity
        //Subtract the value from current distance.

        //Clamped Distance (minmax) stored in desiredDistance

    }

    // POSITIONING THE CAMERA IN TWO METHODS. Telling where the camera should
    // move and after that we will actually move it. We separate these to make
    //movement smoothly.

    void DesiredPosition()
            //rotationX, rotationY, Distance(from Proto in orbit)
    //Úse data from HandleInput() to calculate the desired position.
    //Smoothing:
            // Position here means the location on orbital ring
            // Distance is the radius(ympyran sade) of the orbital ring.
            //Smoothing Distance transition and orbital transition.

    {
        ResetDesiredDist();
        //Process smoothing for distance.
        //Solving Distance >> use to solve desired position
        Distance = Mathf.SmoothDamp(Distance, desiredDist, ref velocityDist, distSmoothing);

        //Calculating the desired position
        desiredPosition = CalculatePosition(Vertical, Horizontal, Distance);

    }

        Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
    //returns Vector3(position)<< this is used to get the desiredPosition
    {
        Vector3 direction = new Vector3(0, 0, -Distance);
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        return ProtoLookAt.position + rotation * direction;

        //So... direction * rotation + offset of target = desiredPosition(Vector3)

    }

    //Checking here from and to the "pyramid" of N.C.P. and ProtoLookAt
    float CheckCameraPoints(Vector3 from, Vector3 to)
    {
        //series of comparisons, IF any of the 5 determined points is hit, what is
        //nearest of desired target for cam.

        //nearest distance can never be -1. It can be 0, but not less than that.
        //If this returns any other value than -1, there is a collision
        var nearestDistance = -1f; //if this is coming out, there has not been a collision. 

        //Linecast is similar to raycast. Raycast also needed for check

        RaycastHit hitInfo;

        //let's calculate the clipPoints of plane. By calling helper :D HELP! HELPER!
        Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneNear(to);

        //Draw Lines in editor for visualization. To see the actual rays. 
        //JUST DEBUGGING!!! Visible in editing-mode while running game.
        Debug.DrawLine(from, to + transform.forward * -Camera.main.nearClipPlane, Color.green); //from = protoLookAt, to is a point in space... oo..
        Debug.DrawLine(from, clipPlanePoints.UpLeft);
        Debug.DrawLine(from, clipPlanePoints.UpRight);
        Debug.DrawLine(from, clipPlanePoints.LowLeft);
        Debug.DrawLine(from, clipPlanePoints.LowRight);
//still debugging.... 
        Debug.DrawLine(clipPlanePoints.UpLeft, clipPlanePoints.UpRight);
        Debug.DrawLine(clipPlanePoints.UpRight, clipPlanePoints.LowRight);
        Debug.DrawLine(clipPlanePoints.LowRight, clipPlanePoints.LowLeft);
        Debug.DrawLine(clipPlanePoints.LowLeft, clipPlanePoints.UpLeft);
        //DEBUG ENDS HERE

        //Throwing lines to check for collision.
        if (Physics.Linecast(from, clipPlanePoints.UpLeft, out hitInfo) && hitInfo.collider.tag != "Proto")
            nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.LowLeft, out hitInfo) && hitInfo.collider.tag != "Proto")
            //if this point is closer to collision than the previous point, then this becomes the nearest distance.
            if (hitInfo.distance < nearestDistance || nearestDistance == -1)
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.UpRight, out hitInfo) && hitInfo.collider.tag != "Proto")
            //if this point is closer than the previous point, then this becomes the nearest distance.
            if (hitInfo.distance < nearestDistance || nearestDistance == -1)
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.LowRight, out hitInfo) && hitInfo.collider.tag != "Proto")
            //if this point is closer than the previous point, then this becomes the nearest distance.
            if (hitInfo.distance < nearestDistance || nearestDistance == -1)
                nearestDistance = hitInfo.distance;

        //This is the bumper-point behind the camera. This actually should never happen.
        if (Physics.Linecast(from, to + transform.forward * -Camera.main.nearClipPlane))
          //  if this point is closer than the previous point, then this becomes the nearest distance.
            if (hitInfo.distance < nearestDistance || nearestDistance == -1)
                nearestDistance = hitInfo.distance;


        return nearestDistance;

    }

    bool CheckIfOccluded(int count) //Moving camera if occluded here.
    {
        var isOccluded = false;
        var nearestDistance = CheckCameraPoints(ProtoLookAt.position, desiredPosition);

        if (nearestDistance != -1)
        {
            if (count < MaxOccChecks) //something was hit
            { //if checks are not in max we move cam.

                isOccluded = true;
                Distance = OcclusionDistance;

                if (Distance < 0.25f) //Making sure that distance is never 0. 0.25 proved to be ok.
                    Distance = 0.25f; //This will be changed to FPS after I make it so. :D

            }
            else
            {
                Distance = nearestDistance - Camera.main.nearClipPlane;
                desiredDist = Distance; //Forcing cam with brute force. No smoothing.
                distSmoothing = DistResumeSmooth;
            }

        }
        //returnin true or false depending on previous checks
        return isOccluded;
    }

    void ResetDesiredDist()
    {
        if(desiredDist < preOccludedDist)
        {
            //counts a new position 
            var pos = CalculatePosition(Vertical, Horizontal, preOccludedDist);

            var nearestDistance = CheckCameraPoints(ProtoLookAt.position, pos);

            if(nearestDistance == -1 || nearestDistance > preOccludedDist)
            {
                desiredDist = preOccludedDist;
            } 
        }
    }

    void UpdatePosition()
    //Smooth transition from current location to the desired position.
    {
        //Calculate final position (Vector3). PosX, PosY and PosZ, where Stmoothing added.
        //Value over time from StartPos to DesiredPos

        //Assign position. Moving the camera.
        //Done with the Mathf.SmoothDamp to all 3 axises
        var PositionX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, xSmooth);
        var PositionY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, ySmooth);
        var PositionZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, xSmooth);

        position = new Vector3(PositionX, PositionY, PositionZ);//gathering all together to one VEctor3 value

        transform.position = position; //assigning the previous value

        //Look at ProtoLookAt
        transform.LookAt(ProtoLookAt);
    }
    public void Reset()
    //setting initial values of camera position and distance
    {
        //Private floats:
        //mouseX //rotation about each axis
        //mouseY // -''- NOT WORLD axises, but mouseAxial rotation values.
        //Setting MouseX + MouseY to default values.<-TempValues
        //X is behind Proto and Y is just above Proto.
        Horizontal = 0;
        Vertical = 10;

        //desiredDistance(Des.Dist. is where we want to be from Proto)



        //Distance = StartDistance, depending when Reset is called.
        Distance = startDistance;

        //DesiredDistance = set to Distance, -''-
        desiredDist = Distance;
    }

    public static void UseOrCreate()
    //This method will look for cam, if there isn't one, it makes one.
    {
        //Declaring few GameObject variables:
        GameObject temporaryCam;
        GameObject ProtoLookAt;
        CameraCtrl myCamera;

        if (Camera.main != null) //if cam, then that's the cam
        {
            temporaryCam = Camera.main.gameObject;
        }
        else //if not cam, we make a cam.
        {
            temporaryCam = new GameObject("Main Camera");
            temporaryCam.AddComponent<Camera>();
            temporaryCam.tag = "MainCamera";
        }

        //Adding this script to the newly created camera.
        temporaryCam.AddComponent<CameraCtrl>();
        myCamera = temporaryCam.GetComponent("CameraCtrl") as CameraCtrl;

        ProtoLookAt = GameObject.Find("Proto") as GameObject;

        if (ProtoLookAt == null)
        { //If ProtoLookAt returns null, camera will position itself to 0.0.0
            ProtoLookAt = new GameObject("targetLookAt");
            ProtoLookAt.transform.position = Vector3.zero;
        }
        //And here is the end result of the previous lines:
        myCamera.ProtoLookAt = ProtoLookAt.transform;
    }

    
    

    }




