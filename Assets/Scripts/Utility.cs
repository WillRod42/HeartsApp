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
