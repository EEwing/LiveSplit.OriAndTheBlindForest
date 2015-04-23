﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.OriAndTheBlindForest {
	public partial class OriAndTheBlindForestSettings : UserControl {
		public List<Devil.Split> splitsState = new List<Devil.Split>();

		private bool isLoading = false;
		public OriComponent parent;

		public OriAndTheBlindForestSettings(OriComponent component) {
			InitializeComponent();
			parent = component;
		}

		private void btnAddSplit_Click(object sender, EventArgs e) {
			SplitSettings setting = new SplitSettings();
			setting.cboName.DisplayMember = "SplitName";
			setting.cboName.ValueMember = "Type";
			setting.cboName.DataSource = SplitComboData();
			setting.cboName.Text = "Start";
			setting.txtValue.Text = "True";
			AddHandlers(setting);

			flowMain.Controls.Add(setting);
			UpdateSettings();
		}
		public void LoadSettings() {
			isLoading = true;

			for (int i = flowMain.Controls.Count - 1; i > 1; i--) {
				flowMain.Controls.RemoveAt(i);
			}
			foreach (var split in splitsState) {
				string name = split.name;
				string type = Devil.OriTriggers.availableSplits[name];
				string value = split.value;

				SplitSettings setting = new SplitSettings();
				setting.cboName.DisplayMember = "SplitName";
				setting.cboName.ValueMember = "Type";
				setting.cboName.DataSource = SplitComboData();
				setting.cboName.Text = name;
				setting.txtValue.Text = value;
				AddHandlers(setting);

				flowMain.Controls.Add(setting);
			}
			isLoading = false;
		}

		private void AddHandlers(SplitSettings setting) {
			setting.cboName.SelectedIndexChanged += cboName_SelectedIndexChanged;
			setting.btnRemove.Click += btnRemove_Click;
			setting.btnUp.Click += btnUp_Click;
			setting.btnDown.Click += btnDown_Click;
		}
		private void RemoveHandlers(SplitSettings setting) {
			setting.cboName.SelectedIndexChanged -= cboName_SelectedIndexChanged;
			setting.btnRemove.Click -= btnRemove_Click;
			setting.btnUp.Click -= btnUp_Click;
			setting.btnDown.Click -= btnDown_Click;
		}

		void btnDown_Click(object sender, EventArgs e) {
			for (int i = flowMain.Controls.Count - 2; i > 1; i--) {
				Control c = flowMain.Controls[i];
				if (c.Contains((Control)sender)) {
					flowMain.Controls.SetChildIndex(c, i + 1);
					UpdateSettings();
					break;
				}
			}
		}
		void btnUp_Click(object sender, EventArgs e) {
			for (int i = flowMain.Controls.Count - 1; i > 2; i--) {
				Control c = flowMain.Controls[i];
				if (c.Contains((Control)sender)) {
					flowMain.Controls.SetChildIndex(c, i - 1);
					UpdateSettings();
					break;
				}
			}
		}
		void cboName_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateSettings();
		}
		void btnRemove_Click(object sender, EventArgs e) {
			for (int i = flowMain.Controls.Count - 1; i > 1; i--) {
				if (flowMain.Controls[i].Contains((Control)sender)) {
					RemoveHandlers((SplitSettings)((Button)sender).Parent);

					flowMain.Controls.RemoveAt(i);
					break;
				}
			}
			UpdateSettings();
		}

		public void UpdateSettings() {
			if (isLoading) return;

			splitsState.Clear();
			foreach (Control c in flowMain.Controls) {
				if (c is SplitSettings) {
					SplitSettings setting = (SplitSettings)c;
					if (!string.IsNullOrEmpty(setting.cboName.Text) && !string.IsNullOrEmpty(setting.txtValue.Text)) {
						string name = setting.cboName.Text;
						string value = setting.txtValue.Text;

						Devil.Split split = new Devil.Split();
						split.name = name;
						split.value = value;

						splitsState.Add(split);
					}
				}
			}

			parent.oriState.UpdateSplits(splitsState);
		}

		public XmlNode GetSettings(XmlDocument document) {
			var settingsNode = document.CreateElement("Settings");

			var splitsNode = document.CreateElement("Splits");
			settingsNode.AppendChild(splitsNode);

			foreach (var split in splitsState) {
				string name = split.name;
				string value = split.value;

				var splitNode = document.CreateElement("Split");
				splitNode.InnerText = name;

				var valueAttribute = document.CreateAttribute("Value");
				valueAttribute.Value = value;
				splitNode.Attributes.Append(valueAttribute);

				splitsNode.AppendChild(splitNode);
			}

			return settingsNode;
		}

		public void SetSettings(XmlNode settings) {
			write(settings.FirstChild.Name);

			XmlNodeList splitNodes = settings.SelectNodes("//Splits/Split");

			write(splitNodes.Count.ToString());
			splitsState.Clear();
			foreach (XmlNode splitNode in splitNodes) {
				string name = splitNode.InnerText;
				string value = splitNode.Attributes["Value"].Value;

				Devil.Split split = new Devil.Split();
				split.name = name;
				split.value = value;

				splitsState.Add(split);
			}
		}

		public DataTable SplitComboData() {
			DataTable dt = new DataTable();
			dt.Columns.Add("SplitName", typeof(string));
			dt.Columns.Add("Type", typeof(string));
			foreach (var pair in Devil.OriTriggers.availableSplits) {
				dt.Rows.Add(pair.Key, pair.Value);
			}
			return dt;
		}

		private void Settings_Load(object sender, EventArgs e) {
			LoadSettings();
		}

		private void write(string str) {
			StreamWriter wr = new StreamWriter("test.log", true);
			wr.WriteLine("[" + DateTime.Now + "] " + str);
			wr.Close();
		}
	}
}
