using System;
using System.Collections;
using System.Windows.Forms;

namespace AionLogAnalyzer
{
    public enum ListViewSortOrder
    {
        None = 0,
        Ascending = 1,
        Descending = 2,
    }

    public class ListViewSorter : IComparer
    {
        private int sortColumn;
        private ListViewSortOrder sortOrder;
        private int[] stringCompareColumnIndex;

        private ListViewSorter()
        {
            sortColumn = 0;
            sortOrder = ListViewSortOrder.None;
        }
        public ListViewSorter(int[] stringCompareColumnIndex) : base()
        {
            this.stringCompareColumnIndex = stringCompareColumnIndex;
        }


        public int SortColumn
        {
            set
            {
                sortColumn = value;
            }
            get
            {
                return sortColumn;
            }
        }

        public ListViewSortOrder SortOrder
        {
            set
            {
                sortOrder = value;
            }
            get
            {
                return sortOrder;
            }
        }

        public int Compare(object x, object y)
        {
            int result = 0;
            ListViewItem itemx = (ListViewItem)x;
            ListViewItem itemy = (ListViewItem)y;

            bool bNumberCompare = true;
            if (this.stringCompareColumnIndex != null)
            {
                foreach (int i in this.stringCompareColumnIndex)
                {
                    if (i == sortColumn)
                    {
                        bNumberCompare = false;
                        break;
                    }
                }
            }

            if (bNumberCompare)
            {
                int a = Convert.ToInt32(itemx.SubItems[sortColumn].Text.GetDigits());
                int b = Convert.ToInt32(itemy.SubItems[sortColumn].Text.GetDigits());

                if (a > b)
                {
                    result = 1;
                }

                else
                {
                    result = -1;
                }
            }

            else
            {
                result = String.Compare(itemx.SubItems[sortColumn].Text, itemy.SubItems[sortColumn].Text);
            }

            if (sortOrder == ListViewSortOrder.Ascending)
            {
                return result;
            }

            else if (sortOrder == ListViewSortOrder.Descending)
            {
                return (-result);
            }

            else
            {
                return 0;
            }
        }
    }
}
