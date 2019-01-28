using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts_AI
{
    class Trick
    {
        public static readonly int TRICK_SIZE = Game.NUM_OF_PLAYERS;
        private string leadSuit = "";
        private int points = 0;

        //these two lists should remain parallel to find the correct winner
        private List<Card> cards = new List<Card>();
        private List<Player> players = new List<Player>();
        
        
        //a new card played by a player is required to create a new trick
        public Trick()
        {
            
        }

        public Trick(Trick trick_to_copy)
        {
            this.leadSuit = trick_to_copy.leadSuit;
            this.points = trick_to_copy.points;
            this.cards = new List<Card>();
            this.players = new List<Player>();

            foreach(Card cardToCopy in trick_to_copy.cards)
            {
                this.cards.Add(new Card(cardToCopy));
            }
            foreach(Player playerToCopy in trick_to_copy.players)
            {
                this.players.Add(new Player(playerToCopy));
            }
        }

        public int CardCount
        {
            get { return this.cards.Count; }
        }

        public string LeadSuit
        {
            get { return this.leadSuit; }
            set { this.leadSuit = value; }
        }

        public Player Leader
        {
            get { return this.players[0]; }
        }

        public int Points
        {
            get { return this.points; }
        }

        public List<Card> Cards
        {
            get { return this.cards; }
        }

        //adds a card played to the trick
        public void addCardToTrick(Player player, Card card)
        {
            this.players.Add(player);
            this.cards.Add(card);
            this.points += card.Points;

            if(this.leadSuit == "")
            {
                this.leadSuit = card.Suit;
            }
        }

        //checks for the highest card played by all 4 players and returns the winning player
        public Player findTrickWinner()
        {
            Player winner = null;
            int numOfValues = Deck.VALUES.Length;

            //iterate top down through the values in a deck.
            for (int v = numOfValues - 1; v >= 0; v--)
            {
                string deckValue = Deck.VALUES[v];
                //the first highest card in the lead will always be the winner
                for (int c = 0; c < cards.Count; c++)
                {
                    string cardValue = cards[c].Value;
                    string cardSuit = cards[c].Suit;

                    if (cardSuit == this.leadSuit && cardValue == deckValue)
                    {
                        winner = players[c];
                        break;
                    }
                }

                if (winner != null)
                    break;
            }

            return winner;
        }

        public void addTrickPoints(Player player)
        {
            player.RoundScore += this.points;
        }
    }
}
