﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;

#if IPHONE
using OpenTK.Graphics.ES11;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.OpenGLES;

using TextureTarget = OpenTK.Graphics.ES11.All;
using TextureParameterName = OpenTK.Graphics.ES11.All;
using EnableCap = OpenTK.Graphics.ES11.All;
using BlendingFactorSrc = OpenTK.Graphics.ES11.All;
using BlendingFactorDest = OpenTK.Graphics.ES11.All;
using PixelStoreParameter = OpenTK.Graphics.ES11.All;
using VertexPointerType = OpenTK.Graphics.ES11.All;
using ColorPointerType = OpenTK.Graphics.ES11.All;
using ClearBufferMask = OpenTK.Graphics.ES11.All;
using TexCoordPointerType = OpenTK.Graphics.ES11.All;
using BeginMode = OpenTK.Graphics.ES11.All;
using MatrixMode = OpenTK.Graphics.ES11.All;
using PixelInternalFormat = OpenTK.Graphics.ES11.All;
using PixelFormat = OpenTK.Graphics.ES11.All;
using PixelType = OpenTK.Graphics.ES11.All;
using ShaderType = OpenTK.Graphics.ES11.All;
using VertexAttribPointerType = OpenTK.Graphics.ES11.All;
using ProgramParameter = OpenTK.Graphics.ES11.All;
using ShaderParameter = OpenTK.Graphics.ES11.All;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
#else
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using osum.Input;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
#endif

using System.Text;
using OpenTK;

namespace osum.Graphics
{
    public class pTexture : IDisposable
    {
        public string assetName;
        public bool fromResourceStore;
        internal int Width;
        internal int Height;
        internal int X;
        internal int Y;
        internal int LastAccess;
        private bool isDisposed;
#if DEBUG
        internal int id;
        internal static int staticid = 1;
#endif
        //public SkinSource Source;

        ~pTexture()
        {
#if DEBUG
            //Console.WriteLine("TEXTURE FINALIZER");
#endif
            Dispose(false);
        }

        public pTexture(TextureGl textureGl, int width, int height)
        {
            TextureGl = textureGl;
            Width = width;
            Height = height;

#if DEBUG
            id = staticid++;
#endif
        }

        private pTexture()
        {
#if DEBUG
            id = staticid++;
#endif
        }

        internal TextureGl TextureGl;
        internal bool TrackAccessTime;

