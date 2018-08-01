using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace AES
{
    partial class MainForm
    {


        #region 테스트 버튼
        private void button1_Click(object sender, EventArgs e)
        {
            if (xmlHandler.ItemList == null) return;
            this.dataGridView.CellValueChanged -= new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            RemoveRow();
            ClearRow();
            Stat test = new Stat();
            test.클래스 = Information.InfoClassDefaultList["살성"];
            test.오른손 = xmlHandler.ItemList["dagger_n_e1_pc_65a"];
            test.오른손.SetLevelOrCount(10);
            test.왼손 = xmlHandler.ItemList["sword_n_e1_pc_65a"];
            test.왼손.SetLevelOrCount(10);
            test.상의 = xmlHandler.ItemList["lt_torso_n_e1_pc_65a"];
            test.상의.SetLevelOrCount(1);
            test.어깨 = xmlHandler.ItemList["lt_shoulder_n_e1_pc_65a"];
            test.어깨.SetLevelOrCount(2);
            test.장갑 = xmlHandler.ItemList["lt_glove_n_e1_pc_65a"];
            test.장갑.SetLevelOrCount(1);
            test.하의 = xmlHandler.ItemList["lt_pants_n_e1_pc_65a"];
            test.하의.SetLevelOrCount(1);
            test.신발 = xmlHandler.ItemList["lt_shoes_n_e1_pc_65a"];
            test.신발.SetLevelOrCount(2);

            test.머리 = xmlHandler.ItemList["lt_head_n_e1_pc_65a_new"];
            test.목걸이 = xmlHandler.ItemList["necklac_n_e1_pc_65a_new"];
            test.귀고리1 = xmlHandler.ItemList["earring_n_e1_pc_65a_new"];
            test.귀고리2 = xmlHandler.ItemList["earring_n_e1_pc_65a_new"];
            test.반지1 = xmlHandler.ItemList["ring_n_e1_pc_65a_new"];
            test.반지2 = xmlHandler.ItemList["ring_n_e1_pc_65a_new"];
            test.허리띠 = xmlHandler.ItemList["belt_n_e1_pc_65a_new"];
            test.날개 = xmlHandler.ItemList["wing_n_e1_nq_60a"];
            test.이디안 = xmlHandler.ItemList["cash_phyatt_e_polish_enchant_01a"];
            test.이디안.RandomOptionGroupID = 7;
            test.타이틀 = xmlHandler.TitleList["all_title_266"];

            EntityItem matter = xmlHandler.ItemList["matter_option_r_physicalattack_50"];
            matter.SetLevelOrCount(28);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["special_matter_option_pattack_70c"];
            matter.SetLevelOrCount(7);
            test.마석목록.Add(matter);



            LoadStat(test);
            this.dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (xmlHandler.ItemList == null) return;
            this.dataGridView.CellValueChanged -= new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            RemoveRow();
            ClearRow();
            Stat test = new Stat();
            test.클래스 = Information.InfoClassDefaultList["살성"];
            test.오른손 = xmlHandler.ItemList["orb_n_m_idruneweapon_wi_a"];
            test.오른손.SetLevelOrCount(10);
            test.상의 = xmlHandler.ItemList["rb_torso_n_m_idruneweapon_wi_a"];
            test.상의.SetLevelOrCount(1);
            test.어깨 = xmlHandler.ItemList["rb_shoulder_n_m_idruneweapon_wi_a"];
            test.어깨.SetLevelOrCount(2);
            test.장갑 = xmlHandler.ItemList["rb_glove_n_m_idruneweapon_wi_a"];
            test.장갑.SetLevelOrCount(1);
            test.하의 = xmlHandler.ItemList["rb_pants_n_m_idruneweapon_wi_a"];
            test.하의.SetLevelOrCount(1);
            test.신발 = xmlHandler.ItemList["rb_shoes_n_m_idruneweapon_wi_a"];
            test.신발.SetLevelOrCount(2);

            test.머리 = xmlHandler.ItemList["lt_head_n_e1_pc_65a_new"];
            test.목걸이 = xmlHandler.ItemList["necklace_n_m_idruneweapon_65a"];
            test.귀고리1 = xmlHandler.ItemList["earring_n_m_idruneweapon_65a"];
            test.귀고리2 = xmlHandler.ItemList["earring_n_m_idruneweapon_65a"];
            test.반지1 = xmlHandler.ItemList["ring_n_m_idruneweapon_65a"];
            test.반지2 = xmlHandler.ItemList["ring_n_m_idruneweapon_65a"];
            test.허리띠 = xmlHandler.ItemList["belt_n_e1_pc_65a_new"];
            test.날개 = xmlHandler.ItemList["wing_n_e1_nq_60a"];
            test.이디안 = xmlHandler.ItemList["cash_phyatt_e_polish_enchant_01a"];
            test.이디안.RandomOptionGroupID = 7;
            test.타이틀 = xmlHandler.TitleList["all_title_266"];
            LoadStat(test);
            this.dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (xmlHandler.ItemList == null) return;
            this.dataGridView.CellValueChanged -= new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            RemoveRow();
            ClearRow();
            Stat test = new Stat();
            test.클래스 = Information.InfoClassDefaultList["검성"];
            test.오른손 = xmlHandler.ItemList["dagger_d_d_e1_p_60a"];
            test.오른손.SetLevelOrCount(15);
            test.왼손 = xmlHandler.ItemList["sword_n_e1_60a"];
            test.왼손.RandomOptionGroupID = 1;
            test.왼손.SetLevelOrCount(15);

            test.상의 = xmlHandler.ItemList["pl_torso_d_a_e2_60c"];
            test.상의.SetLevelOrCount(15);
            test.어깨 = xmlHandler.ItemList["pl_shoulder_d_a_e2_60c"];
            test.어깨.SetLevelOrCount(15);
            test.장갑 = xmlHandler.ItemList["pl_glove_d_a_e2_60c"];
            test.장갑.SetLevelOrCount(15);
            test.하의 = xmlHandler.ItemList["pl_pants_d_a_e2_60c"];
            test.하의.SetLevelOrCount(15);
            test.신발 = xmlHandler.ItemList["pl_shoes_d_a_e2_60c"];
            test.신발.SetLevelOrCount(15);

            test.머리 = xmlHandler.ItemList["pl_head_d_a_e2_60a"];
            test.목걸이 = xmlHandler.ItemList["necklace_d_a_e2_65a"];
            test.귀고리1 = xmlHandler.ItemList["earring_n_e1_pc_65a_new"];
            test.귀고리2 = xmlHandler.ItemList["earring_n_e1_pc_65a_new"];
            test.반지1 = xmlHandler.ItemList["ring_n_e1_pc_65a_new"];
            test.반지2 = xmlHandler.ItemList["ring_n_e1_pc_65a_new"];
            test.허리띠 = xmlHandler.ItemList["belt_n_e1_pc_65a_new"];
            test.날개 = xmlHandler.ItemList["event_wing_n_e2_60a"];
            test.타이틀 = xmlHandler.TitleList["light_title41"];

            EntityItem matter = xmlHandler.ItemList["matter_option_l_pcritical_accuracy_50"];
            matter.SetLevelOrCount(6);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pcritical_accuracy_60"];
            matter.SetLevelOrCount(6);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pattack_accuracy_50"];
            matter.SetLevelOrCount(7);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pattack_accuracy_60"];
            matter.SetLevelOrCount(6);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pattack_pcritical_50"];
            matter.SetLevelOrCount(3);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pcritical_pattack_50"];
            matter.SetLevelOrCount(2);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_r_physicalattack_50"];
            matter.SetLevelOrCount(12);
            test.마석목록.Add(matter);



            LoadStat(test);
            this.dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (xmlHandler.ItemList == null) return;
            this.dataGridView.CellValueChanged -= new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            RemoveRow();
            ClearRow();
            Stat test = new Stat();
            test.클래스 = Information.InfoClassDefaultList["검성"];
            test.오른손 = xmlHandler.ItemList["polearm_n_m_idunderrune_65a"];
            test.오른손.SetLevelOrCount(15);
            test.오른손.RandomOptionGroupID = 7;
            test.왼손 = xmlHandler.ItemList["polearm_n_e1_pc_65a_new"];
            test.이디안 = xmlHandler.ItemList["gacha_phyatt_e_polish_enchant_01a"];
            test.이디안.RandomOptionGroupID = 10;

            test.상의 = xmlHandler.ItemList["pl_torso_d_a_e2_60c"];
            test.상의.SetLevelOrCount(15);
            test.어깨 = xmlHandler.ItemList["pl_shoulder_d_a_e2_60c"];
            test.어깨.SetLevelOrCount(15);
            test.장갑 = xmlHandler.ItemList["pl_glove_d_a_e2_60c"];
            test.장갑.SetLevelOrCount(15);
            test.하의 = xmlHandler.ItemList["pl_pants_d_a_e2_60c"];
            test.하의.SetLevelOrCount(15);
            test.신발 = xmlHandler.ItemList["pl_shoes_d_a_e2_60c"];
            test.신발.SetLevelOrCount(15);

            test.머리 = xmlHandler.ItemList["pl_head_d_a_e2_60a"];
            test.목걸이 = xmlHandler.ItemList["necklace_d_a_e2_65a"];
            test.귀고리1 = xmlHandler.ItemList["earring_n_e1_pc_65a_new"];
            test.귀고리2 = xmlHandler.ItemList["earring_n_e1_pc_65a_new"];
            test.반지1 = xmlHandler.ItemList["ring_n_e1_pc_65a_new"];
            test.반지2 = xmlHandler.ItemList["ring_n_e1_pc_65a_new"];
            test.허리띠 = xmlHandler.ItemList["belt_n_e1_pc_65a_new"];
            test.날개 = xmlHandler.ItemList["event_wing_n_e2_60a"];
            test.타이틀 = xmlHandler.TitleList["light_title41"];

            EntityItem matter = xmlHandler.ItemList["special_matter_option_pcritical_70"];
            matter.SetLevelOrCount(2);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pcritical_accuracy_50"];
            matter.SetLevelOrCount(12);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pcritical_accuracy_60"];
            matter.SetLevelOrCount(8);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pattack_accuracy_50"];
            matter.SetLevelOrCount(7);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pattack_accuracy_60"];
            matter.SetLevelOrCount(6);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pattack_pcritical_50"];
            matter.SetLevelOrCount(3);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pcritical_pattack_50"];
            matter.SetLevelOrCount(2);
            test.마석목록.Add(matter);



            LoadStat(test);
            this.dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (xmlHandler.ItemList == null) return;
            this.dataGridView.CellValueChanged -= new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            RemoveRow();
            ClearRow();
            Stat test = new Stat();
            test.클래스 = Information.InfoClassDefaultList["검성"];
            test.오른손 = xmlHandler.ItemList["bow_n_e1_pc_65a"];
            test.오른손.SetLevelOrCount(10);
            test.왼손 = xmlHandler.ItemList["bow_d_n_e1_pvp_55b"];
            test.이디안 = xmlHandler.ItemList["gacha_phyatt_e_polish_enchant_01a"];
            test.이디안.RandomOptionGroupID = 1;

            test.상의 = xmlHandler.ItemList["pl_torso_d_a_e2_60c"];
            test.상의.SetLevelOrCount(15);
            test.어깨 = xmlHandler.ItemList["pl_shoulder_d_a_e2_60c"];
            test.어깨.SetLevelOrCount(15);
            test.장갑 = xmlHandler.ItemList["pl_glove_d_a_e2_60c"];
            test.장갑.SetLevelOrCount(15);
            test.하의 = xmlHandler.ItemList["pl_pants_d_a_e2_60c"];
            test.하의.SetLevelOrCount(15);
            test.신발 = xmlHandler.ItemList["pl_shoes_d_a_e2_60c"];
            test.신발.SetLevelOrCount(15);

            test.머리 = xmlHandler.ItemList["pl_head_d_a_e2_60a"];
            test.목걸이 = xmlHandler.ItemList["necklace_d_a_e2_65a"];
            test.귀고리1 = xmlHandler.ItemList["earring_n_e1_pc_65a_new"];
            test.귀고리2 = xmlHandler.ItemList["earring_n_e1_pc_65a_new"];
            test.반지1 = xmlHandler.ItemList["ring_n_e1_pc_65a_new"];
            test.반지2 = xmlHandler.ItemList["ring_n_e1_pc_65a_new"];
            test.허리띠 = xmlHandler.ItemList["belt_n_e1_pc_65a_new"];
            test.날개 = xmlHandler.ItemList["event_wing_n_e2_60a"];
            test.타이틀 = xmlHandler.TitleList["light_title41"];

            EntityItem matter = xmlHandler.ItemList["matter_option_l_pcritical_accuracy_50"];
            matter.SetLevelOrCount(6);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pcritical_accuracy_60"];
            matter.SetLevelOrCount(6);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pattack_accuracy_50"];
            matter.SetLevelOrCount(7);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pattack_accuracy_60"];
            matter.SetLevelOrCount(6);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pattack_pcritical_50"];
            matter.SetLevelOrCount(3);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pcritical_pattack_50"];
            matter.SetLevelOrCount(2);
            test.마석목록.Add(matter);

            //matter = xmlHandler.ItemList["matter_option_r_physicalattack_50"];
            //matter.SetLevelOrCount(12);
            //test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["special_matter_option_pcritical_70"];
            matter.SetLevelOrCount(1);
            test.마석목록.Add(matter);


            matter = xmlHandler.ItemList["matter_option_r_physicalcritical_60"];
            matter.SetLevelOrCount(5);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_r_physicalcritical_70"];
            matter.SetLevelOrCount(4);
            test.마석목록.Add(matter);

            LoadStat(test);
            this.dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (xmlHandler.ItemList == null) return;
            this.dataGridView.CellValueChanged -= new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            RemoveRow();
            ClearRow();
            Stat test = new Stat();
            test.클래스 = Information.InfoClassDefaultList["검성"];
            test.오른손 = xmlHandler.ItemList["dagger_d_d_u2_p_55a"];
            test.오른손.SetLevelOrCount(10);
            test.왼손 = xmlHandler.ItemList["shield_d_n_e1_pvp_pr_55b"];
            test.왼손.SetLevelOrCount(11);

            test.상의 = xmlHandler.ItemList["pl_torso_d_a_e2_60c"];
            test.상의.SetLevelOrCount(15);
            test.어깨 = xmlHandler.ItemList["pl_shoulder_d_a_e2_60c"];
            test.어깨.SetLevelOrCount(15);
            test.장갑 = xmlHandler.ItemList["pl_glove_d_a_e2_60c"];
            test.장갑.SetLevelOrCount(15);
            test.하의 = xmlHandler.ItemList["pl_pants_d_a_e2_60c"];
            test.하의.SetLevelOrCount(15);
            test.신발 = xmlHandler.ItemList["pl_shoes_d_a_e2_60c"];
            test.신발.SetLevelOrCount(15);

            test.머리 = xmlHandler.ItemList["pl_head_d_a_e2_60a"];
            test.목걸이 = xmlHandler.ItemList["necklace_d_a_e2_65a"];
            test.귀고리1 = xmlHandler.ItemList["earring_n_e1_pc_65a_new"];
            test.귀고리2 = xmlHandler.ItemList["earring_n_e1_pc_65a_new"];
            test.반지1 = xmlHandler.ItemList["ring_n_e1_pc_65a_new"];
            test.반지2 = xmlHandler.ItemList["ring_n_e1_pc_65a_new"];
            test.허리띠 = xmlHandler.ItemList["belt_n_e1_pc_65a_new"];
            test.날개 = xmlHandler.ItemList["event_wing_n_e2_60a"];
            test.타이틀 = xmlHandler.TitleList["light_title41"];

            EntityItem matter = xmlHandler.ItemList["matter_option_l_pcritical_accuracy_50"];
            matter.SetLevelOrCount(6);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pcritical_accuracy_60"];
            matter.SetLevelOrCount(6);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pattack_accuracy_50"];
            matter.SetLevelOrCount(7);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pattack_accuracy_60"];
            matter.SetLevelOrCount(6);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pattack_pcritical_50"];
            matter.SetLevelOrCount(3);
            test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_l_pcritical_pattack_50"];
            matter.SetLevelOrCount(2);
            test.마석목록.Add(matter);

            //matter = xmlHandler.ItemList["matter_option_r_physicalattack_50"];
            //matter.SetLevelOrCount(12);
            //test.마석목록.Add(matter);

            matter = xmlHandler.ItemList["matter_option_r_block_60"];
            matter.SetLevelOrCount(10);
            test.마석목록.Add(matter);

            LoadStat(test);
            this.dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);

        }
        #endregion


    }
}
