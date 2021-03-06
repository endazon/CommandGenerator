﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using System.IO;
using CommandGenerator.Class.Storage;
using CommandGenerator.Class.Display;
using System.Security.Permissions;
using System.Drawing;
using System.Linq;

namespace CommandGenerator
{
	public partial class FormEdit : Form
	{
		private CommandCsvStorage.CommandCsvObject CommandList { get; set; } = new CommandCsvStorage.CommandCsvObject();
		private string FileName { get; set; } = "";
		private InputScreen ScreenObj { get; set; } = new InputScreen(null);

		public FormEdit() 
		{
			InitializeComponent();

			ScreenObj = new InputScreen(SplitContainer.Panel2);
		}
		public FormEdit(string name, string version)
			: this()
		{
			CommandList.Name    = name;
			CommandList.Version = version;
		}

		//ウィンドウを閉じるボタン・ショートカットを無効化
		protected override CreateParams CreateParams
		{
			[SecurityPermission(SecurityAction.Demand,
				Flags = SecurityPermissionFlag.UnmanagedCode)]
			get
			{
				const int CS_NOCLOSE = 0x200;
				CreateParams cp = base.CreateParams;
				cp.ClassStyle = cp.ClassStyle | CS_NOCLOSE;

				return cp;
			}
		}

		private CommandCsvStorage.Item ConvertJsonItemToCsvItem(CommandJsonStorage.Item src)
		{
			var dst = new CommandCsvStorage.Item();

			dst.Command = string.Join("", src.MessageGeneration().ToArray());
			dst.Length  = (ulong)src.Length;
			dst.Type    = src.Name;
			dst.Tag     = src;

			return dst;
		}

		public virtual void LocationUpdate(int x, int y)
		{
			int buff_x = x >= 0 ? x : Location.X;
			int buff_y = y >= 0 ? y : Location.Y;

			Location = new Point(buff_x - 5, buff_y);
		}

		public virtual void SizeUpdate(int width, int height)
		{
			int buff_width  = width >= 0  ? width  : Size.Width;
			int buff_height = height >= 0 ? height : Size.Height;

			Size = new Size(buff_width, buff_height);
		}

		public virtual void chgListBoxFont(Font f)
		{
			CommandListBox.Font = f;
		}

		public void Clear()
		{
			CommandList.Items.Clear();
			FileName = "";
			CommandListBox.Items.Clear();
			ScreenObj.Clear();
		}

		public void Add(object obj)
		{
			if (obj.GetType().Name != "Item") { return; }

			CommandJsonStorage.Item item = (CommandJsonStorage.Item)obj;
			var csvObj                   = ConvertJsonItemToCsvItem(item);
			csvObj.Name                  = new FormInputTextBox("表示名を入力して下さい。",
															    "表示名の入力",
																item.Name
															   ).GetInputText();

			if (csvObj.Name != "") {
				//for (int i = 0; i < 100; i++)
				CommandListBox.Items.Add(csvObj.Clone());

				// リストボックスの一番上を選択
				CommandListBox.SetSelected(CommandListBox.Items.Count - 1, true);
			}
		} 

		public void Update(object obj)
		{
			if (obj.GetType().Name != "CommandCsvObject") { return; }

			foreach (var item in ((CommandCsvStorage.CommandCsvObject)obj).Items)
			{
				CommandListBox.Items.Add(item.Clone());
			}
			CommandListBox.SetSelected(CommandListBox.Items.Count - 1, true);
		}

		public void Save()
		{
			#region 前処理
			try
			{
				ScreenObj.Save(((CommandJsonStorage.Item)((CommandCsvStorage.Item)CommandListBox.SelectedItem).Tag).Controls);

				foreach (var item in CommandListBox.Items)
				{
					var jsonObj  = (CommandJsonStorage.Item)((CommandCsvStorage.Item)item).Tag;
					var csvObj   = ConvertJsonItemToCsvItem(jsonObj);
					csvObj.Name  = item.ToString();
					CommandList.Items.Add(csvObj);
				}
			}
			catch
			{
				return;
			}
			#endregion

			#region 保存先ファイル選択
			try
			{
				if (FileName == "")
				{
					string fileName = "";

					//ダイアログを表示する
					if (saveFileDialogCsv.ShowDialog() == DialogResult.OK)
					{
						//ファイル名取得(パス込み)
						fileName = saveFileDialogCsv.FileName;
					}
					//Console.WriteLine(fileName);

					FileName = fileName;
				}
			}
			catch
			{
				return;
			}
			#endregion

			#region CSVファイル書き込み
			try
			{
				// 出力用のファイルを開く
				using (var sw = new StreamWriter(FileName, false, Encoding.UTF8))
				{
					sw.WriteLine(CommandList.Serialize());
				}
			}
			catch (Exception e)
			{
				// ファイルを開くのに失敗したときエラーメッセージを表示
				Console.WriteLine(e.Message);
				return;
			}
			#endregion

			#region 後処理
			{
				CommandList.Items.Clear();
			}
			#endregion
		}

		public object GetSelectItem()
		{
			if (CommandListBox.SelectedItem == null) { return null; }
			var item = CommandListBox.SelectedItem;

			var jsonObj = (CommandJsonStorage.Item)((CommandCsvStorage.Item)item).Tag;
			var csvObj = ConvertJsonItemToCsvItem(jsonObj);
			csvObj.Name = item.ToString();

			return csvObj;
		}

		#region Menu
		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Clear();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Save();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FileName = "";
			Save();
		}
		#endregion

		#region ListBox
		private void CommandListBox_DoubleClick(object sender, EventArgs e)
		{
			int index = ((ListBox)sender).SelectedIndex;

			if (index < 0) { return; }

			CommandListBox.Items.RemoveAt(index);
			ScreenObj.Clear();
		}

		private List<CommandJsonStorage.Detail> z1Items = null;
		private void CommandListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			var target = ((ListBox)sender).SelectedItem;
			if (target == null) { return; }

			var items = ((CommandJsonStorage.Item)((CommandCsvStorage.Item)target).Tag).Controls;

			if (items == null) { return; }

			ScreenObj.Save(z1Items);
			ScreenObj.Update(items);

			z1Items = items;
		}

		#endregion

		#region Button
		public virtual void buttonRunning_Click(object sender, EventArgs e)
		{

		}
		#endregion

		#region Window
		public virtual void FormEdit_Load(object sender, EventArgs e)
		{

		}
		public virtual void FormEdit_FormClosed(object sender, FormClosedEventArgs e)
		{

		}
		#endregion
	}
}
