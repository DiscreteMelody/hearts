using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts_AI
{
    [Serializable]
    class Player
    {
        public static readonly string[] Nicknames = { "South", "West", "North", "East" };

        private string nickname = "";    //north south east or west
        private string name = "";        //actual names, feature to be implemented later
        private Hand hand = new Hand();

        private int totalScore = 0; //point total after a round ends
        private int roundScore = 0; //points accumulated during a round
        private bool turn = false;
        private bool isBot = false;
        private int place = 1;

        public Player()
        {
            
        }

        public Player(string player_name, string player_nickname)
        {
            this.name = player_name;
            this.nickname = player_nickname;
        }

        public int TotalScore
        {
            get { return this.totalScore; }
            set { this.totalScore = value; }
        }

        public int RoundScore
        {
            get { return this.roundScore; }
            set { this.roundScore = value; }
        }

        public bool IsBot
        {
            get { return this.isBot; }
            set { this.isBot = value; }
        }

        public Hand Hand
        {
            get { return this.hand; }
        }

        public bool Turn
        {
            get { return this.turn; }
            set { this.turn = value; }
        }

        public string Nickname
        {
            get { return this.nickname; }
            set { this.nickname = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public int Place
        {
            get { return this.place; }
            set { this.place = value; }
        }
    }
}
