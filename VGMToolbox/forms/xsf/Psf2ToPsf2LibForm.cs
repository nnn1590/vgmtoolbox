﻿using System;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms.xsf
{
    public partial class Psf2ToPsf2LibForm : AVgmtForm
    {
        public Psf2ToPsf2LibForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_Title"];
            this.btnDoTask.Text = ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_DoTaskButton"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_IntroText1"] + Environment.NewLine;
            this.tbOutput.Text += ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_IntroText2"] + Environment.NewLine;
            this.tbOutput.Text += ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_IntroText3"] + Environment.NewLine;

            InitializeComponent();

            // messages
            this.BackgroundWorker = new Psf2toPsf2LibWorker();
            this.BeginMessage = ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_MessageBegin"];
            this.CompleteMessage = ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_MessageComplete"];
            this.CancelMessage = ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_MessageCancel"];

            this.grpSource.Text =
                ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_GroupSource"];
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_GroupOptions"];
            this.lblOutputFilePrefix.Text =
                ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_LblOutputFilePrefix"];

        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            Psf2toPsf2LibWorker.Psf2ToPsf2LibStruct psf2Struct = new Psf2toPsf2LibWorker.Psf2ToPsf2LibStruct();
            psf2Struct.sourcePath = tbSourceDirectory.Text;
            psf2Struct.libraryName = tbFilePrefix.Text;

            base.backgroundWorker_Execute(psf2Struct);
        }
        private void btnSourceDirBrowse_Click(object sender, EventArgs e)
        {
            tbSourceDirectory.Text = base.browseForFolder(sender, e);
        }
    }
}