using System.ComponentModel;
using System.Text;
using System.Collections.Generic;
using System.IO;
using BrawlLib.IO;
using System.Collections.ObjectModel;

namespace System.Windows.Forms
{
    public class StageModuleConverter : Form
    {
        public const string FILTER = "Module files (*.rel)|*.rel";

        #region Definition of "Stage" inner class
        public class Stage {
            /// <summary>
            /// The stage ID, as used by the module files and the Custom SSS code.
            /// </summary>
            public byte ID { get; private set; }
            /// <summary>
            /// The stage name (e.g. "Flat Zone 2").
            /// </summary>
            public string Name { get; private set; }
            /// <summary>
            /// The .rel filename (e.g. "st_gw.rel").
            /// </summary>
            public string RelName { get; private set; }
            /// <summary>
            /// The name of the .pac file, minus "STG" and anything    
            /// after an underscore (e.g. "gw" or "emblem").
            /// </summary>
            public string PacBasename { get; private set; }

            public bool ContainsPac(string filename) {
                int i = filename.IndexOfAny(new char[] { '.', '_' });
                if (filename.Length < 3 || i < 0) return false;

                string input_basename = filename.Substring(3, i - 3);
                return String.Equals(input_basename.ToLower(), PacBasename.ToLower(), StringComparison.InvariantCultureIgnoreCase);
            }

            public Stage(byte id, string name, string relname, string pac_basename) {
                this.ID = id;
                this.Name = name;
                this.RelName = relname;
                this.PacBasename = pac_basename;
            }

            public override string ToString() { return Name; }
        }
        #endregion

        public readonly static Stage[] Stages = new Stage[] {
            //        ID    Display Name              .rel filename        Name without STG
            new Stage(0x00, "STGCUSTOM##.pac",        "st_custom##.rel",   "custom"),
            new Stage(0x01, "Battlefield",            "st_battle.rel",     "battlefield"),
            new Stage(0x02, "Final Destination",      "st_final.rel",      "final"),
            new Stage(0x03, "Delfino Plaza",          "st_dolpic.rel",     "dolpic"),
            new Stage(0x04, "Luigi's Mansion",        "st_mansion.rel",    "mansion"),
            new Stage(0x05, "Mushroomy Kingdom",      "st_mariopast.rel",  "mariopast"),
            new Stage(0x06, "Mario Circuit",          "st_kart.rel",       "kart"),
            new Stage(0x07, "75 m",                   "st_donkey.rel",     "donkey"),
            new Stage(0x08, "Rumble Falls",           "st_jungle.rel",     "jungle"),
            new Stage(0x09, "Pirate Ship",            "st_pirates.rel",    "pirates"),
            new Stage(0x0B, "Norfair",                "st_norfair.rel",    "norfair"),
            new Stage(0x0C, "Frigate Orpheon",        "st_orpheon.rel",    "orpheon"),
            new Stage(0x0D, "Yoshi's Island (Brawl)", "st_crayon.rel",     "crayon"),
            new Stage(0x0E, "Halberd",                "st_halberd.rel",    "halberd"),
            new Stage(0x13, "Lylat Cruise",           "st_starfox.rel",    "starfox"),
            new Stage(0x14, "Pokemon Stadium 2",      "st_stadium.rel",    "stadium"),
            new Stage(0x15, "Spear Pillar",           "st_tengan.rel",     "tengan"),
            new Stage(0x16, "Port Town Aero Dive",    "st_fzero.rel",      "fzero"),
            new Stage(0x17, "Summit",                 "st_ice.rel",        "ice"),
            new Stage(0x18, "Flat Zone 2",            "st_gw.rel",         "gw"),
            new Stage(0x19, "Castle Siege",           "st_emblem.rel",     "emblem"),
            new Stage(0x1C, "WarioWare Inc.",         "st_madein.rel",     "madein"),
            new Stage(0x1D, "Distant Planet",         "st_earth.rel",      "earth"),
            new Stage(0x1E, "Skyworld",               "st_palutena.rel",   "palutena"),
            new Stage(0x1F, "Mario Bros.",            "st_famicom.rel",    "famicom"),
            new Stage(0x20, "New Pork City",          "st_newpork.rel",    "newpork"),
            new Stage(0x21, "Smashville",             "st_village.rel",    "village"),
            new Stage(0x22, "Shadow Moses Island",    "st_metalgear.rel",  "metalgear"),
            new Stage(0x23, "Green Hill Zone",        "st_greenhill.rel",  "greenhill"),
            new Stage(0x24, "PictoChat",              "st_pictchat.rel",   "pictchat"),
            new Stage(0x25, "Hanenbow",               "st_plankton.rel",   "plankton"),
            new Stage(0x26, "ConfigTest",             "st_config.rel",     "configtest"),
            new Stage(0x29, "Temple",                 "st_dxshrine.rel",   "dxshrine"),
            new Stage(0x2A, "Yoshi's Island (Melee)", "st_dxyorster.rel",  "dxyorster"),
            new Stage(0x2B, "Jungle Japes",           "st_dxgarden.rel",   "dxgarden"),
            new Stage(0x2C, "Onett",                  "st_dxonett.rel",    "dxonett"),
            new Stage(0x2D, "Green Greens",           "st_dxgreens.rel",   "dxgreens"),
            new Stage(0x2E, "Pokemon Stadium",        "st_dxpstadium.rel", "dxpstadium"),
            new Stage(0x2F, "Rainbow Cruise",         "st_dxrcruise.rel",  "dxrcruise"),
            new Stage(0x30, "Corneria",               "st_dxcorneria.rel", "dxcorneria"),
            new Stage(0x31, "Big Blue",               "st_dxbigblue.rel",  "dxbigblue"),
            new Stage(0x32, "Brinstar",               "st_dxzebes.rel",    "dxzebes"),
            new Stage(0x33, "Bridge of Eldin",        "st_oldin.rel",      "oldin"),
            new Stage(0x34, "Homerun",                "st_homerun.rel",    "homerun"),
            new Stage(0x35, "Edit",                   "st_stageedit.rel",  "edit"),
            new Stage(0x36, "Heal",                   "st_heal.rel",       "heal"),
            new Stage(0x37, "Online Training",        "st_otrain.rel",     "onlinetraining"),
            new Stage(0x38, "TargetBreak",            "st_tbreak.rel",     "targetlv"),
        };

