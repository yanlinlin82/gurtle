#region License, Terms and Author(s)
//
// Gurtle - IBugTraqProvider for Google Code
// Copyright (c) 2008 Atif Aziz. All rights reserved.
//
//  Author(s):
//
//      Atif Aziz, http://www.raboof.com
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
#endregion

namespace Gurtle
{
    using System.Windows.Forms;

    internal static class TextBoxExtensions
    {
        /// <remarks>
        /// According to <see cref="TextBoxBase.ShortcutsEnabled"/> docs, 
        /// <see cref="TextBox"/> does not support shortcuts and therefore 
        /// the useful <kbd>CTRL+A</kbd> shortcut to select all text does 
        /// not work. This method enables that shortcut. Do not call
        /// this method more than once for the same <see cref="TextBox"/> 
        /// instance.</remarks>

        internal static void EnableShortcutToSelectAllText(this TextBox textBox)
        {
            textBox.KeyPress += (sender, e) =>
            {
                //
                // Credit: http://www.redmountainsw.com/wordpress/archives/shortcutsenabled-on-textbox-winforms-dotnet-does-not-perform-selectall-with-controla
                //

                if (e.KeyChar != 1) return;
                textBox.SelectAll();
                e.Handled = true;
            };
        }
    }
}
