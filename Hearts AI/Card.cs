using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts_AI
{
    class Card
    {
        public static readonly int MIN_VALUE = 1;  //the bottom of a suit is represented as this number in strength
        public static readonly int NONE_REMAIN_VALUE = 0;   //the value of a card if no other player holds this suit

        private string value;
        private string suit;
        private int points;

        public Card(string card_value, string card_suit, int card_points = 0)
        {
            this.value = card_value;
            this.suit = card_suit;
            this.points = card_points;
        }

        public string Value
        {
            get { return this.value; }
            //read only currently
        }

        public string Suit
        {
            get { return this.suit; }
            //read only currently
        }

        public int Points
        {
            get { return this.points; }
            set { this.points = value; }
        }

        //for the bots to better understand their dangerous suits
        //returns an integer value based on the strength of the card value regardless of cards remaining
        public int getAbsoluteValue()
        {
            int absoluteValue = 0;

            Dictionary<string, int> absoluteValues = new Dictionary<string, int>
            {
                { "2", 1 },
                { "3", 2 },
                { "4", 3 },
                { "5", 4 },
                { "6", 5 },
                { "7", 6 },
                { "8", 7 },
                { "9", 8 },
                { "10", 9 },
                { "J", 10 },
                { "Q", 11 },
                { "K", 12 },
                { "A", 13 }
            };

            absoluteValue = absoluteValues[this.value];

            return absoluteValue;
        }

        //iterates over the cards left in a round and cards in a players hand. returns the strength of the card_to_check compared to remaining cards
        //if two touching cards are held, their relative value is the same. ie if the 2 and 3 of diamonds are held, their relative value should be the same
        //likewise, if the 2 and 5 of diamonds are held and the 3 and 4 have already been played, the 2 and 5 should have the same relative value
        public int getRelativeValue(Hand hand_to_check, Round round_in_progress)
        {
            List<Card> cardsLeft = round_in_progress.CardsRemaining[Deck.SUIT_INDEXES[this.Suit]];
            int numOfSuitLeft = cardsLeft.Count;
            int relativeValue = Card.MIN_VALUE;

            //0 indicates all of the remaining cards of the suit are held
            if (hand_to_check.getHeldSuit(this.Suit).Count == cardsLeft.Count)
                return 0;            

            for(int i = 0; i < numOfSuitLeft; i++)
            {
                Card cardLeft = cardsLeft[i];
                bool hasThisCard = hand_to_check.cardIsHeld(cardLeft);
                bool hasNextCard = false;

                //it is not possible to hold an imaginary card
                if(i < numOfSuitLeft - 1)
                {
                    hasNextCard = hand_to_check.cardIsHeld(cardsLeft[i + 1]);
                }

                if (cardLeft.Suit == this.Suit && cardLeft.Value == this.Value)
                {
                    break;
                }

                //if consecutive cards are held, do nothing
                if (hasThisCard && hasNextCard)
                {
                    continue;
                }

                relativeValue++;

            }

            return relativeValue;
        }

        public string getCardImagePath()
        {
            string combinePhrase = "_of_";
            string imageType = ".png";
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"\Images";
            string file = this.value.ToLower() + combinePhrase + this.suit + imageType;
            string path = Path.Combine(projectPath, file);

            return path;
        }
    }
}
