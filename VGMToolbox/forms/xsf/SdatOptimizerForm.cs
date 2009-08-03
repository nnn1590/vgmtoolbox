﻿using System;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms.xsf
{
    public partial class SdatOptimizerForm : AVgmtForm
    {
        public SdatOptimizerForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_SdatOptimizer_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_SdatOptimizer_IntroText"] + Environment.NewLine;

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            InitializeComponent();

            // messages
            this.BackgroundWorker = new SdatOptimizerWorker();
            this.BeginMessage = ConfigurationSettings.AppSettings["Form_SdatOptimizer_MessageBegin"];
            this.CompleteMessage = ConfigurationSettings.AppSettings["Form_SdatOptimizer_MessageComplete"];
            this.CancelMessage = ConfigurationSettings.AppSettings["Form_SdatOptimizer_MessageCancel"];

            this.grpSource.Text =
                ConfigurationSettings.AppSettings["Form_SdatOptimizer_GroupSource"];
            this.lblDragNDrop.Text =
                ConfigurationSettings.AppSettings["Form_SdatOptimizer_LblDragNDrop"];
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_SdatOptimizer_GroupOptions"];
            this.lblStartingSequence.Text =
                ConfigurationSettings.AppSettings["Form_SdatOptimizer_LblStartingSequence"];
            this.lblEndingSequence.Text =
                ConfigurationSettings.AppSettings["Form_SdatOptimizer_LblEndingSequence"];
            this.cbIncludeAllSseq.Text =
                ConfigurationSettings.AppSettings["Form_SdatOptimizer_CheckBoxIncludeAllSequences"];
        }

        private void tbSourceSdat_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            SdatOptimizerWorker.SdatOptimizerStruct soptStruct = new SdatOptimizerWorker.SdatOptimizerStruct();
            soptStruct.SourcePaths = s;

            if (!cbIncludeAllSseq.Checked)
            {
                soptStruct.startSequence = tbStartSequence.Text;
                soptStruct.endSequence = tbEndSequence.Text;
            }

            base.backgroundWorker_Execute(soptStruct);
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
        private void cbIncludeAllSseq_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIncludeAllSseq.Checked)
            {
                tbStartSequence.ReadOnly = true;
                tbEndSequence.ReadOnly = true;
            }
            else
            {
                tbStartSequence.ReadOnly = false;
                tbEndSequence.ReadOnly = false;            
            }
        }
    }
}