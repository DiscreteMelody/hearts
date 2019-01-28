using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts_AI
{
    class Bot : Player
    {
        private int chosenCardIndex = 0;
        private Memory[] memoryOfPlayers = new Memory[Game.NUM_OF_PLAYERS - 1]; //the bot remembers all other players except for itself

        //instantiate constructor
        public Bot(string player_name, string player_nickname)
        {
            this.Name = player_name;
            this.Nickname = player_nickname;
            this.IsBot = true;
        }

        /// <summary>
        /// Creates a deep copy of a Bot object.
        /// </summary>
        /// <param name="bot_to_copy">The Bot object to deep copy.</param>
        public Bot(Bot bot_to_copy)
        {
            this.memoryOfPlayers = new Memory[Game.NUM_OF_PLAYERS - 1];

            for (int i = 0; i < bot_to_copy.memoryOfPlayers.Length; i++)
            {
                this.memoryOfPlayers[i] = new Memory(bot_to_copy.memoryOfPlayers[i]);
            }

            this.nickname = bot_to_copy.nickname;
            this.name = bot_to_copy.name;
            this.hand = new Hand(bot_to_copy.hand);
            this.totalScore = bot_to_copy.totalScore;
            this.roundScore = bot_to_copy.roundScore;
            this.turn = bot_to_copy.turn;
            this.isBot = bot_to_copy.isBot;
            this.place = bot_to_copy.place;
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

        public async void chooseCard(Game game)
        {
            Game copiedGame = new Game(game);
            List<Card> legalCards = copiedGame.Round.getLegalCardsInHand(this);
            int[] cardScores = new int[legalCards.Count];
            List<int> iterators = new List<int>();
            List<List<Card>> possibleHeldCards = new List<List<Card>>();    //a 2D list of cards possibly held by others
            int playersLeftToPlay = Game.NUM_OF_PLAYERS - (copiedGame.Round.Trick.CardCount + 1);
            List<Card> permutation = new List<Card>();
            int numOfPermutations = 0;
            int averageScore = 0;
            bool permutating = true;

            //add an iterator for each player left to play in the trick and add a list of possible cards for each upcoming player
            for (int i = 0; i < playersLeftToPlay; i++)
            {
                iterators.Add(0);
                possibleHeldCards.Add(this.getPossibleCards(this.memoryOfPlayers[i], copiedGame.Round));
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int c = 0; c < legalCards.Count; c++)
            {
                permutating = true;
                numOfPermutations = 0;
                averageScore = 0;

                while (permutating)
                {
                    permutation.Clear();
                    permutation.Add(legalCards[c]);

                    if(iterators.Count > 0)
                    {
                        for (int i = 0; i < iterators.Count; i++)
                        {
                            List<Card> currentHand = possibleHeldCards[i];
                            int currentIterator = iterators[i];

                            permutation.Add(currentHand[currentIterator]);
                        }

                        for (int i = 0; i < iterators.Count; i++)
                        {
                            if (iterators[i] < possibleHeldCards[i].Count - 1)
                            {
                                iterators[i]++;
                                break;
                            }
                            else
                            {
                                iterators[i] = 0;
                                continue;
                            }
                        }


                        //if the iterators have all been reset to 0, all permutations have been checked
                        if (iterators.Distinct().Count() == 1 && iterators[0] == 0)
                        {
                            permutating = false;
                        }

                        //to prevent multiple of a card from being permutated
                        if (permutation.Count != permutation.Distinct().Count())
                        {
                            continue;
                        }

                        copiedGame.Round = new Round(game.Round);
                        for(int i = 0; i < permutation.Count; i++)
                        {
                            if(i > 0)
                                copiedGame.Round.Trick.addCardToTrick(this.memoryOfPlayers[i - 1].MemorizedPlayer, permutation[i]);
                            else
                                copiedGame.Round.Trick.addCardToTrick(this, permutation[i]);
                        }

                        if (Simulator.isPossibleTrick(copiedGame.Round, this) == false)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        permutating = false;
                    }

                    copiedGame = new Game(game);

                    for(int i = 0; i < permutation.Count; i++)
                    {
                        if(i > 0)
                        {
                            copiedGame.playCardFromPlayer(new Player(this.memoryOfPlayers[i - 1].MemorizedPlayer), permutation[i]);
                        }
                        else
                        {
                            copiedGame.playCardFromPlayer(new Bot(this), permutation[0]);
                        }
                    }

                    averageScore += Simulator.scoreGame(copiedGame, new Bot(this));
                    numOfPermutations++;

                    if (numOfPermutations % 500 == 0)
                    {
                        System.Diagnostics.Debug.WriteLine(stopwatch.ElapsedMilliseconds);
                        stopwatch.Reset();
                        stopwatch.Start();
                    }
                        
                }

                averageScore /= numOfPermutations;
                cardScores[c] = averageScore;
            }

            this.chooseBestCard(cardScores, legalCards);

        }

        private void updateChosenIndex(Card card_chosen)
        {
            List<Card> hand = this.Hand.CardsHeld;

            for(int i = 0; i < hand.Count; i++)
            {
                if(hand[i].Suit == card_chosen.Suit && hand[i].Value == card_chosen.Value)
                {
                    this.chosenCardIndex = i;
                    return;
                }
            }
        }

        private void chooseBestCard(int[] scores, List<Card> legal_cards)
        {
            int max = scores[0];
            int maxIndex = 0;

            for(int i = 0; i < scores.Length; i++)
            {
                if(max > scores[i])
                {
                    max = scores[i];
                    maxIndex = i;
                }
                System.Diagnostics.Debug.Write(scores[i] + ", ");
            }
            System.Diagnostics.Debug.WriteLine("\n");


            this.updateChosenIndex(this.hand.CardsHeld[maxIndex]);
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

        private List<Card> getCardsNotHeld(Round round_in_progress)
        {
            List<Card> cardsNotHeld = new List<Card>();

            foreach(Card card in round_in_progress.getCardsRemaining())
            {
                if(this.hand.cardIsHeld(card) == false)
                {
                    cardsNotHeld.Add(card);
                }
            }

            return cardsNotHeld;
        }

        /// <summary>
        /// returns a list of possible cards held by another player based on memory of that player
        /// </summary>
        private List<Card> getPossibleCards(Memory memory, Round round_in_progress)
        {
            List<Card> possibleCards = new List<Card>();
            List<Card> nonHeldCards = this.getCardsNotHeld(round_in_progress);
            int suitIndex = 0;

            foreach(Card nonHeldCard in nonHeldCards)
            {
                suitIndex = Deck.SUIT_INDEXES[nonHeldCard.Suit];

                if (cardHeldByOtherPlayer(memory, nonHeldCard) == true)
                    continue;

                if (memory.isKnownVoid(nonHeldCard.Suit) == true)
                    continue;

                if (memory.isUnknownLength(nonHeldCard.Suit) == false && memory.isKnownCard(nonHeldCard) == false)
                    continue;

                possibleCards.Add(nonHeldCard);
            }

            return possibleCards;
        }

        private bool cardHeldByOtherPlayer(Memory memory, Card card)
        {
            foreach (Memory memorizedHand in this.memoryOfPlayers)
            {
                if (memorizedHand.isKnownCard(card) && memorizedHand != memory)
                    return true;
            }

            return false;
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
