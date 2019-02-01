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
        private static float pointsToLowBonus = 3f;    //score per point given to the winner
        private static float pointsToLoserPenalty = -1f;  //per place behind 1st
        private static float pointsPerHighCardPenalty = -1f;  //for each high card above a Card.MIN_VALUE relative value
        private static float pointsForWestLeadingBonus = 1f;    //if a card would result in West leading
        private static float pointsForEndingPenalty = -10000000f;     //for ending the game. if player is in first, this is multiplied by -1
        private static float penaltyForTakingPoints = -10000f;       //for taking points

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

            for(int i = 0; i < sample_round.Trick.Cards.Count; i++)
            {
                if (sample_round.Trick.Cards[i].Suit == leadSuit)
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
            bool heartsAreBroken = game_to_score.Round.HeartsAreBroken;
            float moveScore = 0;

            //to encourage taking tricks where hearts are broken
            if (pointsLeftInRound >= 25 && pointsInTrick < 13)
                pointsInTrick = -1;

            if(trickWinner.Place == 1 && trickWinner != bot_to_score)
            {
                moveScore += (pointsInTrick * pointsToLowBonus);
            }
            else if(trickWinner == bot_to_score)
            {
                moveScore += pointsInTrick * (penaltyForTakingPoints);
            }
            else
            {
                moveScore += pointsInTrick * pointsToLoserPenalty * trickWinner.Place;
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

            Card card;
            for(int i = 0; i < handToScore.CardsHeld.Count; i++)
            {
                card = handToScore.CardsHeld[i];
                moveScore += card.getRelativeValue(handToScore, round);
            }

            return moveScore;
        }
    }
}
