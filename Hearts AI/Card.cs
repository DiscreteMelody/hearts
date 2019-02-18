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

        private string value = "";
        private string suit = "";
        private int points = 0;
        private int relativeStrength = 0;
        private int absoluteStrength = 0;

        public Card(string card_value, string card_suit, int absolute_strength, int card_points = 0)
        {
            this.value = card_value;
            this.suit = card_suit;
            this.points = card_points;
            this.absoluteStrength = absolute_strength;
        }

        public Card(Card card_to_copy)
        {
            this.value = card_to_copy.value;
            this.suit = card_to_copy.suit;
            this.points = card_to_copy.points;
            this.relativeStrength = card_to_copy.relativeStrength;
            this.absoluteStrength = card_to_copy.absoluteStrength;
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

        public int RelativeStrength
        {
            get { return this.relativeStrength; }
            set { this.relativeStrength = value; }
        }

        //for the bots to better understand their dangerous suits
        //returns an integer value based on the strength of the card value regardless of cards remaining
        public int AbsoluteStrength
        {
            get { return this.absoluteStrength; }
            //read only
        }

        public int Points
        {
            get { return this.points; }
            set { this.points = value; }
        }

        public void lowerStrength(int amount = 1)
        {
            this.relativeStrength -= amount;
        }

        public void setRelativeStrength(int strength)
        {
            this.relativeStrength = strength;
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
