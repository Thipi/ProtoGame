using UnityEngine;


  public static class Helper
    {

       public struct ClipPlanePoints
    {
        //3D locations in vector3 space to store data of Near Clipping Plane.
        public Vector3 UpLeft; //Upper left corner of N. C. P.
        public Vector3 UpRight;//and so on...
        public Vector3 LowLeft;
        public Vector3 LowRight;
    }

        public static float ClampAngle(float angle, float min, float max)
    {
        do
        {
            if (angle < -360)
                angle += 360;

            if (angle > 360)
                angle -= 360;

        } while (angle < -360 || angle > 360);

        return Mathf.Clamp(angle, min, max);
    }


    //Determining the 4 points of Near clipping plane here.
    public static ClipPlanePoints ClipPlaneNear(Vector3 position)
    {
        //ClipPlanePoints calculated here. 

        var clipPlanePoints = new ClipPlanePoints(); //local var to store clipPoints

        if (Camera.main == null) //idiotcheck.
            return clipPlanePoints; //This is zero everything. Cause there is no cam.


        //MATHTIME:
        var tranform = Camera.main.transform; 

        //Taking the FOV and dividing it to 2 and converting degrees to radians.
        var halfFOV = (Camera.main.fieldOfView / 2) * Mathf.Deg2Rad;

        var aspect = Camera.main.aspect; //kuvasuhde

        //distance between the nearClipPlane and camera
        var distance = Camera.main.nearClipPlane;

        //To get height: distance is multiplied by tanget of half of FOV 
        //that was calculated few lines above.
        var height = distance * Mathf.Tan(halfFOV);

        //getting widht. Also increases width relative to aspect ratio.
        var width = height * aspect;

        //Now to get the corners of Near Clipping plane!


        //CALCULATING CORNERS

    //Calculating Lower Right Corner
        clipPlanePoints.LowRight = position + tranform.right * width;
        clipPlanePoints.LowRight -= tranform.up * height; //what a weird way to say: "down"
        //Moving this point away from camera, so offset:
        clipPlanePoints.LowRight += tranform.forward * distance;

        //Calculating Lower Left Corner
        clipPlanePoints.LowLeft = position - tranform.right * width; //left = -right
        clipPlanePoints.LowLeft -= tranform.up * height;
        //Moving this point away from camera, so offset:
        clipPlanePoints.LowLeft += tranform.forward * distance;

        //Calculating Upper Right Corner
        clipPlanePoints.UpRight = position + tranform.right * width;
        clipPlanePoints.UpRight += tranform.up * height;
        //Moving this point away from camera, so offset:
        clipPlanePoints.UpRight += tranform.forward * distance;

        //Calculating Upper Left Corner
        clipPlanePoints.UpLeft = position - tranform.right * width;
        clipPlanePoints.UpLeft += tranform.up * height;
        //Moving this point away from camera, so offset:
        clipPlanePoints.UpLeft += tranform.forward * distance;

        return clipPlanePoints; //here the whole package is returned to camCtrl.
    }
    
    }

