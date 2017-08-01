using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;   

namespace ServerManager
{
    public partial class Suite : Form
    {
        //Control definitions
        private PictureBox headerArea = new PictureBox();
        private PictureBox dropShadow = new PictureBox();
        private PictureBox dropShadow1 = new PictureBox();
        private PictureBox sideArea = new PictureBox();
        private Label headerText = new Label();
        private FlowLayoutPanel serverArea = new FlowLayoutPanel();
        //private Panel tesgt = new Panel();
       
        public int picBox_ID = 0;


        //ArrayList testBruh = new ArrayList();

        //Server Array List
        public ArrayList serverIPs = new ArrayList();
        public ArrayList serverNames = new ArrayList();

        //Variables
        public int startX = 5;
        public int startY = 5;
        public int cardSizeX = 200;
        public int cardSizeY = 150;
        public string serverName;
        public string serverIP;
        public string serverSpace;
        public string serverMS;
        public int serverCount = 0;

        public Suite()
        {
            InitializeComponent();

        }

        //Form loading
        private void Suite_Load(object sender, EventArgs e)
        {
            //Load GUI
            guiInit();
            
            //Excel read
            excelRead();

            MessageBox.Show(serverIPs.Count.ToString());
            //serverCard make
            excelToCard(serverIPs.Count);

            //making stuff invisible
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            pictureBox2.Visible = false;

            //FORDEV
            pictureBox1.Visible = false;
        }

        //making cards using excel
        private void excelToCard(int num)
        {
            for(int i = 0; i < num; i++)
            {
                serverCard(1, i);

            }
        }

        //reading excel
        private void excelRead()
        {
            {

                //Create COM Objects. Create a COM object for everything that is referenced
                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"S:\Utils\documents\ServerManager\serverlist.xlsx");
                Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
                Excel.Range xlRange = xlWorksheet.UsedRange;

                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;

                //iterate over the rows and columns and print to the console as it appears in the file
                //excel is not zero based!!
                for (int i = 2; i <= rowCount; i++)
                {
                    for (int j = 1; j <= colCount; j++)
                    {
                        //new line
                        if (j == 1)
                        {
                            //Console.Write("\r\n");
                            if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                            {
                                Console.Write(xlRange.Cells[i, j].Value2.ToString() + "\t");
                                serverNames.Add(xlRange.Cells[i, j].Value2.ToString());
                            }
                            
                        }

                        if (j == 2)
                        {
                            //Console.Write("\r\n");
                            if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                            {
                                //Console.Write(xlRange.Cells[i, j].Value2.ToString() + "\t");
                                serverIPs.Add(xlRange.Cells[i, j].Value2.ToString());
                            }

                        }
                    }
                }

                //cleanup
                GC.Collect();
                GC.WaitForPendingFinalizers();

                //rule of thumb for releasing com objects:
                //  never use two dots, all COM objects must be referenced and released individually
                //  ex: [somthing].[something].[something] is bad

                //release com objects to fully kill excel process from running in the background
                Marshal.ReleaseComObject(xlRange);
                Marshal.ReleaseComObject(xlWorksheet);

                //close and release
                xlWorkbook.Close();
                Marshal.ReleaseComObject(xlWorkbook);

                //quit and release
                xlApp.Quit();
                Marshal.ReleaseComObject(xlApp);
            }
        }
        //Sets up the default GUI for the program
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
            this.dropShadow.Size = new System.Drawing.Size(140, 20);
            this.dropShadow.TabIndex = 10;
            this.dropShadow.TabStop = false;
            this.dropShadow.BackColor = Color.FromArgb(225, 225, 225);
            this.Controls.Add(dropShadow);

            //Drop Shadow
            this.dropShadow1.Image = global::ServerManager.Properties.Resources.dropshadow2;
            this.dropShadow1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.dropShadow1.Location = new System.Drawing.Point(140, 37);
            this.dropShadow1.Name = "dropShadow1";
            this.dropShadow1.Size = new System.Drawing.Size(1920, 20);
            this.dropShadow1.TabIndex = 10;
            this.dropShadow1.TabStop = false;
            this.dropShadow1.BackColor = Color.FromArgb(233, 233, 233);
            this.Controls.Add(dropShadow1);

