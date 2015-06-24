using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IceBlink2
{
    public partial class IBMessageBox : IBForm
    {
        //private Game game;

        public IBMessageBox()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// setMessage method is used to display a message on
        /// the form. The message is displayed in a Label control.
        /// </summary>
        /// <param name="messageText">Message which needs to be displayed to user.</param>
        private void setMessage(string messageText)
        {
            this.lblMessageText.Text = messageText;
        }

        /// <summary>
        /// setBoxSize method is used to adjust the 
        /// form size based on the message length.
        /// </summary>
        /// <param name="messageText">Message which needs to be displayed to user.</param>
        private void setBoxSize(string messageText)
        {
            //label1.Text = messageText.Length.ToString();
            if (messageText.Length < 60)
            {
                this.Width = 600;
                this.Height = 300;
                this.MaximumSize = new Size(400, 300);
                this.MinimumSize = new Size(400, 300);
            }
            else if (messageText.Length < 240)
            {
                this.Width = 900;
                this.Height = 210 + (messageText.Length / 30) * 40;
                if (this.Height < 300) { this.Height = 300; }
                this.MaximumSize = new Size(this.Width, this.Height);
                this.MinimumSize = new Size(this.Width, this.Height);
            }
            else
            {
                this.Width = 1200;
                this.Height = 210 + (messageText.Length / 40) * 40;
                if (this.Height < 300) { this.Height = 300; }
                this.MaximumSize = new Size(this.Width, this.Height);
                this.MinimumSize = new Size(this.Width, this.Height);
            }
        }

        /// <summary>
        /// This method is used to add button(s) on the message box.
        /// </summary>
        /// <param name="MessageButton">MessageButton is type of enumMessageButton
        /// through which determines the types of button which need to be displayed.</param>
        private void addButton(GameView gv, enumMessageButton MessageButton)
        {
            switch (MessageButton)
            {
                case enumMessageButton.OK:
                    {
                        IceBlinkButtonMedium btnOk = new IceBlinkButtonMedium();
                        btnOk.setupAll(gv);
                        btnOk.Text = "";
                        btnOk.TextIB = "OK";
                        btnOk.Font = gv.drawFontReg;
                        btnOk.DialogResult = DialogResult.OK;
                        //btnOk.FlatStyle = FlatStyle.Popup;
                        //btnOk.FlatAppearance.BorderSize = 0;
                        btnOk.SetBounds((this.ClientSize.Width / 2) - 40, this.ClientSize.Height - 35, 85, 25);
                        this.Controls.Add(btnOk);
                    }
                    break;
                case enumMessageButton.OKCancel:
                    {
                        IceBlinkButtonMedium btnOk = new IceBlinkButtonMedium();
                        btnOk.setupAll(gv);
                        btnOk.Text = "";
                        btnOk.TextIB = "OK";
                        btnOk.Font = gv.drawFontReg;
                        btnOk.DialogResult = DialogResult.OK;
                        //btnOk.FlatStyle = FlatStyle.Popup;
                        //btnOk.FlatAppearance.BorderSize = 0;
                        btnOk.SetBounds((this.ClientSize.Width / 2) - 85, this.ClientSize.Height - 35, 85, 25);
                        this.Controls.Add(btnOk);

                        IceBlinkButtonMedium btnCancel = new IceBlinkButtonMedium();
                        btnCancel.setupAll(gv);
                        btnCancel.Text = "";
                        btnCancel.TextIB = "CANCEL";
                        btnCancel.Font = gv.drawFontReg;
                        btnCancel.DialogResult = DialogResult.Cancel;
                        //btnCancel.FlatStyle = FlatStyle.Popup;
                        //btnCancel.FlatAppearance.BorderSize = 0;
                        btnCancel.SetBounds((this.ClientSize.Width / 2) + 5, this.ClientSize.Height - 35, 85, 25);
                        this.Controls.Add(btnCancel);
                    }
                    break;
                case enumMessageButton.YesNo:
                    {
                        IceBlinkButtonMedium btnNo = new IceBlinkButtonMedium();
                        btnNo.setupAll(gv);
                        btnNo.Text = "";
                        btnNo.TextIB = "NO";
                        btnNo.Font = gv.drawFontReg;
                        btnNo.DialogResult = DialogResult.No;
                        //btnNo.FlatStyle = FlatStyle.Popup;
                        //btnNo.FlatAppearance.BorderSize = 0;
                        btnNo.SetBounds((this.ClientSize.Width / 2) + 5, this.ClientSize.Height - 35, 85, 25);
                        this.Controls.Add(btnNo);

                        IceBlinkButtonMedium btnYes = new IceBlinkButtonMedium();
                        btnYes.setupAll(gv);
                        btnYes.Text = "";
                        btnYes.TextIB = "YES";
                        btnYes.Font = gv.drawFontReg;
                        btnYes.DialogResult = DialogResult.Yes;
                        //btnYes.FlatStyle = FlatStyle.Popup;
                        //btnYes.FlatAppearance.BorderSize = 0;                        
                        btnYes.SetBounds((this.ClientSize.Width / 2) - 85, this.ClientSize.Height - 35, 85, 25);
                        this.Controls.Add(btnYes);
                    }
                    break;
            }
        }

        /// <summary>
        /// Show method (which is overloaded) is used to display the message.
        /// This is a static method so we don't need to create an
        /// object of this class to call this method.
        /// </summary>
        /// <param name="messageText">Message which needs to be displayed to user.</param>
        public static DialogResult Show(GameView gv, string messageText)
        {
            IBMessageBox frmMessage = new IBMessageBox();
            frmMessage.IceBlinkButtonResize.setupAll(gv);
            frmMessage.IceBlinkButtonResize.Enabled = false;
            frmMessage.IceBlinkButtonResize.Visible = false;
            //frmMessage.IceBlinkButtonClose.Enabled = false;
            //frmMessage.IceBlinkButtonClose.Visible = false;
            frmMessage.IceBlinkButtonClose.setupAll(gv);
            frmMessage.setupAll(gv);
            frmMessage.lblMessageText.Font = gv.drawFontReg;
            frmMessage.BackColor = Color.Black;
            frmMessage.ForeColor = Color.White;
            frmMessage.setMessage(messageText);
            frmMessage.setBoxSize(messageText);
            frmMessage.addButton(gv, enumMessageButton.OK);
            frmMessage.StartPosition = FormStartPosition.CenterScreen;
            DialogResult dr = frmMessage.ShowDialog();
            return dr;
        }
        public static DialogResult Show(GameView gv, string messageText, enumMessageButton messageButton)
        {
            IBMessageBox frmMessage = new IBMessageBox();
            frmMessage.IceBlinkButtonResize.setupAll(gv);
            frmMessage.IceBlinkButtonResize.Enabled = false;
            frmMessage.IceBlinkButtonResize.Visible = false;
            //frmMessage.IceBlinkButtonClose.Enabled = false;
            //frmMessage.IceBlinkButtonClose.Visible = false;
            frmMessage.IceBlinkButtonClose.setupAll(gv);
            frmMessage.setupAll(gv);
            frmMessage.lblMessageText.Font = gv.drawFontReg;
            //frmMessage.BackColor = gv.module.ModuleTheme.StandardBackColor;
            frmMessage.BackColor = Color.Black;
            frmMessage.ForeColor = Color.White;
            frmMessage.setMessage(messageText);
            frmMessage.setBoxSize(messageText);
            frmMessage.addButton(gv, messageButton);
            frmMessage.StartPosition = FormStartPosition.CenterScreen;
            DialogResult dr = frmMessage.ShowDialog();
            return dr;
        }

        /*protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }*/
    }

    public enum enumMessageButton
    {
        OK,
        YesNo,
        OKCancel
    }
}
