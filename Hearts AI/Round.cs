using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts_AI
{
    [Serializable]
    class Round
    {
        public static readonly int NUM_OF_TRICKS = 13;
        public enum PassDirections { left = 1, right = 3, across = 2, keep = 0 };

        private Trick trick = new Trick();
        private int tricksRemaining = NUM_OF_TRICKS;
        private bool isOpeningTrick = true;
        private int roundCounter = 0;
        private bool heartsAreBroken = false;
        private int numOfCardsRemaining = Deck.NUM_OF_CARDS;
        private int pointsRemaining = 26;
        private List<Card>[] cardsRemaining = new List<Card>[4] { new List<Card>(), new List<Card>(), new List<Card>(), new List<Card>()};

        public Round()
        {
            
        }

        public Round(Round round_to_copy)
        {
            this.trick = new Trick(round_to_copy.trick);
            this.tricksRemaining = round_to_copy.tricksRemaining;
            this.isOpeningTrick = round_to_copy.isOpeningTrick;
            this.roundCounter = round_to_copy.roundCounter;
            this.heartsAreBroken = round_to_copy.heartsAreBroken;
            this.numOfCardsRemaining = round_to_copy.numOfCardsRemaining;
            this.cardsRemaining = new List<Card>[4] { new List<Card>(), new List<Card>(), new List<Card>(), new List<Card>() };

            for (int s = 0; s < round_to_copy.cardsRemaining.Length; s++)
            {
                for(int v = 0; v < round_to_copy.cardsRemaining[s].Count; v++)
                {
                    this.cardsRemaining[s].Add(new Card(round_to_copy.cardsRemaining[s][v]));
                }
            }
        }

        public int TricksRemaining
        {
            get { return this.tricksRemaining; }
            set { this.tricksRemaining = value; }
        }

        public int PointsRemaining
        {
            get
            {
                int pointsLeft = 0;

                foreach(Card card in this.getCardsRemaining())
                {
                    pointsLeft += card.Points;
                }

                return pointsLeft;
            }
        }

        public Trick Trick
        {
            get { return this.trick; }
            set { this.trick = value; }
        }

        public bool IsOpeningTrick
        {
            get { return this.isOpeningTrick; }
            set { this.isOpeningTrick = value; }
        }

        public int RoundCounter
        {
            get { return this.roundCounter; }
            set { this.roundCounter = value; }
        }

        /// <summary>
        /// returns a 2-dimensional list array of cards remaining sorted by suit
        /// </summary>
        public List<Card>[] CardsRemaining
        {
            get { return this.cardsRemaining; }
        }

        public int NumOfCardsRemaining
        {
            get { return this.numOfCardsRemaining; }
        }

        /// <summary>
        /// returns a 1-dimensional list of cards remaining
        /// </summary>
        public List<Card> getCardsRemaining()
        {
            List<Card> cardsLeft = new List<Card>();

            foreach(List<Card> suit in this.cardsRemaining)
            {
                foreach(Card card in suit)
                {
                    cardsLeft.Add(card);
                }
            }

            return cardsLeft;
        }

        public void startNewTrick()
        {
            this.isOpeningTrick = false;
            this.trick = new Trick();
            this.tricksRemaining--;
        }

        public void startNewRound()
        {
            this.tricksRemaining = Round.NUM_OF_TRICKS;
            this.trick = new Trick();
            this.isOpeningTrick = true;
            this.heartsAreBroken = false;
            this.roundCounter++;
            this.resetCardsRemaining();
            this.numOfCardsRemaining = Deck.NUM_OF_CARDS;
        }

        private void resetCardsRemaining()
        {
            int points = 0;
            string suit = "";
            string value = "";

            for(int s = 0; s < Deck.SUITS.Length; s++)
            {
                this.cardsRemaining[s].Clear();

                for(int v = 0; v < Deck.VALUES.Length; v++)
                {
                    points = 0;
                    suit = Deck.SUITS[s];
                    value = Deck.VALUES[v];

                    if (value == "Q" && suit == "spades")
                        points = 13;
                    else if (suit == "hearts")
                        points = 1;

                    this.cardsRemaining[s].Add(new Card(value, suit, points));
                }
            }

            this.numOfCardsRemaining = Deck.NUM_OF_CARDS;
        }

        public void removeFromRemainingCards(Card card_to_remove)
        {
            List<Card> suit = this.cardsRemaining[Deck.SUIT_INDEXES[card_to_remove.Suit]];

            foreach(Card card in suit)
            {
                if(card.Value == card_to_remove.Value)
                {
                    suit.Remove(card);
                    break;
                }
            }

            this.numOfCardsRemaining--;

            if (card_to_remove.Suit == "hearts")
                this.heartsAreBroken = true;
        }

        //returns whether a specific card played by a player is legal or not
        public bool isLegalPlay(Player player, Card card)
        {
            bool isLegalCard = false;

            //check if the card played is in the list of legal cards from that player's hand
            foreach(Card legalCard in this.getLegalCardsInHand(player))
            {
                if (legalCard.Value == card.Value && legalCard.Suit == card.Suit)
                {
                    isLegalCard = true;
                    break;
                }  
            }

            if (isLegalCard == false)
                return false;
            else
                return player.Turn;
        }

        public List<Card> getLegalCardsInHand(Player player)
        {
            List<Card> legalCards = new List<Card>();
            bool hasLeadSuit = false;
            bool hasOnlyHearts = true;

            //a preliminary search to determine if a player can lead hearts when not broken or play offsuit
            foreach(Card card in player.Hand.CardsHeld)
            {
                if (card.Suit == this.trick.LeadSuit)
                    hasLeadSuit = true;
                if (card.Suit != "hearts")
                    hasOnlyHearts = false;
            }

            foreach(Card card in player.Hand.CardsHeld)
            {
                //if the player is leading
                if (this.trick.LeadSuit == "")
                {
                    //if it is the first trick
                    if (this.isOpeningTrick)
                    {
                        if (card.Value == "2" && card.Suit == "clubs")
                        {
                            legalCards.Add(card);
                            return legalCards;
                        }
                    }
                    //if hearts are not broken and the player has a non-heart card
                    else if(this.heartsAreBroken == false && hasOnlyHearts == false)
                    {
                        if (card.Suit != "hearts")
                        {
                            legalCards.Add(card);
                        }
                    }
                    //if none of the previous conditions are met, a player can lead any card
                    else
                    {
                        legalCards.Add(card);
                    }
                }
                //if the player is following a lead
                else if(this.trick.LeadSuit != "")
                {
                    //if the player has the lead suit
                    if (hasLeadSuit == true)
                    {
                        if (card.Suit == this.trick.LeadSuit)
                        {
                            legalCards.Add(card);
                        }
                    }
                    //if the player does not have the lead suit
                    else
                    {
                        //if it is the first trick
                        if (this.isOpeningTrick)
                        {
                            //no point cards can be played offsuit on the first trick
                            if(card.Points <= 0)
                            {
                                legalCards.Add(card);
                            }
                        }
                        //the player may play anything offsuit if they wish
                        else
                        {
                            legalCards.Add(card);
                        }
                    }
                    
                }
            }

            return legalCards;
        }

        public int getNumOfSuitLeft(string suit)
        {
            return this.cardsRemaining[Deck.SUIT_INDEXES[suit]].Count;
        }
    }
}
