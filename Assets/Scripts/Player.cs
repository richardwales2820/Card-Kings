using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : Unit
{
	public int idNumber;
	public string password;
	public int wins;
	public int losses;
	public double largestPotWin;
    public double winLossRatio;
    public bool playerTurn;
    
    private GameObject playerObject;
    private GameObject slider;
	private GameObject playerChipsText;
    private List<GameObject> UIElements;
	private Voice voice;
	private Text statusText;
	private AudioManager audioManager;
    #region Old Constructor
    /*
    public Player(string n, int id, string pass, int w, int l, double lPW, int chps)
	{
		name = n;
		idNumber = id;
		password = pass;
		wins = w;
		losses = l;
		largestPotWin = lPW;
		chips = chps;
		wins = 1;
        winLossRatio = wins / (wins+losses);
	}
    */
    #endregion

    private void Start()
    {
        // Pull info from the database and get all the respective data
        // Old constructor above ^
        // name = PullFromDatabase(name);
        // Do database calls and get the info that way
        // The player ID can be passed so that we know what data to pull
        // Log in screen will give us the ID
        name = "Player";
        playerTurn = false;

        managerObject = Factory.manager;
		manager = managerObject.GetComponent<Manager>();

        playerObject = Factory.player;

		playerChipsText = Instantiate (Factory.playerChipsTextObject, new Vector3(0,0,0), Quaternion.identity, Factory.playerUi.transform);
		playerChipsText.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (100,-50, 0);
		playerChipsText.GetComponent<Text> ().text = chips + " chips";
		voice = Factory.voiceObject.GetComponent<Voice> ();
		statusText = Factory.statusText.GetComponent<Text> ();
		audioManager = Factory.audioManager;
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

    public void PutCardsOnScreenTexas()
    {
        string c1Name = CardToString(hand[0]);
        string c2Name = CardToString(hand[1]);
        
        GameObject c1 = Factory.card1Object;
        GameObject c2 = Factory.card2Object;

        c1.SetActive(true);
        c2.SetActive(true);

        c1.GetComponent<MeshFilter>().mesh = GameObject.Find(c1Name).GetComponent<MeshFilter>().mesh;
        c2.GetComponent<MeshFilter>().mesh = GameObject.Find(c2Name).GetComponent<MeshFilter>().mesh;
    }

	public void PutCardsOnScreenPineapple()
	{
		string c1Name = CardToString(hand[0]);
		string c2Name = CardToString(hand[1]);
		string c3Name = CardToString(hand[2]);

		GameObject c1 = Factory.card1Object;
		GameObject c2 = Factory.card2Object;
		GameObject c3 = Factory.card3Object;

		c1.SetActive(true);
		c2.SetActive(true);
		c3.SetActive (true);

		c1.GetComponent<MeshFilter>().mesh = GameObject.Find(c1Name).GetComponent<MeshFilter>().mesh;
		c2.GetComponent<MeshFilter>().mesh = GameObject.Find(c2Name).GetComponent<MeshFilter>().mesh;
		c3.GetComponent<MeshFilter>().mesh = GameObject.Find(c3Name).GetComponent<MeshFilter>().mesh;
	}

	// Returns true on success
	public bool ParseString(string move)
	{
		voice.ResetVoice ();
		Debug.Log (move);
		move = move.ToLower ();

		if (move.Substring (0, 3).Equals ("bet")) {
			int chipAmount = 0;

			if (int.TryParse (move.Substring (4), out chipAmount)) {
				Debug.Log ("Betting " + chipAmount + " chips");
			} else {
				Debug.Log ("Failed to parse");
				return false;
			}
		} else if (move.Substring (0, 4).Equals ("fold")) {
			Fold ();
			return true;
		} else if (move.Substring (0, 4).Equals ("call")) {
			int chipAmount = 0;

			if (int.TryParse (move.Substring (5), out chipAmount)) {
				Debug.Log ("Calling " + chipAmount + " chips");
			} else {
				Debug.Log ("Failed to parse");
				return false;
			}
		} else if (move.Substring (0, 5).Equals ("check")) {
			Check ();
			return true;
		} else if (move.Substring (0, 5).Equals ("raise")) {
			int chipAmount = 0;

			if (int.TryParse (move.Substring (6), out chipAmount)) {
				Debug.Log ("Raising " + chipAmount + " chips");
			} else {
				Debug.Log ("Failed to parse");
				return false;
			}
		}

		return false;
	}

    private void Update()
    {
		playerChipsText.GetComponent<Text> ().text = chips + " chips";
        // The manager will set this bool to true when we want input
        if (playerTurn)
        {
			voice.playersTurn = true;

			if (voice.GetVoice () != null) 
			{
				if (ParseString (voice.GetVoice ())) 
				{
					//playerTurn = false;
					//manager.NextTurn ();
				} 
				else 
				{
					voice.ResetVoice ();
				}
			}

            if (allIn)
            {
                playerTurn = false;
                manager.NextTurn();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                // Fold
                manager.UnitFolded(playerObject);
                playerTurn = false;

                Debug.Log("Player folded");

                manager.NextTurn();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                // Raise

                // Get this amount from the player somehow (slider?)
                int amountRaise = manager.topBet + 5;

                // If we owe something
                if (manager.topBet - manager.amountBet[playerObject] != 0)
                {
                    if (chips >= amountRaise)
                    {
                        int given = amountRaise - manager.amountBet[playerObject];
                        chips -= given;
                        manager.pot += given;
                        manager.amountBet[playerObject] += given;
                        manager.topBet = amountRaise;

                        // Need to do a new turn of betting, set the manager endIndex
                        // to be 1 before its current position
                        manager.endIndex = manager.turnIndex - 1;
                        if (manager.endIndex == -1)
                            manager.endIndex = manager.turns.Count - 1;

                        Debug.Log("Player made a raise to " + amountRaise + " chips");
                        Debug.Log("Gave " + given + " chips");

                        playerTurn = false;
                        manager.NextTurn();
                    }

                    // We can't afford the current raise
                    else
                    {
                        Debug.Log("You do not have enough chips");
                        Debug.Log("Have (" + chips + ") Need (" + amountRaise + ")");
                    }
                }

                // If we cannot raise
                else
                {
                    Debug.Log("No valid raise available");
                }
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                // Call

                // If the top needed bet is not how much the player has put in,
                // then we can make a valid call, giving the deficit
                if (manager.topBet - manager.amountBet[playerObject] != 0)
                {
                    // If we have enough chips to call, then give the chips to the pot
                    if (chips >= manager.topBet - manager.amountBet[playerObject])
                    {
                        int giving = manager.topBet - manager.amountBet[playerObject];

                        manager.amountBet[playerObject] += giving;
                        manager.pot += giving;
                        chips -= giving;

                        Debug.Log("Player called " + manager.topBet + " chips");
                        Debug.Log("Player had to give " + giving + " chips");
                    }

                    // If we don't have enough chips, subtract everything, then make
                    // a side pot
                    else
                    {
                        Debug.Log("Player called " + chips + " chips");

                        manager.pot += chips;
                        chips = 0;

                        // Make a side pot somehow in manager
                        // Also mark the player as all-in so that we do not bother them for
                        // Betting, calling, checking anymore
                    }

                    playerTurn = false;
                    manager.NextTurn();
                }

                // There is nothing to call
                else
                {
                    Debug.Log("There is no bet to call");
                }
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                // Bet
                // Test bet just adds 5
                // Will get the amount from a slider of some sorts
                int betAmount = 5;

                // We can only "Bet" if we do not have a chip deficit
                if (manager.topBet - manager.amountBet[playerObject] == 0)
                {
                    // If we can afford the bet
                    if (chips >= betAmount)
                    {
                        manager.pot += betAmount;
                        manager.amountBet[playerObject] += betAmount;
                        chips -= betAmount;
                        manager.topBet = betAmount;

                        // Need to do a new turn of betting, set the manager endIndex
                        // to be 1 before its current position
                        manager.endIndex = manager.turnIndex - 1;
                        if (manager.endIndex == -1)
                            manager.endIndex = manager.turns.Count - 1;

                        Debug.Log("Player bet " + betAmount + " chips");
                        playerTurn = false;
                        manager.NextTurn();
                    }

                    else
                    {
                        Debug.Log("You do not have enough chips");
                        Debug.Log("Have (" + chips + ") Need (" + betAmount + ")");
                    }
                }
                
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                // Check, don't do anything to the pot

                // First check that we can make a check. If there is no chip
                // deficit with the player, then we can check
                if (manager.topBet - manager.amountBet[playerObject] == 0)
                {
                    Debug.Log("Player check");
                    playerTurn = false;
                    manager.NextTurn();
                }
                
                // We can't check, alert the player
                else
                {
                    Debug.Log("Can't check! You owe " + (manager.topBet - manager.amountBet[playerObject]) + " chips!");
                }
            }
        }
    }
    
    // Based on the current game state, show buttons for valid moves
    // Also set sliders to possible amounts
    public void SetupUI()
    {
        if (allIn)
            return;

        UIElements = new List<GameObject>();

		Vector3 startV = Vector3.zero;
        GameObject foldButton = Instantiate(Factory.foldButtonObject, startV, Quaternion.identity, Factory.playerUi.transform);
        foldButton.GetComponent<Button>().onClick.AddListener(delegate { Fold(); });

        slider = Instantiate(Factory.sliderObject, startV, 
            Quaternion.identity, Factory.playerUi.transform);
		slider.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0, 50, 0);
        slider.GetComponent<Slider>().minValue = 1;

        GameObject prevSlider = GameObject.Find("SliderText(Clone)");
        if (prevSlider != null)
            Destroy(prevSlider);

        GameObject sliderText = Instantiate(Factory.sliderTextObject, startV + new Vector3(foldButton.GetComponent<RectTransform>().rect.width + slider.GetComponent<RectTransform>().rect.width, 20 + foldButton.GetComponent<RectTransform>().rect.height),
            Quaternion.identity, Factory.playerUi.transform);
        
		sliderText.GetComponent<RectTransform>().anchoredPosition = new Vector3 (200, 50, 0);
        
		if (manager.amountBet == null)
			Debug.Log ("Null amount bet");

		if (manager.topBet == null)
			Debug.Log ("Null top bet");

        // If we owe something
        if (manager.topBet - manager.amountBet[playerObject] != 0)
        {
			GameObject raiseBut = Instantiate(Factory.raiseButObject, startV,
				Quaternion.identity, Factory.playerUi.transform);
			raiseBut.GetComponent<RectTransform> ().anchoredPosition = new Vector3(0,15,0);
			GameObject callBut = Instantiate(Factory.callButObject, new Vector3(0, 0, 0),
				Quaternion.identity, Factory.playerUi.transform);
			callBut.GetComponent<RectTransform> ().anchoredPosition = new Vector3(-80,15,0);

			raiseBut.GetComponent<Button>().onClick.AddListener(delegate { Raise(); });
			callBut.GetComponent<Button>().onClick.AddListener(delegate { Call(); });

            sliderText.GetComponent<Text>().text = "+1 chip(s)";
            slider.GetComponent<Slider>().onValueChanged.AddListener(delegate { sliderText.GetComponent<Text>().text = ("+" + (int)slider.GetComponent<Slider>().value).ToString() + " chip(s)"; });
            slider.GetComponent<Slider>().maxValue = chips - (manager.topBet - manager.amountBet[playerObject]);

            UIElements.Add(raiseBut);
			UIElements.Add(callBut);
        }
        
        // If we don't owe anything, then we can bet or check
        else
        {
			GameObject betBut = Instantiate(Factory.betButObject, startV,
                Quaternion.identity, Factory.playerUi.transform);
			betBut.GetComponent<RectTransform> ().anchoredPosition = new Vector3(0,15,0);
			GameObject checkBut = Instantiate(Factory.checkButObject, new Vector3(0, 0, 0),
				Quaternion.identity, Factory.playerUi.transform);
			checkBut.GetComponent<RectTransform> ().anchoredPosition = new Vector3(-80,15,0);

			betBut.GetComponent<Button>().onClick.AddListener(delegate { Bet(); });
			checkBut.GetComponent<Button>().onClick.AddListener(delegate { Check(); });

            sliderText.GetComponent<Text>().text = "1 chip(s)";
            slider.GetComponent<Slider>().onValueChanged.AddListener(delegate { sliderText.GetComponent<Text>().text = ((int)slider.GetComponent<Slider>().value).ToString() + " chip(s)"; });
            slider.GetComponent<Slider>().maxValue = chips;

            UIElements.Add(betBut);
			UIElements.Add(checkBut);
        }

        UIElements.Add(foldButton);
        UIElements.Add(slider);
        UIElements.Add(sliderText);

    }

    public void DestroyUI()
    {
        foreach(GameObject g in UIElements)
        {
            Destroy(g);
        }
    }

    public void SetID(int id)
    {
        idNumber = id;
    }

    public void Bet()
	{
        int betAmount = (int)slider.GetComponent<Slider>().value;

        manager.pot += betAmount;
        manager.amountBet[playerObject] += betAmount;
        chips -= betAmount;
        manager.topBet = betAmount;

        // Need to do a new turn of betting, set the manager endIndex
        // to be 1 before its current position
        manager.endIndex = manager.turnIndex - 1;
        if (manager.endIndex == -1)
            manager.endIndex = manager.turns.Count - 1;

        Debug.Log("Player bet " + betAmount + " chips");
        playerTurn = false;

        if (chips == 0)
        {
            allIn = true;
            Debug.Log("Player went allin!");
        }

		statusText.text = "Player bet " + betAmount + " chips";

		audioManager.PlayChip ();

        DestroyUI();
        manager.NextTurn();
    }

    public void Call()
    {
        // If we have enough chips to call, then give the chips to the pot
        if (chips >= manager.topBet - manager.amountBet[playerObject])
        {
            if (manager.amountBet[playerObject] > manager.topBet)
                manager.amountBet[playerObject] = 0;
            int giving = manager.topBet - manager.amountBet[playerObject];

            manager.amountBet[playerObject] += giving;
            manager.pot += giving;
            chips -= giving;

            Debug.Log("Player called " + manager.topBet + " chips");
            Debug.Log("Player had to give " + giving + " chips");
			statusText.text = "Player called " + manager.topBet + " chips";
        }

        // If we don't have enough chips, subtract everything, then make
        // a side pot
        else
        {
            Debug.Log("Player called " + chips + " chips");

            manager.pot += chips;
            chips = 0;

            allIn = true;
            Debug.Log("Player went allin!");
			statusText.text = "Player went all-in with " + chips + " chips";
            // Make a side pot somehow in manager
            // Also mark the player as all-in so that we do not bother them for
            // Betting, calling, checking anymore
        }
		audioManager.PlayChip ();

        DestroyUI();
        manager.NextTurn();
    }

    public void Fold()
    {
        manager.UnitFolded(playerObject);
        playerTurn = false;

        Debug.Log("Player folded");
		statusText.text = "Player folded";
		audioManager.PlayLoss ();

        DestroyUI();
        manager.NextTurn();
    }

    public void Raise()
    {
        int amountRaise = manager.topBet + (int)slider.GetComponent<Slider>().value;
        int given = amountRaise - manager.amountBet[playerObject];
        chips -= given;
        manager.pot += given;
        manager.amountBet[playerObject] += given;
        manager.topBet = amountRaise;

        // Need to do a new turn of betting, set the manager endIndex
        // to be 1 before its current position
        manager.endIndex = manager.turnIndex - 1;
        if (manager.endIndex == -1)
            manager.endIndex = manager.turns.Count - 1;

        Debug.Log("Player made a raise to " + amountRaise + " chips");
        Debug.Log("Gave " + given + " chips");
		statusText.text = "Player made a raise to " + amountRaise + " chips";
        if (chips == 0)
        {
            allIn = true;
            Debug.Log("Player went allin!");
			statusText.text = "Player made a raise to " + amountRaise + " chips and went all-in";
        }

		audioManager.PlayChip ();

        playerTurn = false;
        DestroyUI();
        manager.NextTurn();
    }
    
    public void Check()
    {
        Debug.Log("Player check");
		statusText.text = "Player checked";
        playerTurn = false;
        DestroyUI();
        manager.NextTurn();
    }

    public void SetLargestPotWin(int currentPot)
    {
        if(currentPot > largestPotWin)
        {
            largestPotWin = currentPot;
        }
    }
}
