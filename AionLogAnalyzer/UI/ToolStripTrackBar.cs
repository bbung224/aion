using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Drawing;

namespace AionLogAnalyzer
{
    public class ToolStripTrackBar : ToolStripControlHost
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public ToolStripTrackBar()
            : base(new TrackBar())
        {
            this.AutoSize = false;
            TrackBar.AutoSize = false;
            TrackBar.TickStyle = TickStyle.None;
            TrackBar.Size = new Size(100, 20);
            TrackBar.SetRange(0, 70);
        }

        /// <summary>
        /// Create a strongly typed property called TrackBar - handy to prevent casting everywhere.
        /// </summary>
        /// 
        public TrackBar TrackBar
        {
            get
            {
                return Control as TrackBar;
            }
        }

        public int Value18;


        public int Value
        {
            get
            {
                Value18 = TrackBar.Value;
                return TrackBar.Value;
            }
            set
            {
                TrackBar.Value = value;
                Value18 = TrackBar.Value;
            }
        }

        /// <summary>
        /// Attach to events we want to re-wrap
        /// </summary>
        /// <param name="control"></param>
        protected override void OnSubscribeControlEvents(Control control)
        {
            base.OnSubscribeControlEvents(control);
            TrackBar trackBar = control as TrackBar;
            trackBar.ValueChanged += new EventHandler(trackBar_ValueChanged);
        }
        /// <summary>
        /// Detach from events.
        /// </summary>
        /// <param name="control"></param>
        protected override void OnUnsubscribeControlEvents(Control control)
        {
            base.OnUnsubscribeControlEvents(control);
            TrackBar trackBar = control as TrackBar;
            trackBar.ValueChanged -= new EventHandler(trackBar_ValueChanged);
        }

        /// <summary>
        /// Routing for event
        /// TrackBar.ValueChanged -> ToolStripTrackBar.ValueChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void trackBar_ValueChanged(object sender, EventArgs e)
        {
            // when the trackbar value changes, fire an event.
            if (this.ValueChanged != null)
            {
                ValueChanged(sender, e);
            }
        }
        // add an event that is subscribable from the designer.
        public event EventHandler ValueChanged;

        // set other defaults that are interesting
        /*
        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 10);
            }
        }
         */
    }
}
