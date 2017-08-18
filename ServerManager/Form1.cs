using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Management;
using System.IO;
using System.DirectoryServices.AccountManagement;
using System.Net.Mail;

namespace ServerManager
{
    public partial class Suite : Form
    {
        //Control definitions
        public PictureBox headerArea = new PictureBox();
        public PictureBox dropShadow = new PictureBox();
        public PictureBox dropShadow1 = new PictureBox();
        public PictureBox sideArea = new PictureBox();
        public Label headerText = new Label();
        public FlowLayoutPanel serverArea = new FlowLayoutPanel();
        public Label extraText = new Label();
        public Panel panel = new Panel();
        public Label header = new Label();
        public Label user = new Label();
        public Label pass = new Label();
        public TextBox userForm = new TextBox();
        public TextBox passForm = new TextBox();
        public Button acceptButton = new Button();


        //Network totals
        float netTotal;
        float netUsed;

        //Setting iteration numbers
        public int picBox_ID = 0;
        public int onlineCount = 0;

        //username and password (make encypt) WILL BE IN XML
        private string username = Properties.Settings.Default.username;
        private string password = Properties.Settings.Default.password;

        //email username and password
        private string emailUsername = "";
        private string emailPassword = "";
        bool loggedIn = false;

        //Server Array List
        public ArrayList serverIPs = new ArrayList();
        public ArrayList serverNames = new ArrayList();
        public ArrayList serverStatus = new ArrayList();

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
        public int ms = -1;
        public bool gbTrue = true;
        public bool startUp = true;
        public bool addingServer = false;
        public bool click = false;
        public string textFile = Properties.Settings.Default.fileLocation;
        public string emailSend = Properties.Settings.Default.emailSend;
        string temp1;
        string temp2;

