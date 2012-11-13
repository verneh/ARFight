using UnityEngine;
using System.Collections;

public class TouchManagerEventArgs
{
    public TouchManagerEventArgs(TouchItem touch) { Touch = touch; }
    public TouchItem Touch { get; private set; }
}

/// <summary>
/// TouchManager processes touches.  It can also simulate single touches when running in the Unity editor.
/// It provides utility functions for detecting Collider touches and rotating Ridigidbodies.
/// Touches will be automatically processed when TouchManager is attached to a GameObject.
/// </summary>
public class TouchManager : MonoBehaviour
{
    public delegate void TouchManagerEventHandler(object sender, TouchManagerEventArgs e);

    // Events
    public static bool LikeIPhone = false;
    public static bool BlockEvents = false;

    public static event TouchManagerEventHandler TouchBeganEvent;
    public static event TouchManagerEventHandler TouchEndedEvent;
    public static event TouchManagerEventHandler TouchMoveEvent;
    public static event TouchManagerEventHandler MoveRightEvent;
    public static event TouchManagerEventHandler MoveLeftEvent;
    public static event TouchManagerEventHandler MoveUpEvent;
    public static event TouchManagerEventHandler MoveDownEvent;
    public static event TouchManagerEventHandler TapEvent;
    public static event TouchManagerEventHandler GameObjectTapEvent;
    public static event TouchManagerEventHandler SwipeEvent;
    public static event TouchManagerEventHandler SwipeUpEvent;
    public static event TouchManagerEventHandler SwipeDownEvent;
    public static event TouchManagerEventHandler SwipeLeftEvent;
    public static event TouchManagerEventHandler SwipeRightEvent;

    private const int MAX_TOUCHES = 2;

    // The last time a finger touched the screen
    public static TouchItem[] touchCache = new TouchItem[MAX_TOUCHES];
    public static float sqrDelta = 8;

    private static int count = 0;

    private static bool mouseLeftButtonDown = false;

    public static TouchItem touchItem;

    /// <summary>
    /// When TouchManager is attached to a GameObject it will automatically process touches on every call to Update, Update is called once per frame.
    /// </summary>
    void Update()
    {
        ProcessTouches();
    }

    /// <summary>
    /// Gets the touch count.
    /// </summary>
    /// <value>
    /// The touch count.
    /// </value>
    public static int touchCount
    {
        get { return count; }
    }

    /// <summary>
    /// Determines whether touch is valid for the specified touch id.
    /// </summary>
    /// <returns>
    /// <c>true</c> if this if the touch is valid for the specified id; otherwise, <c>false</c>.
    /// </returns>
    /// <param name='id'>
    /// Touch id.
    /// </param>
    public static bool IsTouchValid(int id)
    {
        bool retVal = false;

        if (id < touchCount)
        {
            retVal = true;
        }

        return retVal;
    }

    /// <summary>
    /// Gets the touch position.
    /// </summary>
    /// <returns>
    /// The screen location of the touch.
    /// </returns>
    /// <param name='id'>
    /// Touch id.
    /// </param>
    /// <param name='pos'>
    /// Set to screen location of touch.
    /// </param>
    public static bool GetTouchPos(int id, out Vector2 pos)
    {
        bool retVal = IsTouchValid(id);
        if (retVal == true)
        {
            pos = touchCache[id].position;
        }
        else
        {
            pos = Vector2.zero;
        }

        return retVal;
    }

    /// <summary>
    /// Gets the touch start position.
    /// </summary>
    /// <returns>
    /// The screen location of the touch.
    /// </returns>
    /// <param name='id'>
    /// Touch id.
    /// </param>
    /// <param name='pos'>
    /// Set to screen location of touch.
    /// </param>
    public static bool GetTouchStartPos(int id, out Vector2 pos)
    {
        bool retVal = IsTouchValid(id);
        if (retVal == true)
        {
            pos = touchCache[id].startPosition;
        }
        else
        {
            pos = Vector2.zero;
        }

        return retVal;
    }

