using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMan : MonoBehaviour
{

    //Button names are based on Xbox controller.
    //Alternative Keyboard-buttons described on the right.
    // Place the string names of buttons to UnityEditor
    //--> project settings --> Input and create.

    //Axis

    public static float MainHorizontal()
    {
        float r = 0.0f;
        r += Input.GetAxis("Joy_MainHorizontal");
        r += Input.GetAxis("Key_MainHorizontal");
        return Mathf.Clamp(r, -1.0f, 1.0f); //So inputvalue doesn't return 2 in case
                                            // player is using joyst. and keyb. same time.
    }

    public static float MainVertical()
    {
        float r = 0.0f;
        r += Input.GetAxis("Joy_MainVertical");
        r += Input.GetAxis("Key_MainVertical");
        return Mathf.Clamp(r, -1.0f, 1.0f); //So inputvalue doesn't return 2 in case
                                            // player is using joyst. and keyb. same time.
    }

    public static float RightVertical()
    {
        float r = 0.0f;
        r += Input.GetAxis("Joy_RightVertical");
        r += Input.GetAxis("Mouse Y");
        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static float RightHorizontal()
    {
        float r = 0.0f;
        r += Input.GetAxis("Joy_RightHorizontal");
        r += Input.GetAxis("Mouse X");
        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static Vector3 MainJoystick()
    {
        return new Vector3(MainHorizontal(), 0, MainVertical());
    }

    //Buttons

    public static bool A() //Keyboard: space 
    {
        return Input.GetButtonDown("A_Button");
    }

    public static bool B() //Keyboard: B        
    {
        return Input.GetButtonDown("B_Button");
    }
    public static bool X() //Keyboard: Left shift
    {
        return Input.GetButtonDown("X_Button");
    }

    public static bool Y() //Keyboard: V
    {
        return Input.GetButtonDown("Y_Button");
    }

    public static bool RT() //Keyboard: Left Mouse button
    {
        return Input.GetButtonDown("RT_Button");
        return Input.GetMouseButtonDown(0);
    }
    public static bool LT()//Keyboard: Right Mouse button
    {
        return Input.GetButtonDown("LT_Button");
        return Input.GetMouseButtonDown(1);
    }
    public static bool RB()//Keyboard: Q
    { 
        return Input.GetButtonDown("RB_Button");
        
    }

    public static bool LB() //Keyboard: E
    {
        return Input.GetButtonDown("LB_Button");
    }

}