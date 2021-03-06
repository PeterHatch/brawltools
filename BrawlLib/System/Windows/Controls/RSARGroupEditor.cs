﻿using BrawlLib.SSBB.ResourceNodes;
using System.Diagnostics;
using System.IO;

namespace System.Windows.Forms
{
    public partial class RSARGroupEditor : UserControl
    {
        public RSARGroupEditor()
        {
            InitializeComponent();
        }

        RSARGroupNode _targetGroup;
        public void LoadGroup(RSARGroupNode group)
        {
            if ((_targetGroup = group) != null)
            {
                lstFiles.DataSource = group._files;
                cboAllFiles.DataSource = group.RSARNode.Files;
            }
            else
            {
                lstFiles.DataSource = null;
                cboAllFiles.DataSource = null;
            }
        }

        private void lstFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            int sIndex = lstFiles.SelectedIndex;
            int count = lstFiles.Items.Count;
            btnMoveDown.Enabled = sIndex < count - 1 && sIndex >= 0;
            btnMoveUp.Enabled = sIndex > 0 && sIndex < count;
            btnRemove.Enabled = btnEdit.Enabled = sIndex >= 0 && sIndex < count;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cboAllFiles.SelectedIndex != -1)
            {
                RSARFileNode file = cboAllFiles.Items[cboAllFiles.SelectedIndex] as RSARFileNode;
                file._groupRefs.Add(_targetGroup);
                _targetGroup._files.Add(file);
                file.GetName();
                lstFiles.SelectedIndex = lstFiles.Items.Count - 1;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstFiles.SelectedIndex != -1)
            {
                RSARFileNode file = cboAllFiles.Items[cboAllFiles.SelectedIndex] as RSARFileNode;
                file._groupRefs.RemoveAt(lstFiles.SelectedIndex);
                _targetGroup._files.RemoveAt(lstFiles.SelectedIndex);
                file.GetName();
                lstFiles_SelectedIndexChanged(null, null);
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            int sIndex = lstFiles.SelectedIndex;
            RSARFileNode file = _targetGroup._files[sIndex];
            _targetGroup._files.Insert(sIndex - 1, file);
            _targetGroup._files.RemoveAt(sIndex + 1);
            lstFiles.SelectedIndex = sIndex - 1;
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            int sIndex = lstFiles.SelectedIndex;
            RSARFileNode file = _targetGroup._files[sIndex];
            _targetGroup._files.RemoveAt(sIndex);
            _targetGroup._files.Insert(sIndex + 1, file);
            lstFiles.SelectedIndex = sIndex + 1;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            RSARFileNode file = lstFiles.SelectedItem as RSARFileNode;
            if (file is RSARExtFileNode)
            {
                if (File.Exists(file.FullExtPath))
                    Process.Start(file.FullExtPath);
                else
                    using (SoundPathChanger dlg = new SoundPathChanger())
                    {
                        RSARNode rsar = file.RSARNode;
                        dlg.FilePath = file.FullExtPath;
                        dlg.dlg.InitialDirectory = rsar._origPath.Substring(0, rsar._origPath.LastIndexOf('\\'));
                        if (dlg.ShowDialog() == DialogResult.OK)
                            file.FullExtPath = dlg.FilePath;
                    }
            }
            else
                new EditRSARFileDialog().ShowDialog(this, file);
        }
    }
}
