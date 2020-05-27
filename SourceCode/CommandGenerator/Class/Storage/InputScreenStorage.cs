﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CommandGenerator.Class.Storage
{
	class InputScreenStorage
	{
		public class Input
		{
			#region Property
			public Label Label { get; set; } = new Label();
			public Control InputBox { get; set; } = new Control();
			#endregion

			#region Constructor
			public Input()
			{

			}
			public Input(string text, Font font, Point point)
				: this()
			{
				Label = CreateLabel(text, font, point);
			}
			public Input(string text, Font font, Point point, string type, int size, string value)
				: this(text, font, point)
			{
				InputBox = CreateInputBox(type)(text, size, value, font, point);
				InputBox.Tag = type;
			}
			#endregion

			#region Helper
			private Size GetOneCharacterSize(Font font)
			{
				Label label = new Label();
				label.AutoSize = true;
				label.Font = font;
				label.Name = " ";
				label.Text = " ";
				return new Size(label.PreferredWidth - 5, label.PreferredHeight);
			}
			private Func<string, int, string, Font, Point, Control> CreateInputBox(string type)
			{
				Func<string, int, string, Font, Point, Control> result_func = null;
				switch (type)
				{
					case "FILE_SELECT":
						result_func = (string text, int size, string value, Font font, Point point) =>
						{
							return CreateFileSelectBox(text, size, value, font, point);
						};
						break;
					case "SELECT":
						result_func = (string text, int size, string value, Font font, Point point) =>
						{
							return CreateComboBox(text, size, value, font, point);
						};
						break;
					case "DEC":
						result_func = (string text, int size, string value, Font font, Point point) =>
						{
							return CreateNumericUpDown(text, size, value, font, point);
						};
						break;
					case "HEX":
						result_func = (string text, int size, string value, Font font, Point point) =>
						{
							return CreateTextBox(text, size * 2, value, font, point);
						};
						break;
					case "ASCII":
						result_func = (string text, int size, string value, Font font, Point point) =>
						{
							return CreateTextBox(text, size, value, font, point);
						};
						break;
					default:
						result_func = (string text, int size, string value, Font font, Point point) =>
						{
							throw new Exception();
						};
						break;
				}

				return result_func;
			}
			private Label CreateLabel(string text, Font font, Point point)
			{
				Label label       = new Label();
				label.AutoSize    = true;
				label.BorderStyle = BorderStyle.None;
				label.Font        = font;
				label.ForeColor   = SystemColors.Window;
				label.Name        = text + label.GetType().Name;
				label.Text        = text;
				label.Location    = point;

				return label;
			}
			private TextBox CreateTextBox(string text, int size, string value, Font font, Point point)
			{
				TextBox textbox     = new TextBox();
				textbox.BackColor   = SystemColors.WindowText;
				textbox.BorderStyle = BorderStyle.FixedSingle;
				textbox.Font        = font;
				textbox.ForeColor   = SystemColors.Window;
				textbox.MaxLength   = 2 + size;
				textbox.Name        = text + textbox.GetType().Name;
				textbox.Text        = value;
				Label label         = CreateLabel(text, font, point);
				textbox.Location    = new Point(point.X + label.PreferredWidth, point.Y);
				textbox.Size        = new Size(GetOneCharacterSize(font).Width * textbox.MaxLength, label.PreferredHeight);
				textbox.TextChanged += new EventHandler(textBox_TextChanged);

				return textbox;
			}
			private NumericUpDown CreateNumericUpDown(string text, int size, string value, Font font, Point point)
			{
				NumericUpDown numericupdown = new NumericUpDown();
				numericupdown.BackColor     = SystemColors.WindowText;
				numericupdown.BorderStyle   = BorderStyle.FixedSingle;
				numericupdown.Font          = font;
				numericupdown.ForeColor     = SystemColors.Window;
				numericupdown.Minimum       = 0;
				numericupdown.Maximum       = (int)Math.Pow(2, (8 * size)) - 1;
				numericupdown.Name          = text + numericupdown.GetType().Name;
				numericupdown.Text          = value;
				Label label                 = CreateLabel(text, font, point);
				numericupdown.Location      = new Point(point.X + label.PreferredWidth, point.Y);
				int digits                  = (int)Math.Log10((double)numericupdown.Maximum) + 1;
				numericupdown.Size          = new Size(GetOneCharacterSize(font).Width * (digits + 2), label.PreferredHeight);
				numericupdown.ValueChanged += new EventHandler(numericUpDown_ValueChanged);

				return numericupdown;
			}
			private ComboBox CreateComboBox(string text, int size, string value, Font font, Point point)
			{
				ComboBox combobox  = new ComboBox();
				combobox.BackColor = SystemColors.WindowText;
				combobox.FlatStyle = FlatStyle.Flat;
				combobox.Font      = font;
				combobox.ForeColor = SystemColors.Window;
				combobox.MaxLength = 2 + (2 * size);
				combobox.Name      = text + combobox.GetType().Name;
				combobox.Text      = value;
				Label label        = CreateLabel(text, font, point);
				combobox.Location  = new Point(point.X + label.PreferredWidth, point.Y);
				combobox.Size      = new Size(GetOneCharacterSize(font).Width * combobox.MaxLength, label.PreferredHeight);

				return combobox;
			}
			private Panel CreateFileSelectBox(string text, int size, string value, Font font, Point point)
			{
				Panel fileselect     = new Panel();
				fileselect.BackColor = SystemColors.ControlDarkDark;
				fileselect.Name      = text + fileselect.GetType().Name;
				Label label          = CreateLabel(text, font, point);
				fileselect.Location  = new Point(point.X + label.PreferredWidth, point.Y);
				fileselect.SuspendLayout();

				TextBox file     = new TextBox();
				file.AutoEllipsis = true;
				file.BackColor   = SystemColors.WindowText;
				file.BorderStyle = BorderStyle.FixedSingle;
				file.Font        = font;
				file.ForeColor   = SystemColors.Window;
				file.MaxLength   = 20;
				file.Name        = text + file.GetType().Name;
				file.Text        = value;
				file.Location    = new Point(0, 0);
				file.Size        = new Size(GetOneCharacterSize(font).Width * file.MaxLength, label.PreferredHeight);

				Button select    = new Button();
				select.BackColor = SystemColors.Control;
				select.FlatStyle = FlatStyle.Flat;
				select.ForeColor = SystemColors.ControlText;
				select.Name      = text + select.GetType().Name;
				select.Text      = "...";
				select.TextAlign = ContentAlignment.MiddleCenter;
				select.Location  = new Point(file.Location.X + file.Size.Width, file.Location.Y);
				select.Size      = new Size(GetOneCharacterSize(font).Width * select.Text.Length, label.PreferredHeight);
				select.Click    += new EventHandler(button_Click);

				fileselect.Size = new Size(file.Size.Width + select.Size.Width, label.PreferredHeight);
				fileselect.Controls.Add(file);
				fileselect.Controls.Add(select);
				fileselect.ResumeLayout(false);
				fileselect.PerformLayout();

				return fileselect;
			}
			#endregion

			#region Event
			private void textBox_TextChanged(object sender, EventArgs e)
			{
				TextBox target = (TextBox)sender;
				if ((string)target.Tag == "HEX")
				{
					string text = (string)target.Text.Clone();
					int length = text.Length;

					if ((length < 2)
						|| text.Substring(0, 1) != "0"
						|| text.Substring(1, 1) != "x")
					{
						target.Text = "0x";
						target.SelectionStart = target.TextLength;
						System.Media.SystemSounds.Asterisk.Play();
					}
					else if (length == 2) { return; }
					else
					{
						int char_pos = 2;
						foreach (var c in text.Substring(char_pos, length - char_pos))
						{
							if (!Uri.IsHexDigit(c))
							{
								target.Text = text.Remove(char_pos, 1);
								System.Media.SystemSounds.Asterisk.Play();
							}
							char_pos++;
						}
					}
				}
			}
			private void numericUpDown_ValueChanged(object sender, EventArgs e)
			{
				NumericUpDown target = (NumericUpDown)sender;
				decimal value = target.Value;
				decimal maximum = target.Maximum;

				if (maximum < value)
				{
					target.Value = maximum;
				}
			}
			private void button_Click(object sender, EventArgs e)
			{
				Button target   = (Button)sender;
				Panel panel     = (Panel)target.Parent.Controls.Owner;
				Control[] array = new Control[2];
				panel.Controls.CopyTo(array, 0);
				TextBox textBox   = (TextBox)array[0];

				if ((string)panel.Tag == "FILE_SELECT")
				{
					//ダイアログを表示する
					var OpenDialog = new OpenFileDialog();
					if (OpenDialog.ShowDialog() == DialogResult.OK)
					{
						//ファイル名取得(パス込み)
						textBox.Text = OpenDialog.FileName;
					}
				}
			}
			#endregion

			#region PpublicMethod
			public Control[] ToArray()
			{
				List<Control> result = new List<Control>();
				result.Add(Label);
				result.Add(InputBox);

				return result.ToArray();
			}
			public void Enabled()
			{
				Label.Enabled = true;
				InputBox.Enabled = true;
			}
			public void Disabled()
			{
				Label.Enabled = false;
				InputBox.Enabled = false;
			}
			#endregion
		}

		public class InputGroup
		{
			#region Property
			public GroupBox GroupBox { get; set; } = new GroupBox();
			public List<Input> Input { get; set; } = new List<Input>();
			#endregion

			#region Constructor
			public InputGroup()
			{

			}
			public InputGroup(string text, Size size, Font font, Point point)
				: this()
			{
				GroupBox = CreateGroupBox(text, size, font, point);
			}
			#endregion

			#region Helper
			private GroupBox CreateGroupBox(string text, Size size, Font font, Point point)
			{
				GroupBox groupbox   = new GroupBox();
				//groupbox.AutoSize = true;
				groupbox.Font       = font;
				groupbox.ForeColor  = SystemColors.HighlightText;
				groupbox.Name       = text + groupbox.GetType().Name;
				groupbox.Text       = text;
				groupbox.Location   = point;
				groupbox.Size       = size;

				return groupbox;
			}
			#endregion

			#region PpublicMethod
			public void SuspendLayout()
			{
				GroupBox.SuspendLayout();
			}
			public void Update()
			{
				Update(Input);
			}
			public void Update(List<Input> inputList)
			{
				foreach (var item in inputList)
				{
					GroupBox.Controls.AddRange(item.ToArray());
				}
				Input = inputList;
			}
			public void ResumeLayout(bool performLayout)
			{
				GroupBox.ResumeLayout(performLayout);
			}
			public void PerformLayout()
			{
				GroupBox.PerformLayout();
			}
			public void LineUp()
			{
				LineUp(0);
			}
			public void LineUp(int span)
			{
				int max_size = 0;
				foreach (var item in Input)
				{
					max_size = max_size < item.InputBox.Location.X ? item.InputBox.Location.X : max_size;
				}
				foreach (var item in Input)
				{
					item.InputBox.Location = new Point(max_size + span, item.InputBox.Location.Y);
				}
			}
			#endregion
		}

		public class InputScreenObject
		{
			#region Property
			public Panel Panel { get; set; } = new Panel();
			public List<InputGroup> InputGroup { get; set; } = new List<InputGroup>();
			#endregion

			#region Constructor
			public InputScreenObject()
			{

			}
			public InputScreenObject(Panel panel)
				: this()
			{
				Panel = panel;
			}
			#endregion

			#region Helper

			#endregion

			#region PpublicMethod
			public void Clear()
			{
				Panel.Controls.Clear();
				InputGroup.Clear();
			}
			public void Update()
			{
				Update(InputGroup);
			}
			public void Update(List<InputGroup> inputGroupList)
			{
				foreach (var item in inputGroupList)
				{
					Panel.Controls.Add(item.GroupBox);
				}
				InputGroup = inputGroupList;
			}
			#endregion
		}
	}
}
