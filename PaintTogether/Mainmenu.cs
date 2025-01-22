using Microsoft.Xna.Framework;
using Myra;
using Myra.Graphics2D.UI;

namespace PaintTogether;

public class Mainmenu : DrawableGameComponent
{
    private Desktop _desktop;
    private Game1 _game;
    
    public Mainmenu(Game1 game) : base(game)
    {
        _game = game;
        // _desktop = desktop;
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        
        MyraEnvironment.Game = _game;

        var grid = new Grid
        {
            RowSpacing = 8,
            ColumnSpacing = 8
        };

        grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
        grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

        var title = new Label
        {
            Id = "title",
            Text = "Socket Whiteboard!"
        };
        
        title.HorizontalAlignment = HorizontalAlignment.Center;
        grid.Widgets.Add(title);

// Host Button
        var hostBtn = new Button
        {
            Content = new Label
            {
                Text = "Host"
            }
        };
        Grid.SetColumn(hostBtn, 0);
        Grid.SetRow(hostBtn, 1);

        hostBtn.Click += (s, a) =>
        {
            _game.SwitchMode(Mode.HOST);
        };

        hostBtn.HorizontalAlignment = HorizontalAlignment.Center;
        grid.Widgets.Add(hostBtn); 

// Host Button
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
            _game.SwitchMode(Mode.JOIN_FORM);
        };

        joinBtn.HorizontalAlignment = HorizontalAlignment.Center;
        grid.Widgets.Add(joinBtn);
        
        grid.HorizontalAlignment = HorizontalAlignment.Center;
        grid.VerticalAlignment = VerticalAlignment.Center;
        
        // Add it to the desktop
        _desktop = new Desktop();
        _desktop.Root = grid;
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        if (_desktop != null) _desktop.Render();
    }
}