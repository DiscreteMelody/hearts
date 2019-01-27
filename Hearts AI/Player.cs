using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts_AI
{
    class Player
    {
        public static readonly string[] Nicknames = { "South", "West", "North", "East" };

        protected string nickname = "";    //north south east or west
        protected string name = "";        //actual names, feature to be implemented later
        protected Hand hand = new Hand();

        protected int totalScore = 0; //point total after a round ends
        protected int roundScore = 0; //points accumulated during a round
        protected bool turn = false;
        protected bool isBot = false;
        protected int place = 1;

        public Player()
        {
            
        }

        //instantiation constructor
        public Player(string player_name, string player_nickname)
        {
            this.name = player_name;
            this.nickname = player_nickname;
        }

        //copy constructor
        public Player(Player player_to_copy)
        {
            this.nickname = player_to_copy.nickname;
            this.name = player_to_copy.nickname;
            this.totalScore = player_to_copy.totalScore;
            this.roundScore = player_to_copy.roundScore;
            this.turn = player_to_copy.turn;
            this.isBot = player_to_copy.isBot;
            this.place = player_to_copy.place;
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
