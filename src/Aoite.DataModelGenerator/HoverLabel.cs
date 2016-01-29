using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aoite.DataModelGenerator
{
    public class HoverLabel : Label
    {
        public HoverLabel()
        {
            this.Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            base.BackColor = this.HoverColor;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            base.BackColor = this._oldBC;
        }

        private Color _oldBC;
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                this._oldBC = value;
                base.BackColor = value;
            }
        }

        private Color _HoverColor =Color.White;
        public Color HoverColor
        {
            get
            {
                return this._HoverColor;
            }
            set
            {
                this._HoverColor = value;
            }
        }

    }
}
