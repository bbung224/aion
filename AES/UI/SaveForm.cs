using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AES
{
    public partial class SaveForm : Form
    {
        private MainForm main;
        public SaveForm(MainForm main)
        {
            this.main = main;
            InitializeComponent();
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            bool ret = this.main.FileHandle.IsExist(this.textBoxFilename.Text + "." + this.main.FileHandle.Extension);
            if (ret)
            {
                DialogResult dr = MessageBox.Show("같은 이름의 파일이 이미 있습니다.\r덮어쓰시겠습니까?", "저장", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                {

                }
                else if (dr == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
            }

            ret = this.main.FileHandle.Save(this.textBoxFilename.Text, this.textBoxDesc.Text);

            if (ret)
            {
                MessageBox.Show("저장하였습니다.", "저장 성공", MessageBoxButtons.OK);
                this.Hide();
                return;
            }
            else
            {
                MessageBox.Show("저장에 실패하였습니다", "저장 실패", MessageBoxButtons.OK);
                this.Hide();
                return;
            }
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveForm_Load(object sender, EventArgs e)
        {
            this.textBoxFilename.Text = "";
            if (this.main.CurrentStat.OnlyFileName != null)
            {
                this.textBoxFilename.AppendText(this.main.CurrentStat.OnlyFileName.Split('.')[0]);
            }
            else
            {
                if (this.main.CurrentStat.클래스 != null)
                {
                    this.textBoxFilename.AppendText(this.main.CurrentStat.클래스.ClassName + " ");
                }
                this.textBoxFilename.AppendText(DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            if (this.main.CurrentStat.Desc != null) this.textBoxDesc.AppendText(this.main.CurrentStat.Desc);

        }
    }
}
