﻿#region License
//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2009 the Open Toolkit library.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
#if EXPERIMENTAL
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace OpenTK.Compute.CL10
{
    /// <summary>
    /// Defines the format of an OpenCL image.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ImageFormat
    {
        ChannelOrder image_channel_order;
        ChannelType image_channel_data_type;

        /// <summary>
        /// Gets or sets a <see cref="ChannelOrder"/> value, which indicates the order of the image channels.
        /// </summary>
        public ChannelOrder ChannelOrder { get { return image_channel_order; } set { image_channel_order = value; } }

        /// <summary>
        /// Gets or sets a <see cref="ChannelType"/> value, which indicates the data type of each image channel.
        /// </summary>
        public ChannelType ChannelType { get { return image_channel_data_type; } set { image_channel_data_type = value; } }
    }
}
#endif