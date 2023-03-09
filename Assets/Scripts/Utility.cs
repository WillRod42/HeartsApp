using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
	public static void Log(string logHeader, string message)
	{
		logHeader = "-----------------" + logHeader + "-----------------";
		Debug.Log(logHeader);
		Debug.Log(message);
		// PrintLogFooter(logHeader.Length);
	}

	public static void PlaceObjectsInSpread(List<Transform> objs, int size, float width, float height, Vector3 placeAreaCenter, Vector3 placeAreaSize, Vector3 offset)
	{
		Vector3 BottomLeft = new Vector3(placeAreaCenter.x - (placeAreaSize.x / 2), placeAreaCenter.y - (placeAreaSize.y / 2), 0);
		Vector3 BottomRight = new Vector3(placeAreaCenter.x + (placeAreaSize.x / 2), placeAreaCenter.y - (placeAreaSize.y / 2), 0);

		float overlapOffset = ((BottomRight.x - width) - (BottomLeft.x)) / (size - 1);
		for (int i = 0; i < objs.Count; i++)
		{
			GameObject obj = objs[i].gameObject;
			obj.transform.position = new Vector3((BottomLeft.x + (overlapOffset * i) + (width / 2f)) + offset.x, (BottomLeft.y + height / 2) + offset.y, offset.z);
			obj.SetActive(true);

			SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
			if (sr)
			{
				sr.sortingOrder = i;
			}
		}
	}


	private static void PrintLogFooter(int length)
	{
		string footer = "";
		for (int i = 0; i < length; i++)
		{
			footer += "-";
		}

		Debug.Log(footer);
	}
}