            //server panel
            this.serverArea.Size = new Size(843, 550);
            this.serverArea.Location = new Point(140, 55);
            this.serverArea.AutoScroll = true;
            this.Controls.Add(serverArea);

            //sideArea
            this.sideArea.Size = new Size(140, 1000);
            this.sideArea.Location = new Point(0, 0);
            this.sideArea.BackColor = Color.FromArgb(225, 225, 225);
            this.Controls.Add(sideArea);
        }

        //creats servers
        private void button1_Click(object sender, EventArgs e)
        {
            label1.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
            pictureBox2.Visible = true;
             
        }

        //serverCard
        public void serverCard(int num, int count)
        {
            //Vars
            string ip = serverIPs[count].ToString();
            string name = serverNames[count].ToString();

            //Setting false
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            pictureBox2.Visible = false;
            textBox1.Text = "";
            textBox2.Text = "";

            //Temp fix for the scrolling and server card pos issue
            serverArea.AutoScrollPosition = new Point(0, 0);

            //creates a server card, dont know why its in a loop maybe for excel 
            for (int i = 0; i < num; i++)
            {
                //Definitions
                PictureBox cardBack = new PictureBox();
                PictureBox pingLabel = new PictureBox();
                Label serverNameLabel = new Label();
                Label serverIPLabel = new Label();
                Panel tesgt = new Panel();

                //Panel
                tesgt.Size = new Size(cardSizeX, cardSizeY);
                tesgt.Top = startY;
                tesgt.Left = startX;
                tesgt.BackColor = Color.Black;
                tesgt.Name = "tesgt" + picBox_ID;

                //Specifics for card
                cardBack.Size = new Size(cardSizeX, cardSizeY);
                cardBack.BackColor = Color.White;
                cardBack.Left = 0;
                cardBack.Top = 0;
                cardBack.Name = "cardBack" + picBox_ID;

                //Ping
                pingLabel.Size = new Size(cardSizeX, 30);
                pingLabel.BackColor = Color.Red;
                pingLabel.Location = new Point(0, 0);
                pingLabel.Name = "pingLabel" + picBox_ID;

                //Server name
                serverNameLabel.Text = name;
                serverNameLabel.Location = new Point(1, 5);
                serverNameLabel.BackColor = Color.Red;
                serverNameLabel.AutoSize = true;
                serverNameLabel.Font = new System.Drawing.Font("Roboto", 12F);
                serverNameLabel.ForeColor = Color.White;
                serverNameLabel.Name = "serverNameLabel" + picBox_ID;

                //Server IP
                serverIPLabel.Text = "IP: " + ip;
                serverIPLabel.Location = new Point(2, 40);
                serverIPLabel.BackColor = Color.FromArgb(255, 255, 255);
                serverIPLabel.Name = "serverIPLabel" + picBox_ID;

                //Adding to form
                serverArea.Controls.Add(tesgt);
                tesgt.Controls.Add(serverNameLabel);
                tesgt.Controls.Add(pingLabel);
                tesgt.Controls.Add(serverIPLabel);
                tesgt.Controls.Add(cardBack);

                Console.WriteLine(picBox_ID.ToString());
                //Ping
                pingServer(ip, picBox_ID);

                //Addding id
                picBox_ID++;

                //differences in pos
                startX += cardBack.Width + 5;
                if (startX > this.Width - cardSizeX)
                {
                    startY += cardBack.Height + 5;
                    startX = 5;
                }
            }
        }
        //Accept button
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox1.Text == null && textBox1.Text == "" || textBox1.Text == null)
            {
                MessageBox.Show("Fill in the Name and IP fields.", "Error");
            }
            else
            {
                //Storing IP and Name
                serverNames.Add(textBox1.Text);
                serverIPs.Add(textBox2.Text);
                serverCard(1, picBox_ID);
            }

        }

        //Cancel button
        private void button3_Click(object sender, EventArgs e)
        {
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            pictureBox2.Visible = false;
            textBox1.Text = "";
            textBox2.Text = "";
        }

        //ping
        private void pingServer(string ip, int i)
        {
            bool pingable = false;
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send(ip);
                pingable = reply.Status == IPStatus.Success;
                
                //Server is online, do stuff
                if (pingable == true)
                {
                    Console.WriteLine(picBox_ID.ToString());
                    Panel tempPanel = serverArea.Controls.OfType<Panel>().FirstOrDefault(x => x.Name == "tesgt" + i);
                    PictureBox tempBack2 = tempPanel.Controls.OfType<PictureBox>().FirstOrDefault(x => x.Name == "pingLabel" + i);
                    Label tempLabel2 = tempPanel.Controls.OfType<Label>().FirstOrDefault(x => x.Name == "serverNameLabel" + i);

                    tempLabel2.BackColor = Color.Green;
                    tempBack2.BackColor = Color.Green;

                    //debug
                    Console.WriteLine(ip + " is online.");

                }
            }
            catch (Exception e)
            {
                //ignore
                System.Windows.Forms.MessageBox.Show("Server is offline.", "Server Offline");

            }
        }

        //Remove Card
        private void removeCard(int i)
        {
            if (picBox_ID > 0)
            {
                //instance 
                Panel tempPanel = serverArea.Controls.OfType<Panel>().FirstOrDefault(x => x.Name == "tesgt" + i);
                PictureBox tempBack1 = tempPanel.Controls.OfType<PictureBox>().FirstOrDefault(x => x.Name == "cardBack" + i);
                PictureBox tempBack2 = tempPanel.Controls.OfType<PictureBox>().FirstOrDefault(x => x.Name == "pingLabel" + i);
                Label tempLabel1 = tempPanel.Controls.OfType<Label>().FirstOrDefault(x => x.Name == "serverIPLabel" + i);
                Label tempLabel2 = tempPanel.Controls.OfType<Label>().FirstOrDefault(x => x.Name == "serverNameLabel" + i);

                //Removes a serverCard
                serverNames.RemoveAt(i);
                serverIPs.RemoveAt(i);
                tempPanel.Controls.Remove(tempBack1);
                tempPanel.Controls.Remove(tempBack2);
                tempPanel.Controls.Remove(tempLabel1);
                tempPanel.Controls.Remove(tempLabel2);
                serverArea.Controls.Remove(tempPanel);

                //Changes name of items
                for (int j = i + 1; j < picBox_ID; j++)
                {
                    Panel temp = serverArea.Controls.OfType<Panel>().FirstOrDefault(x => x.Name == "tesgt" + j);
                    PictureBox temp1 = temp.Controls.OfType<PictureBox>().FirstOrDefault(x => x.Name == "cardBack" + j);
                    PictureBox temp2 = temp.Controls.OfType<PictureBox>().FirstOrDefault(x => x.Name == "pingLabel" + j);
                    Label temp3 = temp.Controls.OfType<Label>().FirstOrDefault(x => x.Name == "serverIPLabel" + j);
                    Label temp4 = temp.Controls.OfType<Label>().FirstOrDefault(x => x.Name == "serverNameLabel" + j);

                    temp.Name = "tesgt" + (j - 1);
                    temp1.Name = "cardBack" + (j - 1);
                    temp2.Name = "pingLabel" + (j - 1);
                    temp3.Name = "serverIPLabel" + (j - 1);
                    temp4.Name = "serverNameLabel" + (j - 1);

                }
                picBox_ID--;
            }
            else
            {
                MessageBox.Show("There are no servers to delete", "Error");
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //add popup for number to delete
            removeCard(0);
            Refresh();
        }   
    }
}
