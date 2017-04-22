using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class Unit : MonoBehaviour
{
	// Comment for Eric
    public int startingChips = 1000;
    public int chips;
	public int handValue;
    public String name;
    public List<Card> hand;
	public int[][] bucket;
    public List<int> k3kicker;
	public List<int> flushkicker;
	public List<int> k4kicker;
    public List<int> twopkicker;
    public List<int> pairkicker;
    public List<int> highCkicker;
    public bool allIn;

    public GameObject managerObject;
    public Manager manager;

    public Unit()
    {
        chips = startingChips;
        hand = new List<Card>();

		bucket = new int[4] [];
		bucket[0] = new int[13];
		bucket[1] = new int[13];
		bucket[2] = new int[13];
		bucket[3] = new int[13];
    }

    public void UpdateManager(GameObject mo)
    {
        managerObject = mo;
        manager = managerObject.GetComponent<ManagerTexas>();
    }

	// Use this for initialization
	void Start ()
    {
		chips = startingChips;
		hand = new List<Card>();
		flushkicker = new List<int> ();
        k4kicker = new List<int>();
        k3kicker = new List<int>();
        highCkicker = new List<int>();
        twopkicker = new List<int>();
        pairkicker = new List<int>();
		bucket = new int[4] [];
		bucket[0] = new int[13];
		bucket[1] = new int[13];
		bucket[2] = new int[13];
		bucket[3] = new int[13];

        allIn = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void AddCard(Card c)
    {
        hand.Add(c);
    }

	/*
    public virtual int Bet()
    {
        int bet = 10;
        return bet;
    }
    */
	public void ResetHand()
    {
        hand = new List<Card>();
    }	
	
    // Make the card buckets based on suit and value
	public void MakeBucket()
	{
		bucket = new int[4] [];
		bucket[0] = new int[13];
		bucket[1] = new int[13];
		bucket[2] = new int[13];
		bucket[3] = new int[13];
        flushkicker = new List<int>();
        k4kicker = new List<int>();
        k3kicker = new List<int>();
        highCkicker = new List<int>();
        twopkicker = new List<int>();
        pairkicker = new List<int>();
        foreach (Card c in hand) 
		{
			bucket[(int)c.suit][c.value] = 1;
		}
	}

    // Check each scoring possible, then choose the max
    public void ScoreTheHand()
    {
        MakeBucket();
        
        List<int> scores = new List<int>();
        scores.Add(CheckRoyalFlush());
        scores.Add(CheckStraightFlush());
        scores.Add(CheckFourofKind());
        scores.Add(CheckFullhouse());
        scores.Add(CheckFlush());
        scores.Add(CheckStraight());
        scores.Add(CheckThreeofKind());
        scores.Add(CheckTwoPair());
        scores.Add(CheckPair());
        scores.Add(CheckHighCard());

        handValue = scores.Max();
    }


    public int CheckRoyalFlush()
    {
        for(int i = 0; i < 4; i++)
        {
            if(bucket[i][8] == 1 && bucket[i][9] == 1 && bucket[i][10] == 1 && bucket[i][11] == 1 && bucket[i][12] == 1)
            {
                return 10000;
            }
        }
        return 0;
    }

    public int CheckStraightFlush()
    {
        int min = 9000;
        int best=-1;
        int count = 0;
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 13; j++)
            {
                if(bucket[i][j] == 1)
                {
                    for (int k = j; k < 12; k++)
                    {
                        if (bucket[i][k] == 1 && bucket[i][k+1] == 1)
                        {
                            count++;
                        }
                        else break;
                    }
                    if(count >= 4)
                    {
                        best = Math.Max(j+1,best);
                    }
                    count = 0;
                }
            }
        }
		if (bucket [0] [12] == 1 && bucket [0] [0] == 1 && bucket [0] [1] == 1 && bucket [0] [2] == 1 && bucket [0] [3] == 1)
			return min;
		if (bucket [1] [12] == 1 && bucket [1] [0] == 1 && bucket [1] [1] == 1 && bucket [1] [2] == 1 && bucket [1] [3] == 1)
			return min;
		if (bucket [2] [12] == 1 && bucket [2] [0] == 1 && bucket [2] [1] == 1 && bucket [2] [2] == 1 && bucket [2] [3] == 1)
			return min;
		if (bucket [3] [12] == 1 && bucket [3] [0] == 1 && bucket [3] [1] == 1 && bucket [3] [2] == 1 && bucket [3] [3] == 1)
			return min;
        if (best > -1)
        {
            return best + min;
        }
        else
            return 0;
    }

    public int CheckFourofKind ()
    {
        int min = 8000;
        int best = -1;
        
        for (int i = 0; i < 13; i++)
        {
            if (bucket[0][i] == 1 && bucket[1][i] == 1 && bucket[2][i] == 1 && bucket[3][i] == 1)
                best = i;
            else
                if (bucket[0][i] == 1 || bucket[1][i] == 1 || bucket[2][i] == 1 || bucket[3][i] == 1)
                k4kicker.Add(i);
        }

        if (best > -1)
        {
            return best + min;
        }
        else
            return 0;
    }

    public int CheckFullhouse ()
    {
        int min = 7000;
        int bestpair = -1;
        int bestthree = -1;
        int takenValue = -1;
        int paircount = 0;
        int threeofkindcount = 0;
        for (int i = 0; i < 13; i++)
        {
            if (bucket[0][i] == 1)
            {
                threeofkindcount++;
            }
            if (bucket[1][i] == 1)
            {
                threeofkindcount++;
            }
            if (bucket[2][i] == 1)
            {
                threeofkindcount++;
            }
            if (bucket[3][i] == 1)
            {
                threeofkindcount++;
            }
            if (threeofkindcount >= 3)
            {
                bestthree = Math.Max(bestthree, i+26);
                takenValue = i;
            }
            else
                threeofkindcount = 0;

        }
        if (bestthree == -1)
            return 0;
        for (int i = 0; i < 13; i++)
        {
            if (i != takenValue)
            {
                if (bucket[0][i] == 1)
                {
                    paircount++;
                }
                if (bucket[1][i] == 1)
                {
                    paircount++;
                }
                if (bucket[2][i] == 1)
                {
                    paircount++;
                }
                if (bucket[3][i] == 1)
                {
                    paircount++;
                }
                if (paircount >= 2)
                {
                    bestthree = bestthree - (12 - i);
                }
                else
                    paircount = 0;
            }
        }
        if (bestpair == -1)
        {
            return 0;
        }
        else
            return bestthree + min + bestpair;
    }

    public int CheckFlush ()
    {
        int min = 6000;
        int flushvalue = 0;
        int flushcount = 0;
        int current = 0;
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 13; j++)
            {
                if(bucket[i][j] == 1)
                {
                    for(int k = j; k < 13; k++)
                    {
						
                        if(bucket[i][k] == 1)
                        {
                            flushcount++;
                            current += k;
							flushkicker.Add (k);
                        }
                    }
                    if(flushcount>=5)
                    {
                        flushvalue = Math.Max(flushvalue, current);
                    }
                }
                current = 0;
                flushcount = 0;
            }
        }
        if (flushvalue == 0)
        {
            return 0;
        }
        else
            return flushvalue+min;
    }

    public int CheckStraight ()
    {
        int min = 5000;
        int straightcount = 0;
        int best = -1;
            for (int j = 0; j < 13; j++)
            {
                if (bucket[0][j] == 1 || bucket[1][j] == 1 || bucket[2][j] == 1 || bucket[3][j] == 1)
                {
                    for (int k = j; k < 13; k++)
                    {
                        if (bucket[0][k] == 1 || bucket[1][k] == 1 || bucket[2][k] == 1 || bucket[3][k] == 1)
                        {
                            straightcount++;
                        }
                        else break;
                    }
				
                    if (straightcount >= 5)
                    {
                        best = Math.Max(j+1, best);
                    }
                }
			straightcount = 0;
            }
        if ((bucket[0][0] == 1 || bucket[1][0] == 1 || bucket[2][0] == 1 || bucket[3][0] == 1) &&
            (bucket[0][12] == 1 || bucket[1][12] == 1 || bucket[2][12] == 1 || bucket[3][12] == 1) &&
            (bucket[0][1] == 1 || bucket[1][1] == 1 || bucket[2][1] == 1 || bucket[3][1] == 1) &&
            (bucket[0][2] == 1 || bucket[1][2] == 1 || bucket[2][2] == 1 || bucket[3][2] == 1) &&
            (bucket[0][3] == 1 || bucket[1][3] == 1 || bucket[2][3] == 1 || bucket[3][3] == 1))
            best = Math.Max(0, best);

            if (best > -1)
        {
            return best + min;
        }
        else return 0;
    }


    public int CheckThreeofKind ()
    {
        int min = 4000;
        int best = -1;

        for (int i = 0; i < 13; i++)
        {
            if ((bucket[0][i] == 1 && bucket[1][i] == 1 && bucket[2][i] == 1) || 
                (bucket[1][i] == 1 && bucket[2][i] == 1 && bucket[3][i] == 1) ||
                (bucket[0][i] == 1 && bucket[1][i] == 1 && bucket[3][i] == 1) ||
                (bucket[0][i] == 1 && bucket[2][i] == 1 && bucket[3][i] == 1))
                best = i;
            else
                if (bucket[0][i] == 1 || bucket[1][i] == 1 || bucket[2][i] == 1 || bucket[3][i] == 1)
                k3kicker.Add(i);
        }

        if (best > -1)
        {
            return best + min;
        }
        else
            return 0;
    }

    public int CheckTwoPair ()
    {
        int min = 3000;
        int bestpair2 = -1;
        int bestpair1 = -1;
        int takenValue = -1;
        int paircount2 = 0;
        int paircount1 = 0;
        for (int i = 12; i >= 0; i--)
        {
            if (bucket[0][i] == 1)
            {
                paircount1++;
            }
            if (bucket[1][i] == 1)
            {
                paircount1++;
            }
            if (bucket[2][i] == 1)
            {
                paircount1++;
            }
            if (bucket[3][i] == 1)
            {
                paircount1++;
            }
            if (paircount1 >= 2)
            {
                bestpair1 = Math.Max(bestpair1, i+26);
                takenValue = i;
                break;
            }
            else
            {
                paircount1 = 0;
            }
               

        }
        if (bestpair1 <= -1)
            return 0;
        for (int i = 12; i >= 0; i--)
        {
            if (i != takenValue)
            {
                if (bucket[0][i] == 1)
                {
                    paircount2++;
                }
                if (bucket[1][i] == 1)
                {
                    paircount2++;
                }
                if (bucket[2][i] == 1)
                {
                    paircount2++;
                }
                if (bucket[3][i] == 1)
                {
                    paircount2++;
                }
                if (paircount2 >= 2)
                {
                    bestpair1 -= (12-i);
                    bestpair2 = i;
                    paircount2 = 0;
                }
                else
                {
                    paircount2 = 0;
                    twopkicker.Add(i);
                }
                    
            }
        }
        if (bestpair2 == -1)
        {
            return 0;
        }
        else
            return bestpair1 + min;
    }


    public int CheckPair()
    {
        int min = 2000;
        int bestpair1 = -1;
        int paircount1 = 0;
        for (int i = 0; i < 13; i++)
        {
            if (bucket[0][i] == 1)
            {
                paircount1++;
            }
            if (bucket[1][i] == 1)
            {
                paircount1++;
            }
            if (bucket[2][i] == 1)
            {
                paircount1++;
            }
            if (bucket[3][i] == 1)
            {
                paircount1++;
            }
            if (paircount1 >= 2)
            {
                bestpair1 = Math.Max(bestpair1, i);
                paircount1 = 0;
            }
            else
            {
                paircount1 = 0;
                pairkicker.Add(i);
            }
                

        }
        if (bestpair1 == -1)
            return 0;
        else
            return min + bestpair1;
    }

    public int CheckHighCard ()
    {
        int min = 1000;
        int high = 0;
        int foundhigh = 0;
        for(int i = 12; i>=0; i--)
        {
            if (bucket[0][i] == 1 || bucket[1][i] == 1 || bucket[2][i] == 1 || bucket[3][i] == 1)
            {
                if (foundhigh == 0)
                {
                    high = i;
                    foundhigh = 1;
                }
                else
                {
                    highCkicker.Add(i);
                }
            }
                    
        }
        return high+min;
    }



}