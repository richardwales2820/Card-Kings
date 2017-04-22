using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ManagerPineapple : Manager
{

	// Use this for initialization
	void Start () {
		pot = 0;
		topBet = 0;
		turnNumber = 0;
		tableCardNumber = 1;
		Debug.Log ("Manager started");

		playerUi = Factory.playerUi;
		player = Factory.player;
		enemies = Factory.enemies;
		deck = Factory.deck;
		tableCards = new List<Card>();
		allPlayers = new List<GameObject> ();
		besthands = new List<GameObject>();

		if (player.GetComponent<Unit>().chips > 0)
			allPlayers.Add (player);

		else
		{
			Factory.statusText.GetComponent<Text> ().text = "Player ran out of chips, given " + player.GetComponent<Unit> ().startingChips + " chips";
			player.GetComponent<Unit>().chips = player.GetComponent<Unit>().startingChips;
			allPlayers.Add(player);
		}

		foreach (GameObject e in enemies)
		{
			if (e.GetComponent<Unit>().chips > 0)
			{
				allPlayers.Add(e);
			}
			else
			{
				e.GetComponent<Unit>().chips = e.GetComponent<Unit>().startingChips;
				allPlayers.Add(e);
			}
		}

		//playerUi.SetActive(false);

		Debug.Log("Player order");
		foreach(GameObject o in allPlayers)
		{
			Debug.Log(o.GetComponent<Unit>().name);
		}

		ResetTurns();
		ResetBets();

		Deal(2);
		GivePlayerOneCard ();

		player.GetComponent<Player>().PutCardsOnScreenPineapple();

		Debug.Log(player.GetComponent<Unit>().hand[0]);
		Debug.Log(player.GetComponent<Unit>().hand[1]);
		NextTurn();
	}

	public void UnitFolded(GameObject u)
	{
		// If the unit folds, remove them from the turn cycle
		turns.Remove(u);
		if (turnIndex < endIndex)
			endIndex--;
		if (endIndex >= turns.Count)
			endIndex = turns.Count - 1;
		turnIndex--;
	}

	public void ResetBets()
	{
		amountBet = new Dictionary<GameObject, int>();
		topBet = 0;

		foreach (GameObject u in turns)
		{
			amountBet.Add(u, 0);
		}
	}

	public void Deal(int numCards)
	{
		for (int i = 0; i < numCards; i++)
		{
			foreach (GameObject u in turns)
			{
				u.GetComponent<Unit>().AddCard(deck[0]);
				deck.RemoveAt(0);
			}
		}
	}

	public void GivePlayerOneCard()
	{
		player.GetComponent<Unit> ().AddCard (deck [0]);
		deck.RemoveAt (0);
	}

	public void AddTableCards(int numCards)
	{
		for (int i = 0; i < numCards; i++)
		{
			tableCards.Add(deck[0]);
			foreach (GameObject u in turns)
			{
				u.GetComponent<Unit>().AddCard(deck[0]);
			}

			string cName = CardToString(deck[0]);

			GameObject tableCard = GameObject.Find("Table Card " + tableCardNumber);
			GameObject camTableCard = GameObject.Find("Cam Table Card " + tableCardNumber);

			tableCard.GetComponent<MeshRenderer>().enabled = true;
			camTableCard.GetComponent<MeshRenderer>().enabled = true;

			tableCard.GetComponent<MeshFilter>().mesh = GameObject.Find(cName).GetComponent<MeshFilter>().mesh;
			camTableCard.GetComponent<MeshFilter>().mesh = GameObject.Find(cName).GetComponent<MeshFilter>().mesh;

			tableCardNumber++;
			deck.RemoveAt(0);
		}
	}

	private string CardToString(Card c)
	{
		string cName = "";

		if (c.suit == (int)Factory.suits.Clubs)
			cName += "C";
		else if (c.suit == (int)Factory.suits.Diamonds)
			cName += "D";
		else if (c.suit == (int)Factory.suits.Hearts)
			cName += "H";
		else if (c.suit == (int)Factory.suits.Spades)
			cName += "S";

		if (c.value == 12)
			cName += "1";
		else if (c.value == 9)
			cName += "j";
		else if (c.value == 10)
			cName += "q";
		else if (c.value == 11)
			cName += "k";
		else
			cName += (c.value + 2).ToString();

		return cName;
	}


	// Update is called once per frame
	void Update () {

	}

	IEnumerator WaitNextGame()
	{
		yield return new WaitForSeconds(2);
		Factory.factory.NewGame();
	}

	public void ResetTurns()
	{
		turnIndex = -1;
		turns = new List<GameObject>(allPlayers);
		endIndex = turns.Count - 1;

		foreach (GameObject go in turns)
		{
			if (go.GetComponent<Unit>().chips > 0)
				go.GetComponent<Unit>().allIn = false;
			go.GetComponent<Unit>().hand = new List<Card>();
		}
	}

	private void OnDestroy()
	{
		for (int i = 1; i <= 5; i++)
		{
			GameObject.Find("Table Card " + i).GetComponent<MeshRenderer>().enabled = false;
			GameObject.Find("Cam Table Card " + i).GetComponent<MeshRenderer>().enabled = false;
		}
	}

	public void NextTurn()
	{
		// If everyone else folded, award the pot
		if (turns.Count == 1)
		{
			turns[0].GetComponent<Unit>().chips += pot;

			Debug.Log("Pot awarded to " + turns[0].GetComponent<Unit>().name + " (" + pot + ")");
			Debug.Log("Game over");

			pot = 0;

			Factory.factory.NewGame();
			return;
		}

		if (turnIndex == endIndex)
		{
			// Do the next round 
			// i.e. put cards on the table and stuff
			turnNumber++;
			Debug.Log("Next turn!!!!");
			ResetBets();

			turnIndex = -1;
			endIndex = turns.Count - 1;

			Debug.Log("Table Cards");
			if (turnNumber == 1)
				AddTableCards(3);

			else if (turnNumber == 2 || turnNumber == 3)
				AddTableCards(1);

			foreach (Card c in tableCards)
			{
				Debug.Log("Suit: " + c.suit + " Value: " + c.value);
			}

			if (turnNumber == 4)
			{
				// Judge the hands
				// Award the pot to the winner
				GameObject winner = null;
				int bestVal = 0;
				foreach (GameObject go in turns)
				{
					Debug.Log(go.GetComponent<Unit>().name + " hand:");
					foreach (Card c in go.GetComponent<Unit>().hand)
						Debug.Log("Suit: " + c.suit + " Value: " + c.value);

					go.GetComponent<Unit>().ScoreTheHand();
					int hv = go.GetComponent<Unit>().handValue;

					Debug.Log("Scored: " + hv);


				}

				foreach (GameObject winners in turns)
				{
					int winVal = winners.GetComponent<Unit>().handValue;

					if(bestVal<winVal)
					{
						besthands.Clear();
						besthands.Add(winners);
						bestVal = winVal;
					}
					else
						if(winVal == bestVal)
						{
							besthands.Add(winners);
						}
				}


				if(besthands.Count > 1)
				{
					besthands = CheckKickerCards(besthands);
					if (besthands.Count > 1)
					{
						string winners = "";
						Debug.Log("Pot is split " + besthands.Count + " ways");
						foreach (GameObject u in besthands)
						{
							u.GetComponent<Unit>().chips += (pot / besthands.Count);
							Debug.Log(u.GetComponent<Unit>().name + " won " + (pot / besthands.Count));
							winners += u.GetComponent<Unit> ().name + " won " + (pot / besthands.Count) + "\n";
							Debug.Log("Winning hand:");
							foreach (Card c in u.GetComponent<Unit>().hand)
								Debug.Log("Suit: " + c.suit + " Value: " + c.value);
						}
						Factory.statusText.GetComponent<Text> ().text = winners;
					}
					else
					{
						winner = besthands[0];
						Debug.Log(winner.GetComponent<Unit>().name + " won " + pot);
						Factory.statusText.GetComponent<Text> ().text = winner.GetComponent<Unit> ().name + " won " + pot;
						winner.GetComponent<Unit>().chips += pot;
						pot = 0;
						Debug.Log("Winning hand:");
						foreach (Card c in winner.GetComponent<Unit>().hand)
							Debug.Log("Suit: " + c.suit + " Value: " + c.value);
					}
				}
				else
				{
					winner = besthands[0];
					Debug.Log(winner.GetComponent<Unit>().name + " won " + pot);
					Factory.statusText.GetComponent<Text> ().text = winner.GetComponent<Unit> ().name + " won " + pot;
					winner.GetComponent<Unit>().chips += pot;
					pot = 0;
					Debug.Log("Winning hand:");
					foreach (Card c in winner.GetComponent<Unit>().hand)
						Debug.Log("Suit: " + c.suit + " Value: " + c.value);
				}


				Debug.Log("Game over");

				StartCoroutine (WaitNextGame ());

				return; // Return so that it all ends
			}


		}

		turnIndex++;

		if (turnIndex >= turns.Count)
			turnIndex = 0;

		GameObject unitTurn = turns [turnIndex];

		if (unitTurn.name == "Player") 
		{
			player.GetComponent<Player>().playerTurn = true;
			//playerUi.SetActive (true);
			player.GetComponent<Player>().SetupUI();

			Debug.Log ("Player's Turn");
		} 
		else 
		{
			//playerUi.SetActive (false);
			Debug.Log ("Enemy (" + unitTurn.GetComponent<Enemy>().name + ") turn");

			unitTurn.GetComponent<Enemy>().AIThink();
		}
	}


	public List<GameObject> CheckKickerCards(List<GameObject> posWinners)
	{
		int highestkicker1 = -1;
		int[] highestkicker2 = new int[2];
		int[] highestkicker3 = new int[3];
		int[] highestkicker4 = new int[4];
		int[] highestflush = new int[5];
		int flushflag = 0;
		int highestflushchange = 0;
		List<GameObject> finalWinners = new List<GameObject>();
		if (posWinners[0].GetComponent<Unit>().handValue >= 8000)
		{
			foreach (GameObject u in posWinners)
			{
				u.GetComponent<Unit>().k4kicker.Sort();
				if(highestkicker1 < u.GetComponent<Unit>().k4kicker[2])
				{
					finalWinners.Clear();
					finalWinners.Add(u);
					highestkicker1 = u.GetComponent<Unit>().k4kicker[2];
				}
				else
				{
					if(highestkicker1 == u.GetComponent<Unit>().k4kicker[2])
					{
						finalWinners.Add(u);
					}
				}
			}
			return finalWinners;
		}
		if(posWinners[0].GetComponent<Unit>().handValue >= 6000)
		{
			foreach(GameObject u in posWinners)
			{
				u.GetComponent<Unit>().flushkicker.Sort();
				if(flushflag ==0)
				{
					highestflush = u.GetComponent<Unit>().flushkicker.ToArray();
					finalWinners.Add(u);
					flushflag = 1;
				}
				for(int i =4; i>=0; i--)
				{
					if(highestflush[i]<u.GetComponent<Unit>().flushkicker[i])
					{
						finalWinners.Clear();
						finalWinners.Add(u);
						highestflush = u.GetComponent<Unit>().flushkicker.ToArray();
						highestflushchange = 1;
						break;
					}
					else
					{
						if(highestflush[i] > u.GetComponent<Unit>().flushkicker[i])
						{
							break;
						}
					}
				}
				if (highestflushchange == 0)
					finalWinners.Add(u);

			}
			return finalWinners;
		}
		if(posWinners[0].GetComponent <Unit>().handValue >=5000)
		{
			foreach(GameObject u in posWinners)
			{
				finalWinners.Add(u);
			}
			return finalWinners;
		}
		if(posWinners[0].GetComponent <Unit>().handValue >=4000)
		{
			foreach (GameObject u in posWinners)
			{
				u.GetComponent<Unit>().k3kicker.Sort();
				if (highestkicker2[1] < u.GetComponent<Unit>().k3kicker[3])
				{
					finalWinners.Clear();
					finalWinners.Add(u);
					highestkicker2[1] = u.GetComponent<Unit>().k3kicker[3];
				}
				else
				{
					if (highestkicker2[1] == u.GetComponent<Unit>().k3kicker[3])
					{
						if(highestkicker2[0]< u.GetComponent <Unit>().k3kicker[2])
						{
							finalWinners.Clear();
							finalWinners.Add(u);
							highestkicker2[0] = u.GetComponent<Unit>().k3kicker[2];
						}
						else if(highestkicker2[0] == u.GetComponent<Unit>().k3kicker[2])
						{
							finalWinners.Add(u);
						}
					}
				}
			}
			return finalWinners;
		}
		if(posWinners[0].GetComponent <Unit>().handValue>= 3000)
		{
			foreach (GameObject u in posWinners)
			{
				u.GetComponent<Unit>().twopkicker.Sort();
				if (highestkicker1 < u.GetComponent<Unit>().twopkicker[2])
				{
					finalWinners.Clear();
					finalWinners.Add(u);
					highestkicker1 = u.GetComponent<Unit>().twopkicker[2];
				}
				else
				{
					if (highestkicker1 == u.GetComponent<Unit>().twopkicker[2])
					{
						finalWinners.Add(u);
					}
				}
			}
			return finalWinners;
		}
		if(posWinners[0].GetComponent<Unit>().handValue>=2000)
		{
			foreach (GameObject u in posWinners)
			{
				u.GetComponent<Unit>().pairkicker.Sort();
				if (highestkicker3[2] < u.GetComponent<Unit>().pairkicker[3])
				{
					finalWinners.Clear();
					finalWinners.Add(u);
					highestkicker3[2] = u.GetComponent<Unit>().pairkicker[3];
				}
				else
				{
					if (highestkicker3[2] == u.GetComponent<Unit>().pairkicker[3])
					{
						if (highestkicker3[1] < u.GetComponent<Unit>().pairkicker[2])
						{
							finalWinners.Clear();
							finalWinners.Add(u);
							highestkicker3[1] = u.GetComponent<Unit>().pairkicker[2];
						}
						else
						{
							if (highestkicker3[1] == u.GetComponent<Unit>().pairkicker[2])
							{
								if(highestkicker3[0] < u.GetComponent<Unit>().pairkicker[1])
								{
									finalWinners.Clear();
									finalWinners.Add(u);
									highestkicker3[0] = u.GetComponent<Unit>().pairkicker[1];
								}
								else if(highestkicker3[0] == u.GetComponent<Unit>().pairkicker[1])
								{
									finalWinners.Add(u);
								}
							}
						}
					}
				}
			}
			return finalWinners;
		}
		if(posWinners[0].GetComponent<Unit>().handValue >=1000)
		{
			foreach (GameObject u in posWinners)
			{
				u.GetComponent<Unit>().highCkicker.Sort();
				if (highestkicker4[3] < u.GetComponent<Unit>().highCkicker[4])
				{
					finalWinners.Clear();
					finalWinners.Add(u);
					highestkicker4[3] = u.GetComponent<Unit>().highCkicker[4];
				}
				else
				{
					if (highestkicker4[3] == u.GetComponent<Unit>().highCkicker[4])
					{
						if (highestkicker4[2] < u.GetComponent<Unit>().highCkicker[3])
						{
							finalWinners.Clear();
							finalWinners.Add(u);
							highestkicker4[2] = u.GetComponent<Unit>().highCkicker[3];
						}
						else
						{
							if (highestkicker4[2] == u.GetComponent<Unit>().highCkicker[3])
							{
								if (highestkicker4[1] < u.GetComponent<Unit>().highCkicker[2])
								{
									finalWinners.Clear();
									finalWinners.Add(u);
									highestkicker4[1] = u.GetComponent<Unit>().highCkicker[2];
								}
								else 
									if (highestkicker4[1] == u.GetComponent<Unit>().highCkicker[2])
									{
										if (highestkicker4[0] < u.GetComponent<Unit>().highCkicker[1])
										{
											finalWinners.Clear();
											finalWinners.Add(u);
											highestkicker4[0] = u.GetComponent<Unit>().highCkicker[1];
										}
										else if (highestkicker4[0] == u.GetComponent<Unit>().highCkicker[1])
										{
											finalWinners.Add(u);
										}
									}
							}
						}
					}
				}
			}
			return finalWinners;
		}
		return finalWinners;
	}
}
