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
using System.Drawing;

namespace XenAdmin.Core
{
    public static class ProgressBarHelper
    {
        public static Image GetImage(bool isCompleted, Exception error, int percentComplete)
        {
            if (isCompleted)
                return error == null
                    ? Images.StaticImages._000_Tick_h32bit_16
                    : error is CancelledException
                        ? Images.StaticImages.cancelled_action_16
                        : Images.StaticImages._000_error_h32bit_16;

            if (percentComplete < 9)
                return Images.StaticImages.usagebar_0;
            if (percentComplete < 18)
                return Images.StaticImages.usagebar_1;
            if (percentComplete < 27)
                return Images.StaticImages.usagebar_2;
            if (percentComplete < 36)
                return Images.StaticImages.usagebar_3;
            if (percentComplete < 45)
                return Images.StaticImages.usagebar_4;
            if (percentComplete < 54)
                return Images.StaticImages.usagebar_5;
            if (percentComplete < 63)
                return Images.StaticImages.usagebar_6;
            if (percentComplete < 72)
                return Images.StaticImages.usagebar_7;
            if (percentComplete < 81)
                return Images.StaticImages.usagebar_8;
            if (percentComplete < 90)
                return Images.StaticImages.usagebar_9;

            return Images.StaticImages.usagebar_10;
        }
    }
}