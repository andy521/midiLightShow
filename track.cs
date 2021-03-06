﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Xml.Serialization;

namespace midiLightShow
{
    /// <summary>
    /// track class. represents an track in the lightshow
    /// </summary>
    [XmlInclude(typeof(rgbSpotLight))]
    [Serializable]
    public class track
    {
        #region Global variables
        /// <summary>
        /// The name of this track.
        /// </summary>
        public string name = "";
        /// <summary>
        /// The name of the light of this track.
        /// </summary>
        public string LightName = "RGB Spotlight";

        /// <summary>
        /// Indicates if this track plays solo.
        /// </summary>
        public bool solo = false;
        /// <summary>
        /// Indicates if the solo field has changed.
        /// </summary>
        public bool soloSwitch = false;
        /// <summary>
        /// Indicates if this track is muted.
        /// </summary>
        public bool mute = false;
        /// <summary>
        /// Indicates if this track sould be deleted.
        /// </summary>
        public bool delete = false;
        public bool clone = false;

        /// <summary>
        /// The top y-position of this track in the panel.
        /// </summary>
        public int yPos = 0;
        /// <summary>
        /// The bottom y-position of this track in the panel.
        /// </summary>
        public int yEnd = 0;
        /// <summary>
        /// The end time of the last event in this track. 
        /// </summary>
        public int currentMaxTime = 0;
        /// <summary>
        /// Number of events in this track (used as event index)
        /// </summary>
        public int eventCount = 1;
        /// <summary>
        /// Longest event duration in this track. 
        /// </summary>
        public int maxEventLength = 0;
        /// <summary>
        /// The x-position of the last event in this track.
        /// </summary>
        public int lastBlockXPos = 110;

        public string eventColor = "Red";
        /// <summary>
        /// The dmxLight object of this track.
        /// </summary>
        [XmlIgnore]
        public dmxLight light = new dmxLight();
       
        /// <summary>
        /// The events list of this track.
        /// </summary>
        public List<showEvent> events = new List<showEvent>();

        /// <summary>
        /// Panel reference needed for drawing controls.
        /// </summary>
        [XmlIgnore]
        public Panel pTimeLine;
        /// <summary>
        /// Label control for displaying the track name.
        /// </summary>
        [XmlIgnore]
        public Label lbName = new Label();
        /// <summary>
        /// Checkbox for indicating if this track needs to be muted.
        /// </summary>
        [XmlIgnore]
        public CheckBox cbMute = new CheckBox();
        /// <summary>
        /// Checkbox for indicating if this track needs to be played solo.
        /// </summary>
        [XmlIgnore]
        public CheckBox cbSolo = new CheckBox();
        /// <summary>
        /// Button for opening the options dialog for this track.
        /// </summary>
        [XmlIgnore]
        public PictureBox pbOptions = new PictureBox();
        /// <summary>
        /// Button for opening the add-event dialog for this track.
        /// </summary>
        [XmlIgnore]
        public Button bAddEvent = new Button();
        /// <summary>
        /// Options form for this track.
        /// </summary>
        [XmlIgnore]
        public TrackOptionsForm frmOptions = new TrackOptionsForm();
        /// <summary>
        /// Add-event form for this track.
        /// </summary>
        [XmlIgnore]
        public AddShowEvent frmAddShowEvent = new AddShowEvent();
        
        /// <summary>
        /// Static Dictionary for mapping name strings with FullyQualifiedAssemblyNames.
        /// </summary>
        [XmlIgnore]
        public static Dictionary<string, string> typeMap = new Dictionary<string, string>();

        #endregion
        #region Static methods
        /// <summary>
        /// Generate the global typemap.
        /// </summary>
        public static void makeTypeMap()
        {
            track.typeMap.Add("RGB Spotlight", typeof(rgbSpotLight).AssemblyQualifiedName);
            track.typeMap.Add("360 Spotlight", typeof(_360SpotLight).AssemblyQualifiedName);
            track.typeMap.Add("Lazer", typeof(lazer).AssemblyQualifiedName);
            track.typeMap.Add("Mirror light", typeof(mirrorLight).AssemblyQualifiedName);
            Console.WriteLine(DateTime.Now.ToLongTimeString() + "\t\tGenerated TypeMap.");
            
        }
        #endregion
        #region Constructors
        /// <summary>
        /// Empty constructor for deserializing xml.
        /// </summary>
        public track() { }

