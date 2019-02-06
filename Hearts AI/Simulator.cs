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
        private static int pointsToLowBonus = 3;    //score per point given to the winner
        private static int pointsToLoserPenalty = -1;  //per place behind 1st
        private static int pointsPerHighCardPenalty = -1;  //for each high card above a Card.MIN_VALUE relative value
        private static int pointsForWestLeadingBonus = 1;    //if a card would result in West leading
        private static int pointsForEndingPenalty = -1000;     //for ending the game. if player is in first, this is multiplied by -1
        private static int pointsForTakingPointsPenalty = -10;

        public static bool isPossibleTrick(Round sample_round, Bot permutating_bot)
        {
            bool isPossible = true;
            string leadSuit = sample_round.Trick.LeadSuit;
            int numOfLeadHeld = permutating_bot.Hand.getHeldSuit(leadSuit).Count;
            int numOfLeadNotHeld = sample_round.CardsRemaining[Deck.SUIT_INDEXES[leadSuit]].Count - numOfLeadHeld;
            int numOfLeadRequired = 0;

            if (sample_round.Trick.Points > 0 && sample_round.IsOpeningTrick == true)
                return false;

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

        public static float scoreGame(Game game_to_score, Bot bot_to_score)
        {
            Round round = game_to_score.Round;
            Player trickWinner = round.Trick.findTrickWinner();
            int pointsInTrick = round.Trick.Points;
            string leadSuit = round.Trick.LeadSuit;
            Hand handToScore = bot_to_score.Hand;
            int pointsLeftInRound = round.PointsRemaining;
            float moveScore = 0;

            //to encourage taking the first few points in a round
            if (pointsInTrick >= 1 && pointsInTrick < 13 && pointsLeftInRound > 23)
                pointsInTrick = -1;

            if(trickWinner.Place == 1 && trickWinner != bot_to_score)
            {
                moveScore += (pointsInTrick * pointsToLowBonus);
            }
            else if(trickWinner == bot_to_score)
            {
                moveScore += (pointsInTrick * pointsForTakingPointsPenalty);
            }
            else
            {
                moveScore += (pointsInTrick * pointsToLoserPenalty * trickWinner.Place);
            }

            if(trickWinner.RoundScore + trickWinner.TotalScore + pointsInTrick >= Game.ENDING_SCORE)
            {
                if(bot_to_score.Place == 1)
                {
                    moveScore += pointsForEndingPenalty * -1;
                }
                else
                {
                    moveScore += pointsForEndingPenalty;
                }
            }

            if(trickWinner == bot_to_score.MemoryOfPlayers[0].MemorizedPlayer)
            {
                moveScore += pointsForWestLeadingBonus;
            }

            foreach(Card card in handToScore.CardsHeld)
            {
                moveScore += (card.getRelativeValue(handToScore, round) * pointsPerHighCardPenalty);
            }

            return moveScore;
        }
    }
}