    /// <summary>
    /// Gets the touch delta position.
    /// </summary>
    /// <returns>
    /// The touch delta position.
    /// </returns>
    /// <param name='id'>
    /// Touch id.
    /// </param>
    /// <param name='deltaPos'>
    /// Set to the touch delta position
    /// </param>
    public static bool GetTouchDeltaPos(int id, out Vector2 deltaPos)
    {
        bool retVal = IsTouchValid(id);

        if (retVal == true)
        {
            deltaPos = touchCache[id].deltaPosition;
        }
        else
        {
            deltaPos = Vector2.zero;
        }

        return retVal;
    }

    /// <summary>
    /// Gets the touch phase.
    /// </summary>
    /// <returns>
    /// The touch phase.
    /// </returns>
    /// <param name='id'>
    /// Touch id.
    /// </param>
    /// <param name='phase'>
    /// Set to the touch phase.
    /// </param>
    public static bool GetTouchPhase(int id, out TouchPhaseEnum phase)
    {
        phase = TouchPhaseEnum.CANCELED;

        bool retVal = IsTouchValid(id);

        if (retVal == true)
        {
            phase = touchCache[id].phase;
        }

        return retVal;
    }

    public static TouchItem GetTouchItem(int id)
    {
        bool retVal = IsTouchValid(id);

        if (retVal)
            return touchCache[id];
        else
            return TouchItem.zero;
    }

    /// <summary>
    /// Checks to see if a collider was touched
    /// </summary>
    /// <returns>
    /// True if the collider was touched.
    /// </returns>
    /// <param name='id'>
    /// Touch id.
    /// </param>
    /// <param name='collider'>
    /// Collider to check for touch.
    /// </param>
    public static bool TouchedCollider(int id, Collider collider)
    {
        return TouchedCollider(id, collider, Mathf.Infinity);
    }

    /// <summary>
    /// Checks to see if a collider was touched
    /// </summary>
    /// <returns>
    /// True if the collider was touched.
    /// </returns>
    /// <param name='id'>
    /// Touch id.
    /// </param>
    /// <param name='collider'>
    /// Collider to check for touch.
    /// </param>
    /// <param name='distance'>
    /// Distance away from the touch screen location to check for collider intersection
    /// </param>
    public static bool TouchedCollider(int id, Collider collider, float distance)
    {
        Vector2 screenPos;
        Vector3 worldPos;

        return TouchedCollider(id, collider, distance, out screenPos, out worldPos);
    }

    /// <summary>
    /// Checks to see if a collider was touched
    /// </summary>
    /// <returns>
    /// True if the collider was touched.
    /// </returns>
    /// <param name='id'>
    /// Touch id.
    /// </param>
    /// <param name='collider'>
    /// Collider to check for touch.
    /// </param>
    /// <param name='distance'>
    /// Distance away from the touch screen location to check for collider intersection
    /// </param>
    /// <param name='screenPos'>
    /// Set with the screen position of the touch.
    /// </param>
    public static bool TouchedCollider(int id, Collider collider, float distance, out Vector2 screenPos)
    {
        Vector3 worldPos;

        return TouchedCollider(id, collider, distance, out screenPos, out worldPos);
    }

    /// <summary>
    /// Checks to see if a collider was touched
    /// </summary>
    /// <returns>
    /// True if the collider was touched.
    /// </returns>
    /// <param name='id'>
    /// Touch id.
    /// </param>
    /// <param name='collider'>
    /// Collider to check for touch.
    /// </param>
    /// <param name='distance'>
    /// Distance away from the touch screen location to check for collider intersection
    /// </param>
    /// <param name='worldPos'>
    /// Set with the world position of the touch
    /// </param>
    public static bool TouchedCollider(int id, Collider collider, float distance, out Vector3 worldPos)
    {
        Vector2 screenPos;

        return TouchedCollider(id, collider, distance, out screenPos, out worldPos);
    }

