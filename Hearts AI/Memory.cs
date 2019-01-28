using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts_AI
{
    [Serializable]
    class Memory
    {
        private List<Card>[] memoryOfHand = new List<Card>[4] { new List<Card>(), new List<Card>(), new List<Card>(), new List<Card>() };
        private Player memorizedPlayer;
        //am unknown length of a suit is represented by a card with the name of "?", a void is represented as a card with the name of "-"
        public static readonly string voidSymbol = "-";
        public static readonly string unknownLengthSymbol = "?";

        public Memory(Player player_to_memorize)
        {
            this.memorizedPlayer = player_to_memorize;
        }

        public Memory(Memory memory_to_copy)
        {
            this.memorizedPlayer = new Player(memory_to_copy.memorizedPlayer);
            this.memoryOfHand = new List<Card>[4] { new List<Card>(), new List<Card>(), new List<Card>(), new List<Card>() };

            for (int s = 0; s < memory_to_copy.memoryOfHand.Length; s++)
            {
                for(int v = 0; v < memory_to_copy.memoryOfHand[s].Count; v++)
                {
                    this.memoryOfHand[s].Add(new Card(memory_to_copy.memoryOfHand[s][v]));
                }
            }
        }

        public Player MemorizedPlayer
        {
            get { return this.memorizedPlayer; }
        }

        public List<Card>[] MemoryOfPlayer
        {
            get { return this.memoryOfHand; }
        }

        public void removeKnownCard(Card card)
        {
            int suitIndex = Deck.SUIT_INDEXES[card.Suit];

            for(int i = 0; i < this.memoryOfHand[suitIndex].Count; i++)
            {
                string memorizedCardValue = this.memoryOfHand[suitIndex][i].Value;

                if(memorizedCardValue == card.Value)
                {
                    this.memoryOfHand[suitIndex].RemoveAt(i);
                    break;
                }
            }
            

            if(memoryOfHand[suitIndex].Count == 0)
            {
                this.markAsVoid(card.Suit);
            }
        }

        public void addKnownCard(Card card, bool length_is_known = false)
        {
            int index = Deck.SUIT_INDEXES[card.Suit];

            if (this.isKnownCard(card) == true)
                return;

            memoryOfHand[index].Add(card);

            if(length_is_known == true)
            {
                this.removeUnknownSuitLength(card.Suit);
            }
        }

        public void removeUnknownSuitLength(string suit)
        {
            int suitIndex = Deck.SUIT_INDEXES[suit];

            foreach(Card card in this.memoryOfHand[suitIndex])
            {
                if(card.Value == Memory.unknownLengthSymbol)
                {
                    this.memoryOfHand[suitIndex].Remove(card);
                    break;
                }
            }

            if(this.memoryOfHand[suitIndex].Count == 0)
            {
                this.markAsVoid(suit);
            }
        }

        //all lists (suits) become a 1 item long list with a symbol representing an unknown length
        public void resetKnownCards()
        {
            string suit = "";

            for(int i = 0; i < memoryOfHand.Length; i++)
            {
                suit = Deck.SUITS[i];
                memoryOfHand[i].Clear();
                memoryOfHand[i].Add(new Card(unknownLengthSymbol, suit));
            }
        }

        public bool isKnownVoid(string suit)
        {
            int suitIndex = Deck.SUIT_INDEXES[suit];

            foreach(Card card in this.memoryOfHand[suitIndex])
            {
                if(card.Value == voidSymbol)
                {
                    return true;
                }
            }

            return false;
        }

        public bool isUnknownLength(string suit)
        {
            int suitIndex = Deck.SUIT_INDEXES[suit];
            bool isUnknownLength = false;

            foreach(Card memorizedCard in this.memoryOfHand[suitIndex])
            {
                if(memorizedCard.Value == Memory.unknownLengthSymbol)
                {
                    isUnknownLength = true;
                    break;
                }
            }

            return isUnknownLength;
        }

        public bool isKnownCard(Card card)
        {
            int index = Deck.SUIT_INDEXES[card.Suit];

            foreach(Card memorizedCard in this.memoryOfHand[index])
            {
                if (memorizedCard.Value == card.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public void markAsVoid(string suit)
        {
            int suitIndex = Deck.SUIT_INDEXES[suit];

            //do nothing if the bot already knows the suit is void or there is more than 1 known card left in the suit
            if (this.isKnownVoid(suit) == true || this.memoryOfHand[suitIndex].Count > 1)
                return;

            int index = Deck.SUIT_INDEXES[suit];

            this.memoryOfHand[index].Clear();
            this.memoryOfHand[index].Add(new Card(voidSymbol, suit));
        }

        public int getNumOfKnownVoids()
        {
            int numOfVoids = 0;
            string suit = "";

            for (int s = 0; s < Deck.SUITS.Length; s++)
            {
                suit = Deck.SUITS[s];

                if (this.isKnownVoid(suit))
                {
                    numOfVoids++;
                }
            }

            return numOfVoids;
        }

        public int getNumOfKnownCards()
        {
            int numKnown = 0;

            for(int i = 0; i < Deck.SUITS.Length; i++)
            {
                foreach(Card card in this.memoryOfHand[i])
                {
                    if(card.Value != unknownLengthSymbol && card.Value != voidSymbol)
                    {
                        numKnown++;
                    }
                }
            }

            return numKnown;
        }

        public int getNumOfKnownSuit(string suit)
        {
            int numKnown = 0;
            int suitIndex = Deck.SUIT_INDEXES[suit];

            foreach(Card memorizedCard in this.memoryOfHand[suitIndex])
            {
                if(memorizedCard.Value != Memory.unknownLengthSymbol && memorizedCard.Value != Memory.voidSymbol)
                {
                    numKnown++;
                }
            }

            return numKnown;
        }

        public void showMemory()
        {
            string message = this.memorizedPlayer.Nickname + ": ";

            for(int i = 0; i < this.memoryOfHand.Length; i++)
            {
                message += Deck.SUITS[i] + ": ";

                foreach(Card card in this.memoryOfHand[i])
                {
                    message += card.Value + ", ";
                }

                //message += "\n";
            }

            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
