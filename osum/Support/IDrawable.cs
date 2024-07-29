﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using osum.Support;

namespace osum.Graphics.Sprites
{
    internal interface IDrawable : IUpdateable
    {
        /// <summary>
        /// Draws this object to screen.
        /// </summary>
        void Draw();
    }
}
