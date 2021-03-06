﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csharp.memreplay
{
    public partial class ScreenForm : Form
    {
        private Replay _replay;

        public ScreenForm()
        {
            InitializeComponent();
        }

        public ScreenForm(Replay replay) : this()
        {
            _replay = replay;

            this.Text = _replay.Store;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Bitmap screen = _replay.Screen;
            if (screen == null) {


            } else {
                this.SetClientSizeCore(screen.Width, screen.Height);

                this.BackgroundImage = screen;
            }
        }
    }
}
