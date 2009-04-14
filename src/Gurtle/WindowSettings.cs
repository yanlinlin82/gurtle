#region License, Terms and Author(s)
//
// Gurtle - IBugTraqProvider for Google Code
// Copyright (c) 2008 Atif Aziz. All rights reserved.
//
//  Author(s):
//
//      Atif Aziz, http://www.raboof.com
//      Don Kirkby, http://code.google.com/p/donkirkby/
//
// This library is free software; you can redistribute it and/or modify it 
// under the terms of the New BSD License, a copy of which should have 
// been delivered along with this distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY 
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// This class is a derivative work of WindowSettings from project donkirkby:
// http://code.google.com/p/donkirkby/source/browse/trunk/WindowSettings/WindowSettings.cs?spec=svn107&r=107
// It was originally licensed under the MIT License, the terms 
// and conditions of which can be found on-line at:
// http://www.opensource.org/licenses/mit-license.php.
//
#endregion

namespace Gurtle
{
    #region Imports

    using System;
    using System.Drawing;
    using System.Windows.Forms;

    #endregion

    [Serializable]
    public sealed class WindowSettings
    {
        public Point Location { get; set; }
        public Size Size { get; set; }
        public FormWindowState WindowState { get; set; }
        public int[] SplitterDistances { get; set; }

        public void Record(Form form, params SplitContainer[] splitters)
        {
            bool shouldRecordSplitters;
            switch (form.WindowState)
            {
                case FormWindowState.Maximized:
                    RecordWindowPosition(form.RestoreBounds);
                    shouldRecordSplitters = true;
                    break;
                case FormWindowState.Normal:
                    shouldRecordSplitters =
                        RecordWindowPosition(form.Bounds);
                    break;
                default:
                    // Don't record anything when closing while minimized.
                    return;
            }
            WindowState = form.WindowState;
            if (shouldRecordSplitters)
            {
                RecordSplitters(splitters);
            }

        }

        public void Restore(Form form, params SplitContainer[] splitters)
        {
            if (IsOnScreen(Location, Size))
            {
                form.Location = Location;
                form.Size = Size;
                form.WindowState = WindowState;
                RestoreSplitters(splitters);
            }
            else
            {
                form.WindowState = WindowState;
            }
        }

        private void RestoreSplitters(SplitContainer[] splitters)
        {
            for (int i = 0; i < splitters.Length && i < SplitterDistances.Length; i++)
            {
                var splitter = splitters[i];
                int splitterDistance = SplitterDistances[i];
                int splitterSize =
                    splitter.Orientation == Orientation.Vertical
                        ? splitter.Width
                        : splitter.Height;
                bool isDistanceLegal =
                    splitter.Panel1MinSize <= splitterDistance
                    && splitterDistance <= splitterSize - splitter.Panel2MinSize;
                if (isDistanceLegal)
                {
                    splitter.SplitterDistance = splitterDistance;
                }
            }
        }

        private bool RecordWindowPosition(Rectangle bounds)
        {
            bool isOnScreen = IsOnScreen(bounds.Location, bounds.Size);
            if (isOnScreen)
            {
                Location = bounds.Location;
                Size = bounds.Size;
            }
            return isOnScreen;
        }

        private void RecordSplitters(SplitContainer[] splitters)
        {
            SplitterDistances = new int[splitters.Length];
            for (int i = 0; i < splitters.Length; i++)
            {
                SplitterDistances[i] = splitters[i].SplitterDistance;
            }
        }

        private bool IsOnScreen(Point location, Size size)
        {
            return IsOnScreen(location) && IsOnScreen(location + size);
        }

        private bool IsOnScreen(Point location)
        {
            foreach (var screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.Contains(location))
                {
                    return true;
                }
            }
            return false;
        }
    }
}