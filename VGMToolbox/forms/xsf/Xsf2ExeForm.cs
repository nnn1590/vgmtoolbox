﻿using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms.xsf
{
    public partial class Xsf2ExeForm : AVgmtForm
    {        
        public Xsf2ExeForm(TreeNode pTreeNode) : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = 
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_Title"];
            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();            
            
            InitializeComponent();

            // messages
            this.BackgroundWorker = new XsfCompressedProgramExtractorWorker();
            this.BeginMessage = ConfigurationSettings.AppSettings["Form_Xsf2Exe_MessageBegin"];
            this.CompleteMessage = ConfigurationSettings.AppSettings["Form_Xsf2Exe_MessageComplete"];
            this.CancelMessage = ConfigurationSettings.AppSettings["Form_Xsf2Exe_MessageCancel"];

            this.grpXsfPsf2Exe_Source.AllowDrop = true;

            this.grpXsfPsf2Exe_Source.Text =
                ConfigurationSettings.AppSettings["Form_Global_DropSourceFiles"];
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_GroupOptions"];
            this.cbExtractReservedSection.Text =
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_CheckBoxExtractReservedSection"];
            this.cbXsfPsf2Exe_IncludeOrigExt.Text =
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_CheckBoxIncludeOriginalExtension"];
            this.cbXsfPsf2Exe_StripGsfHeader.Text =
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_CheckBoxStripGsfHeader"];
        }

        private void tbXsfPsf2Exe_Source_DragDrop(object sender, DragEventArgs e)
        {            
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            XsfCompressedProgramExtractorWorker.XsfCompressedProgramExtractorStruct xcpeStruct =
                new XsfCompressedProgramExtractorWorker.XsfCompressedProgramExtractorStruct();
            xcpeStruct.SourcePaths = s;
            xcpeStruct.includeExtension = cbXsfPsf2Exe_IncludeOrigExt.Checked;
            xcpeStruct.stripGsfHeader = cbXsfPsf2Exe_StripGsfHeader.Checked;
            xcpeStruct.extractReservedSection = cbExtractReservedSection.Checked;

            base.backgroundWorker_Execute(xcpeStruct);
        }
        
        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
    }
}