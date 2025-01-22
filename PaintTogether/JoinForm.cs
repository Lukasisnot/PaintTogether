using System;
using Microsoft.Xna.Framework;
using Myra;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;

namespace PaintTogether;

public class JoinForm : DrawableGameComponent
{
    private Game1 _game;
    private Desktop _desktop;
    
    public JoinForm(Game1 game) : base(game)
    {
        _game = game;
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        
        MyraEnvironment.Game = _game;
        _desktop = new Desktop();

// Grid 1 X 3
        var grid = new Grid
        {
            RowSpacing = 8,
            ColumnSpacing = 8
        };
        
        grid.HorizontalAlignment = HorizontalAlignment.Center;
        grid.VerticalAlignment = VerticalAlignment.Center;
        
        grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
        grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        
// Host IP textbox
        var title = new Label
        {
            Id = "title",
            Text = "Join a Whiteboard!"
        };
        title.HorizontalAlignment = HorizontalAlignment.Center;
        grid.Widgets.Add(title);

        var ipBox = new TextBox
        {
            Id = "ipBox",
            HintText = "Enter host ip",
            Text = "127.0.0.1"
        };
        Grid.SetColumn(ipBox, 0);
        Grid.SetRow(ipBox, 1);
        title.HorizontalAlignment = HorizontalAlignment.Center;
        grid.Widgets.Add(ipBox);
        
// Join button
        var joinBtn = new Button
        {
            Content = new Label
            {
                Text = "Join LAN"
            }
        };
        Grid.SetColumn(joinBtn, 0);
        Grid.SetRow(joinBtn, 2);

        joinBtn.Click += (s, a) =>
        {
        // Create client
            _game.SwitchMode(Mode.CLIENT, ipBox.Text);
        };

        joinBtn.HorizontalAlignment = HorizontalAlignment.Center;
        grid.Widgets.Add(joinBtn);
        
// Add it to the desktop
        _desktop.Root = grid;
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        _desktop.Render();
    }
}