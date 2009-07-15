﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.tools.vgm;

namespace VGMToolbox.forms
{    
    public partial class Vgm_VgmTagEditorForm : VgmtForm
    {
        IGd3TagFormat vgmData;
        bool isBatchMode;
        
        public Vgm_VgmTagEditorForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            this.isBatchMode = false;
            
            InitializeComponent();

            this.btnDoTask.Text = "Update Tags";
        }

        private void btnBrowseDirectory_Click(object sender, EventArgs e)
        {
            this.tbSourceDirectory.Text = base.browseForFolder(sender, e);
        }
        private void tbSourceDirectory_TextChanged(object sender, EventArgs e)
        {
            this.lbFiles.Items.Clear();
            Type formatType;

            if (Directory.Exists(this.tbSourceDirectory.Text))
            {
                foreach (string f in Directory.GetFiles(this.tbSourceDirectory.Text))
                {
                    formatType = null;

                    using (FileStream fs = File.OpenRead(f))
                    {
                        formatType = FormatUtil.getObjectType(fs);
                        if ((formatType != null) &&
                            typeof(IGd3TagFormat).IsAssignableFrom(formatType))
                        {
                            this.lbFiles.Items.Add(Path.GetFileName(f));
                        }
                    }
                }
            }
            else
            {
                this.lbFiles.Items.Clear();
            }
        }

        private void lbFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbFiles.SelectedIndices.Count > 1)
            {
                this.isBatchMode = true;
                this.disableTrackItems();
            }
            else
            {
                this.isBatchMode = false;
                this.enableTrackItems();
                this.loadSelectedTrack();
            }
        }
        private void disableTrackItems()
        {
            this.lblTrackTitleEn.Enabled = false;
            this.tbTitleEn.Enabled = false;
            this.tbTitleEn.ReadOnly = true;

            this.lblTrackTitleJp.Enabled = false;
            this.tbTitleJp.Enabled = false;
            this.tbTitleJp.ReadOnly = true;
        }
        private void enableTrackItems()
        {
            this.lblTrackTitleEn.Enabled = true;
            this.tbTitleEn.Enabled = true;
            this.tbTitleEn.ReadOnly = false;

            this.lblTrackTitleJp.Enabled = true;
            this.tbTitleJp.Enabled = true;
            this.tbTitleJp.ReadOnly = false;
        }
        private void loadSelectedTrack()
        {
            string selectedFilePath;

            if (lbFiles.SelectedIndices.Count == 1)
            {
                selectedFilePath = Path.Combine(this.tbSourceDirectory.Text, this.lbFiles.Items[this.lbFiles.SelectedIndex].ToString());

                using (FileStream fs =
                    File.Open(selectedFilePath, FileMode.Open, FileAccess.Read))
                {
                    Type formatType = FormatUtil.getObjectType(fs);
                    if (formatType != null)
                    {
                        this.vgmData = (IGd3TagFormat)Activator.CreateInstance(formatType);
                        this.vgmData.Initialize(fs, selectedFilePath);

                        this.tbGameEn.Text = this.vgmData.GetGameTagEn();
                        this.tbGameJp.Text = this.vgmData.GetGameTagJp();
                        this.tbSystemEn.Text = this.vgmData.GetSystemTagEn();
                        this.tbSystemJp.Text = this.vgmData.GetSystemTagJp();
                        this.tbArtistEn.Text = this.vgmData.GetArtistTagEn();
                        this.tbArtistJp.Text = this.vgmData.GetArtistTagJp();
                        this.tbGameDate.Text = this.vgmData.GetDateTag();
                        this.tbRipper.Text = this.vgmData.GetRipperTag();
                        this.tbTitleEn.Text = this.vgmData.GetTitleTagEn();
                        this.tbTitleJp.Text = this.vgmData.GetTitleTagJp();
                        this.tbComments.Text = this.vgmData.GetCommentTag();
                    }
                }
            }
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new VgmTagUpdaterWorker();
        }
        protected override string getCancelMessage()
        {
            return "VGM Tagging...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "VGM Tagging...Complete";
        }
        protected override string getBeginMessage()
        {
            return "VGM Tagging...Begin";
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            int j = 0;
            VgmTagUpdaterWorker.VgmTagUpdaterStruct vtUpdateStruct = new VgmTagUpdaterWorker.VgmTagUpdaterStruct();

            vtUpdateStruct.SourcePaths = new string[this.lbFiles.SelectedIndices.Count];
            foreach (int i in this.lbFiles.SelectedIndices)
            {
                vtUpdateStruct.SourcePaths[j++] = Path.Combine(this.tbSourceDirectory.Text, this.lbFiles.Items[i].ToString());
            }

            vtUpdateStruct.IsBatchMode = this.isBatchMode;

            vtUpdateStruct.TitleTagEn = this.tbTitleEn.Text;
            vtUpdateStruct.TitleTagJp = this.tbTitleJp.Text;
            vtUpdateStruct.GameTagEn = this.tbGameEn.Text;
            vtUpdateStruct.GameTagJp = this.tbGameJp.Text;
            vtUpdateStruct.SystemTagEn = this.tbSystemEn.Text;
            vtUpdateStruct.SystemTagJp = this.tbSystemJp.Text;
            vtUpdateStruct.ArtistTagEn = this.tbArtistEn.Text;
            vtUpdateStruct.ArtistTagJp = this.tbArtistJp.Text;
            vtUpdateStruct.DateTag = this.tbGameDate.Text;
            vtUpdateStruct.RipperTag = this.tbRipper.Text;
            vtUpdateStruct.CommentTag = this.tbComments.Text;

            base.backgroundWorker_Execute(vtUpdateStruct);
        }
    }
}