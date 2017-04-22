using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Unit {
    
	public double bluffChance;

    public GameObject enemyObject; // Should be set on instantiation
	private Text statusText;

	// Use this for initialization
	void Start () 
	{
		// This will seemingly do something eventually?
		bluffChance = UnityEngine.Random.value;
        managerObject = Factory.manager;
        manager = managerObject.GetComponent<Manager>();
		statusText = Factory.statusText.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Bet()
    {
        // Do some AI bet logic here
        
        // Get the betAmount
        int betAmount = (int)(chips * UnityEngine.Random.value);

        if (chips == 1)
            betAmount = 1;
        if (betAmount == 0)
            betAmount = 1;
        manager.topBet = betAmount;
        manager.amountBet[enemyObject] += betAmount;
        manager.pot += betAmount;

        // Need to do a new turn of betting, set the manager endIndex
        // to be 1 before its current position
        manager.endIndex = manager.turnIndex - 1;
        if (manager.endIndex == -1)
            manager.endIndex = manager.turns.Count - 1;

        Debug.Log("Enemy (" + name + ") bet " + betAmount + " chips");
		statusText.text = "Enemy (" + name + ") bet " + betAmount + " chips";

        chips -= betAmount;

        if (chips == 0)
        {
            allIn = true;
            Debug.Log("Enemy (" + name + ") just went allin!");
			statusText.text = "Enemy (" + name + ") bet " + chips + " chips and is all-in";
        }
    }

    public void Call()
    {
        // Get the deficit that we are calling
        int betAmount = manager.topBet - manager.amountBet[enemyObject];

        manager.amountBet[enemyObject] += betAmount;
        manager.pot += betAmount;

		statusText.text = "Enemy (" + name + ") called " + betAmount + " chips";
        Debug.Log("Enemy (" + name + ") called " + betAmount + " chips");

        if (chips == 0)
        {
            allIn = true;
			statusText.text = "Enemy (" + name + ") called " + chips + " chips and is all-in";
            Debug.Log("Enemy (" + name + ") just went allin!");
        }

		chips -= betAmount;
    }

    public void Raise()
    {
        // Max value a raise can be
        int max = (manager.amountBet[enemyObject]+chips) - manager.topBet;

        int raiseAmount = manager.topBet + (int)(max * UnityEngine.Random.value);
        if (raiseAmount == manager.topBet)
            raiseAmount = 1+manager.topBet;
        int giving = raiseAmount - manager.amountBet[enemyObject];
        chips -= giving;
        manager.pot += giving;
        manager.amountBet[enemyObject] += giving;
        manager.topBet = raiseAmount;

        // Need to do a new turn of betting, set the manager endIndex
        // to be 1 before its current position
        manager.endIndex = manager.turnIndex - 1;
        if (manager.endIndex == -1)
            manager.endIndex = manager.turns.Count - 1;

		statusText.text = "Enemy (" + name + ") raised to " + raiseAmount + " chips";
        Debug.Log("Enemy (" + name + ") raised to " + raiseAmount + " chips");
        Debug.Log("(" + name + ") gave " + giving + " chips");

        if (chips == 0)
        {
            allIn = true;
            Debug.Log("Enemy (" + name + ") just went allin!");
			statusText.text = "Enemy (" + name + ") just went allin with " + raiseAmount + " chips";
        }
    }

    public void Fold()
    {
        manager.UnitFolded(enemyObject);
        Debug.Log("Enemy (" + name + ") folded");
		statusText.text = "Enemy (" + name + ") folded";
    }

    public void Check()
    {
        Debug.Log("Enemy (" + name + ") checked");
		statusText.text = "Enemy (" + name + ") checked";
    }

    IEnumerator EnemyWait()
    {
        yield return new WaitForSeconds(2);
        MakeMove();
    }

    public delegate void MoveDelegate();

    public void AIThink()
    {
        if (allIn)
        {
            manager.NextTurn();
            return;
        }  
        // Add delay to wait so that the enemies don't take their turns
        // instantly
        StartCoroutine(EnemyWait());
    }

	public void MakeMove()
	{
        // First get the possible choices we can make
        // based on the current bets, etc.
        // e.g. Can we bet or raise? Can I check?
        List<MoveDelegate> validMoves = new List<MoveDelegate>();

        // We can always Fold
        validMoves.Add(Fold);

        //Ai calculates what it has
        ScoreTheHand();


        // Get the bet deficit
        int betDeficit = manager.topBet - manager.amountBet[enemyObject];

        if (betDeficit == 0)
        {
            validMoves.Add(Check);
            if (chips > 0)
                validMoves.Add(Bet);
        }

        if (betDeficit > 0)
        {
            if (chips >= betDeficit)
                validMoves.Add(Call);
            
            if (chips > betDeficit)
                validMoves.Add(Raise);
        }

        // After getting the possible moves, choose one
        // For now, I will do so randomly. Later, add some AI deicion making
        //int choice = (int)(UnityEngine.Random.value * validMoves.Count);
        int choice;
        if(validMoves.Contains(Check))
        {
            if(handValue>=2000)
            {
                if(validMoves.Contains(Bet))
                {
                    choice = 2;
                }
                else
                {
                    choice = 1;
                }
            }
            else
            {
                choice = (int)(bluffChance * validMoves.Count);
                if(choice == 0)
                {
                    if(handValue>=1010)
                    {
                        choice = 1;
                    }
                    else
                    {
                        choice = 0;
                    }
                }
                
                
                
                
            }
        }else
        {
            if(validMoves.Contains(Call))
            {
                if(handValue>=3000)
                {
                    if(handValue>=6000)
                    {
                        if(validMoves.Contains(Raise))
                        {
                            choice = 2;
                        }
                        else
                        {
                            choice = 1;
                        }
                    }
                    else
                    {
                        choice = 1;
                    }
                }
                else
                {
                    choice = (int)(bluffChance * validMoves.Count);
                }
            }
            else
            {
                choice = 0;
            }
        }

    

        // Call the function stored at position choice
        validMoves[choice]();
        manager.NextTurn();
	}
}
