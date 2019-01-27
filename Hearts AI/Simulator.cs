using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Hearts_AI
{
    static class Simulator
    {

        public static int scoreGame()
        {
            int score = 0;

            return score;
        }

        public static bool isPossibleTrick(Round sample_round, Bot permutating_bot)
        {
            bool isPossible = true;
            string leadSuit = sample_round.Trick.LeadSuit;
            int numOfLeadHeld = permutating_bot.Hand.getHeldSuit(leadSuit).Count;
            int numOfLeadNotHeld = sample_round.CardsRemaining[Deck.SUIT_INDEXES[leadSuit]].Count - numOfLeadHeld;
            int numOfLeadRequired = 0;

            if (numOfLeadHeld < numOfLeadNotHeld)
                numOfLeadRequired = 2;

            foreach(Card card in sample_round.Trick.Cards)
            {
                if (card.Suit == leadSuit)
                    numOfLeadRequired--;
            }

            if (numOfLeadRequired > 0)
                isPossible = false;

            return isPossible;
        }

        public static void scoreGame(Game game_to_score, Bot bot_to_score)
        {

        }
    }
}
