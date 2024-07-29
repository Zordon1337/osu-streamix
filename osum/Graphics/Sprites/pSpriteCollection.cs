﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace osum.Graphics.Sprites
{
    internal class pSpriteCollection
    {
        internal List<pSprite> SpriteCollection;

        internal pSpriteCollection()
        {
            this.SpriteCollection = new List<pSprite>();
        }

        internal pSpriteCollection(IEnumerable<pSprite> sprites)
        {
            this.SpriteCollection = new List<pSprite>(sprites);
        }
    }
}