        //Rounded corners on application
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        //Initialize the form
        public Suite()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 5, 5));

        }

        //Makes borderless window moveable
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST)
                m.Result = (IntPtr)(HT_CAPTION);
        }

        private const int WM_NCHITTEST = 0x84;
        private const int HT_CLIENT = 0x1;
        private const int HT_CAPTION = 0x2;

        //Form loading
        private void Suite_Load(object sender, EventArgs e)
        {
            //locking size
            MaximumSize = new Size(1006, 643);
            MinimumSize = new Size(1006, 643);

            //Add server prompt but making invisible
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            pictureBox2.Visible = false;
            label10.Visible = false;
            pictureBox8.Visible = false;

            editServerBack.Visible = false;
            editServerBoxIp.Visible = false;
            editServerBoxName.Visible = false;
            editServerButtonAc.Visible = false;
            editServerButtonCn.Visible = false;
            editServerDrop.Visible = false;
            editServerLServer.Visible = false;
            editServerName.Visible = false;
            editServerLEdit.Visible = false;

            pictureBox6.Visible = false;
            textBox6.Visible = false;
            button1.Visible = false;
            button7.Visible = false;

            //Loading stuff 
            loadingBar.Visible = false;
            loadingBox.Visible = false;
            loadingLabel.Visible = false;
            totalloadbar.Visible = false;

            //Remove server prompt
            label6.Visible = false;
            label5.Visible = false;
            label4.Visible = false;
            textBox3.Visible = false;
            comboBox2.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            pictureBox1.Visible = false;

            //Setting background color
            BackColor = Color.FromArgb(25, 118, 210);

            //login
            setEmailAccount();
        }

        //read textfile
        private void textRead()
        {
            int name = 0;
            int ip = 0;
            string path;
            try
            {
                path = @textFile;
                string sub;
                string sub2;
                // Open the file to read from.
                string[] readText = File.ReadAllLines(path);
                foreach (string s in readText)
                {
                    int index = s.IndexOf(',');
                    int len = (s.Length - 1) - index;
                    if (len > 0 && index > 0)
                    {
                        Console.WriteLine(len + ", " + index);
                        sub = s.Substring(0, index);
                        sub2 = s.Substring(index + 1, len);
                        serverNames.Add(sub);
                        serverIPs.Add(sub2);
                    }
                }
            }
            catch
            {
                path = @"c:\serverList.txt";
                MessageBox.Show("Loaded backup");
                string sub;
                string sub2;
                // Open the file to read from.
                string[] readText = File.ReadAllLines(path);
                foreach (string s in readText)
                {
                    int index = s.IndexOf(',');
                    int len = (s.Length - 1) - index;
                    if (len > 0 && index > 0)
                    {
                        Console.WriteLine(len + ", " + index);
                        sub = s.Substring(0, index);
                        sub2 = s.Substring(index + 1, len);
                        serverNames.Add(sub);
                        serverIPs.Add(sub2);
                    }
                }
            }
            finally
            {

            }
        }

        //Log in using email account
        private void setEmailAccount()
        {
            //make stuff invisible
            menuStrip1.Visible = false;
            overViewlabel.Visible = false;
            totalLabel.Visible = false;
            onlineLabel.Visible = false;
            pictureBox3.Visible = false;
            pictureBox7.Visible = false;
            pictureBox4.Visible = false;

            //admin login
            label9.Visible = false;
            button4.Visible = false;
            pictureBox5.Visible = false;
            label8.Visible = false;
            label7.Visible = false;
            textBox4.Visible = false;
            textBox5.Visible = false;


            //Form init
            panel.Size = new Size(200, 175);
            panel.Location = new Point((Width / 2) - 100, (Height / 2) - 100);
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.BackColor = Color.FromArgb(233, 233, 233);

            header.AutoSize = false;
            header.Size = new Size(200, 40);
            header.Location = new Point(0, 0);
            header.Text = "Login";
            header.Font = new Font("Roboto", 18F);
            header.ForeColor = Color.FromArgb(245, 245, 245);
            header.BackColor = Color.FromArgb(144, 202, 249);
            header.TextAlign = ContentAlignment.MiddleCenter;

            user.Size = new Size(150, 20);
            user.Location = new Point(25, 50);
            user.Text = "Username";
            user.Font = new Font("Roboto cn", 12F);
            user.ForeColor = Color.Black;
            user.BackColor = Color.FromArgb(233, 233, 233);
            user.TextAlign = ContentAlignment.MiddleCenter;

            userForm.Size = new Size(150, 20);
            userForm.Location = new Point(25, 70);

            pass.Size = new Size(150, 20);
            pass.Location = new Point(25, 90);
            pass.Text = "Password";
            pass.Font = new Font("Roboto cn", 12F);
            pass.ForeColor = Color.Black;
            pass.BackColor = Color.FromArgb(233, 233, 233);
            pass.TextAlign = ContentAlignment.MiddleCenter;

            passForm.Size = new Size(150, 20);
            passForm.Location = new Point(25, 110);
            passForm.PasswordChar = '●';

            acceptButton.Size = new Size(70, 30);
            acceptButton.Location = new Point(65, 135);
            acceptButton.Text = "Login";
            acceptButton.Font = new Font("Robot cn", 12F);
            //Adding to form
            panel.Controls.Add(header);
            panel.Controls.Add(userForm);
            panel.Controls.Add(user);
            panel.Controls.Add(passForm);
            panel.Controls.Add(pass);
            panel.Controls.Add(acceptButton);
            Controls.Add(panel);

            //Pressing login
            acceptButton.Click += new EventHandler(acceptButton_Click);
        }


        private void acceptButton_Click(object sender, EventArgs e)
        {
            //emial user and pass set
            emailUsername = userForm.Text;
            emailPassword = passForm.Text;


            //Login correct?
            if (emailUsername.Contains('@'))
            {
                bool valid = false;
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
                {
                    valid = context.ValidateCredentials(emailUsername, emailPassword);
                }

                if (valid == true)
                {
                    //everything is correct, login and show main screen
                    menuStrip1.Visible = true;
                    overViewlabel.Visible = true;
                    totalLabel.Visible = true;
                    onlineLabel.Visible = true;
                    pictureBox3.Visible = true;
                   // pictureBox7.Visible = true;
                    pictureBox4.Visible = true;
                    panel.Controls.Remove(header);
                    panel.Controls.Remove(userForm);
                    panel.Controls.Remove(user);
                    panel.Controls.Remove(passForm);
                    panel.Controls.Remove(pass);
                    panel.Controls.Remove(acceptButton);
                    Controls.Remove(panel);
                    loggedIn = true;
                    guiInit();
                }
                //Error
                else
                {
                    MessageBox.Show("Username or Password is incorrect.", "Error");
                }
            }
            else
            {
                MessageBox.Show("Enter a valid Email", "Error");
            }
        }
        //editing text file
        private void editText(string ip, string name, bool add, bool edit)
        {
            string path = @textFile;
            Console.WriteLine(name + "," + ip);
            //Adding to text file, adds to end
            if (add == true)
            {
                using (StreamWriter w = File.AppendText(path))
                {
                    string lineToEdit = temp1 + "," + temp2;
                    string lineToWrite = editServerBoxName.Text + "," + editServerBoxIp.Text;
                    if (edit == true)
                    {
                        w.Close();
                        string text = File.ReadAllText(path);
                        text = text.Replace(lineToEdit, lineToWrite);
                        File.WriteAllText(path, text);
                        int test = editServerDrop.SelectedIndex;
                        Console.WriteLine("EditText: " + test);
                        picBox_ID = test;
                        serverCard(1, test);
                    }
                    else
                    {
                        w.WriteLine(name + "," + ip + Environment.NewLine);
                        w.Close();
                    }
                }
            }
            else if (add == false && edit == false)
            {
                Console.WriteLine(name + "," + ip);
                var oldLines = File.ReadAllLines(path);
                var newLines = oldLines.Where(line => !line.Contains(name + "," + ip));
                File.WriteAllLines(path, newLines);       
            }

            
            
        }

        //making cards using text files
        private void textToCard(int num)
        {

            //Adds a card for each server in the text files list
            for (int i = 0; i < num; i++)
            {
                serverCard(1, i);

            }

        }

        //Sets up the default GUI for the program
        private void guiInit()
        {
            //no maximize
            MaximizeBox = false;

            //extra text
            extraText.AutoSize = true;
            extraText.BackColor = Color.FromArgb(30, 136, 229);
            extraText.Font = new Font("Mistral", 22F);
            extraText.ForeColor = SystemColors.ButtonFace;
            extraText.Location = new Point(225, 10);
            extraText.Name = "extraText";
            extraText.Size = new Size(178, 29);
            extraText.TabIndex = 0;
            extraText.Text = "";
            Controls.Add(extraText);

            //text
            headerText.AutoSize = true;
            headerText.BackColor = Color.FromArgb(30, 136, 229);
            headerText.Font = new Font("Roboto Lt", 22F);
            headerText.ForeColor = SystemColors.ButtonFace;
            headerText.Location = new Point(12, 35);
            headerText.Name = "headerText";
            headerText.Size = new Size(178, 29);
            headerText.TabIndex = 0;
            headerText.Text = "Server Manager";
            Controls.Add(headerText);

            //Background GUI
            headerArea.BackColor = Color.FromArgb(30, 136, 229);
            headerArea.Size = new Size(1920, 50);
            headerArea.Location = new Point(0, 30);
            Controls.Add(headerArea);

            //Drop Shadow
            dropShadow.Image = Properties.Resources.dropshadow2;
            dropShadow.BackgroundImageLayout = ImageLayout.Stretch;
            dropShadow.Location = new Point(0, 67);
            dropShadow.Name = "dropShadow";
            dropShadow.Size = new Size(141, 20);
            dropShadow.TabIndex = 10;
            dropShadow.TabStop = false;
            dropShadow.BackColor = Color.FromArgb(66, 66, 66);
            Controls.Add(dropShadow);

            //Drop Shadow
            dropShadow1.Image = Properties.Resources.dropshadow2;
            dropShadow1.BackgroundImageLayout = ImageLayout.Stretch;
            dropShadow1.Location = new Point(140, 67);
            dropShadow1.Name = "dropShadow1";
            dropShadow1.Size = new Size(1920, 20);
            dropShadow1.TabIndex = 10;
            dropShadow1.TabStop = false;
            dropShadow1.BackColor = Color.FromArgb(224, 224, 224);
            Controls.Add(dropShadow1);

            //sideArea
            sideArea.Size = new Size(141, 605);
            sideArea.Location = new Point(0, 30);
            sideArea.Image = Properties.Resources.sidepanel;
            sideArea.BackgroundImageLayout = ImageLayout.None;
            sideArea.BackColor = Color.FromArgb(225, 225, 225);
            Controls.Add(sideArea);

            //server panel
            serverArea.Size = new Size(866, 550);
            serverArea.Location = new Point(140, 85);
            BackColor = Color.FromArgb(224, 224, 224);
            serverArea.AutoScroll = true;
            Controls.Add(serverArea);

            //Timer for 15min interval
            MyTimer.Interval = (60000 * 10);
            MyTimer.Tick += new EventHandler(MyTimer_Tick);
            MyTimer.Start();

            //Timer for 24hr interval
            Timer24.Interval = (60000 * 60 * 24);
            Timer24.Tick += new EventHandler(Timer24_Tick);
            Timer24.Start();

            //Loading stuff 
            loadingBar.Visible = false;
            loadingBox.Visible = false;
            totalloadbar.Visible = false;
            loadingLabel.Visible = false;
            Controls.Remove(panel);

            //adming account?
            if (username == "null")
            {
                //admin login
                label9.Visible = true;
                button4.Visible = true;
                pictureBox5.Visible = true;
                label8.Visible = true;
                label7.Visible = true;
                textBox4.Visible = true;
                textBox5.Visible = true;
            }
            else
            {
                //load text file prompt
                if (MessageBox.Show("Load Servers from a text Document?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //text read
                   //MessageBox.Show("Loaded from " + Properties.Settings.Default.fileLocation);
                    selectText();
                    MessageBox.Show("Loaded from " + Properties.Settings.Default.fileLocation);
                    textRead();
                    textToCard(serverIPs.Count);

                }
                else
                {
                    //nothing!
                }
            }
            //Startup isnt happeneing any more
            startUp = false;

        }

        //serverCard
        public void serverCard(int num, int count)
        {
            //Vars
            Console.WriteLine(count);
            int test = serverIPs.IndexOf(editServerBoxIp.Text);
            string ip = serverIPs[count].ToString();
            string name = serverNames[count].ToString();
            int roundTripMS = ms;
            if (startUp == true || addingServer == true)
            {
                loadingBar.Visible = true;
                loadingBox.Visible = true;
                loadingLabel.Visible = true;
                totalloadbar.Visible = true;
            }
            loadingBar.Value = 0; ;
            totalloadbar.Maximum = serverIPs.Count * 10;

            //Setting add prompt false(invisible)
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

            //creates a server card, in a loop for text
            for (int i = 0; i < num; i++)
            {

                //Definitions
                PictureBox cardBack = new PictureBox();
                PictureBox pingLabel = new PictureBox();
                Label serverNameLabel = new Label();
                Label serverIPLabel = new Label();
                Panel cardPanel = new Panel();
                PictureBox dropShadowCard = new PictureBox();
                //CustomProgressBar driveSpace = new CustomProgressBar();

                //setting loading text
                loadingLabel.Text = ("Loading " + ip + "...");

                //Drop shadow
                dropShadowCard.Image = Properties.Resources.fewfew1;
                dropShadowCard.BackgroundImageLayout = ImageLayout.Center;
                dropShadowCard.Location = new Point(0, 0);
                dropShadowCard.Size = new Size(cardSizeX + 5, cardSizeY + 5);
                dropShadowCard.TabStop = false;
                dropShadowCard.BackColor = Color.FromArgb(225, 225, 225);
                dropShadowCard.Name = "dropShadowCard" + picBox_ID;
                loadingBar.PerformStep();

                //Panel
                cardPanel.Size = new Size(cardSizeX + 6, cardSizeY + 6);
                cardPanel.Top = startY - 3;
                cardPanel.Left = startX - 3;
                loadingBar.PerformStep();

                // cardPanel.BackColor = Color.Black;
                cardPanel.AutoScroll = true;
                cardPanel.HorizontalScroll.Enabled = false;
                cardPanel.HorizontalScroll.Visible = false;
                cardPanel.Name = "cardPanel" + picBox_ID;
                loadingBar.PerformStep();

                //Specifics for card
                cardBack.Size = new Size(cardSizeX, cardSizeY);
                cardBack.BackColor = Color.FromArgb(245, 245, 245);
                cardBack.Left = 3;
                cardBack.Top = 3;
                cardBack.Name = "cardBack" + picBox_ID;
                loadingBar.PerformStep();

                //Ping
                pingLabel.Size = new Size(cardSizeX, 30);
                pingLabel.BackColor = Color.FromArgb(229, 57, 53);
                pingLabel.Location = new Point(3, 3);
                pingLabel.Name = "pingLabel" + picBox_ID;
                loadingBar.PerformStep();

                //Server name
                serverNameLabel.Text = name;
                serverNameLabel.Location = new Point(5, 10);
                serverNameLabel.BackColor = Color.FromArgb(229, 57, 53);
                serverNameLabel.AutoSize = true;
                serverNameLabel.Font = new Font("Roboto cn", 12F);
                serverNameLabel.ForeColor = Color.FromArgb(250, 250, 250);
                serverNameLabel.Name = "serverNameLabel" + picBox_ID;
                loadingBar.PerformStep();

                //Server IP and server ms using the ping method with 4 cycles
                string temp = roundTripMS.ToString();
                if (temp == "0")     //Below 1 is <1 because we use ints and dont want 0 to appear
                {
                    temp = "<1";
                }
                serverIPLabel.Text = "IP: " + ip + ", MS: " + temp;
                serverIPLabel.Location = new Point(5, 35);
                serverIPLabel.AutoSize = true;
                serverIPLabel.Font = new Font("Roboto cn", 11F);
                serverIPLabel.BackColor = Color.FromArgb(245, 245, 245);
                serverIPLabel.Name = "serverIPLabel" + picBox_ID;
                loadingBar.PerformStep();

                //Adds the server control if we really want to, if we dont do this memory leak
                if (startUp == true || addingServer == true)
                {
                    serverArea.Controls.Add(cardPanel);
                }

                //Checks for gb in server if we want to, takes ~2 mins with 6 servers with 250+ ms and 30 <1 ms
                if (gbTrue == true || addingServer == true)
                {
                    //GB, i hate this but it works?
                    try
                    {
                        int id = 0;
                        //New connection to servers
                        ConnectionOptions options =
                         new ConnectionOptions();

                        //Set admin account for access
                        options.Username = username;
                        options.Password = password;

                        //Define what server is going to be looked at
                        ManagementScope scope = new ManagementScope("\\\\" + ip + "\\root\\cimv2", options);
                        scope.Connect();

                        //What drives are being looked at
                        ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
                        SelectQuery query1 = new SelectQuery("Select * from Win32_LogicalDisk");

                        //Collect all of the data
                        ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                        ManagementObjectCollection queryCollection = searcher.Get();

                        ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(scope, query1);
                        ManagementObjectCollection queryCollection1 = searcher1.Get();

                        //DEBUG
                        foreach (ManagementObject m in queryCollection)
                        {
                            //Clean up
                            m.Dispose();
                        }
                        int strY = 55;
                        foreach (ManagementObject mo in queryCollection1)
                        {

                            Console.WriteLine("  Disk Name : {0}", mo["Name"]);
                            Console.WriteLine("  Disk Size : {0}", mo["Size"]);
                            Console.WriteLine("  FreeSpace : {0}", mo["FreeSpace"]);
                            Console.WriteLine();

                            CustomProgressBar driveSpace = new CustomProgressBar();
                            Label driveName = new Label();
                            //Progress bar(s)
                            driveSpace.Location = new Point(7, strY);

                            float totalSpace = 0;
                            float freeSpace = 0;

                            if (mo["FreeSpace"] != null)
                            {
                                freeSpace = (ulong)mo["FreeSpace"];
                            }
                            if (mo["Size"] != null)
                            {
                                totalSpace = (ulong)mo["Size"];
                            }

                            freeSpace = freeSpace / 1024 / 1024 / 1024;
                            totalSpace = totalSpace / 1024 / 1024 / 1024;
                            float spaceUsed = 0;
                            float spaceAvail = 0;

                            if (totalSpace != 0)
                            {
                                spaceUsed = totalSpace - freeSpace;
                                spaceAvail = (spaceUsed / totalSpace) * 100;
                            }

                            //Adding totals
                            netTotal = netTotal + totalSpace;
                            netUsed = netUsed + spaceUsed;

                            if (spaceAvail > 1 && spaceUsed > 1)
                            {
                                if ((int)spaceAvail > 65 && (int)spaceAvail <= 84)
                                {
                                    driveSpace.ForeColor = Color.FromArgb(255, 241, 118);
                                }
                                else if ((int)spaceAvail > 84)
                                {
                                    driveSpace.ForeColor = Color.FromArgb(229, 57, 53);
                                }
                                else
                                {
                                    driveSpace.ForeColor = Color.FromArgb(76, 175, 80);
                                }

                                driveSpace.Value = (int)spaceAvail;
                                driveSpace.Size = new Size(100, 15);
                                driveSpace.CustomText = "";
                                driveSpace.Name = "driveSpace" + id;

                                driveName.Location = new Point(107, strY - 1);
                                driveName.BackColor = Color.FromArgb(245, 245, 245);
                                driveName.AutoSize = true;
                                driveName.Text = mo["Name"].ToString() + (int)spaceUsed + "/" + (int)totalSpace + " GB";
                                driveName.Font = new Font("Roboto cn", 11F);
                                driveName.ForeColor = Color.Black;
                                driveName.Name = "driveName" + picBox_ID;

                                cardPanel.Controls.Add(driveName);
                                cardPanel.Controls.Add(driveSpace);

                                id++;
                                strY += 20;
                                loadingBar.PerformStep();
                            }

                            mo.Dispose();
                        }
                        //Cleanup
                        queryCollection.Dispose();
                        searcher.Dispose();
                        queryCollection1.Dispose();
                        searcher1.Dispose();
                        totalloadbar.Step = 10;
                        Console.WriteLine(totalloadbar.Maximum);
                        totalloadbar.PerformStep();
                    }

                    //Access denied error, prevents stupid crashing when dev
                    catch (Exception e)
                    {

                    }
                    Console.ReadLine();
                }

                Console.WriteLine(startUp.ToString());
                //Adding to form
                if (startUp == true || addingServer == true)
                {
                    cardPanel.Controls.Add(serverNameLabel);
                    cardPanel.Controls.Add(pingLabel);
                    cardPanel.Controls.Add(serverIPLabel);
                    cardPanel.Controls.Add(cardBack);
                    cardPanel.Controls.Add(dropShadowCard);
                }
                //Ping
                pingServer(ip, picBox_ID);


                //Addding id
                picBox_ID++;

                //differences in pos
                startX += cardBack.Width + 5;
                if (startX > Width - cardSizeX)
                {
                    startY += cardBack.Height + 5;
                    startX = 5;
                }
            }

            //Sets labels using new info found from GB search
            onlineLabel.Text = (onlineCount + "/" + picBox_ID + " Online");
            totalLabel.Text = ((int)netUsed + "/" + (int)netTotal + " GB");

            //We dont always want to look for GB sizes, takes too long and doesnt change too much
            if (picBox_ID == serverIPs.Count)
            {
                gbTrue = false;
            }

            //RE adding servers to array
            populateCombo();

            if (picBox_ID == (serverIPs.Count))
            {
                addingServer = false;
            }

            //Loading stuff 
            loadingBar.Visible = false;
            totalloadbar.Visible = false;
            loadingBox.Visible = false;
            loadingLabel.Visible = false;
        }

        //Accept button
        private void button2_Click(object sender, EventArgs e)
        {
            //error
            if (textBox1.Text == "" || textBox1.Text == null && textBox1.Text == "" || textBox1.Text == null)
            {
                MessageBox.Show("Fill in the Name and IP fields.", "Error");
            }
            else
            {
                //Storing IP and Name

                if (serverIPs.IndexOf(textBox2.Text) > -1)
                {
                    MessageBox.Show("Server already added.", "Error");
                }
                else
                {
                    addingServer = true;
                    serverNames.Add(textBox1.Text);
                    serverIPs.Add(textBox2.Text);
                    Console.WriteLine(textBox1.Text + textBox2.Text);
                    editText(textBox2.Text, textBox1.Text, true, false);
                    serverCard(1, picBox_ID);

                }
            }

        }

        //Cancel button
        private void button3_Click(object sender, EventArgs e)
        {
            //setting add prompt to be invisible
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

            Console.WriteLine(ip);
            Console.WriteLine(i);
            //local vars
            int pingCount = 0;
            bool pingable = false;
            Ping pinger = new Ping();
            long roundTripMS = 0;
            serverCount = 0;

            try
            {
                //Pings 4 times to make sure server is online
                for (int j = 1; j <= 4; j++)
                {
                    PingReply reply = pinger.Send(ip);
                    pingable = reply.Status == IPStatus.Success;
                    roundTripMS = reply.RoundtripTime;
                    if (pingable == true)
                    {
                        pingCount++;
                    }
                }

                //Server is online, do stuff
                if (pingCount == 4)
                {
                    Console.WriteLine("Ping: " + i);
                    //instances of the server card items
                    Console.WriteLine(picBox_ID.ToString());
                    Panel tempPanel = serverArea.Controls.OfType<Panel>().FirstOrDefault(x => x.Name == "cardPanel" + i);
                    PictureBox tempBack2 = tempPanel.Controls.OfType<PictureBox>().FirstOrDefault(x => x.Name == "pingLabel" + i);
                    Label tempLabel2 = tempPanel.Controls.OfType<Label>().FirstOrDefault(x => x.Name == "serverNameLabel" + i);
                    Label tempLabel3 = tempPanel.Controls.OfType<Label>().FirstOrDefault(x => x.Name == "serverIPLabel" + i);

                    //The # is under 1, dont want 0 appearing
                    string temp = roundTripMS.ToString();
                    if (temp == "0")
                    {
                        temp = "<1";
                    }

                    if (serverStatus.Count != serverIPs.Count)
                    {
                        serverStatus.Add("true");
                    }
                    else
                    {
                        if (serverStatus[i].ToString() == "false")
                        {
                            serverStatus[i] = "True";
                        }
                    }

                    //Setting IP and MS
                    tempLabel2.BackColor = Color.FromArgb(76, 175, 80);
                    tempBack2.BackColor = Color.FromArgb(76, 175, 80);
                    tempLabel3.Text = "IP: " + ip + ", MS: " + temp;

                    //debug
                    Console.WriteLine(ip + " is online.");

                    //Takes count of online servers
                    onlineCount++;
                }

            }
            //Error
            catch (PingException)
            {
                //ignore
                if (serverStatus.Count != serverIPs.Count)
                {
                    serverStatus.Add("false");
                }
                else
                {
                    Console.WriteLine("Ping excep: " + i);
                    Console.WriteLine(serverStatus.Count);
                    if (serverStatus[i].ToString() == "true")
                    {
                        serverStatus[i] = "false";
                        emailSender(i);
                    }
                }

            }
            //Cleanup
            finally
            {
                pinger.Dispose();
            }

            //Dont know if this is needed, dont want to delete and break stuff
            ms = (int)roundTripMS;
        }

        //Remove Card
        private void removeCard(int i)
        {

            /*++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
             * 
             * subtract the gb of the server removed from the shit(total and used net)
             * 
             * +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++*/
            Console.WriteLine(picBox_ID);
            Console.WriteLine(i);

            if (picBox_ID > 0)
            {
                //setting instance 
                Panel tempPanel = serverArea.Controls.OfType<Panel>().FirstOrDefault(x => x.Name == "cardPanel" + i);
                PictureBox tempBack1 = tempPanel.Controls.OfType<PictureBox>().FirstOrDefault(x => x.Name == "cardBack" + i);
                PictureBox tempBack2 = tempPanel.Controls.OfType<PictureBox>().FirstOrDefault(x => x.Name == "pingLabel" + i);
                Label tempLabel1 = tempPanel.Controls.OfType<Label>().FirstOrDefault(x => x.Name == "serverIPLabel" + i);
                Label tempLabel2 = tempPanel.Controls.OfType<Label>().FirstOrDefault(x => x.Name == "serverNameLabel" + i);
                PictureBox drop = tempPanel.Controls.OfType<PictureBox>().FirstOrDefault(x => x.Name == "dropShadowCard" + i);
                ProgressBar prog = tempPanel.Controls.OfType<ProgressBar>().FirstOrDefault(x => x.Name == "driveSpace" + i);

                //Removes a serverCard
                serverNames.RemoveAt(i);
                serverIPs.RemoveAt(i);
                tempPanel.Controls.Remove(tempBack1);
                tempPanel.Controls.Remove(tempBack2);
                tempPanel.Controls.Remove(tempLabel1);
                tempPanel.Controls.Remove(tempLabel2);
                tempPanel.Controls.Remove(drop);
                tempPanel.Controls.Remove(prog);
                serverArea.Controls.Remove(tempPanel);

                //Changes name of items
                for (int j = i + 1; j < picBox_ID; j++)
                {

                    //setting instances
                    Panel temp = serverArea.Controls.OfType<Panel>().FirstOrDefault(x => x.Name == "cardPanel" + j);
                    PictureBox temp1 = temp.Controls.OfType<PictureBox>().FirstOrDefault(x => x.Name == "cardBack" + j);
                    PictureBox temp2 = temp.Controls.OfType<PictureBox>().FirstOrDefault(x => x.Name == "pingLabel" + j);
                    Label temp3 = temp.Controls.OfType<Label>().FirstOrDefault(x => x.Name == "serverIPLabel" + j);
                    Label temp4 = temp.Controls.OfType<Label>().FirstOrDefault(x => x.Name == "serverNameLabel" + j);
                    PictureBox temp5 = temp.Controls.OfType<PictureBox>().FirstOrDefault(x => x.Name == "dropShadowCard" + j);
                    ProgressBar temp6 = tempPanel.Controls.OfType<ProgressBar>().FirstOrDefault(x => x.Name == "driveSpace" + j);

                    //Re naming the cards so we dont have (server2, server3, server6, ect.)
                    temp.Name = "cardPanel" + (j - 1);
                    temp1.Name = "cardBack" + (j - 1);
                    temp2.Name = "pingLabel" + (j - 1);
                    temp3.Name = "serverIPLabel" + (j - 1);
                    temp4.Name = "serverNameLabel" + (j - 1);
                    // temp5.Name = "dropShadowCard" + (j - 1);
                    // temp5.Name = "driveSpace" + (j - 1);

                }
                //Remove servers from counters
                picBox_ID--;

                bool pingable;
                Ping pinger = new Ping();
                PingReply reply = pinger.Send(textBox3.Text);
                pingable = reply.Status == IPStatus.Success;
                if (pingable == true)
                {
                    onlineCount--;
                }
            }
            //Errors
            else
            {
                MessageBox.Show("There are no servers to delete", "Error");
            }

            //Update count and arrays
            onlineLabel.Text = (onlineCount + "/" + serverIPs.Count + " Online");
            populateCombo();

        }

        //Prompts to add server by making the items visible
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
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

        //deletes a server using IP THIS IS BROKEN DONT KNOW WHY
        private void button6_Click(object sender, EventArgs e)
        {

            int i = serverIPs.IndexOf(textBox3.Text);
            try
            {
                removeCard(i);
                editText(textBox3.Text, comboBox2.Text, false, false);
                Refresh();

                //hiding prompt
                label6.Visible = false;
                label5.Visible = false;
                label4.Visible = false;
                textBox3.Visible = false;
                comboBox2.Visible = false;
                button5.Visible = false;
                button6.Visible = false;
                pictureBox1.Visible = false;
                textBox3.Text = "";

            }
            //Error
            catch(PingException)
            {
                MessageBox.Show("Server IP not found.", "Error");
            }

        }

        //Cancel the delete server operation
        private void button5_Click(object sender, EventArgs e)
        {
            label6.Visible = false;
            label5.Visible = false;
            label4.Visible = false;
            textBox3.Visible = false;
            comboBox2.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            pictureBox1.Visible = false;
            textBox3.Text = "";
        }

        //Prompts to delete a server
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label6.Visible = true;
            label5.Visible = true;
            label4.Visible = true;
            textBox3.Visible = true;
            comboBox2.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            pictureBox1.Visible = true;
        }

        //Deletes all the servers
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete all the servers?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                netTotal = 0;
                netUsed = 0;
                int temp = picBox_ID;
                for (int i = 0; i < temp; i++)
                {
                    //setting instance 
                    Panel tempPanel = serverArea.Controls.OfType<Panel>().FirstOrDefault(x => x.Name == "cardPanel" + i);
                    PictureBox tempBack1 = tempPanel.Controls.OfType<PictureBox>().FirstOrDefault(x => x.Name == "cardBack" + i);
                    PictureBox tempBack2 = tempPanel.Controls.OfType<PictureBox>().FirstOrDefault(x => x.Name == "pingLabel" + i);
                    Label tempLabel1 = tempPanel.Controls.OfType<Label>().FirstOrDefault(x => x.Name == "serverIPLabel" + i);
                    Label tempLabel2 = tempPanel.Controls.OfType<Label>().FirstOrDefault(x => x.Name == "serverNameLabel" + i);
                    PictureBox drop = tempPanel.Controls.OfType<PictureBox>().FirstOrDefault(x => x.Name == "dropShadowCard" + i);
                    ProgressBar prog = tempPanel.Controls.OfType<ProgressBar>().FirstOrDefault(x => x.Name == "driveSpace" + i);

                    //Removes a serverCard
                    tempPanel.Controls.Remove(tempBack1);
                    tempPanel.Controls.Remove(tempBack2);
                    tempPanel.Controls.Remove(tempLabel1);
                    tempPanel.Controls.Remove(tempLabel2);
                    tempPanel.Controls.Remove(drop);
                    tempPanel.Controls.Remove(prog);
                    serverArea.Controls.Remove(tempPanel);
                }
                //Empty the arrays and setting counters to 0
                serverIPs.Clear();
                serverNames.Clear();
                picBox_ID = 0;
                onlineCount = 0;
                onlineLabel.Text = (onlineCount + "0/0" + picBox_ID + " Online");
            }
            else
            {
                // user clicked no
            }
        }

        //Minimise the program
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.Save();
            WindowState = FormWindowState.Minimized;
        }

        //Kill the program
        private void xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.Save();
            Application.Exit();
        }

        //Add pompt visable
        private void pictureBox3_Click(object sender, EventArgs e)
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

        //Animatons
        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)  //down
        {
            pictureBox3.Image = Properties.Resources.plusdown3;
            Invalidate();
            Refresh();
        }
        private void pictureBox3_MouseUp(object sender, MouseEventArgs e)  //up
        {
            pictureBox3.Image = Properties.Resources.plus;
            Invalidate();
            Refresh();
        }

        //Delete prompt visable
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            label6.Visible = true;
            label5.Visible = true;
            label4.Visible = true;
            textBox3.Visible = true;
            comboBox2.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            pictureBox1.Visible = true;
        }

        //animatons
        private void pictureBox4_MouseDown(object sender, MouseEventArgs e)  //down
        {
            pictureBox4.Image = Properties.Resources.minusdown;
            Invalidate();
            Refresh();
        }
        private void pictureBox4_MouseUp(object sender, MouseEventArgs e)  //up
        {
            pictureBox4.Image = Properties.Resources.minus;
            Invalidate();
            Refresh();
        }

        //Adding array to combo box
        private void populateCombo()
        {
            //first remove the stuff
            comboBox2.Items.Clear();

            //then add them cause u need them
            for (int i = 0; i < serverNames.Count; i++)
            {
                comboBox2.Items.Add(serverNames[i]);
            }
        }

        //make sure that your IP and Name match
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int temp = comboBox2.SelectedIndex;
            textBox3.Text = serverIPs[temp].ToString();
        }

        //Timer that goes off every 5 mins
        private void MyTimer_Tick(object sender, EventArgs e)
        {
            pictureBox8.Visible = true;
            label10.Visible = true;
            onlineCount = 0;
            picBox_ID = 0;
            int num = serverIPs.Count;
            for (int i = 0; i < num; i++)
            {
                serverCard(1, i);
            }
            label10.Visible = false;
            pictureBox8.Visible = false;
        }

        //timers that go off every 24 hours
        private void Timer24_Tick(object sender, EventArgs e)
        {
            label10.Visible = true;
            pictureBox8.Visible = true;
            onlineCount = 0;
            picBox_ID = 0;
            int num = serverIPs.Count;
            for (int i = 0; i < num; i++)
            {
                serverCard(1, i);
                gbTrue = true;
            }
            label10.Visible = false;
            pictureBox8.Visible = false;
        }

        //Save button
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string path = @textFile;
            //saving file

            var oldLines = File.ReadAllLines(path);
            var newLines = oldLines;
            File.WriteAllLines(path, newLines);
            MessageBox.Show("Text File Saved.", "Save");
        }

        //load button
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            onlineCount = 0;
            picBox_ID = 0;
            addingServer = true;
            selectText();
            textRead();
            textToCard(serverIPs.Count);

        }

        //prompting user to select a text file
        private void selectText()
        {
            if (textFile == "c://")
            {
                //Setting script location
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                openFileDialog1.InitialDirectory = textFile;
                openFileDialog1.Filter = "Text File (*.txt)|*.txt";
                openFileDialog1.FilterIndex = 0;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    textFile = openFileDialog1.FileName;
                }

                Properties.Settings.Default.fileLocation = textFile;
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.Save();
            }

        }

        //login to admin user
        private void button4_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Test");
            username = textBox5.Text;
            password = textBox4.Text;

            Properties.Settings.Default.username = textBox5.Text;
            Properties.Settings.Default.password = textBox4.Text;
            Properties.Settings.Default.Save();
            //MessageBox.Show(Properties.Settings.Default.username);
            bool valid = false;
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                valid = context.ValidateCredentials(username, password);
            }

            if (valid == true)
            {
                //admin login
                label9.Visible = false;
                button4.Visible = false;
                pictureBox5.Visible = false;
                label8.Visible = false;
                label7.Visible = false;
                textBox4.Visible = false;
                textBox5.Visible = false;

                //load text file prompt
                if (MessageBox.Show("Load Servers from a text Document?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //text read
                    //MessageBox.Show(Properties.Settings.Default.fileLocation);
                    addingServer = true;
                    selectText();
                    textRead();
                    textToCard(serverIPs.Count);

                }
                else
                {
                    //nothing!
                }
            }
            //error
            else
            {
                MessageBox.Show("Username or Password is incorrect.", "Error");
            }
        }

        public void emailSender(int i)
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(emailSend));
            msg.From = new MailAddress(emailUsername);
            msg.Subject = "Server Alert";
            msg.Body = "Server " + serverNames[i] + " is down.";
            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(emailUsername, emailPassword);
            client.Port = 587; // You can use Port 25 if 587 is blocked
            client.Host = "smtp.office365.com";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            try
            {
                client.Send(msg);

            }
            catch (Exception ex)
            {

            }
        }

        private void statsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox6.Visible = true;
            textBox6.Visible = true;
            button1.Visible = true;
            button7.Visible = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            pictureBox6.Visible = false;
            textBox6.Visible = false;
            button1.Visible = false;
            button7.Visible = false;
            textBox6.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            emailSend = textBox6.Text;
            Properties.Settings.Default.emailSend = textBox6.Text;
            Properties.Settings.Default.Save();
            textBox6.Text = Properties.Settings.Default.emailSend;
            pictureBox6.Visible = false;
            textBox6.Visible = false;
            button1.Visible = false;
            button7.Visible = false;

            MessageBox.Show("Notification email set.");
        }

        //edit server
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            //first remove the stuff
            editServerDrop.Items.Clear();

            //then add them cause u need them
            for (int i = 0; i < serverNames.Count; i++)
            {
                editServerDrop.Items.Add(serverNames[i]);
            }

            editServerBack.Visible = true;
            editServerBoxIp.Visible = true;
            editServerBoxName.Visible = true;
            editServerButtonAc.Visible = true;
            editServerButtonCn.Visible = true;
            editServerDrop.Visible = true;
            editServerLServer.Visible = true;
            editServerName.Visible = true;
            editServerLEdit.Visible = true;

        }

        //animatons
        private void pictureBox7_MouseDown(object sender, MouseEventArgs e)  //down
        {
            pictureBox7.Image = Properties.Resources.editdown1;
            Invalidate();
            Refresh();
        }
        private void pictureBox7_MouseUp(object sender, MouseEventArgs e)  //up
        {
            pictureBox7.Image = Properties.Resources.edit2;
            Invalidate();
            Refresh();
        }

        private void editServerDrop_SelectedIndexChanged(object sender, EventArgs e)
        {

            int j = serverNames.IndexOf(editServerDrop.SelectedItem);

            editServerBoxName.Text = serverNames[j].ToString(); //OUT OF RANGE
            editServerBoxIp.Text = serverIPs[j].ToString();

            temp1 = serverNames[j].ToString();
            temp2 = serverIPs[j].ToString();
        }

        private void editServerButtonAc_Click(object sender, EventArgs e)
        {
            int j = serverNames.IndexOf(editServerDrop.SelectedItem);

            Panel tempPanel = serverArea.Controls.OfType<Panel>().FirstOrDefault(x => x.Name == "cardPanel" + j);
            Label tempLabel1 = tempPanel.Controls.OfType<Label>().FirstOrDefault(x => x.Name == "serverIPLabel" + j);
            Label tempLabel2 = tempPanel.Controls.OfType<Label>().FirstOrDefault(x => x.Name == "serverNameLabel" + j);

            string ip = serverIPs[j].ToString();

            string path = @textFile;

            string lineToEdit = temp1 + "," + temp2;

            string lineToWrite = editServerBoxName.Text + "," + editServerBoxIp.Text;

            tempLabel2.Text = editServerBoxName.Text;
             tempLabel1.Text = "IP: " + editServerBoxIp.Text + ", MS: " + ms;
             pingServer(editServerBoxIp.Text, j);

            /*
             * 
             * CALL EDIT TEXT METHOD
             * 
             */

            serverIPs[j] = editServerBoxIp.Text;
            editText(editServerBoxIp.Text, editServerDrop.Text, true, true);


            editServerBack.Visible = false;
            editServerBoxIp.Visible = false;
            editServerBoxName.Visible = false;
            editServerButtonAc.Visible = false;
            editServerButtonCn.Visible = false;
            editServerDrop.Visible = false;
            editServerLServer.Visible = false;
            editServerName.Visible = false;
            editServerLEdit.Visible = false;
        }

        private void editServerButtonCn_Click(object sender, EventArgs e)
        {
            editServerBack.Visible = false;
            editServerBoxIp.Visible = false;
            editServerBoxName.Visible = false;
            editServerButtonAc.Visible = false;
            editServerButtonCn.Visible = false;
            editServerDrop.Visible = false;
            editServerLServer.Visible = false;
            editServerName.Visible = false;
            editServerLEdit.Visible = false;
        }

    }
}