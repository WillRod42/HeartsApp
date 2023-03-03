using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawOutlineEditor : MonoBehaviour
{
	public Color lineColor;

	public void OnDrawGizmos()
	{
		Vector3 position = transform.position;
		Vector3 size = GetComponent<SpriteRenderer>().bounds.size;

		Vector3 TopLeft = new Vector3(position.x - (size.x / 2), position.y + (size.y / 2), 0);
		Vector3 TopRight = new Vector3(position.x + (size.x / 2), position.y + (size.y / 2), 0);
		Vector3 BottomLeft = new Vector3(position.x - (size.x / 2), position.y - (size.y / 2), 0);
		Vector3 BottomRight = new Vector3(position.x + (size.x / 2), position.y - (size.y / 2), 0);

		Gizmos.color = lineColor;
		Gizmos.DrawLine(TopLeft, TopRight);
		Gizmos.DrawLine(TopRight, BottomRight);
		Gizmos.DrawLine(BottomRight, BottomLeft);
		Gizmos.DrawLine(BottomLeft, TopLeft);
	}
}