        /// <summary>
        /// Create new track
        /// </summary>
        /// <param name="name">The name of the track</param>
        /// <param name="yPos">The top y-position of the track</param>
        /// <param name="yEnd">The bottom y-position of the track</param>
        /// <param name="p">The panel to draw controls on</param>
        public track(string name, int yPos, int yEnd, Panel p)
        {
            this.name = name;
            this.yPos = yPos;
            this.yEnd = yEnd;
            this.pTimeLine = p;
            this.frmOptions.tbName.Text = this.name;
            this.frmOptions.tbTitle.Text = "Options for '" + this.name + "'";
            this.frmOptions.cbLights.Text = this.LightName;
            this.frmOptions.cdEventColor.Color = Color.FromName(this.eventColor);
            this.frmOptions.lbColorPreview.ForeColor = Color.FromName(this.eventColor);
            Type targetType = Type.GetType(track.typeMap[this.frmOptions.cbLights.SelectedItem.ToString()],true);
            this.light = Activator.CreateInstance(targetType) as dmxLight;
            this.frmAddShowEvent.light = this.light;
            this.pTimeLine.Invalidate();
        }
        #endregion
        #region Track methods
        /// <summary>
        /// Draw the controls for this track on the panel
        /// </summary>
        public void drawControls()
        {
            this.lbName.Text = this.name;
            this.lbName.Location = new Point(4, this.yPos + 3);
            this.lbName.Bounds = new Rectangle(lbName.Location, new Size(165, 18));
            this.pTimeLine.Controls.Add(lbName);

            this.cbMute.Text = "Mute";
            this.cbMute.Location = new Point(4, this.yPos + 30);
            this.cbMute.Bounds = new Rectangle(cbMute.Location, new Size(50, 20));
            this.cbMute.CheckedChanged += cbMute_CheckedChanged;
            this.pTimeLine.Controls.Add(cbMute);

            this.cbSolo.Text = "Solo";
            this.cbSolo.Location = new Point(60, this.yPos + 30);
            this.cbSolo.Bounds = new Rectangle(cbSolo.Location, new Size(50, 20));
            this.cbSolo.CheckedChanged += cbSolo_CheckedChanged;
            this.pTimeLine.Controls.Add(cbSolo);

            this.pbOptions.Image = global::midiLightShow.Properties.Resources.options_icon;
            this.pbOptions.Cursor = Cursors.Hand;
            this.pbOptions.Tag = "Track";
            this.pbOptions.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pbOptions.Size = new Size(25, 25);
            this.pbOptions.Location = new Point(120, this.yPos + 30);
            //this.bOptions.Bounds = new Rectangle(bOptions.Location, new Size(20, 50));
            this.pbOptions.Click += bOptions_Click;
            this.pTimeLine.Controls.Add(pbOptions);

            this.bAddEvent.Text = "Add event";
            this.bAddEvent.Location = new Point(170, this.yPos + 3);
            this.bAddEvent.Size = new Size(50, 47);
            this.bAddEvent.BackColor = SystemColors.ControlDarkDark;
            this.bAddEvent.ForeColor = SystemColors.Highlight;
            this.bAddEvent.Tag = "Track";
            this.bAddEvent.FlatStyle = FlatStyle.Flat;
            this.bAddEvent.FlatAppearance.BorderSize = 0;
            this.bAddEvent.FlatAppearance.MouseDownBackColor = Color.FromArgb(64, 64, 64);
            this.bAddEvent.FlatAppearance.MouseOverBackColor = SystemColors.ControlDark;
            this.bAddEvent.MouseEnter += bAddEvent_MouseEnter;
            this.bAddEvent.MouseLeave += bAddEvent_MouseLeave;
            this.bAddEvent.Bounds = new Rectangle(this.bAddEvent.Location, new Size(50, 47));
            this.bAddEvent.Click += bAddEvent_Click;
            this.pTimeLine.Controls.Add(bAddEvent);

            Console.WriteLine(DateTime.Now.ToLongTimeString() + "\t\tGenerated controls for track '" + this.name + "'.");
        }

        void bAddEvent_MouseLeave(object sender, EventArgs e)
        {
            this.bAddEvent.ForeColor = SystemColors.Highlight;
        }

