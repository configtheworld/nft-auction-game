using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System;
using System.IO;

namespace LP.TurnBasedStrategy
{

    public class GameController : MonoBehaviour
    {

        // Auctioner
        [SerializeField] public GameObject auctioner = null;
        private Animator auctionAnimator;
        public Canvas AuctionerDbox;
        public Text AuctionerDText;

        // player
        [SerializeField] private GameObject player = null;
        private Animator playerAnimator;
        public Canvas playerDbox;
        public Text playerDText;

        // enemy1
        [SerializeField] private GameObject enemy = null;
        private Animator enemy1Animator;
        public Canvas Bot1Dbox;
        public Text Bot1DText;

        // enemy2
        [SerializeField] private GameObject enemy2 = null;
        private Animator enemy2Animator;
        public Canvas Bot2Dbox;
        public Text Bot2DText;


        // 2d ui fields
        [SerializeField] private Text playerBid = null;
        [SerializeField] private Text enemyBid = null;
        [SerializeField] private Text highestBid = null;

        [SerializeField] private Button bidBtn = null;
        [SerializeField] private Button withdrawBtn = null;
        [SerializeField] private InputField iField = null;

        [SerializeField] private Text resultBid = null;
        [SerializeField] private Text winnerTxt = null;


        [SerializeField] private Button joinBtn = null;
        [SerializeField] private Button startBtn = null;

        // 2d canvases
        public Canvas BeginCanvas;
        public Canvas AuctionCanvas;
        public Canvas ResultCanvas;


        // game parameters
        private bool isPlayerTurn = true;
        private int finalCountdown = 1;
        private bool botFlag = false; // for turn
        private bool playerFlag = false; // for joining the game

        // json attributes
        private int totalTurn = 0;
        private int winnerBid = 0;
        private int playerNo = 2;

        [Serializable]
        public class Auction
        {
            public Auction()
            {
                this.auctionName = "example";
                this.Date = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            }
            public string auctionName;
            public string Date;
            public int totalTurn;
            public int playerNo;
            public string winner;
            public string winnerBid;
        }

        [Serializable]
        public class AuctionList
        {
            public AuctionList()
            {
                this.auctions = new List<Auction>();
            }
            public List<Auction> auctions;
        }


        void Start()
        {
            BeginCanvas = BeginCanvas.GetComponent<Canvas>();
            BeginCanvas.enabled = true;
            joinBtn.interactable = false;
            AuctionCanvas = AuctionCanvas.GetComponent<Canvas>();
            AuctionCanvas.enabled = false;
            ResultCanvas = ResultCanvas.GetComponent<Canvas>();
            ResultCanvas.enabled = false;

            StartCoroutine(CreateDialog(AuctionerDbox, AuctionerDText, 3, "Welcome dear players! Auction about to start"));

            Bot1Dbox.enabled = false;
            Bot2Dbox.enabled = false;
            playerDbox.enabled = false;

            // animations 
            auctionAnimator = auctioner.GetComponent<Animator>();
            playerAnimator = player.GetComponent<Animator>();
            enemy1Animator = enemy.GetComponent<Animator>();
            enemy2Animator = enemy2.GetComponent<Animator>();

        }


