using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum RightMouseState
{
    NoState,
    DetectingPan,
    Panning
}

public class CameraControl : MonoBehaviour
{
    Camera cam;
    Vector2 rightClickPos = Vector2.zero;
    RightMouseState rmbState = RightMouseState.NoState;
    Vector2 clickCenterScreenPos = Vector2.zero;

    void Start()
    {
        if (!cam)
            cam = this.GetComponent<Camera>();
    }

    void Update()
    {
        /* zoom in */
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            float curZoom = cam.orthographicSize;
            Vector2 beforeMouseWorldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
            curZoom -= Mathf.Max(2, curZoom / 3);
            if (curZoom < 5)
                curZoom = 5;
            cam.orthographicSize = curZoom;
            Vector2 afterMouseWorldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 difference = beforeMouseWorldPoint - afterMouseWorldPoint;
            cam.transform.position += new Vector3(difference.x, difference.y);
        }
        /* zoom out */
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            Vector2 beforeMouseWorldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
            float curZoom = cam.orthographicSize;
            curZoom += Mathf.Max(2, curZoom / 3);
            cam.orthographicSize = curZoom;
            Vector2 afterMouseWorldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 difference = beforeMouseWorldPoint - afterMouseWorldPoint;
            cam.transform.position += new Vector3(difference.x, difference.y);
        }

        /* RIGHT MOUSE - panning */
        if (Input.GetMouseButtonDown(1)) // start detecting the drag
        {
            rmbState = RightMouseState.DetectingPan;
            rightClickPos = Input.mousePosition;
            clickCenterScreenPos = cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)) / 2;
        }

        // panning
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            if (rmbState == RightMouseState.DetectingPan) // if mouse moves 14 pixels away, we've got a drag!
            {
                Vector2 curPos = Input.mousePosition;
                if ((rightClickPos - curPos).sqrMagnitude > 200)
                {
                    // if RMB is pressed, we're trying to pan
                    if (Input.GetKey(KeyCode.Mouse1))
                    {
                        rmbState = RightMouseState.Panning;
                    }
                }
            }
            if (rmbState == RightMouseState.Panning) // while panning
            {
                Vector2 curPos = Input.mousePosition;
                Vector2 deltaScreen = rightClickPos - curPos;
                Vector2 deltaWorld = cam.ScreenToWorldPoint(new Vector3(deltaScreen.x, deltaScreen.y));
                float divider = 2;
                Vector2 newCamPos = (deltaWorld / divider) + clickCenterScreenPos;
                cam.transform.position = new Vector3(newCamPos.x, newCamPos.y, -10);
            }
        }
        if (Input.GetMouseButtonUp(1)) // stop dragging
        {
            rmbState = RightMouseState.NoState;
        }
    }
}
