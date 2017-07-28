using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ServerManager
{
    public partial class Suite : Form
    {
        private PictureBox headerArea = new PictureBox();
        private PictureBox dropShadow = new PictureBox();
        private Label headerText = new Label();
        private Panel serverArea = new Panel();
        

        //test
        //Variables
        public int startX = 5;
        public int startY = 5;
        public int cardSizeX = 200;
        public int cardSizeY = 150;

        PaintEventArgs e;

        public Suite()
        {
            InitializeComponent();

        }

        private void Suite_Load(object sender, EventArgs e)
        {
            guiInit();
        }
        
        private void guiInit()
        {
            //text
            this.headerText.AutoSize = true;
            this.headerText.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.headerText.Font = new System.Drawing.Font("Roboto Lt", 22F);
            this.headerText.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.headerText.Location = new System.Drawing.Point(12, 5);
            this.headerText.Name = "headerText";
            this.headerText.Size = new System.Drawing.Size(178, 29);
            this.headerText.TabIndex = 0;
            this.headerText.Text = "Server Manager";
            this.Controls.Add(headerText);

            //Background GUI
            this.headerArea.BackColor = Color.DarkSlateBlue;
            this.headerArea.Size = new System.Drawing.Size(1920, 50);
            this.headerArea.Location = new System.Drawing.Point(0, 0);
            this.Controls.Add(headerArea);

            //Drop Shadow
            this.dropShadow.Image = global::ServerManager.Properties.Resources.dropshadow2;
            this.dropShadow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.dropShadow.Location = new System.Drawing.Point(0, 37);
            this.dropShadow.Name = "dropShadow";
            this.dropShadow.Size = new System.Drawing.Size(1920, 20);
            this.dropShadow.TabIndex = 10;
            this.dropShadow.TabStop = false;
            this.Controls.Add(dropShadow);

            
            //server panel
            this.serverArea.Size = new Size(843, this.Height - 55);
            this.serverArea.Location = new Point(140, 55);
            this.serverArea.AutoScroll = true;
            this.Controls.Add(serverArea);

            
        }

        private void button1_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < 1; i++)
            {
                //Definitions
                Button cardBack = new Button();
                //Specifics
                cardBack.Size = new Size(cardSizeX, cardSizeY);
                cardBack.BackColor = Color.White;
                cardBack.Left = startX;
                cardBack.Top = startY;
                cardBack.TabIndex = 5;
                serverArea.Controls.Add(cardBack);
                startX += cardBack.Width + 5;
                if (startX > this.Width - cardSizeX)
                {
                    startY += cardBack.Height + 5;
                    startX = 5;
                }
                
            }
        }
    }
}