        private void MakeBid(GameObject bidder, float amount)
        {
            if(bidder == player)
            {
                if (Int16.Parse(playerBid.text) < amount)
                {
                    playerBid.text = amount.ToString();
                    
                    if(amount > Int16.Parse(highestBid.text))
                    {
                        highestBid.text = amount.ToString();
                        //finalCountdown = 0;
                        StartCoroutine(TriggerAuctionerAnimation());
                        StartCoroutine(TriggerPlayerOfferAnimation());
                        StartCoroutine(CreateDialog(playerDbox, playerDText, 2, amount.ToString()));
                        StartCoroutine(CreateDialog(AuctionerDbox, AuctionerDText, 2, "There is new bid "+ amount.ToString()+" from YOU"));

                    }
                    else
                    {
                        //check no more bid increment
                        finalCountdown++;
                        StartCoroutine(CreateDialog(AuctionerDbox, AuctionerDText, 1, "Is there more I am counting " + (4 - finalCountdown).ToString()));
                    }
                }

            }
            else
            {
                if (Int16.Parse(enemyBid.text) < amount)
                {
                    enemyBid.text = amount.ToString();

                    if (amount > Int16.Parse(highestBid.text))
                    {
                        highestBid.text = amount.ToString();
                        finalCountdown=0;
                        if (botFlag)
                        {
                            StartCoroutine(TriggerAuctionerAnimation());
                            StartCoroutine(CreateDialog(Bot1Dbox, Bot1DText, 2, amount.ToString()));
                            StartCoroutine(TriggerBotOfferAnimation1());
                            StartCoroutine(CreateDialog(AuctionerDbox, AuctionerDText, 1, "Oh, I hear " + amount.ToString() + " from BotJennet"));
                            
                        }
                        else
                        {
                            StartCoroutine(TriggerAuctionerAnimation());
                            StartCoroutine(CreateDialog(Bot2Dbox, Bot2DText, 2, amount.ToString()));
                            StartCoroutine(TriggerBotOfferAnimation2());
                            StartCoroutine(CreateDialog(AuctionerDbox, AuctionerDText, 1, "New offer from gentleman on right " + amount.ToString() + " from BotRick"));
                            
                        }
                        botFlag = !botFlag;
                    }
                    else
                    {
                        //check no more bid increment
                        finalCountdown++;
                        StartCoroutine(CreateDialog(AuctionerDbox, AuctionerDText, 1, "Is there more I am counting " + (4 - finalCountdown).ToString()));


                    }
                }
            }
            ChangeTurn();
        }

        private void ChangeTurn()
        {
            if(finalCountdown > 3)
            {
                FinalizeAuction();

            }
            else
            {
                // player joined to auction
                if (playerFlag)
                {
                    isPlayerTurn = !isPlayerTurn;

                    if (!isPlayerTurn)
                    {
                        bidBtn.interactable = false;
                        withdrawBtn.interactable = false;

                        StartCoroutine(EnemyTurn());
                    }
                    else
                    {
                        bidBtn.interactable = true;
                        withdrawBtn.interactable = true;
                    }
                }
                else
                {
                    StartCoroutine(EnemyTurn());
                }
                totalTurn++;
                
            }
           
        }

        private IEnumerator EnemyTurn()
        {
            yield return new WaitForSeconds(3);

            if (playerFlag)
            {
                // if already highest
                if (Int16.Parse(enemyBid.text) != Int16.Parse(highestBid.text))
                {

                    enemyLogic();
                }
                else if (Int16.Parse(enemyBid.text) == Int16.Parse(playerBid.text))
                {
                    enemyLogic();
                }
                else
                {
                    //skip turn
                    finalCountdown++;
                    StartCoroutine(CreateDialog(AuctionerDbox, AuctionerDText, 1, "Is there more I am counting " + (4 - finalCountdown).ToString()));
                    ChangeTurn();
                }
            }
            else
            {
                enemyLogic();
            }
            

            
        }

        private void enemyLogic()
        {
            int random = 0;
            random = Random.Range(0, 10);
            //Debug.Log(random);

            if (Int16.Parse(highestBid.text) < 50)
            {
                if (random > 7)
                {
                    MakeBid(enemy, Int16.Parse(playerBid.text) + 5);
                }
                else
                {
                    MakeBid(enemy, Int16.Parse(enemyBid.text) + 10);
                }
            }
            else
            {
                // behave less bidder
                if(random == 3)
                {
                    MakeBid(enemy, Int16.Parse(playerBid.text) + 1);
                }
                else
                {
                    // stay same
                    finalCountdown++;
                    StartCoroutine(CreateDialog(AuctionerDbox, AuctionerDText, 1, "Is there more I am counting " + (4 - finalCountdown).ToString()));

                    ChangeTurn();
                }
            }
            
        }

