using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hearts_AI
{
    class Deck
    {
        public static readonly string[] SUITS = new string[] { "clubs", "diamonds", "spades", "hearts" };
        public static readonly string[] VALUES = new string[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        public static readonly int NUM_OF_CARDS = Deck.SUITS.Length * Deck.VALUES.Length;
        public static readonly Dictionary<string, int> SUIT_INDEXES = new Dictionary<string, int>()
        {
            { "clubs", 0 },
            { "diamonds", 1 },
            { "spades", 2 },
            { "hearts", 3 }
        };

        private List<Card> cards = new List<Card>();
        private int cardsLeft;

        Random rng = new Random();

        public Deck()
        {
            int points = 0;
            foreach(string value in VALUES)
            {
                foreach(string suit in SUITS)
                {
                    if (value == "Q" && suit == "spades")
                        points = 13;
                    else if (suit == "hearts")
                        points = 1;
                    else
                        points = 0;

                    cards.Add(new Card(value, suit, points));
                    
                }
            }

            cardsLeft = cards.Count;
        }

        public void shuffle()
        {
            Card bottomCard = this.cards[cardsLeft - 1];
            int randomIndex = 0;
            bool isShuffled = false;

            while (isShuffled == false)
            {
                if (this.cards[0].Suit == bottomCard.Suit && this.cards[0].Value == bottomCard.Value)
                    isShuffled = true;
                
                randomIndex = rng.Next(cardsLeft + 1);
                this.cards.Insert(randomIndex, this.cards[0]);
                this.cards.RemoveAt(0);
            }
        }

        //deals a random card to a specified player
        public void dealToPlayer(Player player)
        {
            if(cardsLeft > 0)
            {
                player.Hand.addCardToHand(this.cards[0]);
                cards.RemoveAt(0);
                cardsLeft--;
            }
            
        }
    }
}
