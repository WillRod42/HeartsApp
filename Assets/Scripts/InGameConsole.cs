  using UnityEngine;
	using TMPro;
  
  public class ConsoleToGUI : MonoBehaviour
  {
		[SerializeField] private GameObject Console;

    string myLog = "*begin log";
		string filename = ".\\Logs\\GameLogs-1.txt";
    bool doShow = false;
    int kChars = 700;

    void OnEnable() { Application.logMessageReceived += Log; }
    void OnDisable() { Application.logMessageReceived -= Log; }

		private void Awake()
		{
			if (filename != "")
			{
				if (!System.IO.Directory.Exists(".\\Logs"))
				{
					try { System.IO.Directory.CreateDirectory(".\\Logs"); }
					catch { Debug.Log("Failed to create log directory"); }
				}

				// filename pattern: GameLogs-[number].txt
				while (System.IO.File.Exists(filename))
				{
					string[] filenameParts = filename.Split("-");
					int fileNumber = int.Parse(filenameParts[1].Split(".")[0]) + 1;

					filename = filenameParts[0] + "-" + fileNumber + ".txt";
				}
			}
		}

    public void ToggleConsole()
		{ 
			doShow = !doShow; 
			Console.SetActive(doShow);
		}

    public void Log(string logString, string stackTrace, LogType type)
    {
    // for onscreen...
      myLog = myLog + "\n" + logString;
      if (myLog.Length > kChars) { myLog = myLog.Substring(myLog.Length - kChars); }

			try { System.IO.File.AppendAllText(filename, logString + "\n"); }
			catch { }

			Console.GetComponentInChildren<TMP_Text>().text = myLog;
    }

    // void OnGUI()
    // {
    //   if (!doShow) { return; }
    //   GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
    //   new Vector3(Screen.width / 1200.0f, Screen.height / 800.0f, 1.0f));
    //   GUI.TextArea(new Rect(10, 10, 540, 370), myLog);
    // }
  }
