using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace tpscabinet
{
    public partial class frm_notify : Form
    {
        private delegate void delegate_Hide();
        private System.Timers.Timer FadeOutTimer;
        private System.Timers.Timer DelayTimer;
        public int HideDelay = 4000;
        private GraphicsPath BorderPath;
        private GraphicsPath RegionPath;
        private Pen BorderPen;
        private string strTip_package1;
        private string strTip_package2;
        private bool strTip_package1_shown;
        private bool strTip_package2_shown;
        private string saved_lbl_Info;
        private Image saved_pic_Graph;
        private CabinetScraper tps_cabinet;

        public frm_notify()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            strTip_package1_shown = false;
            strTip_package2_shown = false;
            saved_pic_Graph = null;
            saved_lbl_Info = "";
            lbl_package1.Text = "";
            lbl_package2.Text = "";
            strTip_package1 = "";
            strTip_package2 = "";

            BorderPen = new Pen(Color.FromArgb(118, 118, 118));
            this.SetStyle(ControlStyles.Opaque, true);

            FadeOutTimer = new System.Timers.Timer(120);
            DelayTimer = new System.Timers.Timer(this.HideDelay);
            FadeOutTimer.Elapsed += (o, e) => {
                try {
                    this.Invoke(new delegate_FadeOutAnimation(FadeOutAnimation));
                }
                catch {}
            };
            DelayTimer.Elapsed += (o, e) => {
                try {
                    this.Invoke(new delegate_StartFadeOutAnimation(StartFadeOutAnimation));
                }
                catch {}
            };
            
            /// set for all controls transparent BG, and click handler
            foreach (Control ctrl in this.Controls)
            {
                ctrl.BackColor = Color.Transparent;
                ctrl.Click += ctrl_Click;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.BorderPath = helper.CreateRoundRect(new Rectangle(0, 0, this.Width - 1, this.Height - 1), 4, helper.RectangleCorners.All);
            this.RegionPath = helper.CreateRoundRect(new Rectangle(0, 0, this.Width, this.Height), 4, helper.RectangleCorners.All);
            this.Region = new System.Drawing.Region(RegionPath);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (BorderPath != null && RegionPath != null)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.CompositingQuality = CompositingQuality.AssumeLinear;
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                e.Graphics.FillPath(new LinearGradientBrush(new Rectangle(0, 0, this.Width, this.Height), Color.FromArgb(254, 254, 254), Color.FromArgb(228, 229, 240), LinearGradientMode.Vertical), BorderPath);
                e.Graphics.DrawPath(BorderPen, BorderPath);
            }
        }

        /// <summary>
        /// Hide window on control click 
        /// </summary>
        void ctrl_Click(object sender, EventArgs e)
        {
            this.DelayTimer.Stop();
            this.FadeOutTimer.Stop();
            this.Hide();
        }

        public void Show(int Delay)
        {
            this.HideDelay = Delay;
            this.DelayedFadeOut();
            base.Show();
        }
        
        public new void Show()
        {
            this.Show(this.HideDelay);
        }

        /// <summary>
        /// show some info
        /// </summary>
        /// <param name="Info">info string</param>
        public void FillData(string Info)
        {
            this.SuspendLayout();
            lbl_Info.Text = Info;
            lbl_Info.Left = 7;
            pic_Graph.Visible = false;
            tbl_Packages.Visible = false;
            this.ResumeLayout();
        }

        /// <summary>
        /// Fill form data
        /// </summary>
        /// <param name="tps_cabinet">scraped cabinet data</param>
        public void FillData(CabinetScraper tps_cabinet)
        {
            this.tps_cabinet = tps_cabinet;
            if (tps_cabinet.Login.Trim() == "" || tps_cabinet.Password.Trim() == "")
                this.FillData("\r\nВы не ввели свои логин\\пароль\r\n\r\n");
            else if (tps_cabinet.LastError != null && tps_cabinet.LastError.isCritical)
                this.FillData("\r\n" + tps_cabinet.LastError.Message + "\r\n\r\n");
            else if (tps_cabinet.LastUpdate == DateTime.MinValue)
                this.FillData("\r\nОбновление...\r\n\r\n");
            else
            {
                this.SuspendLayout();
                tbl_Packages.Visible = true;

                lbl_Info.Left = 91;
                ///// draw traf use piechart ////
                if ((tps_cabinet.TraffUsed + tps_cabinet.TraffLeft) != 0)
                {
                    Bitmap bmp = new Bitmap(pic_Graph.Width, pic_Graph.Height);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.Bicubic;
                        g.Clear(Color.Transparent);
                        g.FillEllipse(Brushes.LightBlue, 0, 0, bmp.Width - 1, bmp.Height - 1);
                        float Prepaid_SweepAngle = tps_cabinet.TraffUsed * 360 / tps_cabinet.TraffAll;
                        g.FillPie(Brushes.Red, 0, 0, bmp.Width - 1, bmp.Height - 1, 0, Prepaid_SweepAngle);
                        if (tps_cabinet.HasPackage) {
#if DEBUG 
                            tps_cabinet.Packages[0].TraffLeft = 600;
                            tps_cabinet.Packages[1].TraffLeft = 600;
#endif
                            /// if have show first package
                            float Package1_SweepAngle = tps_cabinet.Packages[0].TraffLeft * 360 / tps_cabinet.TraffAll;
                            if (Package1_SweepAngle > Prepaid_SweepAngle) Package1_SweepAngle = Prepaid_SweepAngle; // if package traff more prepaid show only part
                            if (tps_cabinet.TraffLeft == 0)
                            {
                                Package1_SweepAngle = tps_cabinet.Packages[0].TraffLeft * 360 / tps_cabinet.Packages[0].TraffAll;
                                g.FillPie(Brushes.DarkSeaGreen, 0, 0, bmp.Width - 1, bmp.Height - 1, 0, -Package1_SweepAngle);
                            }
                            else if (!Properties.Settings.Default.DisablePackageDiagramm)
                                g.FillPie(Brushes.DarkSeaGreen, 7, 7, bmp.Width - 15, bmp.Height - 15, Prepaid_SweepAngle, -Package1_SweepAngle);

                            //// if have second package and have space show it
                            if (tps_cabinet.Packages.Count > 1 && Package1_SweepAngle != Prepaid_SweepAngle)
                            {
                                float Package2_SweepAngle = tps_cabinet.Packages[1].TraffLeft * 360 / tps_cabinet.TraffAll;
                                if (Package2_SweepAngle > (Prepaid_SweepAngle - Package1_SweepAngle)) Package2_SweepAngle = Prepaid_SweepAngle - Package1_SweepAngle; // if package traff more prepaid+package1 show only part
                                if (tps_cabinet.TraffLeft == 0 && !Properties.Settings.Default.DisablePackageDiagramm)
                                {
                                    Package2_SweepAngle = tps_cabinet.Packages[1].TraffLeft * 360 / tps_cabinet.Packages[0].TraffAll;
                                    if (Package2_SweepAngle > (360 - Package1_SweepAngle)) Package2_SweepAngle = 360 - Package1_SweepAngle; // if package traff more package1 show only part
                                    g.FillPie(Brushes.MediumTurquoise, 7, 7, bmp.Width - 15, bmp.Height - 15, -Package1_SweepAngle, -Package2_SweepAngle);
                                }
                                else if (!Properties.Settings.Default.DisablePackageDiagramm)
                                    g.FillPie(Brushes.MediumTurquoise, 19, 19, bmp.Width - 39, bmp.Height - 39, Prepaid_SweepAngle - Package1_SweepAngle, -Package2_SweepAngle);
                            }
                        }
                    }
                    if (pic_Graph.Image != null)
                    {
                        pic_Graph.Image.Dispose();
                        pic_Graph.Image = null;
                    }
                    pic_Graph.Image = bmp;
                    pic_Graph.Visible = true;
                }
                else
                    pic_Graph.Visible = false;
                ////////////////////////////////

                int MinFromLastUpdate = (int)(DateTime.Now - tps_cabinet.LastUpdate).TotalMinutes;
                lbl_Info.Text = "Обновлено: " + tps_cabinet.LastUpdate.ToString("d MMM HH:mm") + " (" + ((MinFromLastUpdate == 0) ? "только что" : MinFromLastUpdate + " мин. назад") + ")\r\n";

                string DayUsed = "";
                if (tps_cabinet.TraffUsed > 0 && tps_cabinet.TraffLeft != 0)
                    DayUsed = " (" + (tps_cabinet.TraffUsed / DateTime.Now.Day).ToString("f2") + " Мб/день)";

                if (tps_cabinet.HasPackage && tps_cabinet.TraffLeft == 0)
                    lbl_Info.Text += "Использовано (П): " + tps_cabinet.Packages[0].TraffUsed + " Мб\r\n";
                else
                    lbl_Info.Text += "Использовано: " + tps_cabinet.TraffUsed + " Мб" + DayUsed + "\r\n";

                string PlaningDayUse = "";
                if (tps_cabinet.TraffLeft > 0 && (DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day) > 0)
                    PlaningDayUse = " (" + (tps_cabinet.TraffLeft / (DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day)).ToString("f2") + " Мб/день)";

                if (tps_cabinet.HasPackage && tps_cabinet.TraffLeft == 0)
                {
                    if (tps_cabinet.Packages[0].TraffLeft > 0)
                    {
                        if (tps_cabinet.Packages[0].DaysLeft > 0)
                            PlaningDayUse = " (" + (tps_cabinet.Packages[0].TraffLeft / (tps_cabinet.Packages[0].DaysLeft)).ToString("f2") + " Мб/день)";
                        else if (tps_cabinet.Packages[0].TotalHoursLeft > 0)
                            PlaningDayUse = " (" + (tps_cabinet.Packages[0].TraffLeft / (tps_cabinet.Packages[0].TotalHoursLeft)).ToString("f2") + " Мб/час)";
                    }
                    lbl_Info.Text += "Остаток (П): " + tps_cabinet.Packages[0].TraffLeft + " Мб" + PlaningDayUse + "\r\n";
                }
                else
                    lbl_Info.Text += "Остаток: " + tps_cabinet.TraffLeft + " Мб" + PlaningDayUse + "\r\n";

                lbl_Info.Text += "Баланс: $ " + tps_cabinet.Balance + " / Курс: " + tps_cabinet.Kurs.ToString("f2") + "\r\n";
                lbl_Info.Text += "К оплате (1-го): " + ((tps_cabinet.AbonentPlata - tps_cabinet.Balance) * tps_cabinet.Kurs).ToString("f2") + "\r\n";

                if (tps_cabinet.Packages.Count > 0)
                {
                    lbl_package1.ForeColor = Color.DarkGreen;
                    //lbl_package1.Text = tps_cabinet.Packages[0].DaysLeft + " " + tps_cabinet.Packages[0].DaysLeftPlural + " - " + tps_cabinet.Packages[0].TraffLeft.ToString("f0") + " Мб";
                    lbl_package1.Text = tps_cabinet.Packages[0].TimeLeftStr + " " + tps_cabinet.Packages[0].TraffLeft.ToString("f0") + " Мб";
                    strTip_package1 = "Пакет: " + tps_cabinet.Packages[0].Name+"\r\n";
                    strTip_package1 += "Активирован: " + tps_cabinet.Packages[0].StartDT.ToString("dd.MM.yy в H:mm") + "\r\n";
                    strTip_package1 += "Кончается: " + tps_cabinet.Packages[0].EndDT.ToString("dd.MM.yy в H:mm")+ "\r\n";
                    strTip_package1 += "Всего:" + tps_cabinet.Packages[0].TraffAll.ToString("f0") + " Мб\r\n";
                    strTip_package1 += "Остаток:" + tps_cabinet.Packages[0].TraffLeft.ToString("f0") + " Мб\r\n";
                }
                else
                {
                    lbl_package1.ForeColor = SystemColors.ControlText;
                    lbl_package1.Text = "Нет пакетов";
                    strTip_package1 = "";

                }
                if (tps_cabinet.Packages.Count > 1)
                {
                    //lbl_package2.Text = tps_cabinet.Packages[1].DaysLeft + " " + tps_cabinet.Packages[1].DaysLeftPlural + " - " + tps_cabinet.Packages[1].TraffLeft.ToString("f0") + " Мб";
                    lbl_package2.Text = tps_cabinet.Packages[1].TimeLeftStr + " " + tps_cabinet.Packages[1].TraffLeft.ToString("f0") + " Мб";
                    strTip_package2 = "Пакет: " + tps_cabinet.Packages[1].Name + "\r\n";
                    strTip_package2 += "Активирован: " + tps_cabinet.Packages[1].StartDT.ToString("dd.MM.yy в H:mm") + "\r\n";
                    strTip_package2 += "Кончается: " + tps_cabinet.Packages[1].EndDT.ToString("dd.MM.yy в H:mm")+ "\r\n";
                    strTip_package2 += "Всего:" + tps_cabinet.Packages[1].TraffAll.ToString("f0") + " Мб\r\n";
                    strTip_package2 += "Остаток:" + tps_cabinet.Packages[1].TraffLeft.ToString("f0") + " Мб\r\n";
                }
                else
                {
                    strTip_package2 = "";
                    lbl_package2.Text = "";
                }
                this.ResumeLayout();

                //AppIcon.Text += "CabinetID = " + tps_cabinet.CabinetID + "\r\n";
                ///AppIcon.Text += "AbonentPlata = " + tps_cabinet.AbonentPlata + "\r\n";
            }
        }



        /// <summary>
        /// Start fadeout animation
        /// </summary>
        delegate void delegate_StartFadeOutAnimation();
        void StartFadeOutAnimation()
        {
            if (this.ClientRectangle.Contains(this.PointToClient(MousePosition))) return;
            DelayTimer.Stop();
            FadeOutTimer.Start();
        }

        /// <summary>
        /// Process fadeout animation
        /// </summary>
        delegate void delegate_FadeOutAnimation();
        void FadeOutAnimation()
        {
            if (this.ClientRectangle.Contains(this.PointToClient(MousePosition)))
            {
                DelayedFadeOut();
                return;
            }
            this.Opacity -= .04;
            if (this.Opacity <= 0.2)
            {
                FadeOutTimer.Stop();
                this.Hide();
            }
        }

        /// <summary>
        /// Delayed fadeout
        /// </summary>
        public void DelayedFadeOut()
        {
            FadeOutTimer.Stop();
            this.Opacity = 1;
            DelayTimer.Stop();

            if (this.HideDelay > 0)
            {
                DelayTimer.Interval = this.HideDelay;
                DelayTimer.Start();
            }
        }

        private void frm_notify_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                FadeOutTimer.Stop();
                DelayTimer.Stop();
                FadeOutTimer.Dispose();
                DelayTimer.Dispose();
                if (BorderPath != null) BorderPath.Dispose();
                if (RegionPath != null) RegionPath.Dispose();
                if (BorderPen != null) BorderPen.Dispose();
            }
            catch { }
        }

        private void lbl_package1_MouseHover(object sender, EventArgs e)
        {
            if (strTip_package1 != "" && !strTip_package1_shown)
            {
                this.SuspendLayout();
                saved_lbl_Info = lbl_Info.Text;
                lbl_Info.Text = strTip_package1;
                this.DrawPackagePieChart(this.tps_cabinet.Packages[0], Brushes.DarkSeaGreen);
                strTip_package1_shown = true;
            }
        }

        private void lbl_package2_MouseHover(object sender, EventArgs e)
        {
            if (strTip_package2 != "" && !strTip_package2_shown)
            {
                this.SuspendLayout();
                saved_lbl_Info = lbl_Info.Text;
                lbl_Info.Text = strTip_package2;
                this.DrawPackagePieChart(this.tps_cabinet.Packages[1], Brushes.LightSeaGreen);
                strTip_package2_shown = true;
            }
        }

        private void DrawPackagePieChart(Package pkg, Brush graphBrush) {
            ///// draw traf use piechart ////
            if (pkg.TraffAll != 0 && pic_Graph.Visible)
            {
                Bitmap bmp = new Bitmap(pic_Graph.Width, pic_Graph.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.Bicubic;
                    g.Clear(Color.Transparent);
                    g.FillEllipse(graphBrush, 0, 0, bmp.Width - 1, bmp.Height - 1);
                    g.FillPie(Brushes.IndianRed, 0, 0, bmp.Width - 1, bmp.Height - 1, 0, pkg.TraffUsed * 360 / pkg.TraffAll);
                }
                if (pic_Graph.Image != null)
                {
                    saved_pic_Graph = (Image)pic_Graph.Image.Clone();
                    pic_Graph.Image.Dispose();
                    pic_Graph.Image = null;
                }
                pic_Graph.Image = bmp;
            }
        }


        private void lbl_package_MouseLeave(object sender, EventArgs e)
        {
            if (strTip_package1_shown || strTip_package2_shown)
            {
                if (pic_Graph.Visible && saved_pic_Graph != null)
                {
                    if (pic_Graph.Image != null)
                    {
                        pic_Graph.Image.Dispose();
                        pic_Graph.Image = null;
                    }
                    pic_Graph.Image = saved_pic_Graph;
                }
                lbl_Info.Text = saved_lbl_Info;
                this.ResumeLayout();
                strTip_package1_shown = false;
                strTip_package2_shown = false;
            }

            
        }



        

    }
}
