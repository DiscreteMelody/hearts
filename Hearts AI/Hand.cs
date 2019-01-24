using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts_AI
{
    class Hand
    {
        //2D list of cards held in the 4 suits
        private List<Card> cardsHeld = new List<Card>();

        public List<Card> CardsHeld
        {
            get { return this.cardsHeld; }
        }

        public List<Card> getHeldSuit(string suit_name)
        {
            List<Card> heldSuit = new List<Card>();

            foreach(Card card in cardsHeld)
            {
                if(card.Suit == suit_name)
                {
                    heldSuit.Add(card);
                }
            }

            return heldSuit;
        }

        public void addCardToHand(Card card)
        {
            cardsHeld.Add(card);
        }

        public void sortHand()
        {
            List<Card> orderedHand = new List<Card>();
            string[] suits = Deck.SUITS;
            string[] values = Deck.VALUES;

            //iterates through suits then values and adds cars from high to low
            //the high cards get shifted to the right resulting in an ascending order
            for(int s = suits.Length - 1; s >= 0; s--)
            {
                for(int v = values.Length - 1; v >= 0; v--)
                {
                    foreach(Card card in this.cardsHeld)
                    {
                        if(card.Suit == suits[s] && card.Value == values[v])
                        {
                            orderedHand.Insert(0, card);
                        }
                    }
                }
            }

            this.cardsHeld = orderedHand;
        }

        //returns the number of clusters of cards in a player's hand
        //a cluster counts as a lone card, each missing card, or a series of cards such as 7, 8, 9
        public int getNumOfClusters(List<Card> cards_in_round)
        {
            List<Card> cardsLeft = cards_in_round;
            int numOfClusters = 0;
            bool holdsCurrentCard = false;
            bool holdsNextCard = false;

            for (int i = 0; i < cardsLeft.Count; i++)
            {
                Card cardLeft = cardsLeft[i];
                holdsCurrentCard = this.cardIsHeld(cardLeft);

                //if it is not the last card in cardsLeft
                if (i < cardsLeft.Count - 1)
                {
                    holdsNextCard = this.cardIsHeld(cardsLeft[i + 1]);
                }
                //it's not possible to hold an imaginary card
                else
                {
                    holdsNextCard = false;
                }

                //if both the current and next card are held in a series, the number of clusters should not increase
                if (holdsCurrentCard && holdsNextCard)
                {
                    continue;
                }

                numOfClusters++;
            }

            return numOfClusters;
        }

        public void removeCard(Card card)
        {
            for(int i = 0; i < cardsHeld.Count; i++)
            {
                if(card.Suit == cardsHeld[i].Suit && card.Value == cardsHeld[i].Value)
                {
                    cardsHeld.RemoveAt(i);
                    break;
                }
            }
        }

        public bool cardIsHeld(Card card_to_check)
        {
            foreach(Card card in this.cardsHeld)
            {
                if(card.Suit == card_to_check.Suit && card.Value == card_to_check.Value)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
