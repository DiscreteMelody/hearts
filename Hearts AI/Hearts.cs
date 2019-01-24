using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hearts_AI
{
    public partial class HeartsWindow : Form
    {
        private List<List<PictureBox>> playerHandsPicBoxes;
        private List<PictureBox> southHandPicBoxes;
        private List<PictureBox> westHandPicBoxes;
        private List<PictureBox> northHandPicBoxes;
        private List<PictureBox> eastHandPicBoxes;
        private List<PictureBox> trickPicBoxes;
        private List<Label> playerLabels;

        public HeartsWindow()
        {
            InitializeComponent();

            southHandPicBoxes = new List<PictureBox>
            {
                southPicBox1, southPicBox2, southPicBox3, southPicBox4,
                southPicBox5, southPicBox6, southPicBox7, southPicBox8,
                southPicBox9, southPicBox10, southPicBox11, southPicBox12,
                southPicBox13
            };

            westHandPicBoxes = new List<PictureBox>
            {
                westPicBox1, westPicBox2, westPicBox3, westPicBox4,
                westPicBox5, westPicBox6, westPicBox7, westPicBox8,
                westPicBox9, westPicBox10, westPicBox11, westPicBox12,
                westPicBox13
            };

            northHandPicBoxes = new List<PictureBox>
            {
                northPicBox1, northPicBox2, northPicBox3, northPicBox4,
                northPicBox5, northPicBox6, northPicBox7, northPicBox8,
                northPicBox9, northPicBox10, northPicBox11, northPicBox12,
                northPicBox13
            };

            eastHandPicBoxes = new List<PictureBox>
            {
                eastPicBox1, eastPicBox2, eastPicBox3, eastPicBox4,
                eastPicBox5, eastPicBox6, eastPicBox7, eastPicBox8,
                eastPicBox9, eastPicBox10, eastPicBox11, eastPicBox12,
                eastPicBox13
            };

            playerHandsPicBoxes = new List<List<PictureBox>>
            {
                southHandPicBoxes, westHandPicBoxes, northHandPicBoxes, eastHandPicBoxes
            };

            trickPicBoxes = new List<PictureBox>
            {
                southTrickPicBox, westTrickPicBox, northTrickPicBox, eastTrickPicBox
            };

            playerLabels = new List<Label>
            {
                southLabel, westLabel, northLabel, eastLabel
            };
        }

        //sets a specified picture box to have a specified image
        public void setPictureBoxImage(PictureBox picBox, Image image)
        {
            picBox.Image = image;
        }

        public List<PictureBox> getHandPicBoxes(int index)
        {
            return this.playerHandsPicBoxes[index];
        }

        public PictureBox getTrickPicBox(int index)
        {
            return this.trickPicBoxes[index];
        }

        public Label getLabel(int index)
        {
            return this.playerLabels[index];
        }

    }
}