    /// <summary>
    /// Checks to see if a collider was touched
    /// </summary>
    /// <returns>
    /// True if the collider was touched.
    /// </returns>
    /// <param name='id'>
    /// Touch id.
    /// </param>
    /// <param name='collider'>
    /// Collider to check for touch.
    /// </param>
    /// <param name='distance'>
    /// Distance away from the touch screen location to check for collider intersection
    /// </param>
    /// <param name='screenPos'>
    /// Set with the screen position of the touch.
    /// </param>
    /// <param name='worldPos'>
    /// Set with the world position of the touch
    /// </param>
    public static bool TouchedCollider(int id, Collider collider, float distance, out Vector2 pos, out Vector3 worldPos)
    {
        bool touchedCollider = false;

        worldPos = Vector3.zero;

        if (TouchManager.GetTouchPos(id, out pos))
        {

            Ray ray = Camera.mainCamera.ScreenPointToRay(pos);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distance))
            {

                if (hit.collider == collider)
                {

                    worldPos = hit.point;

                    touchedCollider = true;

                }
            }
        }

        return touchedCollider;
    }



    public static bool TouchedColliderGO(int id, out GameObject go, float distance, out Vector2 pos, out Vector3 worldPos)
    {
        bool touchedCollider = false;
        go = null;

        worldPos = Vector3.zero;

        if (TouchManager.GetTouchPos(id, out pos))
        {
            Ray ray = Camera.mainCamera.ScreenPointToRay(pos);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distance))
            {

                go = hit.collider.gameObject;
                touchedCollider = true;
            }
        }

        return touchedCollider;
    }

    /// <summary>
    /// Checks to see if a GUIText object was touched
    /// </summary>
    /// <returns>
    /// True if the GUIText object was touched.
    /// </returns>
    /// <param name='id'>
    /// Touch id.
    /// </param>
    /// <param name='guiText'>
    /// GUIText object to check.
    /// </param>
    public static bool TouchedGUIText(int id, GUIText guiText)
    {
        Vector2 pos;

        return TouchedGUIText(id, guiText, out pos);
    }

    /// <summary>
    /// Checks to see if a GUIText object was touched
    /// </summary>
    /// <returns>
    /// True if the GUIText object was touched.
    /// </returns>
    /// <param name='id'>
    /// Touch id.
    /// </param>
    /// <param name='guiText'>
    /// GUIText object to check.
    /// </param>
    /// <param name='pos'>
    /// Set with the screen position of the touch
    /// </param>
    public static bool TouchedGUIText(int id, GUIText guiText, out Vector2 pos)
    {
        bool touchedGUIText = false;

        if (TouchManager.GetTouchPos(id, out pos))
        {

            touchedGUIText = guiText.HitTest(pos);

        }

        return touchedGUIText;
    }


    public static string TouchedGUITexture(int id)
    {
        Vector2 pos = Vector2.zero;
        GUILayer test = Camera.main.GetComponent<GUILayer>();

        if (TouchManager.GetTouchPos(id, out pos))
        {

            if (test.HitTest(pos))
            {
                return test.HitTest(pos).name;
            }

        }

        return null;
    }


    /// <summary>
    /// Calculates the rotation axis and torque for a rigid body based on the current touch position and the last touch position.
    /// </summary>
    /// <returns>
    /// True if rotation axis and torque were successfully calculated
    /// </returns>
    /// <param name='id'>
    /// Touch id.
    /// </param>
    /// <param name='axis'>
    /// Sets rotation axis.
    /// </param>
    /// <param name='torque'>
    /// Sets torque.
    /// </param>
    public static bool RotationAxisTorque(int id, out Vector3 axis, out float torque)
    {
        return RotationAxisTorque(id, 1, out axis, out torque);
    }

    /// <summary>
    /// Calculates the rotation axis and torque for a rigid body based on the current touch position and the last touch position.
    /// </summary>
    /// <returns>
    /// True if rotation axis and torque were successfully calculated
    /// </returns>
    /// <param name='id'>
    /// Touch id.
    /// </param>
    /// <param name='torqueScalar'>
    /// Scalar value applied to torque calculation.
    /// </param>
    /// <param name='axis'>
    /// Sets rotation axis.
    /// </param>
    /// <param name='torque'>
    /// Sets torque.
    /// </param>
    public static bool RotationAxisTorque(int id, float torqueScalar, out Vector3 axis, out float torque)
    {
        bool success = false;

        axis = Vector3.zero;

        torque = 0;

        Vector2 curPos, deltPos, lastPos;

        if (TouchManager.GetTouchPos(id, out curPos))
        {

            Ray curRay = Camera.mainCamera.ScreenPointToRay(curPos);

            TouchManager.GetTouchDeltaPos(id, out deltPos);

            lastPos = curPos - deltPos;

            Ray prevRay = Camera.mainCamera.ScreenPointToRay(lastPos);

            axis = Vector3.Cross(curRay.direction, prevRay.direction);

            torque = (lastPos - curPos).magnitude * torqueScalar;

            success = true;
        }

        return success;
    }

    /// <summary>
    /// Rotates the rigidbody.
    /// </summary>
    /// <returns>
    /// True if rigid body is successfully rotated.
    /// </returns>
    /// <param name='id'>
    /// Touch id.
    /// </param>
    /// <param name='rigidbody'>
    /// Rigid body to rotate.
    /// </param>
    public static bool RotateRigidbody(int id, Rigidbody rigidbody)
    {
        return RotateRigidbody(id, 1, rigidbody);
    }

    /// <summary>
    /// Rotates the rigidbody.
    /// </summary>
    /// <returns>
    /// True if rigid body is successfully rotated.
    /// </returns>
    /// <param name='id'>
    /// Touch id.
    /// </param>
    /// <param name='torqueScalar'>
    /// Scalar value applied to torque calculation.
    /// </param>
    /// <param name='rigidbody'>
    /// Rigid body to rotate.
    /// </param>
    public static bool RotateRigidbody(int id, float torqueScalar, Rigidbody rigidbody)
    {
        bool success = false;

        Vector3 axis;

        float torque;

        if (TouchManager.RotationAxisTorque(id, torqueScalar, out axis, out torque))
        {

            rigidbody.AddTorque(axis * torque);

            success = true;
        }

        return success;
    }

    /// <summary>
    /// Processes the touches.
    /// </summary>
    /// <returns>
    /// The number of touches.
    /// </returns>
    public static int ProcessTouches()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || LikeIPhone)
            return ProcessRealTouches();
        else
            return ProcessMouseEvents();
    }

    private static int ProcessMouseEvents()
    {
        if (Input.GetMouseButtonDown(0) == true && mouseLeftButtonDown == false)
        {
            mouseLeftButtonDown = true;

            touchItem.phase = TouchPhaseEnum.BEGAN;
            touchItem.position = Input.mousePosition;
            touchItem.startPosition = Input.mousePosition;
            touchItem.deltaPosition = Vector2.zero;
            touchItem.gesture = TouchItem.TouchGesture.Other;
            touchItem.distanceX = 0;
            touchItem.distanceY = 0;
            touchItem.valid = true;

            count = 0;
            touchCache[0] = touchItem;

            if (TouchBeganEvent != null && !BlockEvents)
                TouchBeganEvent(touchItem, new TouchManagerEventArgs(touchItem));
        }
        else
        {
            if (Input.GetMouseButtonUp(0) == true && mouseLeftButtonDown == true)
            {
                mouseLeftButtonDown = false;
                touchItem.phase = TouchPhaseEnum.ENDED;

                UpdatePosition();

                touchGistureProceed(ref touchItem);


                touchCache[0] = touchItem;
                count = 0;
   
                if (TouchEndedEvent != null && !BlockEvents)
                    TouchEndedEvent(touchItem, new TouchManagerEventArgs(touchItem));
            }
            else if (mouseLeftButtonDown == true)
            {

                touchItem.phase = TouchPhaseEnum.MOVED;
                
                UpdatePosition();

               
                touchCache[0] = touchItem;
                count = 1;

                if (TouchMoveEvent != null && !BlockEvents)
                    TouchMoveEvent(touchItem, new TouchManagerEventArgs(touchItem));
            }
        }
        return count;
    }

    private static void UpdatePosition()
    {
        Vector2 pos = Input.mousePosition;
        touchItem.deltaPosition = (pos - touchItem.position);
        touchItem.position = pos;
    }

    //#if UNITY_IPHONE
    private static int ProcessRealTouches()
    {
        count = Input.touchCount;

        for (int i = 0; i < count; i++)
        {
            Touch touch = Input.GetTouch(i);
            TouchPhase phase = touch.phase;
            int fingerId = touch.fingerId;

            if (fingerId >= MAX_TOUCHES)
            {
                fingerId = fingerId % MAX_TOUCHES;
            }
	
            touchItem = touchCache[fingerId];

            if (!touchItem.valid)
                phase = TouchPhase.Began;

            // Cache the touch data.
            touchItem.deltaPosition = touch.deltaPosition;
            touchItem.position = touch.position;

            if (touch.deltaPosition.magnitude > sqrDelta)
                touchItem.positions.Add(touch.position);

            try
            {
                switch (phase)
                {

                    case TouchPhase.Ended:
                        touchItem.phase = TouchPhaseEnum.ENDED;
                        touchGistureProceed(ref touchItem);

                        if (TouchEndedEvent != null && !BlockEvents)
                            TouchEndedEvent(touch, new TouchManagerEventArgs(touchItem));

                        break;
                    case TouchPhase.Canceled:
                        touchItem.phase = TouchPhaseEnum.CANCELED;
                        break;
                    case TouchPhase.Began:
                        touchItem.phase = TouchPhaseEnum.BEGAN;
                        touchItem.startPosition = touch.position;
                        touchItem.positions = new ArrayList();
                        touchItem.gesture = TouchItem.TouchGesture.Other;
                        touchItem.distanceX = 0;
                        touchItem.distanceY = 0;
                        touchItem.valid = true;

                        if (TouchBeganEvent != null && !BlockEvents)
                            TouchBeganEvent(touch, new TouchManagerEventArgs(touchItem));

                        break;
                    case TouchPhase.Moved:
                        touchItem.phase = TouchPhaseEnum.MOVED;
                        touchItem.distanceX += Mathf.Abs(touch.deltaPosition.x);
                        touchItem.distanceY += Mathf.Abs(touch.deltaPosition.y);
						
						
                        
                        //float dx = touchItem.position.x - touchItem.startPosition.x,
                        //	  dy = touchItem.position.y - touchItem.startPosition.y;
                        float dx = touch.deltaPosition.x, dy = touch.deltaPosition.y;
                        float dk = 2f;
						Debug.Log(dx * dx + dy * dy > dk * dk);
                        if (dx * dx + dy * dy > dk * dk)
                        {
							if (TouchMoveEvent != null && !BlockEvents) {
								TouchMoveEvent (touch, new TouchManagerEventArgs (touchItem));
							}

                            if (Mathf.Abs(dx / dy) > 1f)
                            {
                                if (dx > 0)
                                {
                                    if (MoveRightEvent != null && !BlockEvents)
                                        MoveRightEvent(touch, new TouchManagerEventArgs(touchItem));
                                }
                                else
                                {
                                    if (MoveLeftEvent != null && !BlockEvents)
                                        MoveLeftEvent(touch, new TouchManagerEventArgs(touchItem));
                                }
                            }
                            else
                            {
                                if (dy > 0)
                                {
                                    if (MoveUpEvent != null && !BlockEvents)
                                        MoveUpEvent(touch, new TouchManagerEventArgs(touchItem));
                                }
                                else
                                {
                                    if (MoveDownEvent != null && !BlockEvents)
                                        MoveDownEvent(touch, new TouchManagerEventArgs(touchItem));
                                }
                            }
                        }
						
                        break;
                    case TouchPhase.Stationary:
                        touchItem.phase = TouchPhaseEnum.STATIONARY;
                        break;

                }
            }
            finally
            {
                touchCache[fingerId] = touchItem;
            }

        }

        return count;
    }
    // #endif

    private static void touchGistureProceed(ref TouchItem touchItem)
    {
        if (tryParseDot(ref touchItem))
        {
            touchItem.gesture = TouchItem.TouchGesture.Dot;

            if (TapEvent != null && !BlockEvents)
                TapEvent(touchItem, new TouchManagerEventArgs(touchItem));

            GetGameObject(touchItem);

            return;
        }

        if (tryParseSwipe(ref touchItem))
        {
            touchItem.gesture = TouchItem.TouchGesture.Swipe;

            if (SwipeEvent != null && !BlockEvents)
                SwipeEvent(touchItem, new TouchManagerEventArgs(touchItem));

            return;
        }

        touchItem.gesture = TouchItem.TouchGesture.Other;
    }

    static bool tryParseSwipe(ref TouchItem touchItem)
    {
        if (touchItem.positions.Count < 4)
            return false;

        float dx = touchItem.position.x - touchItem.startPosition.x,
              dy = touchItem.position.y - touchItem.startPosition.y;
        float k = Mathf.Abs(dx / dy);

        if (k > 1) // 1/Mathf.Sqrt(3)
        {
            if (dx > 0)
            {
                if (SwipeRightEvent != null && !BlockEvents)
                    SwipeRightEvent(touchItem, new TouchManagerEventArgs(touchItem));
            }
            else
            {
                if (SwipeLeftEvent != null && !BlockEvents)
                    SwipeLeftEvent(touchItem, new TouchManagerEventArgs(touchItem));
            }

            return true;
        }
        else
        {
            if (dy > 0)
            {
                if (SwipeUpEvent != null && !BlockEvents)
                    SwipeUpEvent(touchItem, new TouchManagerEventArgs(touchItem));
            }
            else
            {
                if (SwipeDownEvent != null && !BlockEvents)
                    SwipeDownEvent(touchItem, new TouchManagerEventArgs(touchItem));
            }

            return true;
        }
    }

    static bool tryParseDot(ref TouchItem touchItem)
    {
//        if (touchItem.positions.Count > 4)
  //          return false;
        if (touchItem.distanceX + touchItem.distanceY > 4)
            return false;

        return true;
    }

    public static float gistureMidleCircleDelta = 30f;
    public static int gisturePartsDeltaCount = 4;

    private static void GetGameObject(TouchItem touchItem)
    {
        if (GameObjectTapEvent != null && !BlockEvents)
        {
            Ray ray = Camera.main.ScreenPointToRay(touchItem.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10f))
                GameObjectTapEvent(hit.collider.gameObject, new TouchManagerEventArgs(touchItem));
        }
    }

    static bool tryParseCircle(ref TouchItem touchItem)
    {
        int count = touchItem.positions.Count;
        //print("positions count is " + count);
        if (count < 5) return false;

        float cx = 0, cy = 0;
        foreach (Vector2 pos in touchItem.positions)
        {
            cx += pos.x; cy += pos.y;
        }
        cx /= count; cy /= count;

        Vector2 center = new Vector2(cx, cy);
        float midleRadius = 0, minRadius = float.MaxValue, maxRadius = 0;
        foreach (Vector2 pos in touchItem.positions)
        {
            float rad = Vector2.Distance(center, pos);
            midleRadius += rad;
            if (rad < minRadius)
                minRadius = rad;
            if (rad > maxRadius)
                maxRadius = rad;
        }
        midleRadius /= count;

        short pF = axisPart((Vector2)touchItem.positions[1] - (Vector2)touchItem.positions[0]);

        int wrongsCW = 0, wrongsCCW = 0;
        int partChCW = 0, partChCCW = 0;
        short cur = pF;
        string poss = "";
        for (int i = 2; i < count; i++)
        {
            short next = axisPart((Vector2)touchItem.positions[i] - (Vector2)touchItem.positions[i - 1]);
            int cw = axisNext(cur, next, true),
                ccw = axisNext(cur, next, false);
            poss += next.ToString();

            if (cw <= -1)
                wrongsCW++;
            if (ccw <= -1)
                wrongsCCW++;

            if (cw == 1)
                partChCW++;
            if (ccw == 1)
                partChCCW++;

            cur = next;
        }
        partChCW = Mathf.RoundToInt((float)partChCW / 4f);
        partChCCW = Mathf.RoundToInt((float)partChCCW / 4f);
        int wrongs = Mathf.Min(wrongsCCW, wrongsCW),
            partsCh = Mathf.Max(partChCW, partChCCW);

        if (wrongs == wrongsCCW) partsCh = -partsCh;

        wrongs = (int)((float)wrongs / count * 100);

        //print(poss);
        //print("wrongs: " + wrongs + "% rotates:" + partsCh);

        if (wrongs > 25)
        {
            //print("to many wrongs");
            return false;
        }

        int[] counts = new int[4];

        foreach (Vector2 pos in touchItem.positions)
            counts[axisPart(pos - center)]++;

        int midlePartsCount = 0;
        foreach (int x in counts)
            midlePartsCount += x;
        midlePartsCount /= 4;

        foreach (int x in counts)
            if (Mathf.Abs(midlePartsCount - x) > 5)
            {
                //print("incorrect parts count");
                return false;
            }

        float minRadiusD = float.MaxValue, maxRadiusD = 0;
        foreach (Vector2 pos in touchItem.positions)
        {
            float curDelta = Vector2.Distance(pos, center) / midleRadius * 100;
            if (curDelta > maxRadiusD)
                maxRadiusD = curDelta;
            if (curDelta < minRadiusD)
                minRadiusD = curDelta;
        }
        //print("min radius delta: " + minRadiusD + "% max radius delta: " + maxRadiusD + "%");

        if (minRadiusD < 45 || maxRadiusD > 160)
        {
            //print("wrong radiuses");
            return false;
        }

        //print("circle!!");
        GistureCircle param = new GistureCircle();
        param.center = center;
        param.radius = midleRadius;
        param.rotates = partsCh;
        touchItem.gistureParams = param;
        return true;
    }
    static short axisPart(Vector2 v)
    {
        if (v.x > 0 && v.y > 0)
            return 0;
        if (v.x < 0 && v.y > 0)
            return 1;
        if (v.x < 0 && v.y < 0)
            return 2;
        if (v.x > 0 && v.y < 0)
            return 3;
        return 0;
    }
    static short axisPart(Vector2 v, short start)
    {
        if (v.x > 0 && v.y > 0)
            return (short)((4 + 0 - start) % 4);
        if (v.x < 0 && v.y > 0)
            return (short)((4 + 1 - start) % 4);
        if (v.x < 0 && v.y < 0)
            return (short)((4 + 2 - start) % 4);
        if (v.x > 0 && v.y < 0)
            return (short)((4 + 3 - start) % 4);
        return 0;
    }
    static short axisNext(short cur, short next, bool clockwise)
    {
        if (next == cur)
            return 0;

        if (clockwise)
        {
            if ((cur == 1 && next == 0) || (cur == 0 && next == 3) || (cur == 3 && next == 2) || (cur == 2 && next == 1))
                return 1;
            if ((cur == 0 && next == 1) || (cur == 1 && next == 2) || (cur == 2 && next == 3) || (cur == 3 && next == 0))
                return -1;
            return -2;
        }
        else
        {
            if ((cur == 1 && next == 0) || (cur == 0 && next == 3) || (cur == 3 && next == 2) || (cur == 2 && next == 1))
                return -1;
            if ((cur == 0 && next == 1) || (cur == 1 && next == 2) || (cur == 2 && next == 3) || (cur == 3 && next == 0))
                return 1;
            return -2;
        }
    }

}