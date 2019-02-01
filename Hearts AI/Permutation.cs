using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts_AI
{
    class Permutation
    {
        private Card[] cards;
        private float multiplier;

        public Permutation(Card[] permutated_cards, float probability_multiplier)
        {
            this.cards = permutated_cards;
            this.multiplier = probability_multiplier;
        }

        public Card[] Cards
        {
            get { return this.cards; }
        }

        public float Multiplier
        {
            get { return this.multiplier; }
        }
    }
}
