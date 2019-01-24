using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts_AI
{
    class Bot : Player
    {
        private Card chosenCard;
        private int chosenCardIndex = 0;
        private Memory[] memoryOfPlayers = new Memory[Game.NUM_OF_PLAYERS - 1]; //the bot remembers all other players except for itself

        public Bot(string player_name, string player_nickname)
        {
            this.Name = player_name;
            this.Nickname = player_nickname;
            this.IsBot = true;
        }

        public int ChosenCardIndex
        {
            get { return this.chosenCardIndex; }
        }

        public Memory[] MemoryOfPlayers
        {
            get { return this.memoryOfPlayers; }
        }

        public void createMemory(Player[] players_clockwise)
        {
            for (int i = 0; i < Game.NUM_OF_PLAYERS - 1; i++)
            {
                this.memoryOfPlayers[i] = new Memory(players_clockwise[i]);
            }
        }

        public void chooseCard(Game game)
        {
            List<Card> legalCards = game.Round.getLegalCardsInHand(this);
            this.chosenCard = legalCards[legalCards.Count - 1];
            updateChosenIndex();
        }

        private void updateChosenIndex()
        {
            List<Card> hand = this.Hand.CardsHeld;

            for(int i = 0; i < hand.Count; i++)
            {
                if(hand[i].Suit == chosenCard.Suit && hand[i].Value == chosenCard.Value)
                {
                    this.chosenCardIndex = i;
                    return;
                }
            }
        }

        //updates the known cards held by the played_by player using deduction
        public void updateMemory(Game game_in_progress, Player played_by, Card card_played)
        {
            string suitPlayed = card_played.Suit;
            string leadSuit = game_in_progress.Round.Trick.LeadSuit;
            Memory memorizedHand = memoryOfPlayers[getPlayerMemoryIndex(played_by)];

            if (played_by == this)
            {
                return;
            }

            if (memorizedHand.isKnownCard(card_played) == true)
            {
                memorizedHand.removeKnownCard(card_played);
            }

            if (suitPlayed != leadSuit)
            {
                memorizedHand.markAsVoid(leadSuit);
            }

            this.deduceCardsFromVoids(game_in_progress.Round);
            this.deduceVoidsFromCards(game_in_progress.Round);
            this.deduceCardsFromCards();


            //to check if everything is working according to plan, can be removed later
            this.MemoryOfPlayers[0].showMemory();
            this.MemoryOfPlayers[1].showMemory();
            this.MemoryOfPlayers[2].showMemory();

        }

        //returns the index of a player located inside of the memoryOfPlayers
        private int getPlayerMemoryIndex(Player player)
        {
            for(int i = 0; i < this.memoryOfPlayers.Length; i++)
            {
                if(player == this.memoryOfPlayers[i].MemorizedPlayer)
                {
                    return i;
                }
            }

            return 0;
        }

        private int getNumOfVoidPlayers(string suit)
        {
            int numOfVoids = 0;

            foreach(Memory memorizedHand in this.memoryOfPlayers)
            {
                if(memorizedHand.isKnownVoid(suit) == true)
                {
                    numOfVoids++;
                }
            }

            return numOfVoids;
        }

        //deduces if the missing cards of a suit are in a certain player's hand
        private void deduceCardsFromVoids(Round round_in_progress)
        {
            int numOfVoidPlayers = 0;

            foreach(string suit in Deck.SUITS)
            {
                numOfVoidPlayers = this.getNumOfVoidPlayers(suit);

                //if all but 1 other player are void in a suit, the missing cards of the suit are in that hand
                if (numOfVoidPlayers == (Game.NUM_OF_PLAYERS - this.memoryOfPlayers.Length) + 1)
                {
                    foreach (Memory memorizedHand in this.memoryOfPlayers)
                    {
                        if (memorizedHand.isKnownVoid(suit) == false)
                        {
                            List<Card> missingCards = this.getMissingCards(round_in_progress, suit);
                            addMissingCardsToMemory(memorizedHand, missingCards);
                        }
                    }
                }
            }

            foreach(Memory memorizedHand in this.memoryOfPlayers)
            {
                Player memorizedPlayer = memorizedHand.MemorizedPlayer;
                int numOfCardsHeld = memorizedPlayer.Hand.CardsHeld.Count;
                int numOfKnownVoids = memorizedHand.getNumOfKnownVoids();
                List<Card> missingCards = new List<Card>();

                //if the number of known cards is the same as the number of cards held
                foreach(string suit in Deck.SUITS)
                {
                    missingCards = this.getMissingCards(round_in_progress, suit);

                    if (numOfKnownVoids == Deck.SUITS.Length - 1 && 
                        missingCards.Count == numOfCardsHeld && 
                        memorizedHand.isKnownVoid(suit) == false)
                    {
                        this.addMissingCardsToMemory(memorizedHand, missingCards);
                        markAllAsVoid(suit, memorizedPlayer);
                    }
                }
            }
        }

        //deduces if other players are void based on cards held or if no cards remain
        private void deduceVoidsFromCards(Round round_in_progress)
        {
            string suit = "";

            //if none of a suit remain
            for (int s = 0; s < Deck.SUITS.Length; s++)
            {
                suit = Deck.SUITS[s];

                if (round_in_progress.CardsRemaining[s].Count == 0)
                {
                    this.markAllAsVoid(suit);
                }
            }

            //if a suit's remaining cards are in the bot's hand
            foreach(Card cardHeld in this.Hand.CardsHeld)
            {
                if(cardHeld.getRelativeValue(this.Hand, round_in_progress) == Card.NONE_REMAIN_VALUE)
                {
                    this.markAllAsVoid(cardHeld.Suit);
                }
            }

            //if a player has no cards left
            foreach (Memory memorizedHand in this.memoryOfPlayers)
            {
                int numOfCardsHeld = memorizedHand.MemorizedPlayer.Hand.CardsHeld.Count;

                if (numOfCardsHeld == 0)
                {
                    for (int s = 0; s < Deck.SUITS.Length; s++)
                    {
                        suit = Deck.SUITS[s];
                        memorizedHand.markAsVoid(suit);
                    }
                }
            }
        }

        //deduces the cards held by a player based on cards the bot already knows that player holds
        private void deduceCardsFromCards()
        {
            //if the the number of known cards is the same as the number of cards held
            foreach(Memory memorizedHand in this.memoryOfPlayers)
            {
                int numOfCardsHeld = memorizedHand.MemorizedPlayer.Hand.CardsHeld.Count;
                int numOfKnownCards = memorizedHand.getNumOfKnownCards();

                if(numOfKnownCards == numOfCardsHeld)
                {
                    for (int s = 0; s < Deck.SUITS.Length; s++)
                    {
                        string suit = Deck.SUITS[s];

                        //then all unknown length suits are known
                        if (memorizedHand.isUnknownLength(suit) == true)
                        {
                            memorizedHand.removeUnknownSuitLength(suit);
                        }
                    }
                }
            }
        }

        //returns a list of cards in a suit that the bot does not hold
        private List<Card> getMissingCards(Round round_in_progress, string suit)
        {
            List<Card> missingCards = new List<Card>();
            int suitIndex = Deck.SUIT_INDEXES[suit];

            foreach(Card cardLeft in round_in_progress.CardsRemaining[suitIndex])
            {
                if(this.Hand.cardIsHeld(cardLeft) == false)
                {
                    missingCards.Add(cardLeft);
                }
            }

            return missingCards;
        }

        private void addMissingCardsToMemory(Memory memory, List<Card> missing_cards)
        {
            bool lengthIsKnown = true;

            foreach(Card missingCard in missing_cards)
            {
                memory.addKnownCard(missingCard, lengthIsKnown);
            }
        }

        private void markAllAsVoid(string suit, Player excludedPlayer = null)
        {
            foreach(Memory memorizedHand in this.memoryOfPlayers)
            {
                if(excludedPlayer == memorizedHand.MemorizedPlayer)
                {
                    continue;
                }

                memorizedHand.markAsVoid(suit);
            }
        }
    }
}