        #region Designer

        private Label lblOffsetValue;
        private Label lblOffsetDesc;
        private ComboBox stageSelection;
        private Label lblCurrentStageDesc;
        private GroupBox groupBox1;
        private Label lblCurrentStage;
        private Label lblNewStageDesc;
        private Button btnOkay;
        private Button btnCancel;
        private TextBox txtPath;
        private Panel pnlInfo;
        private Panel pnlEdit;
        private GroupBox groupBox2;
        private Panel panel3;
        private Panel panel4;
        private OpenFileDialog dlgOpen;
        private Timer tmrUpdate;
        private IContainer components;
        private Label lblSizeValue;
        private Label lblSizeDesc;
        private Label lblNameValue;
        private Button btnBrowse;
        private Label lblIDValue;
        private Label lblIDDesc;
        private Label label3;
        private ComboBox itemSelection;
        private Label lblItemDesc;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnOkay = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblOffsetValue = new System.Windows.Forms.Label();
            this.lblOffsetDesc = new System.Windows.Forms.Label();
            this.lblNameValue = new System.Windows.Forms.Label();
            this.lblSizeValue = new System.Windows.Forms.Label();
            this.lblSizeDesc = new System.Windows.Forms.Label();
            this.pnlInfo = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblIDValue = new System.Windows.Forms.Label();
            this.lblIDDesc = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.pnlEdit = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.itemSelection = new System.Windows.Forms.ComboBox();
            this.lblItemDesc = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblCurrentStage = new System.Windows.Forms.Label();
            this.lblNewStageDesc = new System.Windows.Forms.Label();
            this.stageSelection = new System.Windows.Forms.ComboBox();
            this.lblCurrentStageDesc = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.pnlInfo.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.pnlEdit.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOkay
            // 
            this.btnOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOkay.Enabled = false;
            this.btnOkay.Location = new System.Drawing.Point(3, 3);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(75, 23);
            this.btnOkay.TabIndex = 0;
            this.btnOkay.Text = "Okay";
            this.btnOkay.UseVisualStyleBackColor = true;
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(80, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.Location = new System.Drawing.Point(0, 0);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(222, 20);
            this.txtPath.TabIndex = 2;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(227, 0);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(25, 20);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblOffsetValue
            // 
            this.lblOffsetValue.Location = new System.Drawing.Point(56, 56);
            this.lblOffsetValue.Name = "lblOffsetValue";
            this.lblOffsetValue.Size = new System.Drawing.Size(96, 20);
            this.lblOffsetValue.TabIndex = 5;
            this.lblOffsetValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblOffsetDesc
            // 
            this.lblOffsetDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOffsetDesc.Location = new System.Drawing.Point(6, 56);
            this.lblOffsetDesc.Name = "lblOffsetDesc";
            this.lblOffsetDesc.Size = new System.Drawing.Size(48, 20);
            this.lblOffsetDesc.TabIndex = 4;
            this.lblOffsetDesc.Text = "Offset:";
            this.lblOffsetDesc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblNameValue
            // 
            this.lblNameValue.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNameValue.Location = new System.Drawing.Point(7, 16);
            this.lblNameValue.Name = "lblNameValue";
            this.lblNameValue.Size = new System.Drawing.Size(145, 20);
            this.lblNameValue.TabIndex = 3;
            this.lblNameValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSizeValue
            // 
            this.lblSizeValue.Location = new System.Drawing.Point(56, 36);
            this.lblSizeValue.Name = "lblSizeValue";
            this.lblSizeValue.Size = new System.Drawing.Size(96, 20);
            this.lblSizeValue.TabIndex = 1;
            this.lblSizeValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSizeDesc
            // 
            this.lblSizeDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSizeDesc.Location = new System.Drawing.Point(6, 36);
            this.lblSizeDesc.Name = "lblSizeDesc";
            this.lblSizeDesc.Size = new System.Drawing.Size(48, 20);
            this.lblSizeDesc.TabIndex = 0;
            this.lblSizeDesc.Text = "Size:";
            this.lblSizeDesc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlInfo
            // 
            this.pnlInfo.Controls.Add(this.groupBox1);
            this.pnlInfo.Controls.Add(this.panel4);
            this.pnlInfo.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlInfo.Location = new System.Drawing.Point(256, 0);
            this.pnlInfo.Name = "pnlInfo";
            this.pnlInfo.Size = new System.Drawing.Size(158, 132);
            this.pnlInfo.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblIDValue);
            this.groupBox1.Controls.Add(this.lblIDDesc);
            this.groupBox1.Controls.Add(this.lblOffsetValue);
            this.groupBox1.Controls.Add(this.lblOffsetDesc);
            this.groupBox1.Controls.Add(this.lblNameValue);
            this.groupBox1.Controls.Add(this.lblSizeValue);
            this.groupBox1.Controls.Add(this.lblSizeDesc);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(158, 103);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Info";
            // 
            // lblIDValue
            // 
            this.lblIDValue.Location = new System.Drawing.Point(56, 76);
            this.lblIDValue.Name = "lblIDValue";
            this.lblIDValue.Size = new System.Drawing.Size(96, 20);
            this.lblIDValue.TabIndex = 7;
            this.lblIDValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblIDDesc
            // 
            this.lblIDDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIDDesc.Location = new System.Drawing.Point(6, 76);
            this.lblIDDesc.Name = "lblIDDesc";
            this.lblIDDesc.Size = new System.Drawing.Size(48, 20);
            this.lblIDDesc.TabIndex = 6;
            this.lblIDDesc.Text = "ID:";
            this.lblIDDesc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.btnOkay);
            this.panel4.Controls.Add(this.btnCancel);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 103);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(158, 29);
            this.panel4.TabIndex = 6;
            // 
            // pnlEdit
            // 
            this.pnlEdit.Controls.Add(this.groupBox2);
            this.pnlEdit.Controls.Add(this.panel3);
            this.pnlEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEdit.Location = new System.Drawing.Point(0, 0);
            this.pnlEdit.Name = "pnlEdit";
            this.pnlEdit.Size = new System.Drawing.Size(256, 132);
            this.pnlEdit.TabIndex = 10;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.itemSelection);
            this.groupBox2.Controls.Add(this.lblItemDesc);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.lblCurrentStage);
            this.groupBox2.Controls.Add(this.lblNewStageDesc);
            this.groupBox2.Controls.Add(this.stageSelection);
            this.groupBox2.Controls.Add(this.lblCurrentStageDesc);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 20);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(256, 112);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // itemSelection
            // 
            this.itemSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.itemSelection.Enabled = false;
            this.itemSelection.FormattingEnabled = true;
            this.itemSelection.Items.AddRange(new object[] {
            "Assist Trophy",
            "Franklin Badge",
            "Banana Peel",
            "Barrel",
            "Beam Sword",
            "Bill (coin mode)",
            "Bob-Omb",
            "Crate",
            "Bumper",
            "Capsule",
            "Rolling Crate",
            "CD",
            "Gooey Bomb",
            "Cracker Launcher",
            "Cracker Launcher Shot",
            "Coin",
            "Superspicy Curry",
            "Superspice Curry Shot",
            "Deku Nut",
            "Mr. Saturn",
            "Dragoon Part",
            "Dragoon Set",
            "Dragoon Sight",
            "Trophy",
            "Fire Flower",
            "Fire Flower Shot",
            "Freezie",
            "Golden Hammer",
            "Green Shell",
            "Hammer",
            "Hammer Head",
            "Fan",
            "Heart Container",
            "Homerun Bat",
            "Party Ball",
            "Manaphy Heart",
            "Maxim Tomato",
            "Poison Mushroom",
            "Super Mushroom",
            "Metal Box",
            "Hothead",
            "Pitfall",
            "Pokéball",
            "Blast Box",
            "Ray Gun",
            "Ray Gun Shot",
            "Lipstick",
            "Lipstick Flower",
            "Lipstick Shot",
            "Sandbag",
            "Screw Attack",
            "Sticker",
            "Motion-Sensor Bomb",
            "Timer",
            "Smart Bomb",
            "Smash Ball",
            "Smoke Screen",
            "Spring",
            "Star Rod",
            "Star Rod Shot",
            "Soccer Ball",
            "Super Scope",
            "Super Scope shot",
            "Star",
            "Food",
            "Team Healer",
            "Lightning",
            "Unira",
            "Bunny Hood",
            "Warpstar"});
            this.itemSelection.Location = new System.Drawing.Point(84, 62);
            this.itemSelection.Name = "itemSelection";
            this.itemSelection.Size = new System.Drawing.Size(166, 21);
            this.itemSelection.TabIndex = 6;
            // 
            // lblItemDesc
            // 
            this.lblItemDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemDesc.Location = new System.Drawing.Point(6, 62);
            this.lblItemDesc.Name = "lblItemDesc";
            this.lblItemDesc.Size = new System.Drawing.Size(72, 21);
            this.lblItemDesc.TabIndex = 5;
            this.lblItemDesc.Text = "Item:";
            this.lblItemDesc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label3.Location = new System.Drawing.Point(4, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(209, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Mouse over the labels for more information.";
            // 
            // lblCurrentStage
            // 
            this.lblCurrentStage.Location = new System.Drawing.Point(84, 14);
            this.lblCurrentStage.Name = "lblCurrentStage";
            this.lblCurrentStage.Size = new System.Drawing.Size(166, 21);
            this.lblCurrentStage.TabIndex = 3;
            this.lblCurrentStage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNewStageDesc
            // 
            this.lblNewStageDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewStageDesc.Location = new System.Drawing.Point(6, 38);
            this.lblNewStageDesc.Name = "lblNewStageDesc";
            this.lblNewStageDesc.Size = new System.Drawing.Size(72, 21);
            this.lblNewStageDesc.TabIndex = 2;
            this.lblNewStageDesc.Text = "New stage:";
            this.lblNewStageDesc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // stageSelection
            // 
            this.stageSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stageSelection.Enabled = false;
            this.stageSelection.FormattingEnabled = true;
            this.stageSelection.Location = new System.Drawing.Point(84, 38);
            this.stageSelection.Name = "stageSelection";
            this.stageSelection.Size = new System.Drawing.Size(166, 21);
            this.stageSelection.TabIndex = 1;
            // 
            // lblCurrentStageDesc
            // 
            this.lblCurrentStageDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentStageDesc.Location = new System.Drawing.Point(6, 14);
            this.lblCurrentStageDesc.Name = "lblCurrentStageDesc";
            this.lblCurrentStageDesc.Size = new System.Drawing.Size(72, 21);
            this.lblCurrentStageDesc.TabIndex = 0;
            this.lblCurrentStageDesc.Text = "Current:";
            this.lblCurrentStageDesc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.txtPath);
            this.panel3.Controls.Add(this.btnBrowse);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(256, 20);
            this.panel3.TabIndex = 14;
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Interval = 10;
            // 
            // StageRelSwitcherDialog
            // 
            this.ClientSize = new System.Drawing.Size(414, 132);
            this.Controls.Add(this.pnlEdit);
            this.Controls.Add(this.pnlInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "StageRelSwitcherDialog";
            this.Text = "Stage REL Switcher";
            this.pnlInfo.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.pnlEdit.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private string _path;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Path { get { return _path; } set { _path = value; } }

        private byte[] _data;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public byte[] Data { get { return _data; } }

        public unsafe FileMap ToFileMap()
        {
            FileMap map = FileMap.FromTempFile(_data.Length);
            byte* ptr = (byte*)map.Address;
            for (int i = 0; i < _data.Length; i++)
                ptr[i] = _data[i];
            return map;
        }

        private string _outputName;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string OutputName { get { return _outputName; } }

        private int _stageIDOffset = -1;

        public StageModuleConverter()
        {
            InitializeComponent();

            stageSelection.Items.AddRange(Stages);
            stageSelection.SelectedIndex = 0;

            #region Tooltips
            ToolTip tooltip = new ToolTip(new Container());
            tooltip.SetToolTip(lblSizeDesc, "The size of the REL file, in bytes.");
            tooltip.SetToolTip(lblSizeValue, "The size of the REL file, in bytes.");
            tooltip.SetToolTip(lblNameValue, "The internal name of the REL file (pointer at 0x74.)");
            tooltip.SetToolTip(lblOffsetDesc, "The offset of the stage ID, in bytes.");
            tooltip.SetToolTip(lblOffsetValue, "The offset of the stage ID, in bytes.");
            tooltip.SetToolTip(lblIDDesc, "The stage ID (automatically detected, with certain hard-coded exceptions for NTSC-U and PAL files.)");
            tooltip.SetToolTip(lblIDValue, "The stage ID (automatically detected, with certain hard-coded exceptions for NTSC-U and PAL files.)");
            tooltip.SetToolTip(lblCurrentStageDesc, "The current target stage of this REL file, as determined by its ID.");
            tooltip.SetToolTip(lblCurrentStage, "The current target stage of this REL file, as determined by its ID.");
            tooltip.SetToolTip(lblNewStageDesc, "The new target stage for this REL file.");
            tooltip.SetToolTip(stageSelection, "The new target stage for this REL file.");
            tooltip.SetToolTip(lblItemDesc, "The item to auto-spawn on the stage. Enabled when you use a StOnlineTrainning base. Replaces four bytes (offsets are hard-coded.)");
            tooltip.SetToolTip(itemSelection, "The item to auto-spawn on the stage. Enabled when you use a StOnlineTrainning base. Replaces four bytes (offsets are hard-coded.)");
            #endregion

            dlgOpen.Filter = FILTER;
        }

        new public DialogResult ShowDialog(IWin32Window owner)
        {
            DialogResult = DialogResult.Cancel;
            try { return base.ShowDialog(owner); }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
                return DialogResult.Cancel;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            if (_path == null)
            {
                // Do not try to load a file if this object was made without defining a path. Just show an empty dialog.
            }
            else if (!LoadFile(_path))
            {
                // If it was made with a path but the file doesn't work, then close.
                // Note that the dialog won't close if you use the browse button to select an invalid file after it has been opened with a valid one.
                Close();
                return;
            }

            base.OnShown(e);
        }

        private bool LoadFile()
        {
            if (dlgOpen.ShowDialog(this) == DialogResult.OK)
                return LoadFile(dlgOpen.FileName);
            return false;
        }
        private bool LoadFile(string path)
        {
            // reset stuff
            _data = null;
            _stageIDOffset = -1;
            stageSelection.Enabled = false;
            itemSelection.Enabled = false;
            lblCurrentStage.Text = lblIDValue.Text = lblOffsetValue.Text = "?";
            btnOkay.Enabled = false;

            _path = path;

            txtPath.Text = _path;
            _data = new byte[new IO.FileInfo(_path).Length];
            FileStream input = new IO.FileStream(_path, FileMode.Open,
                FileAccess.Read, FileShare.Read, 8, FileOptions.SequentialScan);
            input.Read(_data, 0, _data.Length);
            input.Dispose();

            lblSizeValue.Text = _data.Length.ToString();
            lblNameValue.Text = getModuleName();
            _stageIDOffset = findStageIDOffset();
            if (_stageIDOffset < 0)
            {
                MessageBox.Show("Could not find the stage ID offset.");
                return false;
            }
            lblOffsetValue.Text = "0x" + Convert.ToString(_stageIDOffset, 16);
            byte currentID = findCurrentID();
            lblIDValue.Text = "0x" + Convert.ToString(currentID, 16);

            foreach (Stage s in Stages) {
                if (s.ID == currentID) {
                    lblCurrentStage.Text = s.Name;
                    stageSelection.SelectedItem = s;
                    break;
                }
            }

            if (lblNameValue.Text.StartsWith("stOnline"))
            {
                itemSelection.Enabled = true;
                itemSelection.SelectedIndex = _data[1223];
                lblCurrentStage.Text += " / " + itemSelection.Text;
            }
            else
            {
                itemSelection.Enabled = false;
            }

            stageSelection.Enabled = true;
            btnOkay.Enabled = true;
            return true;
        }

        private byte findCurrentID()
        {
            return _data[_stageIDOffset];
        }

        private string getModuleName()
        {
            int offset = _data[116];
            for (int i = 117; i < 120; i++)
            {
                offset *= 256;
                offset += _data[i];
            }

            StringBuilder sb = new StringBuilder();
            while (_data[offset] == 0)
            {
                offset++;
            }
            while (_data[offset] != 0)
            {
                sb.Append((char)_data[offset]);
                offset++;
            }
            return sb.ToString();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            Stage s = (Stage)stageSelection.SelectedItem;
            _data[_stageIDOffset] = s.ID;
            if (itemSelection.Enabled)
            {
                byte b = (byte)itemSelection.SelectedIndex;
                int[] offsets = { 1223, 1371, 1347, 1627 };
                for (int i = 0; i < offsets.Length; i++)
                {
                    _data[offsets[i]] = b;
                }
            }
            _outputName = s.RelName;

            DialogResult = DialogResult.OK;
            Close();
        }

        private int findStageIDOffset()
        {
            // search through pointer
            int length = _data.Length;
            byte?[] searchFor = { 0x38, null, 0x00, null,
                                  0x38, 0xA5, 0x00, 0x00,
                                  0x38, 0x80, 0x00 };

            int indexToCheck = 0;
            for (int i = 0; i < length; i++) {
                byte? b = searchFor[indexToCheck];
                if ((b ?? _data[i]) == _data[i]) {
                    indexToCheck++;
                    if (indexToCheck == searchFor.Length) return i + 1;
                } else {
                    indexToCheck = 0;
                }
            }
            return -1;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            LoadFile();
        }
    }
}
