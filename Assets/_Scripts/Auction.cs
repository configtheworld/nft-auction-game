using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json;

namespace Auction
{
    public class Player
    {
        public string name { get; set; }
        public int balance { get; set; }
        public int currentBid { get; set; }
        public string[] inventory { get; set; }
        public bool animationState { get; set; }

        public Player()
        {
            
        }

    }

    public class Person :Player
    {
        
        public Person()
        {
            this.name = "You";
            this.balance = 200;
            this.currentBid = 0;
            this.animationState = false;
        }

    }

    public class Bot : Player
    {
        public int greedyLvl { get; set; }

        public Bot()
        {
            this.name = this.nameCreator();
            this.balance = this.balanceCreator();
            this.greedyLvl = this.greedyCreator();
            this.currentBid = 0;
            this.animationState = false;
        }

        public string nameCreator()
        {
            int random = 0;
            random = Random.Range(1, 8);
            return "Bot_" + random.ToString();
        }

        public int balanceCreator()
        {
            int random = 0;
            random = Random.Range(1, 16) * 10;
            return random;
        }

        public int greedyCreator()
        {
            int random = 0;
            random = Random.Range(1, 4);
            return random;
        }
    }

    


    public class Auction : MonoBehaviour
    {
        // 2d menus
        public Canvas JoinCanvas;
        public Canvas LeaveCanvas;
        public Canvas BidCanvas;
        public Canvas ResultCanvas;
        // dialog boxes
        public Canvas auctioneerDialogBox;
        public Text auctioneerDialogText;

        public Canvas b1DialogBox;
        public Text b1DialogText;

        public Canvas b2DialogBox;
        public Text b2DialogText;

        // player controls
        private bool PlayerFlag = false;

        //bots game object
        public GameObject PlayerPrefab;
        List<Player> players = new List<Player>();

        


        // Auction Controls
        private bool isPlayerTurn = true;
        private bool isEnemyOne = true;
        private int maxBid = 0;
        float currCountdownValue;


        GameObject b1;
        GameObject b2;
        GameObject p1;

        

        void Start()
        {
            // initial canvas values
            //JoinCanvas = JoinCanvas.GetComponent<Canvas>();
            JoinCanvas.enabled = true;

            // LeaveCanvas = LeaveCanvas.GetComponent<Canvas>();
            LeaveCanvas.enabled = false;

            //BidCanvas = BidCanvas.GetComponent<Canvas>();
            BidCanvas.enabled = false;

            //ResultCanvas = ResultCanvas.GetComponent<Canvas>();
            ResultCanvas.enabled = false;

            //auctioneerDialogBox = auctioneerDialogBox.GetComponent<Canvas>();
            auctioneerDialogBox.enabled = true;
            b1DialogBox.enabled = false;
            b2DialogBox.enabled = false;

            Bot bot1 = new Bot();
            Bot bot2 = new Bot();


            // initial the bots as game object from prefab
            b1 = Instantiate(PlayerPrefab, new Vector3(14, -8, -5), Quaternion.identity);
            b2 = Instantiate(PlayerPrefab, new Vector3(13, -8, -5), Quaternion.identity);



            players.Add(bot1);
            players.Add(bot2);

            AuctionerDialogLogic("initial", 0, bot1);


        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Join()
        {
            if(!PlayerFlag){
                Person player1 = new Person();
                players.Add(player1);
                PlayerFlag = true;
                // hide join canvas
                JoinCanvas.enabled = false;
                // show giveup canvas
                LeaveCanvas = LeaveCanvas.GetComponent<Canvas>();
                LeaveCanvas.enabled = true;
                BidCanvas.enabled = true;
                p1 = Instantiate(PlayerPrefab, new Vector3(13, -8, -8), Quaternion.identity);


            }
            //ShowPlayers();

            
        }

        public void Leave()
        {
            if (PlayerFlag)
            {
                // remove player from list
                players.RemoveAt(players.Count - 1);
                // hide giveup canvas
                LeaveCanvas = LeaveCanvas.GetComponent<Canvas>();
                LeaveCanvas.enabled = false;
                BidCanvas.enabled = false;

                Destroy(p1);
                Destroy(b1);
                Destroy(b2);
            }
            //ShowPlayers();

        }

        public void ShowPlayers()
        {
            foreach (Player player in players)
            {
                Debug.Log(player.name);
            }
        }

        public void AuctionerDialogLogic(string auctionState,int bid ,Player user)
        {
            auctioneerDialogText = auctioneerDialogText.GetComponent<Text>();
            if (auctionState == "initial")
            {
                auctioneerDialogText.text = "Welcome dear players! Auction about to start";
            }
            else if (auctionState == "newbid")
            {
                auctioneerDialogText.text = "There is new bid from "+user.name+" as "+ bid.ToString() ;

            }
            else if (auctionState == "pending")
            {
                auctioneerDialogText.text = "Is there any increase?";
            }
            else if (auctionState == "countdown")
            {
                
                StartCoroutine(StartCountdown());
            }
            else if (auctionState == "final")
            {
                auctioneerDialogText.text = "Auction is finished! The winner is " + user.name + " with the biggest bid " + bid.ToString();
                Finalize();
            }
            
        }

        public IEnumerator StartCountdown(float countdownValue = 5)
        {
            currCountdownValue = countdownValue;
            while (currCountdownValue > 0)
            {
                Debug.Log( currCountdownValue+"...");
                yield return new WaitForSeconds(1.0f);
                auctioneerDialogText.text = currCountdownValue.ToString() + "...";
                currCountdownValue--;
            }
        }

        public void Finalize()
        {
            ResultCanvas.enabled = true;
        }

        // Auction methods

        private void ChangeTurn()
        {
            isPlayerTurn = !isPlayerTurn;

            if (!isPlayerTurn)
            {
                // hide bid btn
                //bidBtn.interactable = false;
                //withdrawBtn.interactable = false;

                StartCoroutine(EnemyTurn());
            }
            else
            {
                // show bid btn
                //bidBtn.interactable = true;
                //withdrawBtn.interactable = true;
            }
        }

        private IEnumerator EnemyTurn()
        {
            yield return new WaitForSeconds(2);

            if (isEnemyOne)
            {
                //bot1.currentBid = CreateBidForBots(maxBid);
                isEnemyOne = false;
            }
            else
            {
                //bot2.currentBid = CreateBidForBots(maxBid);
                isEnemyOne = true;
            }
            
        }

        public int CreateBidForBots(int max)
        {
            int random = 0;
            random = Random.Range(0, 10);
            int bid;
            if (random > 7)
            {
                bid = maxBid + 5;
            }
            else
            {
                bid = maxBid + 10;
            }
            return bid;
        }
    }
}
