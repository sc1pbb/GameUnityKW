using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class MouseOrbitImproved : MonoBehaviour
{

    public Transform target;
    public float xSpeed = 12.0f;
    public float ySpeed = 12.0f;
    public float scrollSpeed = 10.0f;

    public float zoomMin = 1.0f;

    public float zoomMax = 20.0f;

    public float distance;

    public Vector3 position;

    public bool isActivated;

    public bool isDragging;

    float x = 0.0f;

    float y = 0.0f;



    // Use this for initialization

    void Start ()
    {

        Vector3 angles = transform.eulerAngles;

        x = angles.y;

        y = angles.x;
    }



    void LateUpdate ()
    {

       isDragging = Input.touchCount == 0;

        // only update if the mousebutton is held down

        /*     if (Input.GetMouseButtonDown (1))
             {

                 isActivated = true;

             }

             // if mouse button is let UP then stop rotating camera

             if (Input.GetMouseButtonUp (1))

             {

                 isActivated = false;

             }*/


        //      isActivated = true;
        //        if (target && isActivated)

#if UNITY_EDITOR        
        x += CrossPlatformInputManager.GetAxis ("Mouse X") * xSpeed;
            y -= CrossPlatformInputManager.GetAxis ("Mouse Y") * ySpeed;

            transform.RotateAround (target.position, transform.up, x);



            // when mouse moves up and down we actually rotate around the local x axis	

            transform.RotateAround (target.position, transform.right, y);

            x = 0;

            y = 0;

#endif
        if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Moved)
        {
            Touch touch = Input.GetTouch (0);

            //  get the distance the mouse moved in the respective direction

            //  x += Input.GetAxis ("Mouse X") * xSpeed;

            // y -= Input.GetAxis ("Mouse Y") * ySpeed;

            if (Application.isMobilePlatform)
            {
                x += touch.deltaPosition.x * xSpeed * 0.04f;
                y -= touch.deltaPosition.y * xSpeed * 0.04f;
            }
            else
            {
                x += CrossPlatformInputManager.GetAxis ("Mouse X") * xSpeed;
                y -= CrossPlatformInputManager.GetAxis ("Mouse Y") * ySpeed;
            }


            // when mouse moves left and right we actually rotate around local y axis	

            transform.RotateAround (target.position, transform.up, x);



            // when mouse moves up and down we actually rotate around the local x axis	

            transform.RotateAround (target.position, transform.right, y);



            // reset back to 0 so it doesn't continue to rotate while holding the button

                x = 0;

                y = 0;
            isDragging = true;

        }

    }

}