        void bAddEvent_MouseEnter(object sender, EventArgs e)
        {
            this.bAddEvent.ForeColor = SystemColors.HotTrack;
        }

        /// <summary>
        /// Reposition controls after an track has been deleted
        /// </summary>
        public void repositionControls()
        {
            this.lbName.Location = new Point(4, this.yPos + 3);
            this.cbMute.Location = new Point(4, this.yPos + 30);
            this.cbSolo.Location = new Point(60, this.yPos + 30);
            this.pbOptions.Location = new Point(140, this.yPos + 25);
            this.bAddEvent.Location = new Point(170, this.yPos + 3);
        }

        /// <summary>
        /// Click event handler for addEvent button
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        void bAddEvent_Click(object sender, EventArgs e)
        {
            openAddEventDialog();
        }

        /// <summary>
        /// Open the addShowEvent dialog of this track
        /// </summary>
        private void openAddEventDialog()
        {
            this.frmAddShowEvent.reset();
            this.frmAddShowEvent.tbStartTime.Text = this.currentMaxTime.ToString();
            this.frmAddShowEvent.light = this.light;
            this.frmAddShowEvent.tbTitle.Text = "Add show event to '" + this.name + "'";
            this.frmAddShowEvent.init();
            DialogResult dr = this.frmAddShowEvent.ShowDialog();
            if (dr == DialogResult.OK)
            {
                checkAndShowEvent();
                return;
            }
        }
        /// <summary>
        /// Checks if the recently entered event is valid and creates and adds an showEvent object to the event list of this track
        /// </summary>
        private void checkAndShowEvent()
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString() + "\t\tTrying to create new event...");
            int duration = Convert.ToInt32(this.frmAddShowEvent.tbDuration.Text);
            int start = Convert.ToInt32(this.frmAddShowEvent.tbStartTime.Text);
            bool valid = true;
            if (start < 0 || duration < 1)
            {
                valid = false;
            }
            foreach (showEvent ev in this.events)
            {
                if (start > ev.startTime && start < ev.startTime + ev.duration)
                {
                    valid = false;
                }
                if (start + duration > ev.startTime && start + duration < ev.startTime + ev.duration)
                {
                    valid = false;
                }
            }
            if (!valid)
            {
                DMXStudioMessageBox.Show("show events cannot overlap or have negative values!");
                Console.WriteLine(DateTime.Now.ToLongTimeString() + "\t\tFailed! (Invalid event timing for new event).");
                return;
            }