        public bool IsDisposed
        {
            get
            {
                if (TrackAccessTime)
                    LastAccess = -1;
                return isDisposed;
            }
            set { isDisposed = value; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of textures but leaves the GC finalizer in place.
        /// This is used to temporarily unload sprites which are not accessed in a while.
        /// </summary>
        internal void TemporalDispose()
        {
            isDisposed = true;

            if (TextureGl != null)
            {
                TextureGl.Dispose();
                TextureGl = null;
            }
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposed) return;
            isDisposed = true;

            if (TextureGl != null)
            {
                TextureGl.Dispose();
                TextureGl = null;
            }
        }
		
		/// <summary>
		/// Unloads texture without fully disposing. It may be able to be restored by calling ReloadIfPossible
		/// </summary>
		internal void UnloadTexture()
		{
			if (TextureGl != null)
			{
				TextureGl.Dispose();
				TextureGl = null;
			}
		}
		
		internal void ReloadIfPossible()
		{
			if (TextureGl == null)
			{
				if (assetName != null)
				{
					pTexture reloadedTexture = FromFile(assetName);
					this.TextureGl = reloadedTexture.TextureGl;
					reloadedTexture.TextureGl = null; //deassociate with temporary pTexture to avoid disposal.
				}
			}
		}

        #endregion

        public void SetData(byte[] data)
        {
            SetData(data, 0, 0);
        }

        public void SetData(byte[] data, int level, PixelFormat format)
        {
            if (TextureGl != null)
            {
                if ((int)format != 0)
                    TextureGl.SetData(data, level, format);
                else
                    TextureGl.SetData(data, level);
            }
        }

        /// <summary>
        /// Read a pTexture from the ResourceStore (ouenresources project).
        /// </summary>
        public static pTexture FromResourceStore(string filename)
        {
            //byte[] bytes = ResourcesStore.ResourceManager.GetObject(filename) as byte[];
            byte[] bytes = null;

            if (bytes == null)
                return null;

            pTexture pt = new pTexture();

            pt.assetName = filename;
            pt.fromResourceStore = true;

            using (Stream stream = new MemoryStream(bytes))
            {
                using (HaxBinaryReader br = new HaxBinaryReader(stream))
                {
                    //XNA pipeline header crap.  Fuck it all.
                    br.ReadBytes(10);
                    int typeCount = br.Read7BitEncodedInt();
                    for (int i = 0; i < typeCount; i++)
                    {
                        br.ReadString();
                        br.ReadInt32();
                    }
                    br.Read7BitEncodedInt();
                    br.Read7BitEncodedInt();
                    //And that's the header dealt with.

                    br.ReadInt32(); // skip SurfaceFormat
                    pt.Width = br.ReadInt32();
                    pt.Height = br.ReadInt32();
                    int numberLevels = br.ReadInt32();

                    pt.TextureGl = new TextureGl(pt.Width, pt.Height);

                    for (int i = 0; i < numberLevels; i++)
                    {
                        int count = br.ReadInt32();
                        byte[] data = br.ReadBytes(count);
                        pt.SetData(data, i, 0);
                    }
                }
            }

            return pt;
        }

        /// <summary>
        /// Read a pTexture from an arbritrary file.
        /// </summary>
        public static pTexture FromFile(string filename)
        {
            //todo: this is a redundant check?
            if (!File.Exists(filename)) return null;

            try
            {
#if IPHONE
				using (UIImage image = UIImage.FromFile(filename))
                    return FromUIImage(image,filename);
#else
                using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                    return FromStream(stream, filename);
#endif
            }
            catch
            {
                return null;
            }
        }

        public static pTexture FromStream(Stream stream, string assetname)
        {
            return FromStream(stream, assetname, false);
        }

#if IPHONE
		public unsafe static pTexture FromUIImage(UIImage textureImage, string assetname)
		{
            if (textureImage == null)
                return null;

            int texWidth = (int)textureImage.Size.Width;

            int texHeight = (int)textureImage.Size.Height;

            byte[] textureData = new byte[texWidth * texHeight * 4];

            CGBitmapContext textureContext;

            fixed (byte* pTextureData = textureData) {

                textureContext = new CGBitmapContext((IntPtr) pTextureData,
                        texWidth, texHeight, 8, texWidth * 4,
                        textureImage.CGImage.ColorSpace, CGImageAlphaInfo.PremultipliedLast);

                textureContext.DrawImage(new RectangleF (0, 0, texWidth, texHeight), textureImage.CGImage);

                textureContext.Dispose();

            }
			
			//todo: we can call this using the fixed context above and pass on an IntPtr for loading?
            pTexture tex = FromRawBytes(textureData,(int)texWidth, (int)texHeight);
			tex.assetName = assetname;
			return tex;
		}
#endif

        /// <summary>
        /// Read a pTexture from an arbritrary file.
        /// </summary>
        public unsafe static pTexture FromStream(Stream stream, string assetname, bool saveToFile)
        {
            try
            {
#if IPHONE
				return FromUIImage(UIImage.LoadFromData(NSData.FromStream(stream)),assetname);
#else
                using (Bitmap b = (Bitmap)Image.FromStream(stream, false, false))
                {
                    BitmapData data = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly,
                                                 System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    if (saveToFile)
                    {
                        byte[] bitmap = new byte[b.Width * b.Height * 4];
                        Marshal.Copy(data.Scan0, bitmap, 0, bitmap.Length);
                        File.WriteAllBytes(assetname, bitmap);
                    }

                    pTexture tex = FromRawBytes(data.Scan0, b.Width, b.Height);
                    tex.assetName = assetname;
                    b.UnlockBits(data);
                    return tex;
                }
#endif
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public static pTexture FromBytes(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
                return FromStream(ms, "");
        }

        public static pTexture FromBytesSaveRawToFile(byte[] data, string filename)
        {
            using (MemoryStream ms = new MemoryStream(data))
                return FromStream(ms, filename, true);
        }

        public static pTexture FromRawBytes(IntPtr location, int width, int height)
        {
            pTexture pt = new pTexture();
            pt.Width = width;
            pt.Height = height;

            try
            {
                //OpenGL outperforms XNA in this case as we can remain in native unsafe territory.
                pt.TextureGl = new TextureGl(pt.Width, pt.Height);
                pt.TextureGl.SetData(location, 0, 0);

            }
            catch
            {
            }

            return pt;
        }

        public static pTexture FromRawBytes(byte[] bitmap, int width, int height)
        {
            pTexture pt = new pTexture();
            pt.Width = width;
            pt.Height = height;

            try
            {
                pt.TextureGl = new TextureGl(pt.Width, pt.Height);
                pt.SetData(bitmap);
            }
            catch
            {
            }

            return pt;
        }

        /*public static pTexture FromText(string text, SizeF dim, UITextAlignment alignment, string fontName, float fontSize) {
            UIFont font = UIFont.FromName(fontName, fontSize);
			
            int width = (int)dim.Width;
            if (width != 1 && (width & (width - 1)) != 0) {
                int i = 1;
                while (i < width) {
                    i *= 2;
                }
				
                width = i;
            }
			
            int height = (int)dim.Height;
            if (height != 1 && (height & (height - 1)) != 0) {
                int i = 1;
                while (i < height) {
                    i *= 2;
                }
                height = i;
            }
			
            CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB(); //CGColorSpace.CreateDeviceGray();
			
            byte[] data = new byte[width * height];
			
            unsafe {
                fixed (byte* dataPb = data) {
                    using (CGContext context = new CGBitmapContext((IntPtr)dataPb, width, height, 8, width, colorSpace, CGImageAlphaInfo.None)) {
                        context.SetGrayFillColor(1f, 1f);
                        context.TranslateCTM(0f, height);
                        context.ScaleCTM(1f, -1f);
                        UIGraphics.PushContext(context);
                        //text.DrawInRect(new RectangleF(0, 0, dim.Width, dim.Height), font, UILineBreakMode.WordWrap, alignment);
                        UIGraphics.PopContext();
                    }
                }
            }
            colorSpace.Dispose();
			
            return null;
            //FromRawBytes(
            //InitWithData(data, Texture2DPixelFormat.A8, width, height, dim);
        }*/
    }

    /// <summary>
    /// BinaryWriter exposing protected MS function.
    /// </summary>
    internal class HaxBinaryReader : BinaryReader
    {
        public HaxBinaryReader(Stream input)
            : base(input)
        {
        }

        public HaxBinaryReader(Stream input, Encoding encoding)
            : base(input, encoding)
        {
        }

        public new int Read7BitEncodedInt()
        {
            byte num3;
            int num = 0;
            int num2 = 0;
            do
            {
                if (num2 == 0x23)
                {
                    return -1;
                }
                num3 = ReadByte();
                num |= (num3 & 0x7f) << num2;
                num2 += 7;
            } while ((num3 & 0x80) != 0);
            return num;
        }
    }
}