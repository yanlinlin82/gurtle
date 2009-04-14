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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    #endregion

    /// <summary>
    /// Saves and restores the location, size and state of a form to and 
    /// from a specific <see cref="SettingsBase" /> object.
    /// </summary>

    [Serializable]
    internal sealed class WindowSettings
    {
        private Form _form;
        private SettingsBase _settings;
        private SettingsProperty _location;
        private SettingsProperty _size;
        private SettingsProperty _windowState;

        public WindowSettings(SettingsBase settings, Form form) :
            this(settings, form, null) {}

        public WindowSettings(SettingsBase settings, Form form, string prefix)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            if (form == null) throw new ArgumentNullException("form");

            _form = form;
            _settings = settings;

            if (string.IsNullOrEmpty(prefix))
                prefix = form.GetType().Name;

            var properties = settings.Properties;
            _location = properties[prefix + "Location"];
            _size = properties[prefix + "Size"];
            _windowState = properties[prefix + "WindowState"];

            form.Load += OnFormLoad;
            form.Closing += OnFormClosing;
            form.Disposed += OnFormDisposed;
        }

        public Point? Location
        {
            get { return (Point?) _settings[_location.Name]; }
            set { _settings[_location.Name] = value; }
        }

        public Size? Size
        {
            get { return (Size?) _settings[_size.Name]; }
            set { _settings[_size.Name] = value; }
        }

        public FormWindowState? WindowState
        {
            get { return (FormWindowState?) _settings[_windowState.Name]; }
            set { _settings[_windowState.Name] = value; }
        }

        private void OnFormDisposed(object sender, EventArgs e)
        {
            var form = _form;

            _form = null;
            _settings = null;
            _location = _size = _windowState = null;

            Debug.Assert(form != null);

            form.Load -= OnFormLoad;
            form.Closing -= OnFormClosing;
            form.Disposed -= OnFormDisposed;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            Recall();
        }

        private void OnFormClosing(object sender, CancelEventArgs e)
        {
            Remember();
        }

        public void Remember()
        {
            var form = _form;
            if (form == null)
                throw new InvalidOperationException();

            var bounds = FormBoundsFromWindowState(form);

            if (bounds != null)
            {
                if (!IsOnScreen(bounds.Value.Location, bounds.Value.Size))
                    return;

                Location = bounds.Value.Location;
                Size = bounds.Value.Size;
            }

            WindowState = form.WindowState;
        }

        public void Recall()
        {
            var form = _form;
            if (form == null)
                throw new InvalidOperationException();
            
            var location = Location ?? form.Location;
            var size = Size ?? form.Size;

            if (IsOnScreen(location, size))
            {
                form.Location = location;
                form.Size = size;
            }

            var windowState = WindowState;
            if (windowState != null)
                form.WindowState = WindowState.Value;
        }

        private static bool IsOnScreen(Point location, Size size)
        {
            return IsOnScreen(location) && IsOnScreen(location + size);
        }

        private static bool IsOnScreen(Point location)
        {
            return Screen.AllScreens.Any(screen => screen.WorkingArea.Contains(location));
        }

        private static Rectangle? FormBoundsFromWindowState(Form form)
        {
            Debug.Assert(form != null);

            switch (form.WindowState)
            {
                case FormWindowState.Maximized: return form.RestoreBounds;
                case FormWindowState.Normal: return form.Bounds;
                default: return null; // Don't record anything when closing while minimized.
            }
        }
    }
}