            this.events.Add(new showEvent(Convert.ToInt32(this.frmAddShowEvent.tbStartTime.Text), this.frmAddShowEvent.duration, this.frmAddShowEvent.cbFunctions.Text, this.frmAddShowEvent.paraString, this.eventCount));
            this.currentMaxTime += this.frmAddShowEvent.duration;
            string[] paras = new string[this.frmAddShowEvent.parameters.Count];
            this.frmAddShowEvent.parameters.CopyTo(paras);
            this.events[this.events.Count - 1].parameters = paras.ToList();
            this.debugNewEvent(this.events[this.eventCount - 1]);
            if (this.events[this.eventCount - 1].startTime + this.events[this.eventCount - 1].duration > this.maxEventLength)
            {
                this.maxEventLength = this.events[this.eventCount - 1].startTime + this.events[this.eventCount - 1].duration;
            }
            this.eventCount++;
            this.pTimeLine.Invalidate();
            return;
        }

        /// <summary>
        /// Writes debug information to the debug window about the given showEvent object
        /// </summary>
        /// <param name="sEvent">The showEvent object to write debug information about</param>
        private void debugNewEvent(showEvent sEvent)
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString() + "\t\tComplete, created event with the following data:");
            FieldInfo[] fields = sEvent.GetType().GetFields();
            foreach(FieldInfo field in fields)
            {
                Console.WriteLine(string.Format("\t{0}: {1}", field.Name, field.GetValue(sEvent).ToString()));
            }
        }

        /// <summary>
        /// Click event handler of the trackOptions button
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        void bOptions_Click(object sender, EventArgs e)
        {
            openTrackOptionsDialog();
        }

        /// <summary>
        /// Opens the trackOptions dialog of this track and optionally updates various track options
        /// </summary>
        private void openTrackOptionsDialog()
        {
            DialogResult dr = this.frmOptions.ShowDialog();
            if (dr == DialogResult.OK)
            {
                this.name = frmOptions.tbName.Text;
                this.lbName.Text = this.name;
                this.lbName.Bounds = new Rectangle(lbName.Location, new Size(165, 18));
                if(this.LightName != this.frmOptions.cbLights.SelectedItem.ToString())
                {
                    if (this.events.Count > 0)
                    {
                        if (DMXStudioMessageBox.Show("Changing the light type will remove all show events on this track.\nAre you sure you want to change the light type?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            this.LightName = this.frmOptions.cbLights.SelectedItem.ToString();
                            this.light = Activator.CreateInstance(Type.GetType(track.typeMap[this.frmOptions.cbLights.SelectedItem.ToString()])) as dmxLight;
                            this.events.Clear();
                            this.frmAddShowEvent.light = this.light;
                            Console.WriteLine(DateTime.Now.ToLongTimeString() + "\t\tChanged light type for track '" + this.name + "' to '"+ this.LightName+"'.");
                        }
                    }
                    else
                    {
                        this.LightName = this.frmOptions.cbLights.SelectedItem.ToString();
                        this.light = Activator.CreateInstance(Type.GetType(track.typeMap[this.frmOptions.cbLights.SelectedItem.ToString()])) as dmxLight;
                        this.frmAddShowEvent.light = this.light;
                        Console.WriteLine(DateTime.Now.ToLongTimeString() + "\t\tChanged light type for track '" + this.name + "' to '" + this.LightName + "'.");
                    }
                }
                
                this.light.comPort = this.frmOptions.cbComPorts.Text;
                this.light.live = this.frmOptions.cbLive.Checked;
                this.eventColor = this.frmOptions.cdEventColor.Color.Name;
                this.light.connectDMX();
                this.frmOptions.tbTitle.Text = "Options for '" + this.name + "'";
                this.pTimeLine.Invalidate();
            }
            else if (dr == DialogResult.Abort)
            {
                this.delete = true;
                this.pTimeLine.Invalidate();
            }
            else if (dr == DialogResult.Retry)
            {
                this.clone = true;
                this.pTimeLine.Invalidate();
            }
        }

        /// <summary>
        /// Event handler for the CheckedChanged event of the mute checkbox
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        void cbMute_CheckedChanged(object sender, EventArgs e)
        {
           if(!this.cbMute.Checked)
           {
               this.mute = false;
           }
           else
           {
               this.mute = true;
           }
           Console.WriteLine(DateTime.Now.ToLongTimeString() + "\t\tToggled mute option for track '" + this.name + "'.");
           this.pTimeLine.Invalidate();
        }

        /// <summary>
        /// Event handler for the CheckedChanged event of the solo checkbox
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        void cbSolo_CheckedChanged(object sender, EventArgs e)
        {
            if(!this.cbSolo.Checked)
            {
                this.solo = false;
                this.soloSwitch = true;
            }
            else
            {
                this.solo = true;
            }
            this.pTimeLine.Invalidate();
        }

        /// <summary>
        /// Remove controls from the panel
        /// </summary>
        public void removeControls()
        {
            this.pTimeLine.Controls.Remove(this.lbName);
            this.pTimeLine.Controls.Remove(this.bAddEvent);
            this.pTimeLine.Controls.Remove(this.pbOptions);
            this.pTimeLine.Controls.Remove(this.cbMute);
            this.pTimeLine.Controls.Remove(this.cbSolo);
        }

        /// <summary>
        /// Set some track options after a file has been imported
        /// </summary>
        public void afterImport()
        {
            this.frmOptions.tbName.Text = this.name;
            this.frmOptions.Text = "Options for '" + this.name + "'";
            this.frmOptions.cbLights.Text = this.LightName;
            this.frmOptions.cdEventColor.Color = Color.FromName(this.eventColor);
            this.frmOptions.lbColorPreview.ForeColor = this.frmOptions.cdEventColor.Color;
            Type targetType = Type.GetType(track.typeMap[this.frmOptions.cbLights.SelectedItem.ToString()], true);
            this.light = Activator.CreateInstance(targetType) as dmxLight;
            this.lbName.Text = this.name;
            this.lbName.Bounds = new Rectangle(lbName.Location, new Size(165, 18));
        }
        #endregion
    }
}
