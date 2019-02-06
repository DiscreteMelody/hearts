using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hearts_AI
{
    class GameController
    {
        private HeartsWindow heartsForm;
        private Game game;

        public const int PAUSE_DURATION = 2000;

        public GameController(HeartsWindow window)
        {
            this.heartsForm = window;
            this.createEventHandlers();
            game = new Game(
                new Player("You", "South"),
                new Bot("Matilda", "West"),
                new Bot("Wendy", "North"),
                new Bot("Morton", "East")
            );

            this.game.startRound();
            this.showCards();
            this.updateScoreLabels();
        }

        private void setFormPicBoxToCard(PictureBox picBox, Card card)
        {
            string filepath = card.getCardImagePath();
            Image image = Image.FromFile(filepath);

            picBox.Visible = true;
            heartsForm.setPictureBoxImage(picBox, image);
        }

        private void clearFormPicBox(PictureBox picBox)
        {
            picBox.Image = null;
            picBox.Visible = false;
        }

        private void updateScoreLabels()
        {
            Player player = null;
            string labelText = "";

            for(int i = 0; i < Game.NUM_OF_PLAYERS; i++)
            {
                player = this.game.Players[i];
                labelText = player.Nickname + "\n" + player.TotalScore.ToString() + " (" + player.RoundScore.ToString() + ")";
                this.heartsForm.getLabel(i).Text = labelText;
            }
        }

        private void showCards()
        {
            Player player = null;
            PictureBox picBoxToChange = null;
            Card card = null;
            
            for(int p = 0; p < this.game.Players.Length; p++)
            {
                player = this.game.Players[p];

                for (int c = 0; c < Round.NUM_OF_TRICKS; c++)
                {
                    picBoxToChange = this.heartsForm.getHandPicBoxes(p)[c];

                    if (c < player.Hand.CardsHeld.Count)
                    {
                        card = player.Hand.CardsHeld[c];
                        setFormPicBoxToCard(picBoxToChange, card);
                    }
                    else
                    {
                        clearFormPicBox(picBoxToChange);
                    }
                }
            }
        }

        private void createEventHandlers()
        {
            for(int i = 0; i < Game.NUM_OF_PLAYERS; i++)
            {
                foreach (PictureBox picBox in this.heartsForm.getHandPicBoxes(i))
                {
                    picBox.Click += new EventHandler(this.onCardClicked);
                }
            }
        }

        private async void onCardClicked(object sender, EventArgs e)
        {
            Card cardClicked = null;
            PictureBox pictureBoxClicked = null;
            PictureBox boxToChange = null;
            Player playerHandClicked = null;

            for(int playerCounter = 0; playerCounter < Game.NUM_OF_PLAYERS; playerCounter++)
            {
                for(int picBoxCounter = 0; picBoxCounter < Round.NUM_OF_TRICKS; picBoxCounter++)
                {
                    if (sender == this.heartsForm.getHandPicBoxes(playerCounter)[picBoxCounter])
                    {
                        playerHandClicked = game.Players[playerCounter];
                        pictureBoxClicked = this.heartsForm.getHandPicBoxes(playerCounter)[picBoxCounter];
                        cardClicked = playerHandClicked.Hand.CardsHeld[picBoxCounter];
                        boxToChange = heartsForm.getTrickPicBox(playerCounter);
                        break;
                    }
                }
            }

            if (this.game.Round.isLegalPlay(playerHandClicked, cardClicked) == false)
                return;

            setFormPicBoxToCard(boxToChange, cardClicked);
            clearFormPicBox(pictureBoxClicked);
            game.playCardFromPlayer(playerHandClicked, cardClicked);
            showCards();

            if (this.game.Round.Trick.CardCount == 4)
            {
                await Task.Delay(2000);
                hideTrickPicBoxes();

                Player winner = this.game.Round.Trick.findTrickWinner();
                this.game.Round.Trick.addTrickPoints(winner);
                this.game.Round.startNewTrick();

                if (this.game.Round.TricksRemaining == 0)
                {
                    this.game.endRound();
                    showCards();
                }
                else if(this.game.Round.TricksRemaining > 0)
                {
                    winner.Turn = true;
                }

                updateScoreLabels();
            }

            this.makeBotMove();
        }

        private async void makeBotMove()
        {
            Bot bot = null;
            PictureBox picBoxToClick;
            int playerIndex = 0;
            int picBoxIndex = 0;
            int delay = 500;

            for(int i = 0; i < Game.NUM_OF_PLAYERS; i++)
            {
                if(this.game.Players[i].Turn == true && this.game.Players[i].IsBot == true)
                {
                    bot = (Bot)this.game.Players[i];
                    playerIndex = i;
                    break;
                }
            }

            if (bot == null)
                return;

            bot.chooseCard(this.game);
            picBoxIndex = bot.ChosenCardIndex;
            picBoxToClick = this.heartsForm.getHandPicBoxes(playerIndex)[picBoxIndex];

            await Task.Delay(delay);

            this.onCardClicked(picBoxToClick, EventArgs.Empty);
        }

        private void hideTrickPicBoxes()
        {
            for(int i = 0; i < Game.NUM_OF_PLAYERS; i++)
            {
                this.clearFormPicBox(heartsForm.getTrickPicBox(i));
            }
        }
    }
}
