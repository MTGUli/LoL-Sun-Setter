using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using LoLCameraSharp.MemoryFunctions;
using LoL_Sun_Changer.Sun_Classes;
using Util;

namespace LoL_Sun_Changer
{
    public partial class SunChanger : Form
    {
        r3dSun sun = new r3dSun();
        String sunPattern = @"\x89\x35\x00\x00\x00\x00\xE8\x00\x00\x00\x00\x8B\xCE";
        String sunMask = "xx????x????xx";
        IntPtr sunAddress;

        //Memory Functions:
        MemoryEditor m = new MemoryEditor("League of Legends");
        Pattern p = new Pattern();

        public SunChanger()
        {
            InitializeComponent();
        }

        private void SunChanger_Load(object sender, EventArgs e)
        {
            
        }

        private void btnFindGame_Click(object sender, EventArgs e)
        {
            
            if (m.gameFound)
            {
                if (GetSunPointer())
                    PopulateUI();
            }
            else
            {
                if (m.FindGame("League of Legends"))
                    if (GetSunPointer())
                        PopulateUI();
            }
        }

        private void PopulateUI()
        {
            nudUnkX.Value = (decimal)sun.sunLight.unk.x;
            nudUnkY.Value = (decimal)sun.sunLight.unk.y;
            nudUnkZ.Value = (decimal)sun.sunLight.unk.z;

            nudFlags.Value = (decimal)sun.sunLight.flags;
            nudType.Value = (decimal)sun.sunLight.Type;

            nudIntensity.Value = (decimal)sun.sunLight.intensity;
            nudRadius1.Value = (decimal)sun.sunLight.radius1;
            nudRadius2.Value = (decimal)sun.sunLight.radius2;

            nudDirectionX.Value = (decimal)sun.sunLight.direction.x;
            nudDirectionY.Value = (decimal)sun.sunLight.direction.y;
            nudDirectionZ.Value = (decimal)sun.sunLight.direction.z;

            nudSpotAngle.Value = (decimal)sun.sunLight.spotAngle;
            nudFallofAngle.Value = (decimal)sun.sunLight.fallOfAngle;

            nudAtt0.Value = (decimal)sun.sunLight.att0;
            nudAtt1.Value = (decimal)sun.sunLight.att1;
            nudAtt2.Value = (decimal)sun.sunLight.att2;

            nudR.Value = (decimal)sun.sunLight.R;
            nudG.Value = (decimal)sun.sunLight.G;
            nudB.Value = (decimal)sun.sunLight.B;

            nudR1.Value = (decimal)sun.sunLight.R1;
            nudB1.Value = (decimal)sun.sunLight.B1;
            nudG1.Value = (decimal)sun.sunLight.G1;

            currentMousePos.X = (int)Scale((double)nudDirectionX.Value, 1, -1, 0, 144);
            currentMousePos.Y = (int)Scale((double)nudDirectionX.Value, -1, 1, 0, 144);
            picSunLocation.Invalidate();
        }

        private void UpdateSunFromUI()
        {
            sun.sunLight.unk.x = (float)nudUnkX.Value;
            sun.sunLight.unk.y = (float)nudUnkY.Value;
            sun.sunLight.unk.z = (float)nudUnkZ.Value;

            sun.sunLight.flags = (UInt32)nudFlags.Value;
            sun.sunLight.Type = (UInt32)nudType.Value;

            sun.sunLight.intensity = (float)nudIntensity.Value;
            sun.sunLight.radius1 = (float)nudRadius1.Value;
            sun.sunLight.radius2 = (float)nudRadius2.Value;

            sun.sunLight.direction.x = (float)nudDirectionX.Value;
            sun.sunLight.direction.y = (float)nudDirectionY.Value;
            sun.sunLight.direction.z = (float)nudDirectionZ.Value;

            sun.sunLight.spotAngle = (float)nudSpotAngle.Value;
            sun.sunLight.fallOfAngle = (float)nudFallofAngle.Value;

            sun.sunLight.att0 = (float)nudAtt0.Value;
            sun.sunLight.att1 = (float)nudAtt1.Value;
            sun.sunLight.att2 = (float)nudAtt2.Value;

            sun.sunLight.R = (float)nudR.Value;
            sun.sunLight.G = (float)nudG.Value;
            sun.sunLight.B = (float)nudB.Value;

            sun.sunLight.R1 = (float)nudR1.Value;
            sun.sunLight.G1 = (float)nudG1.Value;
            sun.sunLight.B1 = (float)nudB1.Value;
        }

        private bool GetSunPointer()
        {
            uint patternAddr = p.FindPattern(sunPattern, sunMask, ref m);

            if (patternAddr == 0)
                return false; //Pattern is out of date

            //Increment Addr by one to get to the location where the pointer is
            patternAddr += 2;

            sunAddress = (IntPtr)m.ReadUInt((IntPtr)m.ReadUInt((IntPtr)patternAddr));
            sun = Utility.ToStructure<r3dSun>(m.ReadBytes(sunAddress, 0xA8));

            return true;
        }

        private void btnUpdateSun_Click(object sender, EventArgs e)
        {
            UpdateSunFromUI();
            m.WriteBytes(sunAddress, Utility.ToByteArray(sun));
        }

        private bool mouseDown = false;
        private Point currentMousePos;

        private void picSunLocation_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
        }

        private void picSunLocation_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
        private void picSunLocation_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                currentMousePos = e.Location;
                picSunLocation.Invalidate();


                nudDirectionX.Value = (decimal)Scale((double)e.Location.X, 0, 144, 1, -1);
                if (nudDirectionX.Value > 1)
                    nudDirectionX.Value = 1;
                if (nudDirectionX.Value < -1)
                    nudDirectionX.Value = -1;
                nudDirectionZ.Value = (decimal)Scale((double)e.Location.Y, 0, 144, -1, 1);
                if (nudDirectionZ.Value > 1)
                    nudDirectionZ.Value = 1;
                if (nudDirectionZ.Value < -1)
                    nudDirectionZ.Value = -1;

                if (m.gameFound)
                {
                    UpdateSunFromUI();
                    m.WriteBytes(sunAddress, Utility.ToByteArray(sun));
                }
            }
        }

        public static double Scale(double elementToScale,
              double rangeMin, double rangeMax,
              double scaledRangeMin, double scaledRangeMax)
        {
            var scaled = scaledRangeMin + ((elementToScale - rangeMin) * (scaledRangeMax - scaledRangeMin) / (rangeMax - rangeMin));
            return scaled;
        }

        private void picSunLocation_Paint(object sender, PaintEventArgs e)
        {
            using (Pen p = new Pen(System.Drawing.Color.Blue, 3.0F))
            {
                Rectangle currentRect = Rectangle.FromLTRB(
                                                           currentMousePos.X - 1,
                                                           currentMousePos.Y - 1,
                                                           currentMousePos.X + 1,
                                                           currentMousePos.Y + 1);
                e.Graphics.DrawRectangle(p, currentRect);
            }

            using (Pen p = new Pen(System.Drawing.Color.Black, 3.0F))
            {
                Rectangle currentRect = Rectangle.FromLTRB(
                                                           72 - 1,
                                                           72 - 1,
                                                           72 + 1,
                                                           72 + 1);
                e.Graphics.DrawRectangle(p, currentRect);
            }
        }
    }
}
