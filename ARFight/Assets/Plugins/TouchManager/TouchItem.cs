
//
using UnityEngine;
using System.Collections;

/// <summary>
/// Structure used by TouchManager
/// </summary>
public struct TouchItem
{
	public bool valid;
	
    public Vector2 position;
	public Vector2 deltaPosition;
	public Vector2 startPosition;
	
	public float distanceX;
	public float distanceY;
	
	public ArrayList positions;
	public enum TouchGesture{Other, Circle, Dot, Swipe}
	public TouchGesture gesture;
	public object gistureParams;
	
	public TouchPhaseEnum phase;
	
	static public TouchItem zero = new TouchItem();
}

public class GistureCircle
{
	public Vector2 center;
	public float radius;
	public int rotates;
}
