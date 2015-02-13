using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace Mass_Flow_Controller
{
    public partial class Form1 : Form
    {
        public SerialPort comPort = new SerialPort();
        public BackgroundWorker bgWorker = new BackgroundWorker();
        public byte MAC_ID;
        public byte[] recvData;
        public bool portBusy = false;
        public int currentSetPoint;
        public int purgeTime = 0;
        public int rampTime = 0;
        public int offTime = 0;
        public bool purgeOn = false;
        public enum Key { lockit, unlockit };

        public Form1()
        {
            InitializeComponent();
            //this background worker constantly check and updates values on the control
            bgWorker.WorkerSupportsCancellation = false;
            bgWorker.WorkerReportsProgress = true;
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgressChanged);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            statusLabel.Text = "Not connected";
            timeLabel.Text = "";
            setPointBox.Enabled = false;
            timeBox.Enabled = false;
            purgeButton.Enabled = false;
            rampButton.Enabled = false;
            offButton.Enabled = false;
            flowTitleLabel.Visible = false;
            flowLabel.Visible = false;
            tempTitleLabel.Visible = false;
            tempLabel.Visible = false;
            pressureTitleLabel.Visible = false;
            pressureLabel.Visible = false;
            pressureButton.Enabled = false;

            //AVAILABLE COM PORTS
            string[] ArrayComPortsNames = null;
            int index = -1;
            string ComPortName = null;

            ArrayComPortsNames = SerialPort.GetPortNames();
            do
            {
                index += 1;
                comBox.Items.Add(ArrayComPortsNames[index]);
            }
            while (!((ArrayComPortsNames[index] == ComPortName) || (index == ArrayComPortsNames.GetUpperBound(0))));
            Array.Sort(ArrayComPortsNames);

            if (index == ArrayComPortsNames.GetUpperBound(0))
            {
                ComPortName = ArrayComPortsNames[0];
            }
            comBox.Text = ArrayComPortsNames[0];
        }

        async Task PutTaskDelay(int x)
        {
            Task t = new Task (() => Thread.Sleep(1000));
            t.Start();
            await t;
        } 

        //Button click functions
        
        private async void purgeButton_Click(object sender, EventArgs e)
        {
            purgeOn = true;
            if(portControl(Key.lockit))
            {
                bool completed = newSetPoint(100);
                portControl(Key.unlockit);
                if (completed)
                {
                    currentSetPoint = 1000;
                    //Purge for 2 minutes
                    statusLabel.Text = "Purging...";
                    statusLabel.Refresh();
                    purgeButton.Enabled = false;
                    rampButton.Enabled = false;

                    for (purgeTime = 120; purgeTime >= 0; purgeTime--)
                    {
                        TimeSpan t = TimeSpan.FromSeconds(purgeTime);
                        timeLabel.Text = t.ToString("mm\\:ss");
                        timeLabel.Refresh();
                        await PutTaskDelay(1000);
                    }

                    if (portControl(Key.lockit))
                    {
                        completed = newSetPoint(0);
                        portControl(Key.unlockit);
                    }
                    if (completed)
                    {
                        statusLabel.Text = "Purge Complete.";
                        statusLabel.Refresh();
                    }
                    purgeButton.Enabled = true;
                    rampButton.Enabled = true;
                    timeLabel.Text = "";
                    timeLabel.Refresh();
                }
            }
            purgeOn = false;
        }

        private async void rampButton_Click(object sender, EventArgs e)
        {
            purgeButton.Enabled = false;
            rampButton.Enabled = false;

            if (checkSetPoint() && checkTime() && portControl(Key.lockit))
            {
                int time = (Convert.ToInt32(timeBox.Text)) * 1000; //ramptime seconds to milliseconds
                //Set Ramp Time
                ramp(time);
                if (listen(1) == 'A') //ACK recieved
                {
                    if (listen(1) == 'A')
                    {
                        int setPoint = Convert.ToInt32(setPointBox.Text);
                        int startPoint = currentSetPoint;
                        currentSetPoint = setPoint;
                        bool complete = newSetPoint(setPoint / 10); //convert mL/min to %, accepted range: 0 to 1000 mL/min
                        portControl(Key.unlockit);
                        if (complete)
                        {
                            for (rampTime = (time/1000); rampTime >= 0; rampTime--)
                            {
                                int totalSec = time / 1000;
                                int elapsedTime = totalSec - rampTime;
                                currentSetPoint = (int)(((double)elapsedTime / (double)totalSec) * (setPoint - startPoint)) + startPoint;
                                //current mL/min = ((elapsed time/ramp length)*(set point - start point))+ start point
                                statusLabel.Text = "Ramping... (@ " + currentSetPoint + " mL/min)";
                                statusLabel.Refresh();
                                
                                TimeSpan t = TimeSpan.FromSeconds(rampTime);
                                timeLabel.Text = t.ToString("mm\\:ss");
                                timeLabel.Refresh();
                                await PutTaskDelay(1000);
                            }
                        }
                    }
                    else
                    {
                        statusLabel.Text = "Unable to set ramp time.";
                        statusLabel.Refresh();
                    }
                }
                else
                {
                    statusLabel.Text = "Packet Error: Unable to process message.(Ramp)";
                    statusLabel.Refresh();
                }
            }
            purgeButton.Enabled = true;
            rampButton.Enabled = true;
            statusLabel.Text = "Current set point: " + currentSetPoint + " mL/min";
            statusLabel.Refresh();
            timeLabel.Text = "";
            timeLabel.Refresh();
        }

        private async void offButton_Click(object sender, EventArgs e)
        {
            purgeTime = 0;
            rampTime = 0;
            purgeButton.Enabled = false;
            rampButton.Enabled = false;
            timeBox.Text = "";
            timeBox.Refresh();
            if(portControl(Key.lockit))
            {
                int time = (currentSetPoint / 20)*1000;

                if(purgeOn) //if purging, just shut flow off
                {
                    ramp(0);
                }
                else //if not purging, ramp down to zero
                {
                    ramp(time);
                }
                
                if (listen(1) == 'A') //ACK recieved
                {
                    if (listen(1) == 'A')
                    {
                        int startPoint = currentSetPoint;
                        bool complete = newSetPoint(0);
                        portControl(Key.unlockit);
                        if (complete)
                        {
                            for (offTime = (time / 1000); offTime >= 0; offTime--)
                            {
                                int totalSec = time/1000;
                                int elapsedTime = totalSec - offTime;
                                currentSetPoint = (int)(((double)elapsedTime / (double)totalSec) * (0 - startPoint)) + startPoint;
                                //current mL/min = ((elapsed time/ramp length)*(set point - start point))+ start point
                                statusLabel.Text = "Ramping down to 0... (@ " + currentSetPoint + " mL/min)";
                                statusLabel.Refresh();
                                TimeSpan t = TimeSpan.FromSeconds(offTime);
                                timeLabel.Text = t.ToString("mm\\:ss");
                                timeLabel.Refresh();
                                await PutTaskDelay(1000);
                            }
                        }
                    }
                    else
                    {
                        statusLabel.Text = "Unable to set ramp time.";
                        statusLabel.Refresh();
                    }
                }
                else
                {
                    statusLabel.Text = "Packet Error: Unable to process message.(Zero)";
                    statusLabel.Refresh();
                }
            }
            purgeButton.Enabled = true;
            rampButton.Enabled = true;
            statusLabel.Text = "Current set point: " + currentSetPoint + " mL/min";
            statusLabel.Refresh();
            timeLabel.Text = "";
            timeLabel.Refresh();
        }

        private async void pressure_Click(object sender, EventArgs e)
        {
            purgeButton.Enabled = false;
            rampButton.Enabled = false;
            offButton.Enabled = false;
            
            MessageBox.Show("Disconnect supply pressure to the Mass Flow Controller. Click \"OK\" when ready.", "Calibration Instructions");
            newSetPoint(100);
            statusLabel.Text = "Please wait...";
            statusLabel.Refresh();

            for (int i = 5; i >= 0; i--)
            {
                TimeSpan t = TimeSpan.FromSeconds(i);
                timeLabel.Text = t.ToString("mm\\:ss");
                timeLabel.Refresh();
                await PutTaskDelay(1000);
            }

            calibratePressure(0);
            newSetPoint(0);
            MessageBox.Show("Reconnect supply pressure to the Mass Flow Controller. Click \"OK\" when ready.", "Calibration Complete");
            purgeButton.Enabled = true;
            rampButton.Enabled = true;
            offButton.Enabled = true;
            timeLabel.Text = "";
            timeLabel.Refresh();
        }

        private void comButton_Click(object sender, EventArgs e)
        {
            //Set up COM connection
            
            //Open Port
            try
            {
                //Set Port Settings
                comPort.PortName = Convert.ToString(comBox.SelectedItem);
                comPort.BaudRate = 9600;
                comPort.DataBits = 8;
                comPort.Handshake = Handshake.None;
                comPort.StopBits = StopBits.One;
                comPort.Parity = Parity.None;
                comPort.ErrorReceived += new SerialErrorReceivedEventHandler(ErrorReceivedHandler);
                comPort.Open();
                comBox.Enabled = false;
                statusLabel.Text = "Connected";
                statusLabel.Refresh();
                comButton.Enabled = false;
                comBox.Enabled = false;
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (getMACID()) //if MAC ID is found
            {
                //Initialize Unit
                if(initializeUnit())
                {
                    //Enable user boxes
                    setPointBox.Enabled = true;
                    timeBox.Enabled = true;
                    purgeButton.Enabled = true;
                    rampButton.Enabled = true;
                    offButton.Enabled = true;

                    //Start checking current flow, temp, and pressure
                    bgWorker.RunWorkerAsync();
                    flowLabel.Visible = true;
                    flowTitleLabel.Visible = true;
                    tempTitleLabel.Visible = true;
                    tempLabel.Visible = true;
                    pressureTitleLabel.Visible = true;
                    pressureLabel.Visible = true;
                    pressureButton.Enabled = true;

                    statusLabel.Text = "Initialization complete.";
                    statusLabel.Refresh();
                }
                else
                {
                    statusLabel.Text = "Unable to initialize unit.";
                    statusLabel.Refresh();
                }
            }
        }

        //Unit initialization functions (triggered by comButton_click)
        
        private bool getMACID()
        {
            bool found = false;
            int MFC_ID = 33; //MFC only at addresses 33 thru 47 

            byte[] MACpacket = new byte[9];
            MACpacket[1] = 0x02; //STX
            MACpacket[2] = 0x80; //Read
            MACpacket[3] = 0x03; //Packet length
            MACpacket[4] = 0x03; //Class ID
            MACpacket[5] = 0x01; //Instance ID
            MACpacket[6] = 0x01; //Attribute ID
            MACpacket[7] = 0x00; //Pad


            while (!found && MFC_ID < 48) //while MAC ID is not found check MFC addresses # 33 thru 47
            {
                MACpacket[0] = (byte)(MFC_ID); //Target MFC Controller Address
                MACpacket[8] = (byte)(checksum(MACpacket)); //checksum

                sendPacket(MACpacket); //Send Packet

                char response = listen(1);
                if (response == 'A') //ACK received
                {
                    if (listen(10) == 'D') //Data received
                    {
                        if (checksum(recvData) == recvData[recvData.Length - 1]) //Verify checksum of packet
                        {
                            //if packet checksum is correct
                            MAC_ID = recvData[7];
                            statusLabel.Text = "MAC ID found. MFC #" + MAC_ID;
                            statusLabel.Refresh();
                            found = true;
                        }
                        else
                        {
                            //if packet checksum is incorrect
                            statusLabel.Text = "MAC ID Checksum Error";
                            statusLabel.Refresh();
                        }
                    }
                }
                else if (response == 'N') //NAK or Error received
                {
                    MessageBox.Show("NAK received.", "COM Error");
                }
                else if (response == 'E') //NAK or Error received
                {
                    MessageBox.Show("Error received.", "COM Error");
                }
                else if (response == 'T') //Timeout received
                {
                    statusLabel.Text = "No MFC at ID #" + MFC_ID;
                    statusLabel.Refresh();
                    MFC_ID++; //try next MFC #
                }
            }

            if (!found)
            {
                statusLabel.Text = "No MFC found.";
                statusLabel.Refresh();
            }
            return found;
        }

        private bool initializeUnit()
        {
            if (digitalMode()) //set to digital mode
            {
                if (freezeFollow()) //set to act on new set point when recieved
                {
                    if (newSetPoint(0))//New Setpoint: 0
                    {
                        if (autoZero()) //Auto Zero: Enable
                        {
                            statusLabel.Text = "Please wait...";
                            statusLabel.Refresh();
                            for (int i = 90; i >= 0; i--) //Wait 90 seconds
                            {
                                TimeSpan t = TimeSpan.FromSeconds(i);
                                timeLabel.Text = t.ToString("mm\\:ss");
                                timeLabel.Refresh();
                                Thread.Sleep(1000);
                            }
                            timeLabel.Text = "";
                            timeLabel.Refresh();
                            if (requestZero()) //Request Zero Enable
                            {
                                //Query Requested Zero Status
                                bool inProgress = true;
                                int count = 0;
                                while (inProgress && count < 30)
                                {
                                    inProgress = !queryZero();
                                    Thread.Sleep(500);
                                    count++;
                                }
                                if (!inProgress)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool autoZero()
        {
            //Enable Auto Zero
            byte[] AZEpacket = new byte[10];
            AZEpacket[0] = (byte)(MAC_ID); //Target MFC Controller Address
            AZEpacket[1] = 0x02; //STX
            AZEpacket[2] = 0x81; //Write
            AZEpacket[3] = 0x04; //Packet length
            AZEpacket[4] = 0x68; //Class ID
            AZEpacket[5] = 0x01; //Instance ID
            AZEpacket[6] = 0xA5; //Attribute ID
            AZEpacket[7] = 0x01; //Enable
            AZEpacket[8] = 0x00; //Pad
            AZEpacket[9] = (byte)(checksum(AZEpacket)); //checksum

            sendPacket(AZEpacket);
            if (listen(1) == 'A') //ACK recieved
            {
                if (listen(1) == 'A')
                {
                    statusLabel.Text = "Auto zero enabled.";
                    statusLabel.Refresh();
                    return true;
                }
                else
                {
                    statusLabel.Text = "Unable to enable auto zero.";
                    statusLabel.Refresh();
                    return false;
                }
            }
            else
            {
                statusLabel.Text = "Packet Error: Unable to process message. (AZE)";
                statusLabel.Refresh();
                return false;
            }
        }

        private bool requestZero()
        {
            //Request Zero Enable
            byte[] RZEpacket = new byte[10];
            RZEpacket[0] = (byte)(MAC_ID); //Target MFC Controller Address
            RZEpacket[1] = 0x02; //STX
            RZEpacket[2] = 0x81; //Write
            RZEpacket[3] = 0x04; //Packet length
            RZEpacket[4] = 0x68; //Class ID
            RZEpacket[5] = 0x01; //Instance ID
            RZEpacket[6] = 0xBA; //Attribute ID
            RZEpacket[7] = 0x01; //Enable
            RZEpacket[8] = 0x00; //Pad
            RZEpacket[9] = (byte)(checksum(RZEpacket)); //checksum

            sendPacket(RZEpacket);
            if (listen(1) == 'A') //ACK recieved
            {
                if (listen(1) == 'A')
                {
                    statusLabel.Text = "Request zero enabled.";
                    statusLabel.Refresh();
                    return true;
                }
                else
                {
                    statusLabel.Text = "Unable to request auto zero.";
                    statusLabel.Refresh();
                    return false;
                }
            }
            else
            {
                statusLabel.Text = "Packet Error: Unable to process message. (RZE)";
                statusLabel.Refresh();
                return false;
            }
        }

        private bool queryZero()
        {
            //Query Zero Status
            byte[] QZSpacket = new byte[9];
            QZSpacket[0] = (byte)(MAC_ID); //Target MFC Controller Address
            QZSpacket[1] = 0x02; //STX
            QZSpacket[2] = 0x80; //Read
            QZSpacket[3] = 0x03; //Packet length
            QZSpacket[4] = 0x68; //Class ID
            QZSpacket[5] = 0x01; //Instance ID
            QZSpacket[6] = 0xBA; //Attribute ID
            QZSpacket[7] = 0x00; //Pad
            QZSpacket[8] = (byte)(checksum(QZSpacket)); //checksum

            sendPacket(QZSpacket);
            if (listen(1) == 'A') //ACK recieved
            {
                if (listen(10) == 'D')
                {
                    statusLabel.Text = "Queried zero status.";
                    statusLabel.Refresh();
                    if (recvData[7] == 0x00) //if zero complete
                    {
                        return true;
                    }
                    else //if zero not complete
                    {
                        return false;
                    }
                }
                else
                {
                    statusLabel.Text = "Unable to query zero status.";
                    statusLabel.Refresh();
                    return false;
                }
            }
            else
            {
                statusLabel.Text = "Packet Error: Unable to process message. (QZS)";
                statusLabel.Refresh();
                return false;
            }
        }

        private bool freezeFollow()
        {
            //Configure MFC to act upon a new set point when recieved
            byte[] FFpacket = new byte[10];
            FFpacket[0] = (byte)(MAC_ID); //Target MFC Controller Address
            FFpacket[1] = 0x02; //STX
            FFpacket[2] = 0x81; //Write
            FFpacket[3] = 0x04; //Packet length
            FFpacket[4] = 0x69; //Class ID
            FFpacket[5] = 0x01; //Instance ID
            FFpacket[6] = 0x05; //Attribute ID
            FFpacket[7] = 0x01; //Enable
            FFpacket[8] = 0x00; //Pad
            FFpacket[9] = (byte)(checksum(FFpacket)); //checksum

            sendPacket(FFpacket);
            if (listen(1) == 'A') //ACK recieved
            {
                if (listen(1) == 'A')
                {
                    statusLabel.Text = "Freeze follow set.";
                    statusLabel.Refresh();
                    return true;
                }
                else
                {
                    statusLabel.Text = "Unable to set freeze follow";
                    statusLabel.Refresh();
                    return false;
                }
            }
            else
            {
                statusLabel.Text = "Packet Error: Unable to process message. (FF)";
                statusLabel.Refresh();
                return false;
            }
        }

        private bool digitalMode()
        {
            //Turn MFC controller digital mode on
            byte[] DMpacket = new byte[10];
            DMpacket[0] = (byte)(MAC_ID); //Target MFC Controller Address
            DMpacket[1] = 0x02; //STX
            DMpacket[2] = 0x81; //Write
            DMpacket[3] = 0x04; //Packet length
            DMpacket[4] = 0x69; //Class ID
            DMpacket[5] = 0x01; //Instance ID
            DMpacket[6] = 0x03; //Attribute ID
            DMpacket[7] = 0x01; //1- Digital, 2- Analog
            DMpacket[8] = 0x00; //Pad
            DMpacket[9] = (byte)(checksum(DMpacket)); //checksum

            sendPacket(DMpacket);
            if (listen(1) == 'A') //ACK recieved
            {
                if (listen(1) == 'A')
                {
                    statusLabel.Text = "Digital mode set.";
                    statusLabel.Refresh();
                    return true;
                }
                else
                {
                    statusLabel.Text = "Unable to set digital mode";
                    statusLabel.Refresh();
                    return false;
                }
            }
            else
            {
                statusLabel.Text = "Packet Error: Unable to process message. (DM)";
                statusLabel.Refresh();
                return false;
            }
        }

        //Receive data functions

        private char listen(int expectedBytes)
        {
            recvData = new byte[expectedBytes];
            int waitCount = 0;
            while(comPort.BytesToRead<1 && waitCount<4) //wait for data in buffer to read
            {
                Thread.Sleep(50);
                waitCount++;
            }
            if (waitCount == 4)
            {
                return 'T'; //if data is not found in buffer after 5 sec- stop
            }

            recvData[0] = (byte)comPort.ReadByte(); //read in first byte to determine if ACK, NAK, or Data
            
            if(expectedBytes == 1) //if only 1 byte ACK or NAK is expected
            {
                if (recvData[0] == 0x06) //if ACK recieved
                {
                    return 'A';
                }
                else if (recvData[0] == 0x16) //if NAK recieved
                {
                    return 'N';
                }
                else //Error
                {
                    return 'E';
                }
            }
            else //if Data is expected
            {
                if (recvData[0] == 0x16) //if NAK recieved
                {
                    return 'N';
                }
                else //if Data is recieved
                {
                    for (int i = 1; i < expectedBytes; i++) //read in entire data packet
                    {
                        waitCount = 0;
                        while (comPort.BytesToRead < 1 && waitCount < 4) //wait for data in buffer to read
                        {
                            Thread.Sleep(5);
                            waitCount++;
                        }
                        if (comPort.BytesToRead < 1)
                        {
                            return 'T'; //if data is not found in buffer after 5 sec- stop
                        }
                        recvData[i] = (byte)comPort.ReadByte(); //read in data 1 byte at a time
                    }
                    if(checksum(recvData) == recvData[recvData.Length-1])
                    {
                        return 'D';
                    }
                    else
                    {
                        return 'E';
                    }
                    
                }
            }
        }

        private void ErrorReceivedHandler(object sender, SerialErrorReceivedEventArgs e)
        {
            SerialError error = e.EventType;
            switch (error)
            {
                case SerialError.Frame:
                    MessageBox.Show("*Frame error*");
                    break;
                case SerialError.Overrun:
                    MessageBox.Show("*Overrun error*");
                    break;
                case SerialError.RXOver:
                    MessageBox.Show("*RXOver error*");
                    break;
                case SerialError.RXParity:
                    MessageBox.Show("*RXParity error*");
                    break;
                case SerialError.TXFull:
                    MessageBox.Show("*TXFull error*");
                    break;
            };
        }

        //Send data functions

        private void sendPacket(byte[] msg)
        {
            if(!(comPort.IsOpen))
            {
                MessageBox.Show("No connection. Cannot send message.");
            }
            else //send instructions
            {
                comPort.DiscardOutBuffer(); //Clear output buffer
                comPort.DiscardInBuffer(); //Clear input buffer
                comPort.Write(msg, 0, msg.Length); //Send bytes to serial port
            }
        }

        private int checksum(byte[] pkt)
        {
            int sum = 0;
            for (int i = 1; i < pkt.Length-1; i++)
            {
                sum = sum + pkt[i];
            }
            return sum % 256;
        }

        private bool newSetPoint(int setPoint)
        {
            int setValue = (int)((327.68 * setPoint) + 16384);

            if(setValue < 0)
            {
                setValue = 0;
            }

            byte[] SETpacket = new byte[11];
            SETpacket[0] = (byte)(MAC_ID); //Target MFC Controller Address
            SETpacket[1] = 0x02; //STX
            SETpacket[2] = 0x81; //Write
            SETpacket[3] = 0x05; //Packet length
            SETpacket[4] = 0x69; //Class ID
            SETpacket[5] = 0x01; //Instance ID
            SETpacket[6] = 0xA4; //Attribute ID
            SETpacket[7] = (byte)setValue; //Data Byte #1 LSB
            SETpacket[8] = (byte)(setValue >> 8); //Data Byte #2 MSB
            SETpacket[9] = 0x00; //Pad
            SETpacket[10] = (byte)(checksum(SETpacket)); //checksum

            sendPacket(SETpacket);
            if(listen(1)=='A') //ACK recieved
            {
                if(listen(1)=='A')
                {
                    statusLabel.Text = "Set point: " + (setPoint * 10) + " mL/min"; //convert % to mL/min
                    statusLabel.Refresh();
                    return true;
                }
                else
                {
                    statusLabel.Text = "Unable to process new set point.";
                    statusLabel.Refresh();
                    return false;
                }
            }
            else
            {
                statusLabel.Text = "Packet Error: Unable to process message.(SET) Set Value: " + setValue;
                statusLabel.Refresh();
                return false;
            }
        }

        private void ramp(int time)
        {
            byte[] rampPacket = new byte[11];
            rampPacket[0] = (byte)(MAC_ID); //Target MFC Controller Address
            rampPacket[1] = 0x02; //STX
            rampPacket[2] = 0x81; //Write
            rampPacket[3] = 0x05; //Packet length
            rampPacket[4] = 0x33; //Class ID
            rampPacket[5] = 0x01; //Instance ID
            rampPacket[6] = 0x13; //Attribute ID
            rampPacket[7] = (byte)time; //Data Byte #1 LSB
            rampPacket[8] = (byte)(time >> 8); //Data Byte #2 MSB
            rampPacket[9] = 0x00; //Pad
            rampPacket[10] = (byte)(checksum(rampPacket)); //checksum

            sendPacket(rampPacket);
        }

        private bool calibratePressure(float input)
        {
            byte[] pressure = new byte[4];
            pressure = BitConverter.GetBytes(input);

            //Request Zero Enable
            byte[] CPpacket = new byte[13];
            CPpacket[0] = (byte)(MAC_ID); //Target MFC Controller Address
            CPpacket[1] = 0x02; //STX
            CPpacket[2] = 0x81; //Write
            CPpacket[3] = 0x07; //Packet length
            CPpacket[4] = 0xa8; //Class ID
            CPpacket[5] = 0x01; //Instance ID
            CPpacket[6] = 0x6e; //Attribute ID
            CPpacket[7] = pressure[0];
            CPpacket[8] = pressure[1];
            CPpacket[9] = pressure[2];
            CPpacket[10] = pressure[3];
            CPpacket[11] = 0x00; //Pad
            CPpacket[12] = (byte)(checksum(CPpacket)); //checksum

            if (portControl(Key.lockit))
            {
                sendPacket(CPpacket);
                portControl(Key.unlockit);
                if (listen(1) == 'A') //ACK recieved
                {
                    if (listen(1) == 'A')
                    {
                        statusLabel.Text = "Pressure calibrated.";
                        statusLabel.Refresh();
                        return true;
                    }
                    else
                    {
                        statusLabel.Text = "Unable to calibrate pressure.";
                        statusLabel.Refresh();
                        return false;
                    }
                }
                else
                {
                    statusLabel.Text = "Packet Error: Unable to process message. (CP)";
                    statusLabel.Refresh();
                    return false;
                }
            }
            return false;
        }

        //Textbox error checking functions

        private bool checkTime()
        {
            int value;
            bool result = Int32.TryParse(timeBox.Text, out value);
            if((result == true))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool checkSetPoint()
        {
            int setValue, timeValue;
            bool setResult = Int32.TryParse(setPointBox.Text, out setValue);
            bool timeResult = Int32.TryParse(timeBox.Text, out timeValue);
            if ((setResult == true) && (setValue >= 0) && (setValue <= 1000))
            {
                //max ramp rate is 20mL/sec
                int time = Math.Abs(currentSetPoint - setValue) / 20; //set time = absolute value of(current set point - new set point)/20 mL
                if (timeValue < time)
                {

                    timeBox.Text = Convert.ToString(Math.Abs(currentSetPoint - setValue) / 20);
                }
                return true;
            }
            else
            {
                MessageBox.Show("Set point must be between 0 and 1000 mL/min.");
                return false;
            }
        }

        private void setPointBox_Leave(object sender, EventArgs e)
        {
            int value;
            bool result = Int32.TryParse(setPointBox.Text, out value);
            if ((result == true) && (value >= 0) && (value <= 1000))
            {
                //max ramp rate is 20mL/sec
                int time = Math.Abs(currentSetPoint - value) / 20; //set time = absolute value of(current set point - new set point)/20 mL
                timeBox.Text = Convert.ToString(time);
                timeBox.Refresh();
            }
        }

        //Semaphores

        private bool portControl(Key k)
        {
            if(k == Key.unlockit) //release lock
            {
                portBusy = false;
                return true;
            }
            else //function wants to lock port
            {
                int count = 0;
                while(portBusy && count<5) //while port is busy- wait
                {
                    Thread.Sleep(100);
                    count++;
                }
                if(!portBusy)
                {
                    portBusy = true; //lock port     
                    return true;
                }
                else
                {
                    statusLabel.Text = "Port locked. No access.";
                    statusLabel.Refresh();
                    return false;
                }
            }
        }

        //Background workers

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int flow = -1;
            float temp = -1;
            float pressure = -1;

            while (true)
            {
                if (portControl(Key.lockit)) //if port is free
                {
                    //check current flow
                    byte[] statusPacket = new byte[9];
                    statusPacket[0] = (byte)(MAC_ID); //Target MFC Controller Address
                    statusPacket[1] = 0x02; //STX
                    statusPacket[2] = 0x80; //Read
                    statusPacket[3] = 0x03; //Packet length
                    statusPacket[4] = 0x6A; //Class ID
                    statusPacket[5] = 0x01; //Instance ID
                    statusPacket[6] = 0xA9; //Attribute ID
                    statusPacket[7] = 0x00; //Pad
                    statusPacket[8] = (byte)(checksum(statusPacket)); //checksum

                    sendPacket(statusPacket);
                    if (listen(1) == 'A') //ACK recieved
                    {
                        if (listen(11) == 'D')
                        {
                            flow = recvData[8];
                            flow = flow << 8;
                            flow = flow + recvData[7];
                            flow = ((int)(((double)flow - 16384) / 327.68))*10;
                        }
                    }

                    //check current temp
                    statusPacket[4] = 0xA4; //Class ID
                    statusPacket[5] = 0x01; //Instance ID
                    statusPacket[6] = 0xBD; //Attribute ID
                    statusPacket[8] = (byte)(checksum(statusPacket)); //checksum
                    
                    sendPacket(statusPacket);
                    if (listen(1) == 'A') //ACK recieved
                    {
                        if (listen(13) == 'D')
                        {
                            temp = BitConverter.ToSingle(recvData, 7);
                        }
                    }

                    //check current pressure
                    statusPacket[4] = 0xA8; //Class ID
                    statusPacket[5] = 0x01; //Instance ID
                    statusPacket[6] = 0x98; //Attribute ID
                    statusPacket[8] = (byte)(checksum(statusPacket)); //checksum

                    sendPacket(statusPacket);
                    if (listen(1) == 'A') //ACK recieved
                    {
                        if (listen(13) == 'D')
                        {
                            pressure = BitConverter.ToSingle(recvData, 7) * 100;
                        }
                    }
                    worker.ReportProgress(0, Convert.ToString(flow) + "," + temp.ToString("#.#") + "," + pressure.ToString("#.#"));
                }
                portControl(Key.unlockit);
                Thread.Sleep(500);
            }
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string data = Convert.ToString(e.UserState); //read in data: "flow,temp,pressure"

            string[] values = data.Split(','); //parse data
            for (int i = 0; i < 3; i++)
            {
                if(values[i]=="")
                {
                    values[i] = "0";
                }
            }
            flowLabel.Text = values[0];
            flowLabel.Refresh();
            tempLabel.Text = values[1];
            tempLabel.Refresh();
            pressureLabel.Text = values[2];
            pressureLabel.Refresh();
        }

    }
}