        public void FinalizeAuction()
        {
            isPlayerTurn = false;
            StartCoroutine(TriggerAuctionerWinAnimation());
            StartCoroutine(CreateDialog(AuctionerDbox, AuctionerDText, 1, "Auction has finished!"));

            if (Int16.Parse(playerBid.text) == Int16.Parse(highestBid.text) && Int16.Parse(playerBid.text) > Int16.Parse(enemyBid.text))
            {
                winnerTxt.text = "You";
                resultBid.text = playerBid.text;
                StartCoroutine(TriggerPlayerWinAnimation());
            }
            else
            {
                if (botFlag)
                {
                    winnerTxt.text = "BotJennet";
                    resultBid.text = enemyBid.text;
                    StartCoroutine(TriggerBotWinAnimation1());
                }
                else
                {
                    winnerTxt.text = "BotRick";
                    resultBid.text = enemyBid.text;
                    StartCoroutine(TriggerBotWinAnimation2());

                }
            }

            BeginCanvas.enabled = false;
            ResultCanvas.enabled = true;
            AuctionCanvas.enabled = false;

            //Auction thisAuction = new Auction();
            //thisAuction.winner = winnerTxt.text;
            //thisAuction.winnerBid = resultBid.text;
            //thisAuction.totalTurn = totalTurn;
            //thisAuction.playerNo = playerNo;


            //// json read
            //string path = Application.dataPath + "/Data/" + "auction" + ".json";//JsonUtility.ToJson(thisAuction).ToString(); 
            //string jsonString = File.ReadAllText(path);
            ////Debug.Log(jsonString);
            //AuctionList data = JsonUtility.FromJson<AuctionList>(jsonString);

            ////AuctionList newData = new AuctionList();
            ////newData.auctions = data.auctions;
            ////Debug.Log(newData.ToString());
            ////newData.auctions.Add(thisAuction);
            ////Debug.Log(thisAuction.ToString());
            //data.auctions.Add(thisAuction);

            //File.WriteAllText(path, JsonUtility.ToJson(data));

            
        }
        public IEnumerator CreateDialog(Canvas canvas,Text text,int seconds,string content)
        {
            canvas.enabled = true;
            text.text = content;
            yield return new WaitForSeconds(seconds);
            canvas.enabled = false;
            text.text = " ";
        }
        public IEnumerator TriggerAuctionerAnimation()
        {
            auctionAnimator.SetBool("isOffer", true);
            yield return new WaitForSeconds(2);
            auctionAnimator.SetBool("isOffer", false);

        }
        public IEnumerator TriggerAuctionerWinAnimation()
        {
            auctionAnimator.SetBool("isApplause", true);
            yield return new WaitForSeconds(5);
            auctionAnimator.SetBool("isApplause", false);

        }
        public IEnumerator TriggerBotOfferAnimation1()
        {
            enemy1Animator.SetBool("isAsking", true);
            yield return new WaitForSeconds(2);
            enemy1Animator.SetBool("isAsking", false);

        }
        public IEnumerator TriggerBotOfferAnimation2()
        {
            enemy2Animator.SetBool("isAsking", true);
            yield return new WaitForSeconds(2);
            enemy2Animator.SetBool("isAsking", false);

        }
        public IEnumerator TriggerPlayerOfferAnimation()
        {
            playerAnimator.SetBool("isOffer", true);
            yield return new WaitForSeconds(2);
            playerAnimator.SetBool("isOffer", false);

        }
        public IEnumerator TriggerPlayerWinAnimation()
        {
            playerAnimator.SetBool("isWon", true);
            yield return new WaitForSeconds(4);
            playerAnimator.SetBool("isWon", false);

        }
        public IEnumerator TriggerBotWinAnimation1()
        {
            enemy1Animator.SetBool("isWon", true);
            yield return new WaitForSeconds(4);
            enemy1Animator.SetBool("isWon", false);

        }
        public IEnumerator TriggerBotWinAnimation2()
        {
            enemy2Animator.SetBool("isWon", true);
            yield return new WaitForSeconds(4);
            enemy2Animator.SetBool("isWon", false);

        }
        // btn functions 
        public void BtnBid()
        {
            if(iField.text != ""){
                MakeBid(player, Int16.Parse(iField.text));
            }
            else { 
                ChangeTurn(); 
            }
            
        }
        public void BtnWithDraw()
        {
            ChangeTurn();
        }
        public void StartAuction()
        {
            joinBtn.interactable = true; 
            startBtn.interactable = false;
            ChangeTurn();
        }
        public void JoinAuction()
        {
            playerFlag = true;
            AuctionCanvas.enabled = true;
            BeginCanvas.enabled = false;
            playerNo++;
        }
        public void BackLobby()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(1);
        }
    }
}


// todos
/*
     
 add nice hall like a real with textures 

 check out the animation of people 

 point the player in room with light or smt 

 and create hands up animation when sides make a bit trigger that animation

 create nice metamask login and carry the id and show it

 _____future tasks______
 keep results 
 increase nft examples
 create swap/takas gamecontroller via static NFT prices and images
 create combinatoral gamecontroller 

 make the bids multiplayer

 create server for looks like will handle results input output infos from game
 and execute as soladity
 */