
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SuchByte.MacroDeck.Properties;

namespace SuchByte.MacroDeck.GUI.CustomControls
{
    partial class ActionConfiguratorPluginItem
    {
        /// <summary>
        /// 必需的窗体设计器变量。
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// 释放正在使用的非托管资源，并可选释放托管资源。
        /// </summary>
        /// <param name="disposing">True 表示同时释放托管资源和非托管资源；False 表示仅释放非托管资源。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法，请勿在代码编辑器中修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            pluginIcon = new PictureBox();
            pluginName = new Label();
            lblCountActions = new Label();
            chevron = new PictureBox();
            ((ISupportInitialize)pluginIcon).BeginInit();
            ((ISupportInitialize)chevron).BeginInit();
            SuspendLayout();
            // 
            // pluginIcon
            // 
            pluginIcon.BackgroundImageLayout = ImageLayout.Stretch;
            pluginIcon.Cursor = Cursors.Hand;
            pluginIcon.Location = new Point(33, 19);
            pluginIcon.Name = "pluginIcon";
            pluginIcon.Size = new Size(30, 30);
            pluginIcon.TabIndex = 0;
            pluginIcon.TabStop = false;
            // 
            // pluginName
            // 
            pluginName.Cursor = Cursors.Hand;
            pluginName.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            pluginName.Location = new Point(69, 8);
            pluginName.Name = "pluginName";
            pluginName.Size = new Size(203, 20);
            pluginName.TabIndex = 1;
            pluginName.Text = "label1";
            pluginName.TextAlign = ContentAlignment.MiddleLeft;
            pluginName.UseMnemonic = false;
            // 
            // lblCountActions
            // 
            lblCountActions.Cursor = Cursors.Hand;
            lblCountActions.Font = new Font("Tahoma", 8.25F);
            lblCountActions.Location = new Point(69, 38);
            lblCountActions.Name = "lblCountActions";
            lblCountActions.Size = new Size(203, 20);
            lblCountActions.TabIndex = 2;
            lblCountActions.Text = "label1";
            lblCountActions.TextAlign = ContentAlignment.MiddleLeft;
            lblCountActions.UseMnemonic = false;
            // 
            // chevron
            // 
            chevron.BackgroundImage = Resources.Chevron_Right;
            chevron.BackgroundImageLayout = ImageLayout.Stretch;
            chevron.Cursor = Cursors.Hand;
            chevron.Location = new Point(8, 23);
            chevron.Name = "chevron";
            chevron.Size = new Size(20, 20);
            chevron.TabIndex = 3;
            chevron.TabStop = false;
            // 
            // ActionConfiguratorPluginItem
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(35, 35, 35);
            Controls.Add(chevron);
            Controls.Add(lblCountActions);
            Controls.Add(pluginName);
            Controls.Add(pluginIcon);
            Cursor = Cursors.Hand;
            Font = new Font("Tahoma", 9F);
            ForeColor = Color.White;
            Margin = new Padding(0, 6, 0, 1);
            Name = "ActionConfiguratorPluginItem";
            Size = new Size(415, 70);
            Load += ActionConfiguratorPluginItem_Load;
            ((ISupportInitialize)pluginIcon).EndInit();
            ((ISupportInitialize)chevron).EndInit();
            ResumeLayout(false);
        }

        #endregion

        /// <summary>
        /// 插件图标控件。
        /// </summary>
        private PictureBox pluginIcon;
        /// <summary>
        /// 插件名称标签。
        /// </summary>
        private Label pluginName;
        /// <summary>
        /// 动作数量标签。
        /// </summary>
        private Label lblCountActions;
        /// <summary>
        /// 展开/折叠箭头控件。
        /// </summary>
        private PictureBox chevron;
    }
}
