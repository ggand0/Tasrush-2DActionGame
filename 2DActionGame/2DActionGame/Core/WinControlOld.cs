using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
	public partial class WinControl : Form
	{
		private Game1 game;
		private ClearScene scene;

		public WinControl(Game1 game)
		{
			this.game = game;
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			game.playerName = textBox1.Text;
			this.Visible = false;
			this.Dispose();
			if (!game.inDebugMode) game.EvaluateScore("Ranking.txt");
		}
	}
	public class WinControlManager
	{
		GraphicsDeviceManager graphics;
		Form XnaForm;
		public WinControl ControlForm { get; set; }
		public WinControlManager(Game1 game, GraphicsDeviceManager graphics)
		{
			ControlForm = new WinControl(game);
			this.graphics = graphics;
			XnaForm = Form.FromHandle(game.Window.Handle) as Form;
			XnaForm.FormBorderStyle = FormBorderStyle.None;//本来のXNAの枠を無くす
			XnaForm.Dock = DockStyle.Fill;//左上で揃える
			XnaForm.TopLevel = false;
			ControlForm.Controls.Add(XnaForm);//コントロール追加
			ControlForm.Show();
		}

		
	}
}
