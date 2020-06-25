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
using System.ComponentModel;
using System.Windows.Forms;

namespace XenAdmin.Controls.Ballooning
{
    public partial class MemorySpinner : UserControl
    {
        public event EventHandler SpinnerValueChanged;
        private double valueMiB;
        private string previousUnitsValue;
        private bool initializing = true;

        public MemorySpinner()
        {
            InitializeComponent();
            previousUnitsValue = Messages.VAL_GIGIB;
        }

        public void Initialize(double amount, double static_max)
        {
            amount = Util.CorrectRoundingErrors(amount);

            Units = static_max <= Util.BINARY_GIGA ? Messages.VAL_MEGIB : Messages.VAL_GIGIB;
            ChangeSpinnerSettings();
            previousUnitsValue = Units;
            Initialize(amount, RoundingBehaviour.None);
        }

        public void Initialize(double amount, RoundingBehaviour rounding)
        {
            ValueMiB = Util.ToMiB(amount, rounding);
            setSpinnerValueDisplay(amount);
            initializing = false;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
        public string Units
        {
            get
            {
                return SpinnerUnits.Text;
            }
            set
            {
                SpinnerUnits.Text = value;
            }
        }

        [Browsable(false)]
        public double Value
        {
            get
            {
                return ValueMiB * Util.BINARY_MEGA;
            }
        }

        double ValueMiB
        {
            get
            {
                return valueMiB;
            }

            set
            {
                valueMiB = value;
            }
        }

        private void setSpinnerValueDisplay(double value)
        {
            decimal newValue;
            if (Units == "GiB")
            {
                newValue = (decimal)Util.ToGiB(value, RoundingBehaviour.Nearest, 1);
            }
            else
            {
                newValue = (long)Util.ToMiB(value, RoundingBehaviour.Nearest);
            }
            if (newValue < Spinner.Minimum)
                newValue = Spinner.Minimum;
            if (newValue > Spinner.Maximum)
                newValue = Spinner.Maximum;
            Spinner.Value = newValue;
        }

        public static void CalcMiBRanges(double minBytes, double maxBytes, out double minMiB, out double maxMiB)
        {
            // Round ranges inwards to avoid bugs like CA-34487 and CA-34996
            minMiB = Util.ToMiB(minBytes, RoundingBehaviour.Up);
            maxMiB = Util.ToMiB(maxBytes, RoundingBehaviour.Down);
            if (minMiB > maxMiB)  // just in case...
            {
                minMiB = Util.ToMiB(minBytes, RoundingBehaviour.None);
                maxMiB = Util.ToMiB(maxBytes, RoundingBehaviour.None);
            }
        }

        public static void CalcGiBRanges(double minBytes, double maxBytes, out double minGiB, out double maxGiB)
        {
            // Round ranges inwards to avoid bugs like CA-34487 and CA-34996
            minGiB = Util.ToGiB(minBytes, RoundingBehaviour.Up, 1);
            maxGiB = Util.ToGiB(maxBytes, RoundingBehaviour.Down, 1);
            if (minGiB > maxGiB)  // just in case...
            {
                minGiB = Util.ToGiB(minBytes, RoundingBehaviour.None, 1);
                maxGiB = Util.ToGiB(maxBytes, RoundingBehaviour.None, 1);
            }
        }

        public void SetRange(double min, double max)
        {
            if (min > max)
                return;  // Can happen when we are adjusting several simultaneously: can cause a stack overflow

            double spinnerMin, spinnerMax;
            if (Units == "MiB")
            {
                CalcMiBRanges(min, max, out spinnerMin, out spinnerMax);
            }
            else
            {
                CalcGiBRanges(min, max, out spinnerMin, out spinnerMax);               
            }
            Spinner.Minimum = (decimal)spinnerMin;
            Spinner.Maximum = (decimal)spinnerMax;
        }

        [Browsable(false)]
        public double Increment
        {
            get
            {
               return (double)Spinner.Increment;
            }
            set
            {
                if (Units == "MiB")
                {
                    Spinner.Increment = (decimal)value / Util.BINARY_MEGA;                    
                }
                else
                {
                    // When the units are GB, we simply want the numbers to increase by 1 if the spinner value is greater than 10 GB
                    // and by 0.1 if smaller than 10 GB, this being the reason we ignore the given value.
                    if (valueMiB * Util.BINARY_MEGA < 10 * Util.BINARY_GIGA)
                    {
                        Spinner.Increment = 0.1M;
                    }
                    else
                    {
                        Spinner.Increment = 1; 
                    }
                }
            }
        }

        private void Spinner_ValueChanged(object sender, EventArgs e)
        {
            // We do not want to modify the ValueMB if the user does not modify anything in the Spinner.Value. 
            // When the Memory Settings dialog is initializing and the units change because the new value is > 1 GiB,
            // we do not want any changes to be applied to ValueMB.
            if (initializing)
              return;

            if (Units == "GiB")
            {
                ValueMiB = (double)Spinner.Value * Util.BINARY_KILO;
            }
            else
            {
                ValueMiB = (double)Spinner.Value;
            }

            if (SpinnerValueChanged != null)
                SpinnerValueChanged(this, e);
        }

        private void Spinner_Leave(object sender, EventArgs e)
        {
            var num = sender as NumericUpDown;
            if (num != null)
                num.Text = num.Value.ToString();
        }

        private void ChangeSpinnerSettings()
        {
            if (Units == previousUnitsValue)
                return;

            if (Units == "GiB")
            {
                SetRange((double)Spinner.Minimum * Util.BINARY_MEGA, (double)Spinner.Maximum * Util.BINARY_MEGA);
                Spinner.DecimalPlaces = 1;
            }
            else
            {
                SetRange((double)Spinner.Minimum * Util.BINARY_GIGA, (double)Spinner.Maximum * Util.BINARY_GIGA);
                Spinner.DecimalPlaces = 0;
            }
        }      
    }
}
