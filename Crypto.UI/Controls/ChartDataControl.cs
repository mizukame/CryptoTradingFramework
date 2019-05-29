﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Designer;
using System.IO;
using DevExpress.XtraBars;
using System.Reflection;
using Crypto.Core.Strategies;
using Crypto.Core.Strategies.Custom;
using DevExpress.Utils;

namespace Crypto.UI.Forms {
    public partial class ChartDataControl : XtraUserControl {
        public ChartDataControl() {
            InitializeComponent();
            this.chartControl.UseDirectXPaint = true;
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartControl Chart { get { return this.chartControl; } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IStrategyDataItemInfoOwner Visual { get; set; }

        private void biCustomize_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            ChartDesigner designer = new ChartDesigner(this.chartControl);
            if(designer.ShowDialog() == DialogResult.OK) {
                CheckCreateSettingsFolder();
                DetachePoints();
                this.chartControl.SaveToFile(ChartSettingsFileName);
                AttachPoints();
            }
        }

        protected Dictionary<Series, SeriesPoint[]> DetachedPoints { get; set; }

        private void DetachePoints() {
            DetachedPoints = new Dictionary<Series, SeriesPoint[]>();
            for(int i = 0; i < this.chartControl.Series.Count; i++) {
                Series s = this.chartControl.Series[i];
                if(s.DataSource == null && s.Points.Count > 0) {
                    SeriesPoint[] list = new SeriesPoint[s.Points.Count];
                    int index = 0;
                    for(int ii = 0; ii < s.Points.Count; ii++) {
                        list[index] = s.Points[ii];
                        index++;
                    }
                    s.Points.Clear();
                    DetachedPoints.Add(s, list);
                }
            }
        }

        private void AttachPoints() {
            foreach(Series s in DetachedPoints.Keys) {
                s.Points.AddRange(DetachedPoints[s]);
            }
        }

        protected string SettingsFolder { get { return "StrategiesSettings"; } }
        protected string ChartSettingsFileName { get { return SettingsFolder + "\\Chart_" + Visual.GetType().Name + ".xml"; } }
        private void CheckCreateSettingsFolder() {
            if(!Directory.Exists(SettingsFolder))
                Directory.CreateDirectory(SettingsFolder);
        }

        private void biReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            if(File.Exists(ChartSettingsFileName))
                File.Delete(ChartSettingsFileName);
        }

        private void bsPanes_GetItemData(object sender, EventArgs e) {
            if(this.bsPanes.ItemLinks.Count != 0)
                return;
            XYDiagram dg = (XYDiagram)this.chartControl.Diagram;
            if(dg == null)
                return;
            foreach(XYDiagramPane item in dg.Panes) {
                BarCheckItem ch = new BarCheckItem(this.barManager1) { Caption = item.Name, Checked = item.Visibility == ChartElementVisibility.Visible };
                ch.Tag = item;
                ch.CheckedChanged += OnPaneCheckedChanged;
                this.bsPanes.ItemLinks.Add(ch);
            }
        }

        private void bsSeries_GetItemData(object sender, EventArgs e) {
            if(this.bsSeries.ItemLinks.Count != 0)
                return;
            XYDiagram dg = (XYDiagram)this.chartControl.Diagram;
            if(dg == null)
                return;
            List<XYDiagramPaneBase> panes = new List<XYDiagramPaneBase>();
            panes.Add(dg.DefaultPane);
            foreach(XYDiagramPaneBase pane in dg.Panes)
                panes.Add(pane);
            foreach(XYDiagramPaneBase item in panes) {
                BarSubItem paneMenu = new BarSubItem(this.barManager1, item.Name);
                this.bsSeries.ItemLinks.Add(paneMenu);
                foreach(Series s in this.chartControl.Series) {
                    XYDiagramSeriesViewBase v = (XYDiagramSeriesViewBase)s.View;
                    if(v.Pane != item)
                        continue;

                    BarCheckItem ch = new BarCheckItem(this.barManager1) { Caption = s.Name, Checked = true };
                    ch.Tag = s;
                    ch.CheckedChanged += OnSeriesCheckedChanged;
                    paneMenu.ItemLinks.Add(ch);
                }
            }
        }

        private void OnSeriesCheckedChanged(object sender, ItemClickEventArgs e) {
            Series s = (Series)e.Item.Tag;
            s.Visible = ((BarCheckItem)sender).Checked;
        }
        private void OnPaneCheckedChanged(object sender, ItemClickEventArgs e) {
            XYDiagramPane item = (XYDiagramPane)e.Item.Tag;
            item.Visibility = ((BarCheckItem)e.Item).Checked ? ChartElementVisibility.Visible : ChartElementVisibility.Hidden;
        }

        private void bsIndex_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {

        }

        private void chartControl_MouseMove(object sender, MouseEventArgs e) {
            //ChartHitInfo info = this.chartControl.CalcHitInfo(e.Location, false);
            //try {
            //    if(info.InAnnotation) {
            //        IDetailInfoProvider detailProvider = info.Annotation.Tag as IDetailInfoProvider;
            //        ToolTipControllerShowEventArgs sa = new ToolTipControllerShowEventArgs();
            //        sa.AllowHtmlText = DefaultBoolean.True;
            //        sa.ToolTipType = ToolTipType.SuperTip;
            //        sa.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
            //        sa.ToolTip = detailProvider.DetailString;
            //        sa.SelectedObject = detailProvider;
            //        ToolTipController.DefaultController.ShowHint(sa);
            //    }
            //    else {
            //    }
            //}
            //catch(Exception) {

            //}
        }
    }
}
