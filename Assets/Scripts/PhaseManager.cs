using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
	public delegate void DealingPhase();
	public delegate void PassingPhase();
	public delegate void PlayingPhase();
	public delegate void ScoringPhase();

	public static event DealingPhase onDealingPhase;
	public static event PassingPhase onPassingPhase;
	public static event PlayingPhase onPlayingPhase;
	public static event ScoringPhase onScoringPhase;

	private static Phase currPhase;
	private static int currRound;

  private void Start()
  {
    currPhase = Phase.Dealing;
		currRound = 1;

		RunPhase();
  }

	private static void UpdatePhase()
	{
		if (currPhase == Phase.Scoring)
		{
			currPhase = Phase.Dealing;
		}
		else
		{
			currPhase++;
		}

		Debug.Log("Update Phase: " + currPhase);
	}

	public static Phase GetCurrPhase()
	{
		return currPhase;
	}

	public static int GetCurrRound()
	{
		return currRound;
	}

	public static void RunPhase()
	{
		Debug.Log("Running Phase " + currPhase);

		Phase phase = currPhase;
		UpdatePhase();
		switch (phase)
		{
			case Phase.Dealing: onDealingPhase(); break;
			case Phase.Passing: onPassingPhase(); break;
			case Phase.Playing: onPlayingPhase(); break;
			case Phase.Scoring: onScoringPhase(); currRound++; break;
		}
	}
}

public enum Phase
{
	Dealing,
	Passing,
	Playing,
	Scoring
}
