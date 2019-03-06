/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using XenCenterLib;

namespace XenAdmin.Controls
{
    public class DateTimeMinutes15 : DateTimePicker
    {
        public bool AutoCorrecting;


        public DateTimeMinutes15()
        {
            Format = DateTimePickerFormat.Custom;
            ShowUpDown = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Up)
            {
                Value = Value.AddMinutes(15);
                return true;
            }

            if (keyData == Keys.Down)
            {
                Value = Value.AddMinutes(-15);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32.OCM_NOTIFY || m.Msg == Win32.WM_NOTIFY)
            {
                Win32.NMHDR notify = (Win32.NMHDR)Marshal.PtrToStructure(m.LParam, typeof(Win32.NMHDR));

                if (notify.code == -722)
                {
                    Win32.NMUPDOWN ud = (Win32.NMUPDOWN)Marshal.PtrToStructure(m.LParam, typeof(Win32.NMUPDOWN));
                    Value = ud.delta < 0 ? Value.AddMinutes(-15 - ud.delta) : Value.AddMinutes(15 - ud.delta);
                }
            }
            base.WndProc(ref m);
        }

        public new DateTime Value
        {
            get { return base.Value; }
            set
            {
                try
                {
                    AutoCorrecting = true;
                    base.Value = value;
                }
                finally
                {
                    AutoCorrecting = false;
                }
            }
        }
    }
}
