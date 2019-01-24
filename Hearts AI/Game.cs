using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts_AI
{
    class Game
    {
        public static readonly int NUM_OF_PLAYERS = Player.Nicknames.Length;

        private Player[] players = new Player[NUM_OF_PLAYERS];
        private List<Bot> bots = new List<Bot>();

        private Deck deck = new Deck();
        private Round round = new Round();
        private bool started = false;

        public Game(Player south, Player west, Player north, Player east)
        {
            this.players[0] = south;
            this.players[1] = west;
            this.players[2] = north;
            this.players[3] = east;

            bots.Add((Bot)west);
            bots.Add((Bot)north);
            bots.Add((Bot)east);

            this.createBotMemories();
        }

        public Player[] Players
        {
            get { return this.players; }
        }

        public Round Round
        {
            get { return this.round; }
        }

        public bool Started
        {
            get { return this.started; }
            set { this.started = value; }
        }

        public void dealCards()
        {
            for (int i = 0; i < Round.NUM_OF_TRICKS; i++)
            {
                for (int p = 0; p < this.players.Length; p++)
                {
                    deck.dealToPlayer(players[p]);
                }
            }
        }

        public void startRound()
        {
            //deal cards and order them in players hands
            reshuffleCards();
            dealCards();
            this.sortPlayerHands();
            this.round.startNewRound();
            this.resetBotMemories();
            find2ofClubs();

            this.started = true;
        }

        public void endRound()
        {
            this.resetTurns();
            this.addPointsTaken();
            this.resetRoundPoints();
            this.startRound();
        }

        private void sortPlayerHands()
        {
            foreach (Player player in this.players)
            {
                player.Hand.sortHand();
            }
        }

        private void resetBotMemories()
        {
            foreach(Bot bot in this.bots)
            {
                for(int i = 0; i < bot.MemoryOfPlayers.Length; i++)
                {
                    bot.MemoryOfPlayers[i].resetKnownCards();
                }
            }
        }

        private void find2ofClubs()
        {
            foreach(Player player in this.players)
            {
                foreach(Card card in player.Hand.CardsHeld)
                {
                    if(card.Value == "2" && card.Suit == "clubs")
                    {
                        player.Turn = true;
                    }
                }
            }
        }

        //sets the game so it is no player's turn
        private void resetTurns()
        {
            foreach (Player player in this.players)
            {
                player.Turn = false;
            }
        }

        //once a round is complete, the points taken in a round is added to a player's score
        private void addPointsTaken()
        {
            foreach(Player player in this.players)
            {
                player.TotalScore += player.RoundScore;
            }
        }

        //sets all players' round scores to 0
        private void resetRoundPoints()
        {
            foreach(Player player in this.players)
            {
                player.RoundScore = 0;
            }
        }

        public void playCardFromPlayer(Player player, Card card)
        {
            this.round.Trick.addCardToTrick(player, card);
            player.Hand.removeCard(card);
            this.round.removeFromRemainingCards(card);

            if (this.round.Trick.CardCount < 4)
            {
                moveTurnClockwise();
            }
            else if(this.round.Trick.CardCount == 4)
            {
                player.Turn = false;
                this.round.TricksRemaining--;
            }

            this.bots[0].updateMemory(this, player, card);
            
        }

        public void reshuffleCards()
        {
            deck = new Deck();
        }

        //finds the player clockwise of the player whose turn it is and sets it to their turn
        private void moveTurnClockwise()
        {
            int nextTurnIndex = 0;

            for(int i = 0; i < this.players.Length; i++)
            {
                if(this.players[i].Turn == true)
                {
                    nextTurnIndex = (i + 1) % Game.NUM_OF_PLAYERS;
                    this.players[i].Turn = false;
                    break;
                }
            }

            this.players[nextTurnIndex].Turn = true;
        }

        private void createBotMemories()
        {
            Bot bot;
            int numOfPlayers = Game.NUM_OF_PLAYERS;
            Player[] opponents = new Player[numOfPlayers];

            //south is always skipped, it is assumed south is always a player and not a bot
            for(int i = 1; i < numOfPlayers; i++)
            {
                //it's an eyesore, but a 3 player array going clockwise from the bot is being passed to the createMemory method
                bot = (Bot)players[i];
                opponents[0] = this.players[(i + 1) % numOfPlayers];
                opponents[1] = this.players[(i + 2) % numOfPlayers];
                opponents[2] = this.players[(i + 3) % numOfPlayers];
                bot.createMemory(opponents);
            }
        }
    }
}
