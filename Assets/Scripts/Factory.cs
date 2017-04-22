using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Factory : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject managerPrefab;
	public GameObject managerPineapplePrefab;
    public GameObject enemyPrefab;
    public GameObject foldButton;
    public GameObject raiseBut;
	public GameObject callBut;
    public GameObject betBut;
	public GameObject checkBut;
    public GameObject slider;
    public GameObject sliderText;
	public GameObject playerChipsText;
    public GameObject card1;
    public GameObject card2;
	public GameObject card3;
	public GameObject voice;
	public AudioClip chipA, flipA, winA, lossA;

    public static GameObject player;
    public static GameObject playerUi;
    public static GameObject button;
    public static GameObject manager;
	public static GameObject managerPineapple;
    public static GameObject enemy;
    public static GameObject foldButtonObject;
	public static GameObject raiseButObject;
	public static GameObject callButObject;
    public static GameObject betButObject;
	public static GameObject checkButObject;
    public static GameObject sliderObject;
    public static GameObject sliderTextObject;
	public static GameObject playerChipsTextObject;
    public static GameObject card1Object;
    public static GameObject card2Object;
	public static GameObject card3Object;
	public static GameObject voiceObject;
	public static GameObject statusText;
	public static AudioClip chipAudio, flipAudio, winAudio, lossAudio;
	public static AudioManager audioManager;
    public Cj _cj;
	public static Animator a;
    

    // public static GameObject card;
    public static List<GameObject> enemies;
    public static List<Card> deck;

    public static Factory factory;
	public GameObject instructions;
	private bool destroyedCard;

    public enum suits {
		Clubs, Spades, Hearts, Diamonds
	};

    public enum moves {
		Fold, Raise, Call, Bet, Check
	};

    private static List<string> names;
	public static int numEnemies = 5;

    public void CreatePlayer()
    {
        player = Instantiate(playerPrefab);
        player.name = "Player";
        // player.GetComponent<Player>().name = player.name;
    }

    public void CreateManager()
    {
        manager = Instantiate(managerPrefab);
        manager.name = "Manager";
		destroyedCard = true;
    }

	public void CreatePineappleManager()
	{
		manager = Instantiate(managerPineapplePrefab);
		manager.name = "Manager Pineapple";
		destroyedCard = false;
	}

    public void CreateEnemy()
    {
        GameObject e = Instantiate(enemyPrefab);
        e.name = GetEnemyName();
        e.GetComponent<Enemy>().name = e.name;
        e.GetComponent<Enemy>().enemyObject = e;
        enemies.Add(e);
    }

    public void anim()
    {
        
    }
	public static string GetEnemyName()
	{
		string name;
        if (names == null)
            return "Enemy Unit";
		if (names.Count () > 0) 
		{
			name = names [0];
			names.RemoveAt (0);
		} 

		else
			name = "Enemy Unit";
		
		return name;
	}

    public static void AddName(string name)
    {
        names.Add(name);
    }

    public List<Card> Shuffle(List<Card> deck)
    {
        int n = deck.Count;
        while (n > 1)
        {
            int k = (Random.Range(0, n) % n);
            n--;
            Card value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }

        return deck;
    }

	public List<string> Shuffle(List<string> namesList)
	{
		int n = namesList.Count;
		while (n > 1)
		{
			int k = (Random.Range(0, n) % n);
			n--;
			string value = namesList[k];
			namesList[k] = namesList[n];
			namesList[n] = value;
		}

		return namesList;
	}

    public void NewGame()
    {
        Destroy(manager);

        deck = new List<Card>();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                deck.Add(new Card(i, j));
            }
        }

        deck = factory.Shuffle(deck);
        CreateManager();

        player.GetComponent<Unit>().UpdateManager(manager);
        player.GetComponent<Unit>().ResetHand();
        foreach (GameObject e in enemies)
        {
            e.GetComponent<Unit>().UpdateManager(manager);
            e.GetComponent<Unit>().ResetHand();
        }
    }

    // Use this for initialization
    void Start () {
        //player = new Player("Safa", 0, "password", 0, 0, 0.0, 0);
        factory = this;
        // Instantiate the Player and Manager prefabs
        foldButtonObject = foldButton;
		raiseButObject = raiseBut;
		callButObject = callBut;
		betButObject = betBut;
		checkButObject = checkBut;
        sliderObject = slider;
        sliderTextObject = sliderText;
		playerChipsTextObject = playerChipsText;
        playerUi = GameObject.Find("Player UI");
        card1Object = card1;
        card2Object = card2;
		card3Object = card3;
		voiceObject = voice;
		statusText = GameObject.Find ("StatusText");
		winAudio = winA;
		lossAudio = lossA;
		chipAudio = chipA;
		flipAudio = flipA;
		audioManager = GameObject.Find ("AudioManager").GetComponent<AudioManager>();
        anim();
    }

    private void OnEnable()
    {

        enemies = new List<GameObject>();
        deck = new List<Card>();
        names = new List<string>();

        names.Add("Richie");
        names.Add("Nick");
        names.Add("Cody");
        names.Add("George");
        names.Add("Brad");
        names.Add("Eric");
        names.Add("Safa");
        names.Add("Dr. Nassiff");
        names.Add("Neda");

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                deck.Add(new Card(i, j));
            }
        }

        deck = Shuffle(deck);
        names = Shuffle(names);

        int x = 1;
        foreach (Card card in deck)
        {
            Debug.Log(x.ToString() + ": " + card.value);
            x++;
        }

        CreatePlayer();
        
        for (int i = 0; i < numEnemies; i++)
            CreateEnemy();
    }

    // Update is called once per frame
    void Update () {
		if (Input.GetKeyDown (KeyCode.Escape))
			Application.Quit ();
		if (Input.GetKeyDown (KeyCode.P))
			PlayChipSplit ();
		
		if (!destroyedCard) 
		{
			if (Input.GetMouseButtonDown(0)) {
				RaycastHit hit;
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray, out hit)) {
					Debug.Log (hit.transform.name);
					if (hit.transform.name == "Card3") {
						player.GetComponent<Player>().hand.RemoveAt(2);
						Destroy (card3Object);
						destroyedCard = true;
					}
					else if (hit.transform.name == "Card2") {
						player.GetComponent<Player>().hand.RemoveAt(1);
						Destroy (card2Object);
						destroyedCard = true;
					}
					else if (hit.transform.name == "Card1") {
						player.GetComponent<Player>().hand.RemoveAt(0);
						Destroy (card1Object);
						destroyedCard = true;
					}
				}
			}	
		}
	}

	public void ToggleInstructions()
	{
        
		GameObject instructionsButtonText = GameObject.Find ("Instructions Button");
		if (instructions.activeSelf) {
            instructionsButtonText.GetComponentInChildren<Text>().text = "Instructions";
            instructions.SetActive (false);

		} else {
            instructionsButtonText.GetComponentInChildren<Text>().text = "Hide Instructions";
            instructions.SetActive (true);	
		}
	}

	public void PlayChipSplit()
	{
		Animator a = GameObject.Find ("Chips").GetComponent<Animator>();
		a.Play ("Pot Split");
	}
}
