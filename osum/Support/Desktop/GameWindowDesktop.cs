using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;
using System.Drawing;
using osum.GameModes;
using osum.Support;

namespace osum
{
    class GameWindowDesktop : GameWindow
    {
        /// <summary>Creates a 1024x768 window with the specified title.</summary>
        public GameWindowDesktop() : base(960, 640, GraphicsMode.Default, "osu!m")
        {
            VSync = VSyncMode.Off;
            //GameBase.WindowSize = new Size(960,640);
        }

        public void Run()
        {
            base.Run();
        }

        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            
            GL.Disable(EnableCap.Lighting);
            GL.Enable(EnableCap.Blend);
            
            GameBase.Instance.Initialize();

            KeyPress += new EventHandler<KeyPressEventArgs>(GameWindowDesktop_KeyPress);
        }

        void GameWindowDesktop_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 'a':
                    Player.Autoplay = !Player.Autoplay;
                    break;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (Director.CurrentOsuMode != OsuMode.MainMenu)
            {
                e.Cancel = true;
                Director.ChangeMode(OsuMode.MainMenu,new FadeTransition(200,400));
            }

            base.OnClosing(e);
        }

        /// <summary>
        /// Called when your window is resized. Set your viewport here. It is also
        /// a good place to set up your projection matrix (which probably changes
        /// along when the aspect ratio of your window).
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            GameBase.Instance.SetupScreen();
            
        }

        /// <summary>
        /// Called when it is time to setup the next frame. Add you game logic here.
        /// </summary>
        /// <param name="e">Contains timing information for framerate independent logic.</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            
            if (Keyboard[Key.Escape])
                Exit();
            
            //todo: make update happen from here.
        }

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            
            //ensure the gl context is in the current thread.
            MakeCurrent();
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            GameBase.Instance.Draw(e);
            
            // display
            SwapBuffers();
        }
    }
}
